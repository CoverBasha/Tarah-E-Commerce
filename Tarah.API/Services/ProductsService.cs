using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Tarah.API.Models.Domain;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;
using Tarah.API.Services;

namespace Tarah.API.Service
{
    public class ProductsService
    {
        private readonly IProductsRepository productsRepository;
        private readonly ICategoriesRepository categoriesRepository;
        private readonly IMapper mapper;

        public ProductsService(IProductsRepository repository,
            ICategoriesRepository categoriesRepository,
            IMapper mapper)
        {
            productsRepository = repository;
            this.categoriesRepository = categoriesRepository;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<ListProductsDto>> AllProductsAsync(Guid? categoryId,int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Min(pageSize, 20);

            var result = await productsRepository.GetProductsAsync(categoryId, page, pageSize);

            return new ServiceResponse<ListProductsDto>()
            {
                Status = Status.Success,
                Result = new ListProductsDto
                {
                    Dtos = mapper.Map<List<ProductDto>>(result.Items),
                    Count = result.TotalPages
                }
            };
        }

        public async Task<ServiceResponse<ProductDto>> ProductByIdAsync(Guid id)
        {
            var response = await productsRepository.GetByIdAsync(id);

            return new ServiceResponse<ProductDto>
            {
                Status = response == null ? Status.NotFound : Status.Success,
                Result = response == null ? null : mapper.Map<ProductDto>(response),
                Message = response == null ? "Product not found" : ""
            };
        }

        public async Task<ServiceResponse<ProductDto>> AddProductAsync(AddProductDto dto,Guid userId)
        {
            if (dto.Price < dto.PriceAfterSale)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Price after sale can't be higher than price"
                };
            if (dto.CategoryIds.IsNullOrEmpty())
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Product should have atleast one category"
                };

            var product = mapper.Map<Product>(dto);
            
            product.SellerId = userId;

            var categories = await categoriesRepository.GetByIds(dto.CategoryIds);
            if (!categories.Any())
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.NotFound,
                    Message = "Categories not found"
                };

            product.Categories = categories
                .Select(c => new CategoryItem { CategoryId = c.Id })
                .ToList();

            #region Save Images

            foreach (var image in dto.Images)
            {
                var validation = isValidImage(image);
                if (!validation.Result)
                    return new ServiceResponse<ProductDto>
                    {
                        Status = Status.Forbidden,
                        Message = validation.Message
                    };
            }

            var images = new List<string>();
            foreach (var image in dto.Images)
            {
                var name = await productsRepository.SaveImage(image);
                product.Images.Add(name);
                images.Add(name);
            }

            #endregion

            Product response = new();
            try
            {
                response = await productsRepository.AddProductAsync(product);
            }
            catch (Exception ex)
            {
                foreach(var image in images)
                    productsRepository.DeleteImage(image);

                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = ex.Message
                };
            }

            return response != null ?
                new ServiceResponse<ProductDto>
                {
                    Status = Status.Success,
                    Result = mapper.Map<ProductDto>(response)
                } :
                new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Something went wrong"
                };
        }

        public async Task<ServiceResponse<ProductDto>> UpdateProductAsync(Guid id, UpdateProductDto dto, Guid userId)
        {
            if (dto.Price < dto.PriceAfterSale)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Price after sale can't be higher than price"
                };

            var productInDb = await productsRepository.GetByIdAsync(id);

            if (productInDb == null)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.NotFound,
                    Message = "Product not found"
                };
            if (productInDb.SellerId != userId)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Unauthorized,
                    Message = "You don't own this product"
                };
            if (dto.CategoryIds.IsNullOrEmpty())
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Product should have atleast one category"
                };

            mapper.Map(dto, productInDb);

            var categories = await categoriesRepository.GetByIds(dto.CategoryIds);
            if (!categories.Any())
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.NotFound,
                    Message = "Categories not found"
                };

            productInDb.Categories = categories
                .Select(c => new CategoryItem { CategoryId = c.Id })
                .ToList();

            #region Images Operations

            var incomingFilesByName = dto.Images
                .ToDictionary(i => i.FileName.Trim().ToLower(), i => i);

            var incomingNames = incomingFilesByName.Keys.ToHashSet();

            var existingNames = productInDb.Images
                .Select(i => i.Trim().ToLower())
                .ToHashSet();

            var toAdd = incomingNames.Except(existingNames);
            var toDelete = existingNames.Except(incomingNames);

            foreach (var name in toAdd)
            {
                var validation = isValidImage(incomingFilesByName[name]);
                if (validation.Status == Status.Forbidden)
                    return new ServiceResponse<ProductDto>
                    {
                        Status = validation.Status,
                        Message = validation.Message
                    };
            }
            var addedImages = new List<string>();

            foreach (var name in toAdd)
            {
                var saved = await productsRepository.SaveImage(incomingFilesByName[name]);
                productInDb.Images.Add(saved);
                addedImages.Add(name);
            }

            try
            {
                await productsRepository.UpdateProductAsync();
            }
            catch (Exception ex)
            {
                foreach (var name in addedImages)
                    productsRepository.DeleteImage(name);

                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = ex.Message
                };
            }

            foreach (var name in toDelete)
            {

                productsRepository.DeleteImage(name);
                productInDb.Images.Remove(name);
            }

            #endregion

            return new ServiceResponse<ProductDto>
            {
                Status = Status.Success,
                Result = mapper.Map<ProductDto>(productInDb),
            };
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(Guid id, Guid userId)
        {
            var product = await productsRepository.GetByIdAsync(id);
            if (product == null)
                return new ServiceResponse<bool>
                {
                    Status = Status.NotFound,
                    Message = "Product not found"
                };

            if (product.SellerId != userId)
                return new ServiceResponse<bool>
                {
                    Status = Status.Unauthorized,
                    Message = "You don't own this product"
                };

            var reponse = await productsRepository.DeleteAsync(product);

            return new ServiceResponse<bool>
            {
                Status = Status.Success,
                Result = reponse,
            };
        }

        private static ServiceResponse<bool> isValidImage(IFormFile file)
        {
            if (file.Length > 5242880)
                return new ServiceResponse<bool>()
                {
                    Status = Status.Forbidden,
                    Message = "Image larger than 5MB"
                };

            var supported = new List<string>() { ".jpg", ".png", ".jpeg" };

            var extension = Path.GetExtension(file.FileName);
            if (!supported.Any(item => item.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                return new ServiceResponse<bool>()
                {
                    Status = Status.Forbidden,
                    Message = "File not supported, only .jpg, .png, .jpeg"
                };

            return new ServiceResponse<bool>
            {
                Status = Status.Success,
                Result = true
            };
        }
    }
}
