using OrderManagementSystem.MVVM.Models;

namespace OrderManagementSystem
{
    public partial class MainPage : ContentPage
    {
        private List<Product> products = new List<Product>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void AddProduct_Clicked(object sender, EventArgs e)
        {
            string name = ProductNameEntry.Text;
            decimal price = Convert.ToDecimal(ProductPriceEntry.Text);
            string? category = KategoriaPicker.SelectedItem.ToString();

            Product newProduct = new Product(name, price, category);
            products.Add(newProduct);

            DisplayProducts();
        }

        private void DisplayProducts()
        {
            ProductsStackLayout.Children.Clear();

            foreach (var product in products)
            {
                var label = new Label
                {
                    Text = $"Nazwa: {product.Name}, Cena: {product.Price}, Kategoria: {product.Category}",
                    FontSize = 16
                };

                ProductsStackLayout.Children.Add(label);
            }
        }
    }

}
