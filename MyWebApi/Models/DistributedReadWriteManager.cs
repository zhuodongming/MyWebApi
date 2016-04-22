using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MyWebApi.Models
{
    /// <summary>
    /// redis配置信息加载
    /// </summary>
    public class DistributedReadWriteManager
    {
        /// <summary>
        /// 配置实体信息
        /// </summary>
        public static IList<DistributedReadWriteSection> Instance
        {
            get
            {
                return GetSection();
            }
        }

        private static IList<DistributedReadWriteSection> GetSection()
        {
            var dic = ConfigurationManager.GetSection("DistributedReadWriteSection") as Dictionary<string, DistributedReadWriteSection>;
            return dic.Values.ToList();
        }
    }
}