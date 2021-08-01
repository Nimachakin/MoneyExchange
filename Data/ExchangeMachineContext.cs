using ExchangeApi.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace ExchangeApi.Data
{
    public class ExchangeMachineContext : DbContext
    {
        public DbSet<ExchangeLog> ExchangeLogs { get; set; }

        public ExchangeMachineContext(DbContextOptions<ExchangeMachineContext> options) :base(options)
        {
            Database.EnsureCreated();
        }
    }
}