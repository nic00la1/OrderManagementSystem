namespace OrderManagementSystem.MVVM.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string[]? Category { get; set; }
        public string? Description { get; set; }

        public Product( // Constructor
            int id,
            string? name,
            decimal price,
            string[]? category,
            string? description
            )
        {
            Id = id;
            Name = name;
            Price = price;
            Category = category;
            Description = description;
        }
    }
}
