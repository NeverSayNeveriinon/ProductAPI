using System.Linq.Expressions;
using System.Security.Claims;
using Core.Domain;
using Moq;

using Core.Domain.RepositoryContracts;
using Core.DTO;
using Core.ServiceContracts;
using Core.Services;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace XUnit_Tests.UnitTest.Services;

public class ProductServiceTest
{
    private readonly IProductService _productService;
        
    private readonly IProductRepository _productRepository;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    
    public ProductServiceTest()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productRepository = _productRepositoryMock.Object;

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Object.HttpContext = new DefaultHttpContext();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        _productService = new ProductService(_productRepository, _userManagerMock.Object, _httpContextAccessorMock.Object);
    }
    
    

    #region AddProduct

    [Fact]
    public async Task AddProduct_ShouldThrowArgumentNullException_WhenProductRequestIsNull()
    {
        // Arrange
        ProductRequest? productAddRequest = null;
            
        // Act
        Func<Task> action = async () =>  await _productService.AddProduct(productAddRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddProduct_ShouldThrowArgumentException_WhenNameOfProductRequestIsEmpty()
    {
        // Arrange
        ProductRequest productAddRequest = new ProductRequest()
        {
            Name = "",
            IsAvailable = true
        };

        // Act
        Func<Task> action = async () => await _productService.AddProduct(productAddRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task AddProduct_ShouldThrowArgumentException_WhenSomeValidationsFailed()
    {
        // Arrange
        ProductRequest productAddRequest = new ProductRequest()
        {
            Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", // more than 50 characters
            IsAvailable = true
        };

        // Act
        Func<Task> action = async () => await _productService.AddProduct(productAddRequest);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When 'ProductRequest.Name' is valid then there is no problem, it should add the product object to products list
    [Fact]
    public async Task AddProduct_ShouldAddAndReturnProperObject_WhenThereIsNoProblem()
    {
        // Arrange
        ProductRequest productAddRequest = new ProductRequest()
        {
            Name = "Book no.1",
            IsAvailable = true
        };
        const string userName = "user@example.com";
        ApplicationUser applicationUser = new ApplicationUser()
        {
            UserName = "user@example.com",
            PhoneNumber = "099912345678"
        };
        
        Product product = productAddRequest.Adapt<Product>();
        ProductResponse productResponse_fromTest = product.Adapt<ProductResponse>();
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.AddProduct(It.IsAny<Product>()))
                              .ReturnsAsync(product);   
        
        _httpContextAccessorMock.SetupGet(entity => entity.HttpContext!.User.Identity!.Name)
                                .Returns(userName);
        
        _userManagerMock.Setup(entity => entity.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(applicationUser);   
            
        // Act
        ProductResponse productResponse_fromService = await _productService.AddProduct(productAddRequest);
        productResponse_fromTest.ProduceDate = productResponse_fromService.ProduceDate;
        productResponse_fromTest.ManufactureEmail = productResponse_fromService.ManufactureEmail;
        productResponse_fromTest.ManufacturePhone = productResponse_fromService.ManufacturePhone;
            
        // Assert
        productResponse_fromService.ProduceDate.Should().NotBeSameDateAs(DateTime.MinValue);
        productResponse_fromService.ManufactureEmail.Should().NotBeNullOrEmpty();
        productResponse_fromService.Should().BeEquivalentTo(productResponse_fromTest);
    }

    #endregion
    
    
    #region GetAllProducts

    [Fact]
    public async Task GetAllProducts_ShouldReturnEmptyList_WhenDBListIsEmpty()
    {
        // Arrange
        var emptyList = new List<Product>();
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.GetAllProducts())
                              .ReturnsAsync(emptyList);
            
        // Act
        List<ProductResponse> productResponseList = await _productService.GetAllProducts();

        // Assert
        productResponseList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnProperObjects_WhenThereIsNoProblem()
    {
        // Arrange
        Product product1 = new Product()
        {
            Name = "Book no.1",
            IsAvailable = true

        };
        Product product2 = new Product()
        {
            Name = "Book no.2",
            IsAvailable = false

        };
        List<Product> productList = new List<Product>() { product1, product2 };
        List<ProductResponse> productResponsesList_fromTest = productList.Select(product => product.Adapt<ProductResponse>()).ToList();
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.GetAllProducts())
                              .ReturnsAsync(productList);
            
        // Act
        List<ProductResponse> productResponseList_fromService = await _productService.GetAllProducts();


        // Assert
        productResponseList_fromService.Should().BeEquivalentTo(productResponsesList_fromTest);
    }

    #endregion
    
    
    #region GetProductByKey

    [Fact]
    public async Task GetProductByKey_ShouldThrowArgumentNullException_WhenProductKeyIsNull()
    {
        // Arrange
        ProductKey? productKey = null;

        // Act
        Func<Task> action = async () =>  await _productService.GetProductByKey(productKey);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetProductByKey_ShouldReturnNull_WhenProductIsNotFound()
    {
        // Arrange
        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00"),
        };
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
            .ReturnsAsync(null as Product);
        
        // Act
        ProductResponse? productResponse = await _productService.GetProductByKey(productKey);

        // Assert
        productResponse.Should().BeNull();
    }

    [Fact]
    public async Task GetProductByKey_ShouldReturnProperObject_WhenThereIsNoProblem()
    {
        // Arrange
        Product product = new Product()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00"),
            ManufacturePhone = "09990123456",
            Name = "Book No.1",
            IsAvailable = false
        };
        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00"),
        };  
        ProductResponse productResponse_fromTest = product.Adapt<ProductResponse>();

        _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                              .ReturnsAsync(product);
            
        // Act
        ProductResponse? productResponse_fromService = await _productService.GetProductByKey(productKey);

        // Assert
        productResponse_fromService.Should().BeEquivalentTo(productResponse_fromTest);
    }

    #endregion
    
    
    #region UpdateProduct

    [Fact]
    public async Task UpdateProduct_ShouldThrowArgumentNullException_WhenProductRequestIsNull()
    {
        // Arrange
        ProductRequest? productUpdateRequest = null;
        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
        };
        
        // Act
        Func<Task> action = async () => await _productService.UpdateProduct(productUpdateRequest,productKey);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldThrowArgumentNullException_WhenProductKeyIsNull()
    {
        // Arrange
        ProductRequest productUpdateRequest = new ProductRequest()
        {
            Name = "Book No.1",
            IsAvailable = true
        };
        ProductKey? productKey = null;
        
        // Act
        Func<Task> action = async () => await _productService.UpdateProduct(productUpdateRequest,productKey);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrowArgumentNullException_WhenNameOfProductRequestIsEmpty()
    {
        // Arrange
        ProductRequest productUpdateRequest = new ProductRequest()
        {
            Name = "",
            IsAvailable = true
        };

        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
        };
        
        // Act
        Func<Task> action = async () => await _productService.UpdateProduct(productUpdateRequest, productKey);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNull_WhenProductIsNotFound()
    {
        // Arrange
        ProductRequest productUpdateRequest = new ProductRequest()
        {
            Name = "Book No.1",
            IsAvailable = true
        };
        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
        };
            
        Product updatedProduct = productUpdateRequest.Adapt<Product>();
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                              .ReturnsAsync(null as Product);

        // Act
        ProductResponse? productResponse =  await _productService.UpdateProduct(productUpdateRequest, productKey);
        
        // Assert
        productResponse.Should().BeNull();
    }

    [Fact]
    public async Task UpdateProduct_ShouldUpdateAndReturnProperObject_WhenThereIsNoProblem()
    {
        // Arrange
        Product product = new Product()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00"),
            ManufacturePhone = "09990123456",
            Name = "Book No.1",
            IsAvailable = false
        };
        
        ProductRequest productUpdateRequest = new ProductRequest()
        {
            Name = "Book No.222",
            IsAvailable = true
        };
        ProductKey productKey = new ProductKey()
        {
            ManufactureEmail = "user@example.com",
            ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
        };
            
        Product updatedProduct = productUpdateRequest.Adapt<Product>();
        updatedProduct.ManufactureEmail = product.ManufactureEmail;
        updatedProduct.ProduceDate = product.ProduceDate;
        updatedProduct.ManufacturePhone = product.ManufacturePhone;

        
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>())) 
                              .ReturnsAsync(product);    

        _productRepositoryMock.Setup(entity => entity.UpdateProduct(It.IsAny<Product>(),It.IsAny<Product>()))
                              .ReturnsAsync(updatedProduct);  
            
        // Act
        ProductResponse productResponse_fromTest = updatedProduct.Adapt<ProductResponse>();
        ProductResponse? productResponse_fromService = await _productService.UpdateProduct(productUpdateRequest, productKey);
            
        // Assert
        productResponse_fromService.Should().BeEquivalentTo(productResponse_fromTest);
    }

    #endregion
    
    
    
}