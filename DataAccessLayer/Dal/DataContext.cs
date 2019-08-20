using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace DataAccessLayer
{
    public class DataContext: DbContext
    {
        public DataContext() : base("DataContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

        }

        public virtual DbSet<Postion> Postions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<CalcForum> CalcForums  { get; set; }
        public virtual DbSet<ButtonsStatic> ButtonsStatics { get; set; }
        public virtual DbSet<CalculatedSalaryByUser> CalculatedSalaryByUsers { get; set; }
        public virtual DbSet<SaleImport> SaleImports { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<LogUser> LogUsers { get; set; }
        public virtual DbSet<LogSale> LogSales { get; set; }
        public virtual DbSet<LogCalcForum> LogCalcForums { get; set; }
        public virtual DbSet<LogCalcSalary> LogCalcSalaries { get; set; }
    }

}