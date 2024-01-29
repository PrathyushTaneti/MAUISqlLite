using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUISqlLite.Data;
using MAUISqlLite.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUISqlLite.ViewModels
{
    public partial class ProductsViewModel(DbContext dbContext) : ObservableObject
    {
        private readonly DbContext dbContext = dbContext;

        [ObservableProperty]
        private ObservableCollection<Product> products;

        [ObservableProperty]
        private Product _operatingProduct = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyText;

        // getting all the products
        public async Task LoadProductsAsync()
        {
            await ExecuteAsync(async() =>
            {
                var products = await this.dbContext.GetAllAsync<Product>();
                if (products is not null && products.Any())
                {
                    Products ??= new ObservableCollection<Product>();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                }
            }, "Fetching Products");
        }

        // setting the product details for edit / update into the controls
        [RelayCommand]
        private void SetOperatingProduct(Product? product) => OperatingProduct = product ?? new();

        // for update / insert new product
        [RelayCommand]
        private async Task UpsertProductAsync()
        {
            if (OperatingProduct is null) return;
            string busyText = OperatingProduct.Id == 0 ? "Creating Product" : "Updating Product";
            await ExecuteAsync( async() =>
            {
                if (OperatingProduct.Id == 0)
                {
                    // Create Product
                    await this.dbContext.AddItemAsync<Product>(OperatingProduct);
                }
                else
                {
                    // Update Product
                    await this.dbContext.UpdateItemAsync<Product>(OperatingProduct);
                    var productCopy = OperatingProduct.Clone();
                    var index = Products.IndexOf(OperatingProduct);
                    Products.RemoveAt(index);
                    Products.Insert(index, productCopy);
                }
                SetOperatingProductCommand.Execute(new());
            }, busyText);
        }

        //delete command
        [RelayCommand]
        private async Task DeleteProductAsync(int id)
        {
            await ExecuteAsync(async () =>
            {
                if (await this.dbContext.DeleteItemByKeyAsync<Product>(id))
                {
                    var products = Products.FirstOrDefault(p => p.Id == id);
                    Products.Remove(products);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Cannot Delete", "Product Cant Be Deleted", "Okay");
                }
            }, "Deleting Product");
        }

        private async Task ExecuteAsync(Func<Task> operation, string? busyText = null)
        {
            IsBusy = true;
            BusyText = busyText ?? "Processing";
            try
            {
                operation?.Invoke();
            }
            finally
            {
                IsBusy = false;
                BusyText = "Processing"; 
            }
        }
    }
}
