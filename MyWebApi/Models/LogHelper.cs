using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using log4net;

namespace MyWebApi.Models
{
    public class LogHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(LogHelper));

        public static void Debug(object message, Exception exception = null)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message, exception);
            }
        }

        public static void Info(object message, Exception exception = null)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message, exception);
            }
        }

        public static void Warn(object message, Exception exception = null)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, exception);
            }
        }

        public static void Error(object message, Exception exception = null)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message, exception);
            }
        }

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