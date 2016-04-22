using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Transactions;
using System.Data;

namespace MyWebApi.Models
{
    /// <summary>
    /// EF数据库配置
    /// </summary>
    public class DbContextConfiguration : DbConfiguration
    {
        public DbContextConfiguration()
        {
            DbInterception.Add(new SQLProfiler(50));//配置EF的sql性能监视器
            DbInterception.Add(new EFLogTransactionInterceptor());//配置EF的事务监视器
            DbInterception.Add(new DbMasterSlaveCommandInterceptor());//配置读写分离监视器
        }
    }

    /// <summary>
    /// 读写分离监视器
    /// </summary>
    public class DbMasterSlaveCommandInterceptor : DbCommandInterceptor
    {
        private Lazy<string> masterConnectionString = new Lazy<string>(() => ConfigurationManager.ConnectionStrings["masterConnectionString"].ConnectionString);
        private Lazy<string> slaveConnectionString = new Lazy<string>(() => ConfigurationManager.ConnectionStrings["slaveConnectionString"].ConnectionString);

        public string MasterConnectionString
        {
            get { return this.masterConnectionString.Value; }
        }

        public string SlaveConnectionString
        {
            get { return this.slaveConnectionString.Value; }
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.UpdateToSlave(interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.UpdateToSlave(interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            this.UpdateToMaster(interceptionContext);
        }

        private void UpdateToMaster(DbInterceptionContext interceptionContext)
        {
            foreach (var context in interceptionContext.DbContexts)
            {
                this.UpdateConnectionStringIfNeed(context.Database.Connection, this.MasterConnectionString);
            }
        }

        private void UpdateToSlave(DbInterceptionContext interceptionContext)
        {
            // 判断当前会话是否处于分布式事务中
            bool isDistributedTran = Transaction.Current != null && Transaction.Current.TransactionInformation.Status != TransactionStatus.Committed;
            foreach (var context in interceptionContext.DbContexts)
            {
                // 判断该 context 是否处于普通数据库事务中
                bool isDbTran = context.Database.CurrentTransaction != null;

                // 如果处于分布式事务或普通事务中，则“禁用”读写分离，处于事务中的所有读写操作都指向 Master
                string connectionString = isDistributedTran || isDbTran ? this.MasterConnectionString : this.SlaveConnectionString;

                this.UpdateConnectionStringIfNeed(context.Database.Connection, connectionString);
            }
        }

        /// <summary>
        /// 此处改进了对连接字符串的修改判断机制，确认只在 <paramref name="conn"/> 所使用的连接字符串不等效于 <paramref name="connectionString"/> 的情况下才需要修改。
        /// <para>同时，在必要的情况下才会连接进行 Open 和 Close 操作以及修改 ConnectionString 处理，减少了性能的消耗。</para>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="connectionString"></param>
        private void UpdateConnectionStringIfNeed(DbConnection conn, string connectionString)
        {
            if (!this.ConnectionStringCompare(conn, connectionString))
            {
                this.UpdateConnectionString(conn, connectionString);
            }
        }

        private void UpdateConnectionString(DbConnection conn, string connectionString)
        {
            ConnectionState state = conn.State;
            if (state == ConnectionState.Open)
                conn.Close();

            conn.ConnectionString = connectionString;

            if (state == ConnectionState.Open)
                conn.Open();
        }

        private bool ConnectionStringCompare(DbConnection conn, string connectionString)
        {
            DbConnectionStringBuilder a = new DbConnectionStringBuilder();
            a.ConnectionString = conn.ConnectionString;

            DbConnectionStringBuilder b = new DbConnectionStringBuilder();
            b.ConnectionString = connectionString;

            return a.EquivalentTo(b);
        }
    }

    /// <summary>
    /// sql性能监视器
    /// </summary>
    public class SQLProfiler : DbCommandInterceptor
    {
        private int executionTime = 100;
        /// <summary>
        /// sql性能监视器
        /// </summary>
        /// <param name="executionTime">sql执行时间，超过此时间则进行记录，单位毫秒</param>
        public SQLProfiler(int executionTime)
        {
            this.executionTime = executionTime;
        }
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executing(interceptionContext);
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.ReaderExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executing(interceptionContext);
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executing(interceptionContext);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.ScalarExecuted(command, interceptionContext);
        }

        private void Executing<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = new Stopwatch();
            interceptionContext.UserState = timer;
            timer.Start();
        }

        private void Executed<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = (Stopwatch)interceptionContext.UserState;
            timer.Stop();

            if (interceptionContext.Exception != null)
            {
                Logger.Error("Command {0} failed with exception {1}", LogHelper.GetSql(command), interceptionContext.Exception);
            }
            else if (timer.ElapsedMilliseconds >= executionTime)
            {
                Logger.Info("Sql execution time is {0} ms: {1}", timer.ElapsedMilliseconds, LogHelper.GetSql(command));
            }

            Logger.Debug("Sql:{0}", LogHelper.GetSql(command));

        }
    }

    /// <summary>
    /// 事务监视器 用于记录事务回滚信息
    /// </summary>
    public class EFLogTransactionInterceptor : IDbTransactionInterceptor
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void ConnectionGetting(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {

        }

        public void ConnectionGot(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {

        }

        public void Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void IsolationLevelGetting(DbTransaction transaction, DbTransactionInterceptionContext<System.Data.IsolationLevel> interceptionContext)
        {

        }

        public void IsolationLevelGot(DbTransaction transaction, DbTransactionInterceptionContext<System.Data.IsolationLevel> interceptionContext)
        {

        }

        public void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                Logger.Error<Exception>("Transaction failed with exception {0}", interceptionContext.Exception);
            }
        }

        public void RollingBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }
    }

    public class LogHelper
    {
        /// <summary>
        /// 获取执行带参数的sql、存储过程时的完整sql
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GetSql(DbCommand command)
        {
            string strSql = command.CommandText;
            var parameters = command.Parameters;
            for (int i = 0; i < parameters.Count; i++)
            {
                var p = parameters[i];
                strSql += i == 0 ? " " : ", ";

                strSql += p.ParameterName + "=" + p.Value.ToString();
            }
            return strSql;
        }
    }
}