using DataAccess;
using DataAccess.CustomException;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.CustomException;
using Services.Interfaces;

namespace ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductMangementController : ControllerBase
    {
        private readonly IProductManagementService _productService;
        private readonly ILogger<ProductMangementController> _logger;

        public ProductMangementController(IProductManagementService productService, ILogger<ProductMangementController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductDetailsDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (ProductNotFoundException pnfe)
            {
                // Log the exception and return 404 Not Found response
                _logger.LogError(pnfe, "Product not found.");
                return NotFound(new { message = pnfe.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }

        ///// <summary>
        ///// GET: api/product/{id}
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
            
            return Ok(product);
            }
            catch (ProductNotFoundException pnfe)
            {
                _logger.LogError(pnfe, "Product not found.");
                return NotFound(new { message = pnfe.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// POST: api/product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductDTO product)
        {
            try
            {
                if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
                {
                    return BadRequest(new { message = "Request failed! Invalid data" });
                }

               var productId = await _productService.CreateProduct(product);
                if (productId <= 0) {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Invalid product ID. Please provide a valid ID."
                    });
                }
            return CreatedAtAction(nameof(GetProduct), new { id = productId }, product);
            }
            catch (DatabaseOperationException dbEx)
            {
                _logger.LogError(dbEx, "Error occurred while creating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "A database error occurred while creating the product. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// PUT: api/product/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO product)
        {
            try
            {
                if (product == null || id < 0)
                {
                    return BadRequest(new { message = "Product data is invalid" });
                }

                await _productService.UpdateProduct(id, product);

                return Ok(new { message = $"Product Record with Id {id} Updated succesfully" });
            }
            catch (ProductNotFoundException pnfe)
            {
                _logger.LogError(pnfe, "Product not found.");
                return NotFound(new { message = pnfe.Message });
            }
            catch (DatabaseOperationException dbEx)
            {
                // Log and return a custom error response
                _logger.LogError(dbEx, "Error occurred while updating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "A database error occurred while creating the product. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// DELETE: api/product/{id} 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProduct(id);
            if (!success)
            {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Invalid product ID. Please provide a valid ID."
                    });
                }

            return Ok(new { message = $"Product Record with Id {id} deleted succesfully" });
            }
            catch (ProductNotFoundException pnfe)
            {
                _logger.LogError(pnfe, "Product not found.");
                return NotFound(new { message = pnfe.Message });
            }
            catch (DatabaseOperationException dbEx)
            {
                // Log and return a custom error response
                _logger.LogError(dbEx, "Error occurred while updating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "A database error occurred while creating the product. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating the product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }

        ///PUT : api/products/decrement-stock/{id}/{quantity} <summary>
        /// PUT : api/products/decrement-stock/{id}/{quantity}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("/decrement-stock/{id}/{quantity}")]
        public async Task<IActionResult> UpdateProductDecStock(int id, int quantity)
        {
            if (int.IsNegative(quantity) || int.IsNegative(id))
            {
                return BadRequest(new { message = "Product data is invalid" });
            }
            var isAddStock = false;
            var isStockUpdated = await _productService.UpdateProductStock(id, quantity, isAddStock);
            if (!isStockUpdated)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Invalid product ID. Please provide a valid ID."
                });
            }

            return Ok(new { message = $"Product Stock with Id {id} Updated succesfully" }); 
        }

        /// <summary>
        /// PUT : api/products/add-to-stock/{id}/{quantity} 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("/add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> UpdateProductAddStock(int id, int quantity)
        {
            if (int.IsNegative(quantity) || int.IsNegative(id))
            {
                return BadRequest(new { message = "Product data is invalid" });
            }
            var isAddStock = true;
            var isStockUpdated = await _productService.UpdateProductStock(id, quantity, isAddStock);
            if (!isStockUpdated)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Invalid product ID. Please provide a valid ID."
                });
            }

            return Ok(new { message = $"Product Stock with Id {id} Updated succesfully" }); ;
        }
    }
}
