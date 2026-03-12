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

        public async Task<ServiceResponse<ListProductsDto>> AllProductsAsync(Guid categoryId,int page, int pageSize)
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
            if (userId == Guid.Empty)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "Anonymous request"
                };

            var product = mapper.Map<Product>(dto);

            if(!dto.CategoryIds.IsNullOrEmpty())
            {
                var existingCats = await categoriesRepository.AllCategories();

                product.Categories = existingCats
                    .Where(c => dto.CategoryIds.Any(i => c.Id == i))
                    .Select(c => new CategoryItem
                    {
                        CategoryId = c.Id,
                    })
                    .ToList();
            }

            foreach (var image in dto.Images)
            {
                var validation = isValidImage(image);
                if (!validation.Result)
                    return new ServiceResponse<ProductDto> { Message = validation.Message };
            }

            var images = new List<string>();
            foreach (var image in dto.Images)
                images.Add(await productsRepository.SaveImage(image));

            product.Images = images;
            product.SellerId = userId;

            var response = await productsRepository.AddProductAsync(product);

            return response != null ?
                new ServiceResponse<ProductDto>
                {
                    Status = Status.Success,
                    Result = mapper.Map<ProductDto>(response)
                } :
                new ServiceResponse<ProductDto>
                {
                    Status = Status.Error,
                    Message = "Something went wrong"
                };
        }

        public async Task<ServiceResponse<ProductDto>> UpdateProductAsync(Guid id, UpdateProductDto dto,Guid userId)
        {
            var productInDb = await productsRepository.GetByIdAsync(id);

            if (productInDb==null)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.NotFound,
                    Message = "Product not found"
                };

            if (productInDb.SellerId != userId)
                return new ServiceResponse<ProductDto>
                {
                    Status = Status.Forbidden,
                    Message = "You don't own this product"
                };

            var imagesToAdd = dto.Images
                .Where(i =>
                !productInDb.Images.Any(dbimg => dbimg == i.FileName))
                .ToList();
            var imagesToDelete = productInDb.Images
                .Where(dbimg =>
                !dto.Images.Any(i => i.FileName == dbimg))
                .ToList();
            foreach ( var image in imagesToAdd )
            {
                var validation = isValidImage(image);
                if (validation.Status == Status.Error)
                    return new ServiceResponse<ProductDto>
                    {
                        Status = validation.Status,
                        Message = validation.Message
                    };

                productInDb.Images.Add(
                    await productsRepository.SaveImage(image));
            }
            foreach( var image in imagesToDelete )
            {
                productsRepository.DeleteImage(image);
                productInDb.Images.Remove(image);
            }

            if (!dto.CategoryIds.IsNullOrEmpty())
            {
                var existingCats = await categoriesRepository.AllCategories();

                productInDb.Categories = existingCats.Select(c => new CategoryItem
                {
                    CategoryId = c.Id,
                }).ToList();
            }

            productInDb = mapper.Map<Product>(dto);

            var response = await productsRepository.UpdateProductAsync(id, productInDb);

            return new ServiceResponse<ProductDto>
            {
                Status = Status.Success,
                Result = mapper.Map<ProductDto>(response),
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
                    Status = Status.Forbidden,
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
                    Status = Status.Error,
                    Message = "Image larger than 5MB"
                };

            var supported = new List<string>() { ".jpg", ".png", ".jpeg" };

            var extension = Path.GetExtension(file.FileName);
            if (!supported.Any(item => item.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                return new ServiceResponse<bool>()
                {
                    Status = Status.Error,
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
