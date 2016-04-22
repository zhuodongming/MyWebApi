using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MyWebApi.Models
{
    /// <summary>
    /// DistributedReadWriteForEFSection块，在web.config中提供DistributedReadWriteForEFSection块定义
    /// </summary>
    public class DistributedReadWriteSection : ConfigurationSection
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        [ConfigurationProperty("Ip", DefaultValue = "127.0.0.1")]
        public string Ip { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        [ConfigurationProperty("Port", DefaultValue = "1433")]
        public int Port { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        [ConfigurationProperty("DbName", DefaultValue = "Test")]
        public string DbName { get; set; }

        /// <summary>
        /// 数据库账号
        /// </summary>
        [ConfigurationProperty("UserId", DefaultValue = "sa")]
        public string UserId { get; set; }

        /// <summary>
        /// 数据库密码
        /// </summary>
        [ConfigurationProperty("Password", DefaultValue = "sa")]
        public string Password { get; set; }

    }
}