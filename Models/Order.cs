namespace AAU_Task.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
