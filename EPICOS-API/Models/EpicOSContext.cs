using EPICOS_API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EPICOS_API.Models
{
    public partial class EpicOSContext : DbContext
    {
        public EpicOSContext()
        {
  
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Workpoint> Workpoint { get; set; }
        public virtual DbSet<Hub> Hub { get; set; }
        public virtual DbSet<Floor> Floor { get; set; }
        public virtual DbSet<Telemery> Telemery { get; set; }
        public virtual DbSet<Log> Log { get; set; }

        public EpicOSContext(DbContextOptions<EpicOSContext> options)
            : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DBEpicOS");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}