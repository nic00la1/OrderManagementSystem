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
            _context = context;
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

        private void AddProduct_Clicked(object sender, EventArgs e)
        {
            // Pobierz wartości z kontrolek
            string name = ProductNameEntry.Text;
            decimal price = decimal.Parse(ProductPriceEntry.Text);
            // decimal sellingPrice = decimal.Parse(SellingPriceEntry.Text);
            // string eanNumber = EANNumberEntry.Text;
            // string serialNumber = SerialNumberEntry.Text;
            string? category = WyswietlKategoriaPicker.SelectedItem?.ToString();
            if (category != null)
            {
                DisplayProductsByCategory(category);
            }
            string distribution = ProductDistributionPicker.SelectedItem?.ToString() ?? string.Empty;
            int quantity = int.Parse(ProductQuantity.Text);
            string invoiceNumber = ProductInvoiceNumber.Text;
            DateTime purchaseDate = ProductPurchaseDate.Date;
            string comment = ProductComment.Text;

            // Utwórz nowy produkt
            var newProduct = new Product(
                GenerateNewProductId(), // Zakładam, że masz metodę do generowania unikalnych ID
                name,
                price,
               //  sellingPrice,
                // eanNumber,
                // serialNumber,
                category,
                quantity,
                distribution,
                invoiceNumber,
                purchaseDate,
                comment
            );

            // Dodaj produkt do listy (zakładam, że masz odpowiednią metodę lub kolekcję)
            ProductsList.Add(newProduct);

            // Zapisz produkt w bazie danych
            _context.Products.Add(newProduct);
            _context.SaveChanges();
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

        // Przykładowa metoda do generowania unikalnych ID
        private int GenerateNewProductId()
        {
            return ProductsList.Any() ? ProductsList.Max(p => p.Id) + 1 : 1;
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
            // decimal sellingPrice = Convert.ToDecimal(await DisplayPromptAsync("Zmień cenę sprzedaży", "Wprowadź nową cenę sprzedaży", initialValue: selectedProduct.SellingPrice.ToString()));
            // string eanNumber = await DisplayPromptAsync("Zmień numer EAN", "Wprowadź nowy numer EAN", initialValue: selectedProduct.EANNumber);
            // string serialNumber = await DisplayPromptAsync("Zmień numer seryjny", "Wprowadź nowy numer seryjny", initialValue: selectedProduct.SerialNumber);

            // Pobierz listę kategorii z pliku XAML
            List<string> categories = new List<string>();
            foreach (var item in KategoriaPicker.Items)
            {
                categories.Add(item.ToString());
            }

            // Wyświetl okno dialogowe do wyboru nowej kategorii
            string category = await DisplayActionSheet("Zmień kategorię", "Anuluj", null, categories.ToArray()) ?? string.Empty;

            // Zaktualizuj szczegóły produktu
            selectedProduct.Name = name;
            selectedProduct.Price = price;
            selectedProduct.Category = category;
            selectedProduct.Quantity = quantity;
            selectedProduct.Distribution = distribution;
            selectedProduct.InvoiceNumber = invoiceNumber;
            selectedProduct.PurchaseDate = purchaseDate;
            selectedProduct.Comment = comment;
           //  selectedProduct.SellingPrice = sellingPrice;
            // selectedProduct.EANNumber = eanNumber;
            // selectedProduct.SerialNumber = serialNumber;

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
