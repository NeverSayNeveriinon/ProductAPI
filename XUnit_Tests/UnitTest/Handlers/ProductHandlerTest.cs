using System.Security.Claims;
using Core.Domain;
using Core.Domain.RepositoryContracts;
using Core.Products.Commands.CreateProduct;
using Core.Products.Commands.DeleteProduct;
using Core.Products.Commands.UpdateProduct;
using Core.Products.Common_DTOs;
using Core.Products.Queries.GetAllProducts;
using Core.Products.Queries.GetProduct;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace XUnit_Tests.UnitTest.Handlers;

public class ProductHandlerTest
{
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;
    private readonly DeleteProductCommandHandler _deleteProductCommandHandler;
    
    private readonly GetAllProductsQueryHandler _getAllProductsQueryHandler;
    private readonly GetProductByKeyQueryHandler _getProductByKeyQueryHandler;
    
        
    private readonly IProductRepository _productRepository;
    private readonly Mock<IProductRepository> _productRepositoryMock;
        
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    
    public ProductHandlerTest()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productRepository = _productRepositoryMock.Object;

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Object.HttpContext = new DefaultHttpContext();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Handlers
        _createProductCommandHandler = new CreateProductCommandHandler(_productRepository, _userManagerMock.Object, _httpContextAccessorMock.Object);
        _updateProductCommandHandler = new UpdateProductCommandHandler(_productRepository);
        _deleteProductCommandHandler = new DeleteProductCommandHandler(_productRepository);
        
        _getAllProductsQueryHandler = new GetAllProductsQueryHandler(_productRepository);
        _getProductByKeyQueryHandler = new GetProductByKeyQueryHandler(_productRepository);
    }
    
    

    #region CreateProduct

    [Fact]
    public async Task CreateProductHandler_ShouldThrowArgumentNullException_WhenCreateProductCommandIsNull()
    {
        // Arrange
        CreateProductCommand? createProductCommand = null;
            
        // Act
        Func<Task> action = async () =>  await _createProductCommandHandler.Handle(createProductCommand, CancellationToken.None);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CreateProductHandler_ShouldThrowArgumentException_WhenNameOfCreateProductCommandIsEmpty()
    {
        // Arrange
        CreateProductCommand createProductCommand = new CreateProductCommand()
        {
            Name = "",
            IsAvailable = true
        };

        // Act
        Func<Task> action = async () => await _createProductCommandHandler.Handle(createProductCommand, CancellationToken.None);
        
        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateProductHandler_ShouldThrowArgumentException_WhenSomeValidationsFailed()
    {
        // Arrange
        CreateProductCommand createProductCommand = new CreateProductCommand()
        {
            Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", // more than 50 characters
            IsAvailable = true
        };

        // Act
        Func<Task> action = async () => await _createProductCommandHandler.Handle(createProductCommand, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When 'CreateProductCommand.Name' is valid then there is no problem, it should add the product object to products list
    [Fact]
    public async Task CreateProductHandler_ShouldAddAndReturnProperObject_WhenThereIsNoProblem()
    {
        // Arrange
        CreateProductCommand createProductCommand = new CreateProductCommand()
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
        
        Product product = createProductCommand.Adapt<Product>();
        product.ManufactureEmail = applicationUser.UserName;
        product.ManufacturePhone = applicationUser.PhoneNumber;
        product.ProduceDate = DateTime.Now;
        
        ProductResponse productResponse_fromTest = product.Adapt<ProductResponse>();
            
        // Mocking the Required Methods
        _productRepositoryMock.Setup(entity => entity.AddProduct(It.IsAny<Product>()))
                              .ReturnsAsync(product);
        
        _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                              .ReturnsAsync(product);   
        
        _httpContextAccessorMock.SetupGet(entity => entity.HttpContext!.User.Identity!.Name)
                                .Returns(userName);
        
        _userManagerMock.Setup(entity => entity.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(applicationUser);   
            
        // Act
        ProductResponse productResponse_fromHandler = await _createProductCommandHandler.Handle(createProductCommand, CancellationToken.None);
        productResponse_fromTest.ProduceDate = productResponse_fromHandler.ProduceDate;
            
        // Assert
        productResponse_fromHandler.ProduceDate.Should().NotBeSameDateAs(DateTime.MinValue);
        productResponse_fromHandler.ManufactureEmail.Should().NotBeNullOrEmpty();
        productResponse_fromHandler.Should().BeEquivalentTo(productResponse_fromTest);
    }
    
    #endregion
    
     
     #region GetAllProducts

     [Fact]
     public async Task GetAllProductsHandler_ShouldReturnEmptyList_WhenDBListIsEmpty()
     {
         // Arrange
         var emptyList = new List<Product>();
         
         // Mocking the Required Methods
         _productRepositoryMock.Setup(entity => entity.GetAllProducts())
                               .ReturnsAsync(emptyList);
             
         // Act
         List<ProductResponse> productResponseList = await _getAllProductsQueryHandler.Handle( new GetAllProductsQuery(){FilterByEmail = null}, CancellationToken.None);

         // Assert
         productResponseList.Should().BeEmpty();
     }

     [Fact]
     public async Task GetAllProductsHandler_ShouldReturnProperObjects_WhenThereIsNoProblem()
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
         List<ProductResponse> productResponseList_fromHandler = await _getAllProductsQueryHandler.Handle(new GetAllProductsQuery(){FilterByEmail = null},
                                                                                                            CancellationToken.None);


         // Assert
         productResponseList_fromHandler.Should().BeEquivalentTo(productResponsesList_fromTest);
     }

     #endregion
     
     
     #region GetProductByKey

     [Fact]
     public async Task GetProductByKeyHandler_ShouldThrowArgumentNullException_WhenGetProductByKeyQueryIsNull()
     {
         // Arrange
         GetProductByKeyQuery? query = new GetProductByKeyQuery(){ ProductKey = null };

         // Act
         Func<Task> action = async () =>  await _getProductByKeyQueryHandler.Handle(query, CancellationToken.None);
         
         // Assert
         await action.Should().ThrowAsync<ArgumentNullException>();
     }

     [Fact]
     public async Task GetProductByKeyHandler_ShouldReturnNull_WhenProductIsNotFound()
     {
         // Arrange
         ProductKey productKey = new ProductKey()
         {
             ManufactureEmail = "user@example.com",
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00"),
         };
         GetProductByKeyQuery? query = new GetProductByKeyQuery() { ProductKey = productKey };

             
         // Mocking the Required Methods
         _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
             .ReturnsAsync(null as Product);
         
         // Act
         ProductResponse? productResponse = await _getProductByKeyQueryHandler.Handle(query, CancellationToken.None);

         // Assert
         productResponse.Should().BeNull();
     }

     [Fact]
     public async Task GetProductByKeyHandler_ShouldReturnProperObject_WhenThereIsNoProblem()
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
         GetProductByKeyQuery? query = new GetProductByKeyQuery() { ProductKey = productKey };
         
         ProductResponse productResponse_fromTest = product.Adapt<ProductResponse>();

         _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                               .ReturnsAsync(product);
             
         // Act
         ProductResponse? productResponse_fromHandler = await _getProductByKeyQueryHandler.Handle(query, CancellationToken.None);

         // Assert
         productResponse_fromHandler.Should().BeEquivalentTo(productResponse_fromTest);
     }

     #endregion
     
     
     #region UpdateProduct

     [Fact]
     public async Task UpdateProductHandler_ShouldThrowArgumentNullException_WhenUpdateProductCommandIsNull()
     {
         // Arrange
         UpdateProductCommand? updateProductCommand = null;
         ProductKey productKey = new ProductKey()
         {
             ManufactureEmail = "user@example.com",
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         
         // Act
         Func<Task> action = async () => await _updateProductCommandHandler.Handle(updateProductCommand, CancellationToken.None);
         
         // Assert
         await action.Should().ThrowAsync<ArgumentNullException>();
     }
     
     [Fact]
     public async Task UpdateProductHandler_ShouldThrowArgumentNullException_WhenProductKeyIsNull()
     {
         // Arrange
         ProductKey? productKey = null;
         UpdateProductCommand updateProductCommand = new UpdateProductCommand()
         {
             Name = "Book No.1",
             IsAvailable = true,
             ProductKey = productKey
         };
         
         // Act
         Func<Task> action = async () => await _updateProductCommandHandler.Handle(updateProductCommand, CancellationToken.None);
         
         // Assert
         await action.Should().ThrowAsync<ArgumentNullException>();
     }

     [Fact]
     public async Task UpdateProductHandler_ShouldThrowArgumentNullException_WhenNameOfUpdateProductCommandIsEmpty()
     {
         // Arrange
         ProductKey productKey = new ProductKey()
         {
             ManufactureEmail = "user@example.com",
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         UpdateProductCommand updateProductCommand = new UpdateProductCommand()
         {
             Name = "",
             IsAvailable = true,
             ProductKey = productKey
         };
         
         // Act
         Func<Task> action = async () => await _updateProductCommandHandler.Handle(updateProductCommand, CancellationToken.None);
         
         // Assert
         await action.Should().ThrowAsync<ArgumentException>();
     }

     [Fact]
     public async Task UpdateProductHandler_ShouldReturnNull_WhenProductIsNotFound()
     {
         // Arrange
         ProductKey productKey = new ProductKey()
         {
             ManufactureEmail = "user@example.com",
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         UpdateProductCommand updateProductCommand = new UpdateProductCommand()
         {
             Name = "Book No.1",
             IsAvailable = true,
             ProductKey = productKey
         };
             
         Product updatedProduct = updateProductCommand.Adapt<Product>();
             
         // Mocking the Required Methods
         _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                               .ReturnsAsync(null as Product);

         // Act
         ProductResponse? productResponse = await _updateProductCommandHandler.Handle(updateProductCommand, CancellationToken.None);
         
         // Assert
         productResponse.Should().BeNull();
     }

     [Fact]
     public async Task UpdateProductHandler_ShouldUpdateAndReturnProperObject_WhenThereIsNoProblem()
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
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         UpdateProductCommand updateProductCommand = new UpdateProductCommand()
         {
             Name = "Book No.222",
             IsAvailable = true,
             ProductKey = productKey
         };
             
         Product updatedProduct = updateProductCommand.Adapt<Product>();
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
         ProductResponse? productResponse_fromHandler = await _updateProductCommandHandler.Handle(updateProductCommand, CancellationToken.None);
             
         // Assert
         productResponse_fromHandler.Should().BeEquivalentTo(productResponse_fromTest);
     }

     #endregion
     
     
     #region DeleteProduct

     [Fact]
     public async Task DeleteProductHandler_ShouldThrowArgumentNullException_WhenDeleteProductCommandIsNull()
     {
         // Arrange
         ProductKey? productKey = null;
         DeleteProductCommand deleteProductCommand = new DeleteProductCommand() { ProductKey = productKey }; 
         // Act
         Func<Task> action = async () => await _deleteProductCommandHandler.Handle(deleteProductCommand, CancellationToken.None);
         
         // Assert
         await action.Should().ThrowAsync<ArgumentNullException>();
     }

     [Fact]
     public async Task DeleteProductHandler_ShouldReturnNull_WhenProductIsNotFound()
     {
         // Arrange
         ProductKey productKey = new ProductKey()
         {
             ManufactureEmail = "user@example.com",
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         DeleteProductCommand deleteProductCommand = new DeleteProductCommand() { ProductKey = productKey }; 
         
         // Mocking the Required Methods
         _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                               .ReturnsAsync(null as Product);
             
         // Act
         bool? isDeleted_fromHandler = await _deleteProductCommandHandler.Handle(deleteProductCommand, CancellationToken.None);

         // Assert
         isDeleted_fromHandler.Should().BeNull();
     }

     [Fact]
     public async Task DeleteProductHandler_ShouldDeleteProperObjectAndReturnTrue_WhenThereIsNoProblem()
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
             ProduceDate = DateTime.Parse("2024/06/01 19:00:00")
         };
         DeleteProductCommand deleteProductCommand = new DeleteProductCommand() { ProductKey = productKey }; 


         // Mocking the Required Methods
         _productRepositoryMock.Setup(entity => entity.GetProductByKey(It.IsAny<ProductKey>()))
                               .ReturnsAsync(product);
             
         _productRepositoryMock.Setup(entity => entity.DeleteProduct(It.IsAny<Product>()))
                               .ReturnsAsync(true);
             
         // Act
         bool? isDeleted_fromHandler = await _deleteProductCommandHandler.Handle(deleteProductCommand, CancellationToken.None);

         // Assert
         isDeleted_fromHandler.Should().BeTrue();
     }

     #endregion
}