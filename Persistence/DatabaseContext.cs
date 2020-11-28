using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using System;

namespace Persistence
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }

        public DbSet<CardInquiryLog> Transactions { get; set; }
       
    }
}
