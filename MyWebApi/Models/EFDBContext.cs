using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MyWebApi.Models
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("writeDbConnection")
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //通过反射一次性将表进行映射 FluentAPI
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

            //用于在DbContext中配置model的非公有属性映射到数据库表中
            modelBuilder.Types().Configure(d =>
            {
                var nonPublicProperties = d.ClrType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var p in nonPublicProperties)
                {
                    d.Property(p).HasColumnName(p.Name);
                }
            });
        }
    }
}