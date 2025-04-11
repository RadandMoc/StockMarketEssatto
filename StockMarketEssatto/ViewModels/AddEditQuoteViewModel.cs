using CommunityToolkit.Mvvm.ComponentModel;
using StockMarketEssatto.Models;
using System.ComponentModel.DataAnnotations;

namespace StockMarketEssatto.ViewModels
{
	public partial class AddEditQuoteViewModel : ObservableValidator
	{

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Ticker symbol is required.")]
		[MaxLength(10, ErrorMessage = "Ticker symbol cannot exceed 10 characters.")]
		[RegularExpression("^[a-zA-Z0-9.-]+$", ErrorMessage = "Ticker can only contain letters, numbers, dots, and hyphens.")]
		private string _tickerSymbol;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Company name is required.")]
		[MaxLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
		private string _companyName;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Price is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
		private decimal _price;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[Required(ErrorMessage = "Fetch date is required.")]
		private DateTime _fetchDate = DateTime.Today;

		[ObservableProperty]
		private bool _isActiveTrading;

		public bool IsValid => !HasErrors;

		public AddEditQuoteViewModel()
		{
			ValidateAllProperties();
		}

		public void ValidateAllPropertiesPublic()
		{
			ValidateAllProperties();
			OnPropertyChanged(nameof(IsValid));
		}

		public AddEditQuoteViewModel(StockQuote quoteToEdit)
		{
			if (quoteToEdit == null)
				throw new ArgumentNullException(nameof(quoteToEdit));

			TickerSymbol = quoteToEdit.TickerSymbol;
			CompanyName = quoteToEdit.CompanyName;
			Price = quoteToEdit.Price;
			FetchDate = quoteToEdit.FetchDate;
			IsActiveTrading = quoteToEdit.IsActiveTrading;

			ClearErrors();
			ValidateAllProperties();
		}

		public void ApplyTo(StockQuote quote)
		{
			if (quote == null) 
				return;

			quote.TickerSymbol = this.TickerSymbol;
			quote.CompanyName = this.CompanyName;
			quote.Price = this.Price;
			quote.FetchDate = this.FetchDate;
			quote.IsActiveTrading = this.IsActiveTrading;
		}

		partial void OnTickerSymbolChanged(string value)
		{
			ValidateProperty(value, nameof(TickerSymbol));
			OnPropertyChanged(nameof(IsValid));
		}

		partial void OnCompanyNameChanged(string value)
		{
			ValidateProperty(value, nameof(CompanyName));
			OnPropertyChanged(nameof(IsValid));
		}

		partial void OnPriceChanged(decimal value)
		{
			ValidateProperty(value, nameof(Price));
			OnPropertyChanged(nameof(IsValid));
		}

		partial void OnFetchDateChanged(DateTime value)
		{
			ValidateProperty(value, nameof(FetchDate));
			OnPropertyChanged(nameof(IsValid));
		}
	}
}