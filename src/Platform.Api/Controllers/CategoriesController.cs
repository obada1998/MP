using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Categories;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/stores/{storeId:guid}/categories")]
[Authorize]
public sealed class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> Get(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await categoryService.GetCategoriesAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(Guid storeId, UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await categoryService.CreateCategoryAsync(CurrentUserId, storeId, request, cancellationToken));
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid storeId, Guid categoryId, UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await categoryService.UpdateCategoryAsync(CurrentUserId, storeId, categoryId, request, cancellationToken));
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(Guid storeId, Guid categoryId, CancellationToken cancellationToken)
    {
        await categoryService.DeleteCategoryAsync(CurrentUserId, storeId, categoryId, cancellationToken);
        return NoContent();
    }
}
