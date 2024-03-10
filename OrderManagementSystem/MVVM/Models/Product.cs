namespace OrderManagementSystem.MVVM.Models
{
    public class Product
    {
        private static int nextId = 1;

        public int Id { get; private set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }

        // Constructor
        public Product(string? name, decimal price, string? category)
        {
            Id = nextId++;
            Name = name;
            Price = price;
            Category = category;
        }
    }
}
