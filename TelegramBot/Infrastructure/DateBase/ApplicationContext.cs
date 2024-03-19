using System;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Core;

namespace TelegramBot.Infrastructure.DateBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<MailUser> Users => Set<MailUser>();
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=UsersMail.db");

        }



    }
}
