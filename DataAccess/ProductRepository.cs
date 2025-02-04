using DataAccess.CustomException;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Models;
using Models.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDetailsDTO>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                if (products == null)
                {
                    throw new ProductNotFoundException($"Products not found.");
                }

                var productDTOs = products.Select(p => new ProductDetailsDTO
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                }).ToList();

                return productDTOs;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseConnectionException("A database connection error occurred.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving the product.", ex);
            }
        }

        public async Task<ProductDetailsDTO> GetByIdAsync(int id)
        {

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    throw new ProductNotFoundException($"Product with ID {id} not found.");
                }
                var productDTO = new ProductDetailsDTO
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                };

                return productDTO;
            } 
            catch (SqlException sqlEx)
            {
                throw new DatabaseConnectionException("A database connection error occurred.", sqlEx);
             }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving the product.", ex);
             }
        }

        public async Task<int> AddAsync(ProductDTO productDto)
        {
            try
            {
                var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                StockQuantity = productDto.StockQuantity,
            };
            
            var newId = GenerateProductId();
                using (var context = _context)
                {
                    if (newId > 0)
                        product.ProductId = (int)newId;

                    await context.Products.AddAsync(product);
                    var isAdded = (await _context.SaveChangesAsync() == 1);

                    if (!isAdded)
                    {
                        return 0;
                    }
                }
                return product.ProductId;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DatabaseOperationException("Error occurred while saving the product.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the product.", ex);
            }
        }

        public async Task<bool> UpdateAsync(int id, ProductDTO productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    throw new ProductNotFoundException($"Product with ID {id} not found.");
                }
                product.Name = productDto.Name;
                product.Price = productDto.Price;
                product.Description = productDto.Description;
                product.StockQuantity = productDto.StockQuantity;

                var isUpdated = (await _context.SaveChangesAsync() == 1);
                return isUpdated;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DatabaseOperationException("Error occurred while updating the product.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the product.", ex);
            }
        }

        public async Task<bool> UpdateStockAsync(int id,int newQuantity)
        {
            try
            {
                var product = await _context.Products?.Where(p => p.ProductId == id)?.FirstOrDefaultAsync();

            if (product == null)
            {
                    throw new ProductNotFoundException($"Product with ID {id} not found.");
                }

            product.StockQuantity = newQuantity;

            var isUpdated = (await _context.SaveChangesAsync() == 1);
            return isUpdated;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DatabaseOperationException("Error occurred while updating the product.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the product.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                    throw new ProductNotFoundException($"Product with ID {id} not found.");
                }
            if (_context.Entry(product).State == EntityState.Detached)
            {
                _context.Products.Attach(product);
            }
            _context.Products.Remove(product);
            var isDeleted = (await _context.SaveChangesAsync() == 1);
            return isDeleted;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DatabaseOperationException("Error occurred while deleting the product.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the product.", ex);
            }
        }

        private long GenerateProductId()
        {
            try
            {
                var nextProductId = _context.Database.SqlQueryRaw<long>("SELECT NEXT VALUE FOR ProductIdSequence AS 'id'").ToList();
                return (nextProductId.FirstOrDefault());
            }
            catch (DbUpdateException dbEx)
            {
                throw new DatabaseOperationException("Error occurred while deleting the product.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding the product.", ex);
            }
        }

    }
}
