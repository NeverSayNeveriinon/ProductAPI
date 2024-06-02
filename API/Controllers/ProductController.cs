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
    ///     Get -> "api/product"
    /// 
    /// </remarks>
    /// <response code="200">The Products List is successfully returned</response>
    [HttpGet("/api/Products")]
    // GET: api/Product
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        List<ProductResponse> productsList = await _productService.GetAllProducts();
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
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Post: api/Product
    public async Task<IActionResult> PostProduct(ProductRequest product)
    {
        // No need to do this, because it is done by 'ApiController' attribute in BTS
        // if (!ModelState.IsValid)
        // {
        //     return ValidationProblem(ModelState);
        // }
        
        var productResponse = await _productService.AddProduct(product);
        var productKeyResponse = new ProductKey()
        {
            ManufactureEmail = productResponse.ManufactureEmail,
            ProduceDate = productResponse.ProduceDate
        };
        
        return CreatedAtAction(nameof(GetProduct), new {productKey = productKeyResponse}, product);
    }
    
    
    
    /// <summary>
    /// Get an Existing Product Based On Given ID
    /// </summary>
    /// <returns>The Product That Has Been Found</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Get -> "api/product/0866469B-A885-41AA-915C-F5697514CC26"
    /// 
    /// </remarks>
    /// <response code="200">The Product is successfully found and returned</response>
    /// <response code="404">A Product with Given ID has not been found</response>
    [HttpGet]
    // GET: api/Product/{productID}
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
    /// Update an Existing Product Based on Given ID and New Product Object
    /// </summary>
    /// <returns>Nothing</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Put -> "api/product/0866469B-A885-41AA-915C-F5697514CC26"
    ///     {
    ///        "Name": "DVD No.1",
    ///        "IsAvailable": true 
    ///     }
    /// 
    /// </remarks>
    /// <response code="204">The Product is successfully found and has been updated with New Product</response>
    /// <response code="404">A Product with Given ID has not been found</response>
    [HttpPut("{productID:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Put: api/Product/{productID}
    public async Task<IActionResult> PutProduct([FromBody]ProductRequest product, [FromRoute]ProductKey productKey)
    {
        ProductResponse? existingCity = await _productService.UpdateProduct(product, productKey);
        
        if (existingCity is null)
        {
            return NotFound("notfound:");
        }
        
        return NoContent();
    }
    
    
    /// <summary>
    /// Delete an Existing Product Based on Given ID
    /// </summary>
    /// <returns>Nothing</returns>
    /// <remarks>       
    /// Sample request:
    /// 
    ///     Delete -> "api/product/0866469B-A885-41AA-915C-F5697514CC26"
    /// 
    /// </remarks>
    /// <response code="204">The Product is successfully found and has been deleted from Products List</response>
    /// <response code="404">A Product with Given ID has not been found</response>
    [HttpDelete("{productID:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Delete: api/Product/{productID}
    public async Task<IActionResult> DeleteProduct(ProductKey productKey)
    {
        bool? productObject = await _productService.DeleteProduct(productKey);
        if (productObject is null)
        {
            return NotFound("notfound:");
        }

        return NoContent();
    }
}

