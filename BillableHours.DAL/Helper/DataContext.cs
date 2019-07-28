using BillableHours.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillableHours.DAL.Helper
{
    public class DataContext : DbContext
    {
      
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
    }
}
