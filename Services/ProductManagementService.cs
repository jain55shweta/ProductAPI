using DataAccess;
using Models;
using Models.CustomException;
using Services.Interfaces;

namespace Services
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly IProductRepository _productRepository;

        public ProductManagementService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDetailsDTO>> GetAllProducts()
        {
            var productDTOs =  await _productRepository.GetAllAsync();

            return productDTOs;
        }

        public async Task<ProductDetailsDTO> GetProductById(int id)
        {
            var productDTOs = await _productRepository.GetByIdAsync(id);
           
            return productDTOs;
        }

        public async Task<int> CreateProduct(ProductDTO product)
        {
            var productId = await _productRepository.AddAsync(product);
            return productId;
        }

        public async Task<bool> UpdateProduct(int id, ProductDTO product)
        {
            var isUpdated = await _productRepository.UpdateAsync(id, product);
            return isUpdated;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var isDeleted = await _productRepository.DeleteAsync(id);
            return isDeleted;
        }
        public async Task<bool> UpdateProductStock(int id, int quantity, bool isAddStock)
        {
            try
            {
                var productDTOs = await _productRepository.GetByIdAsync(id);
                if (productDTOs == null)
                {
                    throw new ProductNotFoundException($"Product with ID {id} was not found.");

                }

                if (isAddStock)
                {
                    productDTOs.StockQuantity += quantity;
                }
                else
                {
                    productDTOs.StockQuantity -= quantity;
                }
                var isUpdated = await _productRepository.UpdateStockAsync(id, productDTOs.StockQuantity);
                return isUpdated;
            } 
             catch (Exception ex)
             {
            // Handle any other exceptions
            throw new Exception("An error occurred while creating the product.", ex);
              }

        }
    }
}
