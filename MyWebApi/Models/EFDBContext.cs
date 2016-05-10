using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
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

            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            //.Where(type => !String.IsNullOrEmpty(type.Namespace))
            //.Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
            //    type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            //foreach (var type in typesToRegister)
            //{
            //    dynamic configurationInstance = Activator.CreateInstance(type);
            //    modelBuilder.Configurations.Add(configurationInstance);
            //}

            //用于在DbContext中配置model的非公有属性映射到数据库表中
            modelBuilder.Types().Configure(d =>
            {
                var nonPublicProperties = d.ClrType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var p in nonPublicProperties)
                {
                    d.Property(p).HasColumnName(p.Name);
                }
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}