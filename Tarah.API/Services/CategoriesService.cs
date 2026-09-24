using AutoMapper;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;

namespace Tarah.API.Services
{
    public class CategoriesService
    {
        private readonly ICategoriesRepository categoriesRepository;
        private readonly IProductsRepository productsRepository;
        private readonly IMapper mapper;

        public CategoriesService(ICategoriesRepository repository,IProductsRepository productsRepository,IMapper mapper)
        {
            this.categoriesRepository = repository;
            this.productsRepository = productsRepository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDto>> AllCategories()
        {
            var categories = await categoriesRepository.AllCategories();

            return mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<ServiceResponse<ListProductsDto>> CategoryProductsAsync(Guid categoryId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Min(pageSize, 20);

            var category = await categoriesRepository.CategoryById(categoryId);

            if (category is null)
                return new ServiceResponse<ListProductsDto>()
                {
                    Status = Status.NotFound,
                    Message = "Category not found"
                };


            var result = await categoriesRepository.GetProductsByCategoryAsync(categoryId, page, pageSize);

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

    }
}
