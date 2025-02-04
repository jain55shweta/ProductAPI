using DataAccess;
using Models;
using Moq;
using Services;
using Services.Interfaces;

namespace TestProject1
{
    public class Tests
    {
        private IProductManagementService _productService;
        private Mock<IProductRepository> _repository = new Mock<IProductRepository>();
        private ProductDetailsDTO _productDTO;
        [SetUp]
        public void Setup()
        {
            _productService = new ProductManagementService(_repository.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestUpdateProductStock(bool isAddStock)
        {
            //Assign Data
            var produtcId = 100037;
            var quantity = 11;
            var newquantity = 0;
            _productDTO = new ProductDetailsDTO
            {
                Id = produtcId,
                Name = "test",
                Description = "test",
                Price = 4,
                StockQuantity = 5
            };

            _repository.Setup(x => x.GetByIdAsync(produtcId)).Returns(Task.FromResult(_productDTO));
            if (isAddStock)
            {
                 newquantity = quantity + _productDTO.StockQuantity;
            }
            else {
                 newquantity = _productDTO.StockQuantity - quantity;
            }
            _repository.Setup(x => x.UpdateStockAsync(produtcId, newquantity)).Returns(Task.FromResult(true));
            //Action
            var result =_productService.UpdateProductStock(produtcId, quantity, isAddStock);

            //Asset
            Assert.IsTrue(result.Result);
        }
    }
}