using Microsoft.EntityFrameworkCore;
using StockMarketEssatto.Models;
using System.IO;

namespace StockMarketEssatto.Data
{
	public class StockDbContext : DbContext
	{
		public DbSet<StockQuote> StockQuotes { get; set; }

		public string DbPath { get; }

		public StockDbContext()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			// PATH: C:\Users\TWOJA_NAZWA\AppData\Local\stockdata.db
			DbPath = Path.Join(path, "stockdata.db");
			Console.WriteLine($"Database path: {DbPath}");
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={DbPath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<StockQuote>()
				.HasIndex(sq => sq.TickerSymbol)
				.IsUnique();
		}

		public override int SaveChanges()
		{
			AddTimestamps();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			AddTimestamps();
			return base.SaveChangesAsync(cancellationToken);
		}

		private void AddTimestamps()
		{
			var entries = ChangeTracker
				.Entries()
				.Where(e => e.Entity is StockQuote && (
						e.State == EntityState.Added ||
						e.State == EntityState.Modified));

			foreach (var entityEntry in entries)
			{
				((StockQuote)entityEntry.Entity).LastUpdated = DateTime.UtcNow;
			}
		}
	}
}
