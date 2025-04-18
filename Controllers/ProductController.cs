using AAU_Task.Data;
using AAU_Task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AAU_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(AppDbContext dbContext) : ControllerBase
    {

        [HttpPost]
        [Route("Create")]
        public IActionResult CreateProduct(Product obj)
        {
            
            if (obj.ProductId == 0) // insert product
            {
                dbContext.Products.Add(obj);
            }
            else // update product
            {
                var productFromDb = dbContext.Products.FirstOrDefault(p => p.ProductId == obj.ProductId);
                if (productFromDb == null)
                    return NotFound();
                productFromDb.ProductName = obj.ProductName;
                productFromDb.Price = obj.Price;
                productFromDb.Qty = obj.Qty;   
            }

            dbContext.SaveChanges();

            return Ok();
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeleteProduct(int id) { 

           var product = dbContext.Products.FirstOrDefault( p => p.ProductId == id);
            if (product == null) return NotFound();
            dbContext.Products.Remove(product);
            dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut]
        [Route("Pruduct-Quntity/{id}")]
        public IActionResult PruductQuntity(int id, int Qty) { 
        
            var product = dbContext.Products.FirstOrDefault(p=> p.ProductId == id);
            if (product == null) return NotFound();

            product.Qty += Qty;
            dbContext.Update(product);
            dbContext.SaveChanges();

            return Ok();
        
        }
    }
}
