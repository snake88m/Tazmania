using Microsoft.Maui.Controls;
using Tazmania.Mobile.Models;
using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class IOView : ContentPage
{

	public IOView(IOViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        #if ANDROID
        CollectionViewIOs.SizeChanged += OnSizeChanged;
        _itemsLayout = (GridItemsLayout)CollectionViewIOs.ItemsLayout;
        #endif
    }

    #if ANDROID

    private const int ItemWidth = 240;
    private const int ItemSpacing = 2;
    private const int MaxSpan = 5;
    private const int MinSpan = 2;
    private readonly GridItemsLayout _itemsLayout;

    private async void OnSizeChanged(object sender, EventArgs e)
    {
        var bounds = CollectionViewIOs.Bounds;
        var span = (int)(1 + (bounds.Width - ItemWidth) / (ItemWidth + ItemSpacing));
        if (span < MinSpan)
            span = MinSpan;
        else if (span > MaxSpan)
            span = MaxSpan;

        if (_itemsLayout.Span != span)
        {
            await Task.Delay(100);
            _itemsLayout.Span = span;
        }
    }

    #endif

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        // senza questo evento (anche se vuoto) la proprietà IsLoaded non viene valorizzata!!
    }

    private void Toggle_Clicked(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            ((IOViewModel)BindingContext).ToggleIsActive(((IO)((Button)sender).BindingContext));
        }
    }

    private void ShutterUp_Clicked(object sender, EventArgs e)
    {
        ((IOViewModel)BindingContext).ShutterUp(((IO)((ImageButton)sender).BindingContext));
    }

    private void ShutterDown_Clicked(object sender, EventArgs e)
    {
        ((IOViewModel)BindingContext).ShutterDown(((IO)((ImageButton)sender).BindingContext));
    }

    private void ShutterStop_Clicked(object sender, EventArgs e)
    {
        ((IOViewModel)BindingContext).ShutterStop(((IO)((ImageButton)sender).BindingContext));
    }
}