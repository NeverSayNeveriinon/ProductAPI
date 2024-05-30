using Moq;

using Core.Domain.RepositoryContracts;
using Core.ServiceContracts;
using Core.Services;

namespace XUnit_Tests.UnitTest.Services;

public class ProductServiceTest
{
    private readonly IProductService _productService;
        
    private readonly IProductRepository _productRepository;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public ProductServiceTest()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productRepository = _productRepositoryMock.Object;

        _productService = new ProductService(_productRepository);
    }


}