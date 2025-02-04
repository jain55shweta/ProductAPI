using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IProductRepository
    {
        Task<List<ProductDetailsDTO>> GetAllAsync();
        Task<ProductDetailsDTO> GetByIdAsync(int id);
        Task<int> AddAsync(ProductDTO product);
        Task<bool> UpdateAsync(int id, ProductDTO product);
        Task<bool> UpdateStockAsync(int id, int newQuantity);
        Task<bool> DeleteAsync(int id);
    }
}
