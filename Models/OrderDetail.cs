using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AAU_Task.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal LineTotal { get; set; } 

        [ForeignKey("OrderId")]
        public Order OrderHeader {  get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
