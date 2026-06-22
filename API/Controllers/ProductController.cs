using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class ProductController(IUnitOfWork unit) : BaseApiController
{
   
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        var spec= new ProductSpecification(specParams);
        // var products = await repo.ListAsync(spec);
        // var count= await repo.CountAsync(spec);
        // var pagination=new Pagination<Product>(specParams.PageIndex,specParams.PageSize,count,products);
        // return Ok(pagination);
        return await CreatePageResult(unit.Repository<Product>(),spec,specParams.PageIndex,specParams.PageSize);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        unit.Repository<Product>().Add(product);
        if(await unit.Complete())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        return BadRequest("Failed to create product");
        //CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        
             unit.Repository<Product>().Update(product);
             if(await unit.Complete())
             {
                return NoContent();
             }
        return BadRequest("Failed to update product");
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        unit.Repository<Product>().Remove(product);
        if(await unit.Complete())
             {
                return NoContent();
             }
        return BadRequest("Failed to delete product");
    }
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec= new BrandListSpecification();
        //(await repo.GetBrandsAsync()
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec= new TypeListSpecification();
        //await repo.GetTypesAsync()
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }
    private bool ProductExists(int id)
    {
        return unit.Repository<Product>().Exist(id);
    }

}
