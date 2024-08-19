using OrderManagementSystem.MVVM.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Dataa;

namespace OrderManagementSystem
{
    public partial class MainPage : ContentPage
    {
        private readonly DataContext _context;
        public ObservableCollection<Product> ProductsList { get; set; }

        public MainPage(DataContext context)
        {
            InitializeComponent();
            _context = new DataContext();
            ProductsList = new ObservableCollection<Product>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProductsFromDatabase();
        }

        private async Task LoadProductsFromDatabase()
        {
            var productsFromDb = await _context.Products.ToListAsync();
            ProductsList.Clear();
            foreach (var product in productsFromDb)
            {
                ProductsList.Add(product);
            }
        }

        private static int nextProductId = 1;

        private void AddProduct_Clicked(object sender, EventArgs e)
        {
            int id = nextProductId; // Użyj kolejnego ID produktu
            string name = ProductNameEntry.Text;
            decimal price;
            bool isPriceValid = Decimal.TryParse(ProductPriceEntry.Text, out price);
            string? category = WyswietlKategoriaPicker.SelectedItem?.ToString();
            if (category != null)
            {
                DisplayProductsByCategory(category);
            }

            int quantity;
            bool isQuantityValid = Int32.TryParse(ProductQuantity.Text, out quantity);
            string distribution = ProductDistributionPicker.SelectedItem?.ToString() ?? string.Empty;
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
           
            // Sprawdź, czy produkt o tym samym ID już istnieje w kontekście
            var existingProduct = _context.Products.Local.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                ProductsList.Add(newProduct);

                // Zapisz nowy produkt w bazie danych
                _context.Products.Add(newProduct);
                _context.SaveChanges();

                nextProductId++; // Zwiększ licznik po dodaniu produktu
            }
            else
            {
                DisplayAlert("Błąd", "Produkt o tym samym ID już istnieje.", "OK");
            }

            if (category != null && category != "All" && ProductsList.Any(p => p.Category == category))
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
            // Pobierz wszystkie produkty z bazy danych
            var productsFromDb = _context.Products.ToList();

            // Wyczyść ProductsList i dodaj produkty z bazy danych
            ProductsList.Clear();
            foreach (var product in productsFromDb)
            {
                ProductsList.Add(product);
            }
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
            
            // Pobierz listę dystrybucji z Picker
            var distributionItems = ProductDistributionPicker.ItemsSource.Cast<string>().ToArray();
            string distribution = await DisplayActionSheet("Zmień dystrybucję", "Anuluj", null, distributionItems) ?? string.Empty;

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

            // Zaktualizuj produkt w bazie danych
            _context.Products.Update(selectedProduct);
            await _context.SaveChangesAsync(); // Użyj asynchronicznej wersji SaveChanges

            // Zaktualizuj produkt w ProductsList
            var productInList = ProductsList.FirstOrDefault(p => p.Id == selectedProduct.Id);
            if (productInList != null)
            {
                int index = ProductsList.IndexOf(productInList);
                ProductsList[index] = selectedProduct;
            }

            DisplayProducts();
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            // Pobierz wybrany produkt
            Product selectedProduct = (Product)((Button)sender).BindingContext;

            // Usuń wybrany produkt z listy produktów
            ProductsList.Remove(selectedProduct);

            // Usuń produkt z bazy danych
            _context.Products.Remove(selectedProduct);
            _context.SaveChanges();

            DisplayProducts();
        }

        private void DisplayProductsByCategory(string category)
        {
            // Utwórz nową listę produktów filtrowaną według kategorii
            List<Product> filteredProducts = ProductsList.Where(p => p.Category == category).ToList();

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
