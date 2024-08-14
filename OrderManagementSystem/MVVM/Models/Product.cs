namespace OrderManagementSystem.MVVM.Models
{
    public class Product
    {
        // Properties
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public string Distribution { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Comment { get; set; }



        // Constructor
        public Product(int id, string name, decimal price, string category, int quantity, string distribution, string invoiceNumber, DateTime purchaseDate, string comment)
        {
            Id = id;
            Name = name;
            Price = price;
            Category = category;
            Quantity = quantity;
            Distribution = distribution;
            InvoiceNumber = invoiceNumber;
            PurchaseDate = purchaseDate;
            Comment = comment;
        }
    }
}
