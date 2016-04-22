using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MyWebApi.Models
{
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
}