using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class SettingView : ContentPage
{
	public SettingView(SettingViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}