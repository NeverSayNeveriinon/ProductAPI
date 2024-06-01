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
    public async Task AddProduct_ShouldThrowArgumentNullException_WhenNameOfProductRequestIsNullOrEmpty()
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
        await action.Should().ThrowAsync<ArgumentNullException>();
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
 
}