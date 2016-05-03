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
using System.Timers;
using System.Net.Sockets;
using System.Net;

namespace MyWebApi.Models
{
    /// <summary>
    /// EF数据库配置
    /// </summary>
    public class EFDbContextConfiguration : DbConfiguration
    {
        public EFDbContextConfiguration()
        {
            DbInterception.Add(new SQLProfiler(50));//配置EF的sql性能监视器
            DbInterception.Add(new EFLogTransactionInterceptor());//配置EF的事务监视器
            DbInterception.Add(new DbMasterSlaveCommandInterceptor());//配置读写分离监视器
        }
    }
}