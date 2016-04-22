using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace MyWebApi.Models
{
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
}