using Core.Domain;
using Core.DTO;
using Core.ServiceContracts;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    [Route("/")]
    [ApiExplorerSettings(IgnoreApi = true)] // For not showing in 'Swagger'
    public IActionResult Index()
    {
        return Content("Here is the \"Product\" Home Page");
    }
    
    
    
    /// <summary>
    /// Get All Existing Products
    /// </summary>
    /// <returns>The Products List</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Get -> "api/products"
    /// 
    /// </remarks>
    /// <response code="200">The Products List is successfully returned</response>
    [HttpGet("/api/Products")]
    // GET: api/Products
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery] string? filterEmail)
    {
        List<ProductResponse> productsList = await _productService.GetAllProducts(filterEmail);
        return Ok(productsList);
    }
    
     
    /// <summary>
    /// Add a New Product to Products List
    /// </summary>
    /// <returns>Redirect to 'GetProduct' action to return Product That Has Been Added</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     POST -> "api/product"
    ///     {
    ///        "Name": "DVD No.1",
    ///        "IsAvailable": true 
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">The New Product is successfully added to Products List</response>
    /// <response code="400">There is sth wrong in Validation of properties</response>
    /// <response code="401">Unauthorized: Please Enter Valid JWT Token</response>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Post: api/Product
    public async Task<IActionResult> PostProduct(ProductRequest product)
    {
        var productResponse = await _productService.AddProduct(product);
        
        return CreatedAtAction(nameof(GetProduct), new {ProduceDate = productResponse.ProduceDate,
                                                         ManufactureEmail = productResponse.ManufactureEmail}, productResponse);
    }
    
    
    
    /// <summary>
    /// Get an Existing Product Based On Given Key (Date &amp; Email)
    /// </summary>
    /// <returns>The Product That Has Been Found</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Get -> "api/product/ProduceDate=...&amp;ManufactureEmail=..."
    /// 
    /// </remarks>
    /// <response code="200">The Product is successfully found and returned</response>
    /// <response code="404">A Product with Given Key (Date &amp; Email) has not been found</response>
    [HttpGet]
    // GET: api/Product/ProduceDate=...&ManufactureEmail=...
    public async Task<ActionResult<ProductResponse>> GetProduct([FromQuery]ProductKey productKey)
    {
        ProductResponse? productObject = await _productService.GetProductByKey(productKey);
        if (productObject is null)
        {
            return NotFound("notfound:");
        }
        
        return Ok(productObject);
    }
    
    
    /// <summary>
    /// Update an Existing Product Based on Given Key (Date &amp; Email) and New Product Object
    /// </summary>
    /// <returns>Nothing</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Put -> "api/product/ProduceDate=...&amp;ManufactureEmail=..."
    ///     {
    ///        "Name": "DVD No.1 is Edited",
    ///        "IsAvailable": false 
    ///     }
    /// 
    /// </remarks>
    /// <response code="204">The Product is successfully found and has been updated with New Product</response>
    /// <response code="400">Something Wrong Happened With Permission</response>
    /// <response code="404">A Product with Given Key (Date &amp; Email) has not been found</response>
    /// <response code="401">Unauthorized: Please Enter Valid JWT Token</response>
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Put: api/Product/ProduceDate=...&ManufactureEmail=...
    public async Task<IActionResult> PutProduct([FromBody]ProductRequest product, [FromQuery]ProductKey productKey)
    {
        if (productKey.ManufactureEmail != User.Identity?.Name)
        {
            return Problem("You Can't Update/Delete the product you haven't Created!!!",statusCode:400);
        }
        
        ProductResponse? existingCity = await _productService.UpdateProduct(product, productKey);
        
        if (existingCity is null)
        {
            return NotFound("notfound:");
        }
        
        return NoContent();
    }
    
    
    /// <summary>
    /// Delete an Existing Product Based on Given Key (Date &amp; Email)
    /// </summary>
    /// <returns>Nothing</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Delete -> "api/product/ProduceDate=...&amp;ManufactureEmail=..."
    /// 
    /// </remarks>
    /// <response code="204">The Product is successfully found and has been deleted from Products List</response>
    /// <response code="400">Something Wrong Happened With Permission</response>
    /// <response code="404">A Product with Given Key (Date &amp; Email) has not been found</response>
    /// <response code="401">Unauthorized: Please Enter Valid JWT Token</response>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Delete: api/Product/ProduceDate=...&ManufactureEmail=...
    public async Task<IActionResult> DeleteProduct([FromQuery]ProductKey productKey)
    {
        if (productKey.ManufactureEmail != User.Identity?.Name)
        {
            return Problem("You Can't Update/Delete the product you haven't Created!!!");
        }
        
        bool? productObject = await _productService.DeleteProduct(productKey);
        if (productObject is null)
        {
            return NotFound("notfound:");
        }

        return NoContent();
    }
}

