using Tazmania.Mobile.Models;
using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class IrrigationView : ContentPage
{
	public IrrigationView(IrrigationViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

        #if ANDROID
		OptionsViews.SizeChanged += OnSizeChanged;
		_itemsLayout = (GridItemsLayout)OptionsViews.ItemsLayout;

        IrrigationViews.SizeChanged += OnSizeChangedIrrigation;
		_irrigationItemsLayout = (GridItemsLayout)IrrigationViews.ItemsLayout;
        #endif
    }

    #if ANDROID

    private const int ItemWidth = 240;
    private const int ItemSpacing = 2;
    private const int MaxSpan = 5;
    private const int MinSpan = 2;
    private readonly GridItemsLayout _itemsLayout;

    private const int IrrigationItemWidth = 350;
    private const int IrrigationItemSpacing = 2;
    private const int IrrigationMaxSpan = 3;
    private const int IrrigationMinSpan = 1;
    private readonly GridItemsLayout _irrigationItemsLayout;

    private async void OnSizeChanged(object sender, EventArgs e)
    {
        var bounds = OptionsViews.Bounds;
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

    
    private async void OnSizeChangedIrrigation(object sender, EventArgs e)
    {
        var bounds = IrrigationViews.Bounds;
        var span = (int)(1 + (bounds.Width - IrrigationItemWidth) / (IrrigationItemWidth + IrrigationItemSpacing));
        if (span < IrrigationMinSpan)
            span = IrrigationMinSpan;
        else if (span > IrrigationMaxSpan)
            span = IrrigationMaxSpan;

        if (_irrigationItemsLayout.Span != span)
        {
            await Task.Delay(100);
            _irrigationItemsLayout.Span = span;
        }
    }

    #endif

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        // senza questo evento (anche se vuoto) la proprietà IsLoaded non viene valorizzata!!
    }

    private void StartWatering_Clicked(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            ((IrrigationViewModel)BindingContext).StartWatering(((IrrigationModel)((ImageButton)sender).BindingContext));
        }
    }

    private void StopWatering_Clicked(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            ((IrrigationViewModel)BindingContext).StopWatering(((IrrigationModel)((ImageButton)sender).BindingContext));
        }
    }

    private void MinutesPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            var model = (IrrigationModel)((Picker)sender).BindingContext;
            int minutes;
            switch (((Picker)sender).SelectedIndex)
            {
                case 0: minutes = 5; break;
                case 1: minutes = 7; break;
                case 2: minutes = 10; break;
                case 3: minutes = 15; break;
                default: throw new ArgumentOutOfRangeException();
            }

            ((IrrigationViewModel)BindingContext).SetTimer(model, minutes);
        }
    }

    private void OptionToggle_Clicked(object sender, EventArgs e)
    {
        if (IsLoaded)
        {
            ((IrrigationViewModel)BindingContext).ToggleIsActive(((IO)((Button)sender).BindingContext));
        }
    }
}