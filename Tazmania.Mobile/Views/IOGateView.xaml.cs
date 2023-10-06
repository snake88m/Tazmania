using Tazmania.Mobile.ViewModels;

namespace Tazmania.Mobile.Views;

public partial class IOGateView : ContentPage
{
	public IOGateView(IOGateViewModel viewModel)
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
}