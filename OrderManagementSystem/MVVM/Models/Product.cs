namespace OrderManagementSystem.MVVM.Models
{
    public class Product
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }

        // Constructor
        public Product(string? name, decimal price, string? category)
        {
            Name = name;
            Price = price;
            Category = category;
        }
    }
}
