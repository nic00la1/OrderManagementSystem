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

        private static int nextProductId = 1;

        private void AddProduct_Clicked(object sender, EventArgs e)
        {
            int id = nextProductId; // Use the next product ID
            string name = ProductNameEntry.Text;
            decimal price = Convert.ToDecimal(ProductPriceEntry.Text);
            string? category = KategoriaPicker.SelectedItem.ToString();

            Product newProduct = new Product(id, name, price, category);
            products.Add(newProduct);

            nextProductId++; // Increment the counter after adding the product


            DisplayProducts();
        }

        private void DisplayProducts()
        {
            // Create a new list of products with updated IDs
            List<Product> updatedProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                updatedProducts.Add(new Product(i + 1, product.Name, product.Price, product.Category));
            }

            // Clear the ProductsList and add the updated products
            ProductsList.Clear();
            foreach (var product in updatedProducts)
            {
                ProductsList.Add(product);
            }

            // Replace the old products list with the updated one
            products = updatedProducts;
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
            // Get the selected product
            Product selectedProduct = (Product)((Button)sender).BindingContext;

            // Remove the selected product from the products list
            products.Remove(selectedProduct);

            DisplayProducts();
        }

        private void DisplayProductsByCategory(string category)
        {
            // Create a new list of products filtered by category
            List<Product> filteredProducts = products.Where(p => p.Category == category).ToList();

            // Clear the ProductsList
            ProductsList.Clear();

            if (filteredProducts.Count == 0)
            {
                // If there are no products in the selected category, display a message
                ifNoProductInCategory_Label.IsVisible = true;
                ifNoProductInCategory_Label.Text = "Brak produktow w kategorii: " + category + "!";

                ifNoProductInCategory_Frame.IsVisible = true;
            }
            else
            {
                // If there are products in the selected category, add them to the ProductsList
                foreach (var product in filteredProducts)
                {
                    ProductsList.Add(product);

                    ifNoProductInCategory_Frame.IsVisible = false;
                    ifNoProductInCategory_Label.IsVisible = false;
                }
            }
        }

        private void KategoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? category = WyswietlKategoriaPicker.SelectedItem.ToString();
            if (category == "All")
            {
                DisplayProducts();

                // Hide the message frame and label
                ifNoProductInCategory_Frame.IsVisible = false;
                ifNoProductInCategory_Label.IsVisible = false;
            }
            else
            {
                DisplayProductsByCategory(category);
            }
        }
    }

}
