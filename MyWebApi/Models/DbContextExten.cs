using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyWebApi.Models
{
    /// <summary>
    /// DbContext添加扩展方法
    /// </summary>
    public static class DbContextExten
    {
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<IList<TEntity>> ExecuteStoredProcedureListAsync<TEntity>(this DbContext context, string commandText, params object[] parameters) where TEntity : class
        {
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {

                        commandText += " output";
                    }
                }
            }

            var result = await context.Database.SqlQuery<TEntity>(commandText, parameters).ToListAsync();

            bool acd = context.Configuration.AutoDetectChangesEnabled;
            try
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = context.Set<TEntity>().Attach(result[i]);
            }
            finally
            {
                context.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }
    }
}