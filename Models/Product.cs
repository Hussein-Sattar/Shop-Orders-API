using System.ComponentModel.DataAnnotations;

namespace AAU_Task.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required] public string ProductName { get; set; }
        [Required] public decimal Price { get; set; }
        [Required] public int Qty { get; set; }
    }
}
