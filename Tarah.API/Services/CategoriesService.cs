using AutoMapper;
using Tarah.API.Models.DTOs;
using Tarah.API.Repositories;

namespace Tarah.API.Services
{
    public class CategoriesService
    {
        private readonly ICategoriesRepository repository;
        private readonly IMapper mapper;

        public CategoriesService(ICategoriesRepository repository,IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDto>> AllCategories()
        {
            var categories = await repository.AllCategories();

            return mapper.Map<List<CategoryDto>>(categories);
        }

    }
}
