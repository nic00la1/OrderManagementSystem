using OrderManagementSystem.MVVM.Models;
using System.Collections.ObjectModel;

namespace OrderManagementSystem
{
    public partial class MainPage : ContentPage
    {
        private List<Product> products = new List<Product>();
        public ObservableCollection<Product> ProductsList { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ProductsList = new ObservableCollection<Product>();
            BindingContext = this;
        }


        private void AddProduct_Clicked(object sender, EventArgs e)
        {
            int id = products.Count + 1; // Increment the count to ensure uniqueness
            string name = ProductNameEntry.Text;
            decimal price = Convert.ToDecimal(ProductPriceEntry.Text);
            string? category = KategoriaPicker.SelectedItem.ToString();

            Product newProduct = new Product(id, name, price, category);
            products.Add(newProduct);

            DisplayProducts();
        }

        private void DisplayProducts()
        {
            ProductsList.Clear();
            foreach (var product in products)
            {
                ProductsList.Add(product);
            }
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            // Get the selected product
            Product selectedProduct = (Product)((Button)sender).BindingContext;

            // Show an input dialog to edit the product details
            int id = selectedProduct.Id; // Keep the same id as the original product
            string name = await DisplayPromptAsync("Change Name", "Enter the new name", initialValue: selectedProduct.Name);
            decimal price = Convert.ToDecimal(await DisplayPromptAsync("Change Price", "Enter the new price", initialValue: selectedProduct.Price.ToString()));

            // Get the list of categories from the XAML file
            List<string> categories = new List<string>();
            foreach (var item in KategoriaPicker.Items)
            {
                categories.Add(item.ToString());
            }

            // Show a picker dialog to select the new category
            string category = await DisplayActionSheet("Change Category", "Cancel", null, categories.ToArray());

            // Update the product details
            selectedProduct.Name = name;
            selectedProduct.Price = price;
            selectedProduct.Category = category;

            // Create a new product object
            Product newProduct = new Product(id, name, price, category);

            // Replace the edited product in the products list
            int index = products.IndexOf(selectedProduct);
            products[index] = newProduct;

            DisplayProducts();
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {

        }
    }

}
