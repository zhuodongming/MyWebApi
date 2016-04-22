using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Transactions;
using System.Web;

namespace MyWebApi.Models
{
    /// <summary>
    /// SQL命令拦截器
    /// 实现EF的读写分离 及 对数据库服务器的异常心跳检测
    /// </summary>
    public class DbMasterSlaveCommandInterceptor : DbCommandInterceptor
    {
        private static IList<DistributedReadWriteSection> readConnList;//读库，从库集群，写库不用设置走默认的EF框架
        private static Timer sysTimer = new Timer(5000);//定时器 定期查找没有在线的数据库服务器

        public DbMasterSlaveCommandInterceptor()
        {
            readConnList = DistributedReadWriteManager.Instance;

            sysTimer.Enabled = true;
            sysTimer.Elapsed += sysTimer_Elapsed;
            sysTimer.Start();//启动定时器
        }

        private static void sysTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (readConnList != null && readConnList.Any())
            {
                foreach (var item in readConnList)
                {
                    //心跳测试，将死掉的服务器IP从列表中移除
                    var client = new TcpClient();
                    try
                    {
                        client.Connect(new IPEndPoint(IPAddress.Parse(item.Ip), item.Port));
                        if (!client.Connected)
                        {
                            readConnList.Remove(item);//没有连接上
                        }
                    }
                    catch (SocketException)
                    {
                        readConnList.Remove(item);//异常，没有连接上
                    }
                    finally
                    {
                        client.Dispose();
                    }

                }
            }
        }

        /// <summary>
        /// 处理读库字符串
        /// </summary>
        /// <returns></returns>
        private string GetReadConn()
        {
            if (readConnList != null && readConnList.Any())
            {
                var resultConn = readConnList[Convert.ToInt32(Math.Floor((double)new Random().Next(0, readConnList.Count)))];
                return string.Format(System.Configuration.ConfigurationManager.AppSettings["readDbConnection"]
                    , resultConn.Ip
                    , resultConn.DbName
                    , resultConn.UserId
                    , resultConn.Password);
            }
            return String.Empty;
        }

        /// <summary>
        /// 只读库的选择,加工command对象
        /// 说明：事务中,所有语句都走主库，事务外select走读库,insert,update,delete走主库
        /// 希望：一个ＷＥＢ请求中，读与写的仓储使用一个，不需要在程序中去重新定义
        /// </summary>
        /// <param name="command"></param>
        private void ReadDbSelect(DbCommand command)
        {
            // 判断当前会话是否处于分布式事务中
            bool isDistributedTran = Transaction.Current != null && Transaction.Current.TransactionInformation.Status != TransactionStatus.Committed;

            // 判断当前DbCommand 是否处于普通数据库事务中
            bool isDbTran = command.Transaction != null;

            // 如果处于分布式事务或普通事务中，则“禁用”读写分离，处于事务中的所有读写操作都指向 Master
            if (!(isDistributedTran || isDbTran))
            {
                if (!command.CommandText.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase))
                {
                    string connString = GetReadConn();
                    if (!string.IsNullOrWhiteSpace(connString))
                    {
                        if (command.Connection.State == ConnectionState.Open)
                        {
                            command.Connection.Close();
                            command.Connection.ConnectionString = connString;
                            command.Connection.Open();
                        }
                        else
                        {
                            command.Connection.ConnectionString = connString;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Linq to Entity生成的select,insert
        /// warning:在select语句中DbCommand.Transaction为null，而ef会为每个insert添加一个DbCommand.Transaction进行包裹
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ReadDbSelect(command);
            base.ReaderExecuted(command, interceptionContext);
        }

        /// <summary>
        /// 执行sql语句，并返回第一行第一列，没有找到返回null,如果数据库中值为null，则返回 DBNull.Value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ReadDbSelect(command);
            base.ScalarExecuting(command, interceptionContext);
        }

        /// <summary>
        /// Linq to Entity生成的update,delete
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            base.NonQueryExecuting(command, interceptionContext);//update,delete等写操作直接走主库
        }
    }
}