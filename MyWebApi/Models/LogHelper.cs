using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MyWebApi.Models
{
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