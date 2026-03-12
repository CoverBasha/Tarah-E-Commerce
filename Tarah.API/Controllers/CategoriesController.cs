using Microsoft.AspNetCore.Mvc;
using Tarah.API.Services;

namespace Tarah.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoriesService service;

        public CategoriesController(CategoriesService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> AllCategories()
        {
            var result = await service.AllCategories();
            return Ok(result);
        }
    }
}
