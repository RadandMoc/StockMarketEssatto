using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StockMarketEssatto.Data;
using StockMarketEssatto.Models;
using StockMarketEssatto.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace StockMarketEssatto.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		#region Fields
		private readonly StockDbContext _dbContext;
		private readonly AlphaVantageService _apiService;

		[ObservableProperty]
		private int _totalItems;

		[ObservableProperty]
		private ObservableCollection<StockQuote> _stockQuotes;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(EditCommand))]
		[NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
		private StockQuote _selectedQuote;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
		[NotifyCanExecuteChangedFor(nameof(SearchCommand))]
		private string? _searchText;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(FetchQuoteCommand))]
		private string _symbolToFetch;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(FetchQuoteCommand))]
		[NotifyCanExecuteChangedFor(nameof(EditCommand))]
		[NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
		[NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
		private bool _isLoading;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
		private int _currentPage = 1;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
		[NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
		private int _totalPages = 1;

		[ObservableProperty] 
		private int _pageSize = 10;
		#endregion Fields

		public MainViewModel(StockDbContext dbContext, AlphaVantageService apiService)
		{
			_dbContext = dbContext;
			_apiService = apiService;
			StockQuotes = new ObservableCollection<StockQuote>();
			LoadDataAsync();
		}

		#region DbFunctions
		private async Task LoadDataAsync()
		{
			IsLoading = true;
			try
			{
				var query = _dbContext.StockQuotes.AsQueryable();

				if (!string.IsNullOrWhiteSpace(SearchText))
				{
					string lowerSearchText = SearchText.ToLower();
					query = query.Where(q =>
						q.TickerSymbol.ToLower().Contains(lowerSearchText) ||
						q.CompanyName.ToLower().Contains(lowerSearchText));
				}

				query = query.OrderByDescending(q => q.Id);


				int totalItems = await query.CountAsync();
				TotalItems = totalItems;
				TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
				if (CurrentPage > TotalPages && TotalPages > 0)
					CurrentPage = TotalPages;
				if (CurrentPage < 1)
					CurrentPage = 1;

				var itemsToShow = await query
					.Skip((CurrentPage - 1) * PageSize)
					.Take(PageSize)
					.ToListAsync();

				StockQuotes.Clear();
				foreach (var quote in itemsToShow)
				{
					StockQuotes.Add(quote);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsLoading = false;
			}
		}

		[RelayCommand]
		private async Task RefreshData()
		{
			await LoadDataAsync();
		}

		[RelayCommand(CanExecute = nameof(CanGoNextPage))]
		private async Task NextPage()
		{
			if (CanGoNextPage())
			{
				CurrentPage++;
				await LoadDataAsync();
				NextPageCommand.NotifyCanExecuteChanged();
				PreviousPageCommand.NotifyCanExecuteChanged();
			}
		}
		private bool CanGoNextPage() => CurrentPage < TotalPages && !IsLoading;

		[RelayCommand(CanExecute = nameof(CanGoPreviousPage))]
		private async Task PreviousPage()
		{
			if (CanGoPreviousPage())
			{
				CurrentPage--;
				await LoadDataAsync();
				NextPageCommand.NotifyCanExecuteChanged();
				PreviousPageCommand.NotifyCanExecuteChanged();
			}
		}
		private bool CanGoPreviousPage() => CurrentPage > 1 && !IsLoading;


		[RelayCommand]
		private async Task Search()
		{
			CurrentPage = 1;
			await LoadDataAsync();
			NextPageCommand.NotifyCanExecuteChanged();
			PreviousPageCommand.NotifyCanExecuteChanged();
		}

		[RelayCommand]
		private async Task ExecuteSearch(object parameter)
		{
			if (parameter == null)
			{
				SearchText = string.Empty;
			}
			CurrentPage = 1;
			await LoadDataAsync();
			NextPageCommand.NotifyCanExecuteChanged();
			PreviousPageCommand.NotifyCanExecuteChanged();
		}

		[RelayCommand(CanExecute = nameof(CanFetchQuote))]
		private async Task FetchQuoteAsync()
		{
			IsLoading = true;
			try
			{
				var quote = await _apiService.GetQuoteAsync(SymbolToFetch);

				var existingQuote = await _dbContext.StockQuotes
										.FirstOrDefaultAsync(q => q.TickerSymbol == quote.TickerSymbol);

				if (existingQuote != null)
				{
					existingQuote.Price = quote.Price;
					existingQuote.FetchDate = quote.FetchDate;
					existingQuote.IsActiveTrading = quote.IsActiveTrading;
					_dbContext.StockQuotes.Update(existingQuote);
					MessageBox.Show($"Quote for {quote.TickerSymbol} updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					await _dbContext.StockQuotes.AddAsync(quote);
					MessageBox.Show($"Quote for {quote.TickerSymbol} added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				await _dbContext.SaveChangesAsync();
				SymbolToFetch = string.Empty;
				await LoadDataAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error fetching or saving quote: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsLoading = false;
				NextPageCommand.NotifyCanExecuteChanged();
				PreviousPageCommand.NotifyCanExecuteChanged();
			}
		}
		private bool CanFetchQuote() => !string.IsNullOrWhiteSpace(SymbolToFetch) && !IsLoading;
		#endregion DbFunctions

		#region AddEditDelete
		[RelayCommand]
		private async Task Add()
		{
			var newQuoteVM = new AddEditQuoteViewModel();
			var dialog = new AddEditQuoteWindow(newQuoteVM);

			if (dialog.ShowDialog() == true)
			{
				try
				{
					var newQuote = new StockQuote();
					newQuoteVM.ApplyTo(newQuote);

					await _dbContext.StockQuotes.AddAsync(newQuote);
					await _dbContext.SaveChangesAsync();
					await LoadDataAsync();
					NextPageCommand.NotifyCanExecuteChanged();
					PreviousPageCommand.NotifyCanExecuteChanged();
					MessageBox.Show($"Quote for {newQuote.TickerSymbol} added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (DbUpdateException dbEx)
				{
					var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
					MessageBox.Show($"Database error adding quote: {innerExceptionMessage}\n\nMake sure the Ticker Symbol is unique.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error adding quote: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}


		[RelayCommand(CanExecute = nameof(CanEditOrDelete))]
		private async Task Edit()
		{
			if (SelectedQuote == null) return;

			var editQuoteVM = new AddEditQuoteViewModel(SelectedQuote);
			var dialog = new AddEditQuoteWindow(editQuoteVM);

			if (dialog.ShowDialog() == true)
			{
				try
				{
					editQuoteVM.ApplyTo(SelectedQuote);

					_dbContext.StockQuotes.Update(SelectedQuote);
					await _dbContext.SaveChangesAsync();
					await LoadDataAsync();
					MessageBox.Show($"Quote for updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (DbUpdateException dbEx)
				{
					var innerExceptionMessage = dbEx.InnerException?.Message ?? dbEx.Message;
					MessageBox.Show($"Database error updating quote: {innerExceptionMessage}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error updating quote: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand(CanExecute = nameof(CanEditOrDelete))]
		private async Task Delete()
		{
			var result = MessageBox.Show($"Are you sure you want to delete {SelectedQuote.TickerSymbol}?",
										 "Confirm Delete",
										 MessageBoxButton.YesNo,
										 MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				try
				{
					_dbContext.StockQuotes.Remove(SelectedQuote);
					await _dbContext.SaveChangesAsync();
					SelectedQuote = null;
					await LoadDataAsync();
					NextPageCommand.NotifyCanExecuteChanged();
					PreviousPageCommand.NotifyCanExecuteChanged();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error deleting quote: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private bool CanEditOrDelete() => SelectedQuote != null && !IsLoading;
		#endregion AddEditDelete
	}
}
