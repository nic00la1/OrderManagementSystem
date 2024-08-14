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
            int id = nextProductId; // Użyj kolejnego ID produktu
            string name = ProductNameEntry.Text;
            decimal price;
            bool isPriceValid = Decimal.TryParse(ProductPriceEntry.Text, out price);
            string? category = KategoriaPicker.SelectedItem?.ToString();
            int quantity;
            bool isQuantityValid = Int32.TryParse(ProductQuantity.Text, out quantity);
            string distribution = ProductDistributionPicker.SelectedItem?.ToString();
            string invoiceNumber = ProductInvoiceNumber.Text;
            DateTime purchaseDate = ProductPurchaseDate.Date;
            string comment = ProductComment.Text;

            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage += "Nazwa produktu jest wymagana.\n";
            }

            if (!isPriceValid || price < 0)
            {
                errorMessage += "Nieprawidłowa cena. Proszę wprowadzić dodatnią liczbę.\n";
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                errorMessage += "Kategoria jest wymagana.\n";
            }

            if (!isQuantityValid || quantity < 0)
            {
                errorMessage += "Nieprawidłowa ilość. Proszę wprowadzić dodatnią liczbę.\n";
            }

            if (string.IsNullOrWhiteSpace(distribution))
            {
                errorMessage += "Dystrybucja jest wymagana.\n";
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                DisplayAlert("Błąd", errorMessage, "OK");
                return;
            }

            Product newProduct = new Product(id, name, price, category, quantity, distribution, invoiceNumber, purchaseDate, comment);
            products.Add(newProduct);

            nextProductId++; // Zwiększ licznik po dodaniu produktu

            if (category != null && category != "All" && products.Any(p => p.Category == category))
            {
                ifNoProductInCategory_Frame.IsVisible = false;
                ifNoProductInCategory_Label.IsVisible = false;
            }

            // Wyczyść pola tekstowe
            ProductNameEntry.Text = "";
            ProductPriceEntry.Text = "";
            KategoriaPicker.SelectedItem = null;
            ProductQuantity.Text = "";
            ProductDistributionPicker.SelectedItem = null;
            ProductInvoiceNumber.Text = "";
            ProductPurchaseDate.Date = DateTime.Now;
            ProductComment.Text = "";

            DisplayProducts();
        }

        private void DisplayProducts()
        {
            // Utwórz nową listę produktów z zaktualizowanymi ID
            List<Product> updatedProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                updatedProducts.Add(new Product(i + 1, product.Name, product.Price, product.Category, product.Quantity, product.Distribution, product.InvoiceNumber, product.PurchaseDate, product.Comment));
            }

            // Wyczyść ProductsList i dodaj zaktualizowane produkty
            ProductsList.Clear();
            foreach (var product in updatedProducts)
            {
                ProductsList.Add(product);
            }

            // Zamień starą listę produktów na zaktualizowaną
            products = updatedProducts;
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            // Pobierz wybrany produkt
            Product selectedProduct = (Product)((Button)sender).BindingContext;

            // Wyświetl okno dialogowe do edycji szczegółów produktu
            int id = selectedProduct.Id; // Zachowaj to samo ID jak w oryginalnym produkcie
            string name = await DisplayPromptAsync("Zmień nazwę", "Wprowadź nową nazwę", initialValue: selectedProduct.Name);
            decimal price = Convert.ToDecimal(await DisplayPromptAsync("Zmień cenę", "Wprowadź nową cenę", initialValue: selectedProduct.Price.ToString()));
            int quantity = Convert.ToInt32(await DisplayPromptAsync("Zmień ilość", "Wprowadź nową ilość", initialValue: selectedProduct.Quantity.ToString()));
            string distribution = await DisplayActionSheet("Zmień dystrybucję", "Anuluj", null, "Dystrybucja 1", "Dystrybucja 2", "Dystrybucja 3");
            string invoiceNumber = await DisplayPromptAsync("Zmień nr faktury zakupowej", "Wprowadź nowy nr faktury zakupowej", initialValue: selectedProduct.InvoiceNumber);
            string purchaseDateString = await DisplayPromptAsync("Zmień datę zakupu", "Wprowadź nową datę zakupu (dd/MM/yyyy)", initialValue: selectedProduct.PurchaseDate.ToString("dd/MM/yyyy"));
            DateTime purchaseDate = DateTime.ParseExact(purchaseDateString, "dd/MM/yyyy", null);
            string comment = await DisplayPromptAsync("Zmień komentarz", "Wprowadź nowy komentarz", initialValue: selectedProduct.Comment);

            // Pobierz listę kategorii z pliku XAML
            List<string> categories = new List<string>();
            foreach (var item in KategoriaPicker.Items)
            {
                categories.Add(item.ToString());
            }

            // Wyświetl okno dialogowe do wyboru nowej kategorii
            string category = await DisplayActionSheet("Zmień kategorię", "Anuluj", null, categories.ToArray());

            // Zaktualizuj szczegóły produktu
            selectedProduct.Name = name;
            selectedProduct.Price = price;
            selectedProduct.Category = category;
            selectedProduct.Quantity = quantity;
            selectedProduct.Distribution = distribution;
            selectedProduct.InvoiceNumber = invoiceNumber;
            selectedProduct.PurchaseDate = purchaseDate;
            selectedProduct.Comment = comment;

            // Utwórz nowy obiekt produktu
            Product newProduct = new Product(id, name, price, category, quantity, distribution, invoiceNumber, purchaseDate, comment);

            // Zamień edytowany produkt na nowy w liście produktów
            int index = products.IndexOf(selectedProduct);
            products[index] = newProduct;

            DisplayProducts();
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            // Pobierz wybrany produkt
            Product selectedProduct = (Product)((Button)sender).BindingContext;

            // Usuń wybrany produkt z listy produktów
            products.Remove(selectedProduct);

            DisplayProducts();
        }

        private void DisplayProductsByCategory(string category)
        {
            // Utwórz nową listę produktów filtrowaną według kategorii
            List<Product> filteredProducts = products.Where(p => p.Category == category).ToList();

            // Wyczyść ProductsList
            ProductsList.Clear();

            if (filteredProducts.Count == 0)
            {
                // Jeśli nie ma produktów w wybranej kategorii, wyświetl komunikat
                ifNoProductInCategory_Label.IsVisible = true;
                ifNoProductInCategory_Label.Text = "Brak produktów w kategorii: " + category + "!";

                ifNoProductInCategory_Frame.IsVisible = true;
            }
            else
            {
                // Jeśli są produkty w wybranej kategorii, dodaj je do ProductsList
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

            // Reset the visibility of the message
            ifNoProductInCategory_Frame.IsVisible = false;
            ifNoProductInCategory_Label.IsVisible = false;

            if (category == "All")
            {
                DisplayProducts();
            }
            else
            {
                DisplayProductsByCategory(category);
            }
        }
    }

}
