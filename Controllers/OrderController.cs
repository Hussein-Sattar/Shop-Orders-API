using AAU_Task.Data;
using AAU_Task.Models;
using AAU_Task.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AAU_Task.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost("place-order")]
    public async Task<IActionResult> PlaceOrder([FromBody] List<OrderDetailDTO> items)
    {

        #region Validate The sent items
        if (items == null || items.Count == 0)
            return BadRequest();

        var productIds = items.Select(i => i.ProductId).ToList();
        var validProducts = await dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .Select(p => p.ProductId)
            .ToListAsync();

        var invalidIds = productIds.Except(validProducts).ToList();
        if (invalidIds.Any())
            return BadRequest($"Invalid ProductIds: {string.Join(", ", invalidIds)}");
        #endregion

        var products = await dbContext.Products
                                .Where(p => productIds.Contains(p.ProductId))
                                .ToListAsync();

        #region Validation for the avilable stock
            var outOfStock = new List<string>();
            var productsDictionary = products.ToDictionary(p => p.ProductId);
            foreach (var item in items)
            {
                var prod = productsDictionary[item.ProductId];
            if (item.Qty > prod.Qty)
                outOfStock.Add($"{prod.ProductName} (Available: {prod.Qty}, Requested: {item.Qty})");
            }
        if (outOfStock.Any())
        {
            return BadRequest("Insufficient stock for: " + string.Join("; ", outOfStock));
        }
        #endregion

    
        var order = new Order
        {
            OrderDate = DateTime.Now,
            TotalPrice = items.Sum(i=> i.Qty * productsDictionary[i.ProductId].Price),
            Status = OrderStatus.Pending
        };
         dbContext.Orders.Add(order);
         dbContext.SaveChanges();

        List<OrderDetail> lines = items.Select(i => new OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = i.ProductId,
            Price = productsDictionary[i.ProductId].Price,
            LineTotal = productsDictionary[i.ProductId].Price * i.Qty,
            Qty = i.Qty,
        }).ToList();

       await dbContext.OrderDetails.AddRangeAsync(lines);
        foreach (var item in items)
        {
            productsDictionary[item.ProductId].Qty -= item.Qty;
        }

        dbContext.SaveChanges();
        return Ok(new { orderId = order.OrderId });
        
    }

    [HttpGet]
    [Route("GetAll")]
    public IActionResult GetAll([FromQuery]OrderStatus statusFilter)
    {
        var ordersFromDB = dbContext.Orders.ToList();
        var orders = from order in ordersFromDB
                     select new
                     {
                         orderID = order.OrderId,
                         orderDate = order.OrderDate.ToShortDateString(),
                         TotalPrice = order.TotalPrice,
                         Status = Enum.GetName(typeof(OrderStatus), order.Status)
                     };

        var result = new
        {
            NoOfOrders = ordersFromDB.Count,
            orders = orders
        };
        return Ok(result);
    }

    [HttpPut]
    [Route("AdjustQty")]
    public IActionResult AdjustQty(AdjustOrderDto obj)
    {
        var LineToAdjust = dbContext.OrderDetails
            .Where(od => od.ProductId == obj.productId && od.OrderId == obj.orderId)
            .Include(od => od.Product)
            .Include(od => od.OrderHeader)
            .FirstOrDefault();

        if (LineToAdjust == null)
            return BadRequest();

        if (LineToAdjust.OrderHeader.Status != OrderStatus.Pending)
            return BadRequest("The order Quantites Cannot be Modified, your Order may be Processing or Already Shipped");

        if (obj.Qty == 0)
            return BadRequest("??");
        // The logic below is need to be refactored
        if (obj.Qty > 0) {

            if (obj.Qty > LineToAdjust.Product.Qty)
                return BadRequest("The Requsted Qty is greater than our stock");
            LineToAdjust.OrderHeader.TotalPrice += LineToAdjust.Price * obj.Qty;
            LineToAdjust.LineTotal += LineToAdjust.Price * obj.Qty;
            LineToAdjust.Qty += obj.Qty;
            LineToAdjust.Product.Qty -= obj.Qty;
        }
        else
        {
            // Considering the values of qty here are in NAGATIVE
            LineToAdjust.OrderHeader.TotalPrice += LineToAdjust.Price * obj.Qty;
            LineToAdjust.LineTotal += LineToAdjust.Price * obj.Qty;
            LineToAdjust.Qty += obj.Qty;
            LineToAdjust.Product.Qty += obj.Qty;
        }
        dbContext.SaveChanges();

        return Ok($" new item Qty: {LineToAdjust.Qty}");
    }

}