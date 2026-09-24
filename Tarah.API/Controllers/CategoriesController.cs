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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AllCategories()
        {
            var result = await service.AllCategories();
            return Ok(result);
        }



        [HttpGet]
        [Route("{categoryId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsByCategory([FromRoute] Guid categoryId)
        {
            return RedirectToAction(nameof(GetProductsByCategory), categoryId);
        }



        [HttpGet]
        [Route("{categoryId:guid}/products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsByCategory([FromRoute]Guid categoryId, 
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var response = await service.CategoryProductsAsync(categoryId, pageNumber ?? 1, pageSize ?? 10);

            if (response.Status == Status.NotFound)
                return NotFound(response.Message);

            return Ok(response.Result);
        }

    }
}
