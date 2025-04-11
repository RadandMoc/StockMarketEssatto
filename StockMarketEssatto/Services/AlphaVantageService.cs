using Microsoft.Extensions.Configuration;
using StockMarketEssatto.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace StockMarketEssatto.Services
{
	public class AlphaVantageService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;

		private const string BaseUrl = "https://www.alphavantage.co/query";

		public AlphaVantageService(IConfiguration configuration)
		{
			_httpClient = new HttpClient();
			_apiKey = configuration["AlphaVantage:ApiKey"];
			if (string.IsNullOrWhiteSpace(_apiKey))
				throw new InvalidOperationException("API Key for Alpha Vantage is missing or not configured correctly in User Secrets (AlphaVantage:ApiKey).");
		}

		public async Task<StockQuote> GetQuoteAsync(string symbol)
		{
			if (string.IsNullOrWhiteSpace(symbol))
				throw new ArgumentNullException(nameof(symbol));
			
			var requestUrl = $"{BaseUrl}?function=GLOBAL_QUOTE&symbol={symbol.ToUpper()}&apikey={_apiKey}";

			try
			{
				var response = await _httpClient.GetAsync(requestUrl);
				response.EnsureSuccessStatusCode();

				var apiResponse = await response.Content.ReadFromJsonAsync<AlphaVantageQuoteResponse>();

				if (apiResponse?.GlobalQuote == null || string.IsNullOrWhiteSpace(apiResponse.GlobalQuote.Symbol))
				{
					if (apiResponse?.Note != null && apiResponse.Note.Contains("API key"))
						throw new Exception($"Alpha Vantage API error related to the API key: {apiResponse.Note}");
					
					if (apiResponse?.Note != null)
						throw new Exception($"Alpha Vantage API error: {apiResponse.Note}");
					
					throw new Exception($"Could not retrieve quote for symbol '{symbol}'. Response might be empty or invalid.");
				}

				var quoteData = apiResponse.GlobalQuote;
				var stockQuote = new StockQuote
				{
					TickerSymbol = quoteData.Symbol,
					CompanyName = $"Company for {quoteData.Symbol}",
					Price = quoteData.Price,
					FetchDate = quoteData.LatestTradingDay != DateTime.MinValue ? quoteData.LatestTradingDay : DateTime.UtcNow,
					IsActiveTrading = true,
					LastUpdated = DateTime.UtcNow
				};

				return stockQuote;
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine($"Error fetching data from Alpha Vantage: {ex.Message}");
				throw new Exception($"Network error or bad response from Alpha Vantage for symbol '{symbol}'. Check your internet connection and API key.", ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error processing Alpha Vantage response: {ex.Message}");
				throw;
			}
		}
	}
	public class AlphaVantageQuoteResponse
	{
		[JsonPropertyName("Global Quote")]
		public AlphaVantageGlobalQuote GlobalQuote { get; set; }

		[JsonPropertyName("Note")]
		public string Note { get; set; }
	}

	public class AlphaVantageGlobalQuote
	{
		[JsonPropertyName("01. symbol")]
		public string Symbol { get; set; }

		[JsonPropertyName("05. price")]
		public decimal Price { get; set; }

		[JsonPropertyName("07. latest trading day")]
		public DateTime LatestTradingDay { get; set; }

	}
}
