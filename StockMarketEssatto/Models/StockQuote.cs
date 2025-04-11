using System.ComponentModel.DataAnnotations;

namespace StockMarketEssatto.Models
{
	public class StockQuote
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(10)]
		public string TickerSymbol { get; set; }

		[Required]
		public string CompanyName { get; set; }

		public DateTime FetchDate { get; set; }

		[Required]
		public decimal Price { get; set; }

		public bool IsActiveTrading { get; set; }

		public DateTime LastUpdated { get; set; }
	}
}
