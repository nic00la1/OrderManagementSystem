namespace OrderManagementSystem.MVVM.Models
{
    public class Product
    {
        // Properties
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }

        // Constructor
        public Product(int id, string? name, decimal price, string? category)
        {
            Id = id; // This is a unique identifier for each product
            Name = name;
            Price = price;
            Category = category;
        }
    }
}
