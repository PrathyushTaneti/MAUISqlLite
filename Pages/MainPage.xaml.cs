using MAUISqlLite.ViewModels;

namespace MAUISqlLite.Pages;

public partial class MainPage : ContentPage
{
    private readonly ProductsViewModel productsViewModel;

    public MainPage(ProductsViewModel productsViewModel)
    {
        InitializeComponent();
        this.BindingContext = productsViewModel;
        this.productsViewModel = productsViewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await this.productsViewModel.LoadProductsAsync();
    }
}
