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
            string name = ProductNameEntry.Text;
            decimal price = Convert.ToDecimal(ProductPriceEntry.Text);
            string? category = KategoriaPicker.SelectedItem.ToString();

            Product newProduct = new Product(name, price, category);
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

    }

}
