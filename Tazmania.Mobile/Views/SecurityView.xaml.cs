using Tazmania.Mobile.Models;
using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class SecurityView : ContentPage
{
	public SecurityView(SecurityViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

        #if ANDROID
        AntitheftViews.SizeChanged += OnSizeChangedAntitheft;
        _antitheftItemsLayout = (GridItemsLayout)AntitheftViews.ItemsLayout;

        OptionViews.SizeChanged += OnSizeChangedOption;
        _optionItemsLayout = (GridItemsLayout)OptionViews.ItemsLayout;
        #endif
    }

    #if ANDROID

    private const int AntitheftItemWidth = 240;
    private const int AntitheftItemSpacing = 2;
    private const int AntitheftMaxSpan = 5;
    private const int AntitheftMinSpan = 2;
    private readonly GridItemsLayout _antitheftItemsLayout;

    private const int OptionItemWidth = 240;
    private const int OptionItemSpacing = 2;
    private const int OptionMaxSpan = 5;
    private const int OptionMinSpan = 2;
    private readonly GridItemsLayout _optionItemsLayout;

    private async void OnSizeChangedAntitheft(object sender, EventArgs e)
    {
        var bounds = AntitheftViews.Bounds;
        var span = (int)(1 + (bounds.Width - AntitheftItemWidth) / (AntitheftItemWidth + AntitheftItemSpacing));
        if (span < AntitheftMinSpan)
            span = AntitheftMinSpan;
        else if (span > AntitheftMaxSpan)
            span = AntitheftMaxSpan;

        if (_antitheftItemsLayout.Span != span)
        {
            await Task.Delay(100);
            _antitheftItemsLayout.Span = span;
        }
    }

    private async void OnSizeChangedOption(object sender, EventArgs e)
    {
        var bounds = OptionViews.Bounds;
        var span = (int)(1 + (bounds.Width - OptionItemWidth) / (OptionItemWidth + OptionItemSpacing));
        if (span < OptionMinSpan)
            span = OptionMinSpan;
        else if (span > OptionMaxSpan)
            span = OptionMaxSpan;

        if (_optionItemsLayout.Span != span)
        {
            await Task.Delay(100);
            _optionItemsLayout.Span = span;
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
            ((SecurityViewModel)BindingContext).ToggleIsActive(((IO)((Button)sender).BindingContext));
        }
    }
}