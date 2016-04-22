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
        public EFDbContext() : base("masterConnectionString")
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());//通过反射一次性将表进行映射 FluentAPI
        }
    }
}