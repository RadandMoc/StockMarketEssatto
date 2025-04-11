using StockMarketEssatto.ViewModels;
using System.Windows;

namespace StockMarketEssatto
{
	/// <summary>
	/// Interaction logic for AddEditQuoteWindow.xaml
	/// </summary>
	public partial class AddEditQuoteWindow : Window
	{
		public AddEditQuoteViewModel ViewModel { get; private set; }

		public AddEditQuoteWindow(AddEditQuoteViewModel viewModel)
		{
			InitializeComponent();
			ViewModel = viewModel;
			DataContext = ViewModel;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.ValidateAllPropertiesPublic();
			if (!ViewModel.IsValid)
			{
				MessageBox.Show("Please correct the validation errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}
