using Microsoft.EntityFrameworkCore;
using powerLabel.Models;

namespace powerLabel
{
    public class ComputerSystemContext : DbContext
	{
		static SettingsHandler.Settings settings = SettingsHandler.ReadSettings();

		string ip = settings.databaseIP;
		string database = settings.databaseName;
		string user = settings.databaseUser;
		string password = settings.databasePass;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseMySql($"server={ip};database={database};user={user};password={password};", new MySqlServerVersion(new System.Version(8, 0)));

			// Connection strings for updating the database
            //optionsBuilder.UseMySql($"server=localhost;database=powerlabeldb;user=root;password=;");
        }

		public DbSet<Event> Events { get; set; }
		public DbSet<ComputerSystem> ComputerSystems { get; set; }
		public DbSet<Motherboard> Motherboards { get; set; }
		public DbSet<Bios> Bioses { get; set; }
		public DbSet<Processor> Processors { get; set; }
		public DbSet<MemoryModule> MemoryModules { get; set; }
		public DbSet<MemoryConfig> MemoryConfigs { get; set; }
		public DbSet<Disk> Disks { get; set; }
		public DbSet<DiskConfig> DiskConfigs { get; set; }
		public DbSet<VideoController> VideoControllers { get; set; }
		public DbSet<VideoControllerConfig> VideoControllerConfigs { get; set; }
		public DbSet<OS> OSes { get; set; }
	}
}
