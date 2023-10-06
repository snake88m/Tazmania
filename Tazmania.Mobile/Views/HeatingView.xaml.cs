using Tazmania.Mobile.Models;
using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class HeatingView : ContentPage
{
	public HeatingView(HeatingViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;

        #if ANDROID
        HeatingViews.SizeChanged += OnSizeChangedHeating;
		_heatingItemsLayout = (GridItemsLayout)HeatingViews.ItemsLayout;

        OptionsViews.SizeChanged += OnSizeChangedOption;
		_optionItemsLayout = (GridItemsLayout)OptionsViews.ItemsLayout;
        #endif
    }

#if ANDROID

    private const int HeatingItemWidth = 240;
    private const int HeatingItemSpacing = 2;
    private const int HeatingMaxSpan = 5;
    private const int HeatingMinSpan = 2;
    private readonly GridItemsLayout _heatingItemsLayout;

    private const int OptionItemWidth = 240;
    private const int OptionItemSpacing = 2;
    private const int OptionMaxSpan = 5;
    private const int OptionMinSpan = 2;
    private readonly GridItemsLayout _optionItemsLayout;
    
    private async void OnSizeChangedHeating(object sender, EventArgs e)
    {
        var bounds = HeatingViews.Bounds;
        var span = (int)(1 + (bounds.Width - HeatingItemWidth) / (HeatingItemWidth + HeatingItemSpacing));
        if (span < HeatingMinSpan)
            span = HeatingMinSpan;
        else if (span > HeatingMaxSpan)
            span = HeatingMaxSpan;

        if (_heatingItemsLayout.Span != span)
        {
            await Task.Delay(100);
            _heatingItemsLayout.Span = span;
        }
    }

    private async void OnSizeChangedOption(object sender, EventArgs e)
    {
        var bounds = OptionsViews.Bounds;
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

    private void TemperaturesPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            var model = (HeatingModel)((Picker)sender).BindingContext;
            float temperature;
            switch (((Picker)sender).SelectedIndex)
            {
                case 0: temperature = 18; break;
                case 1: temperature = 18.5f; break;
                case 2: temperature = 19; break;
                case 3: temperature = 19.5f; break;
                case 4: temperature = 20; break;
                case 5: temperature = 20.5f; break;
                case 6: temperature = 21; break;
                case 7: temperature = 21.5f; break;
                case 8: temperature = 22; break;
                case 9: temperature = 22.5f; break;
                case 10: temperature = 23; break;
                default: throw new ArgumentOutOfRangeException();
            }

           ((HeatingViewModel)BindingContext).SetTargetTemperature(model, temperature);
        }
    }

    private void OptionToggle_Clicked(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            ((HeatingViewModel)BindingContext).ToggleIsActive(((IO)((Button)sender).BindingContext));
        }
    }
}