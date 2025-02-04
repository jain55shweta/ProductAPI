using Models;

namespace Services.Interfaces
{
   public interface IProductManagementService
    {
         Task<List<ProductDetailsDTO>> GetAllProducts();

        Task<ProductDetailsDTO> GetProductById(int id);

        Task<int> CreateProduct(ProductDTO product);

        Task<bool> UpdateProduct(int id, ProductDTO product);
        Task<bool> UpdateProductStock(int id, int quantity, bool isAddStock);

        Task<bool> DeleteProduct(int id);
    }
}
