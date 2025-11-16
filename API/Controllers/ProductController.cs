using System;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly StoreContext storeContext;

    public ProductController(StoreContext storeContext)
    {
        this.storeContext = storeContext;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await storeContext.Products.ToListAsync();
        return Ok(products);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await storeContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        storeContext.Products.Add(product);
        await storeContext.SaveChangesAsync();
        return product;
        //CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        storeContext.Entry(product).State = EntityState.Modified;
        try
        {
            await storeContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await storeContext.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        storeContext.Products.Remove(product);
        await storeContext.SaveChangesAsync();
        return NoContent();
    }
    private bool ProductExists(int id)
    {
        return storeContext.Products.Any(e => e.Id == id);
    }

}
