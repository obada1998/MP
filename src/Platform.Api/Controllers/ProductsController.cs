using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Products;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/stores/{storeId:guid}/products")]
[Authorize]
public sealed class ProductsController(IProductService productService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductDto>>> GetProducts(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await productService.GetProductsAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid storeId, Guid productId, CancellationToken cancellationToken)
    {
        return Ok(await productService.GetProductAsync(CurrentUserId, storeId, productId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(Guid storeId, UpsertProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.CreateProductAsync(CurrentUserId, storeId, request, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { storeId, productId = product.Id }, product);
    }

    [HttpPut("{productId:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid storeId, Guid productId, UpsertProductRequest request, CancellationToken cancellationToken)
    {
        return Ok(await productService.UpdateProductAsync(CurrentUserId, storeId, productId, request, cancellationToken));
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid storeId, Guid productId, CancellationToken cancellationToken)
    {
        await productService.DeleteProductAsync(CurrentUserId, storeId, productId, cancellationToken);
        return NoContent();
    }

    [HttpGet("field-definitions")]
    public async Task<ActionResult<IReadOnlyCollection<ProductFieldDefinitionDto>>> GetFields(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await productService.GetFieldDefinitionsAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPost("field-definitions")]
    public async Task<ActionResult<ProductFieldDefinitionDto>> CreateField(Guid storeId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken)
    {
        return Ok(await productService.CreateFieldDefinitionAsync(CurrentUserId, storeId, request, cancellationToken));
    }

    [HttpPut("field-definitions/{fieldId:guid}")]
    public async Task<ActionResult<ProductFieldDefinitionDto>> UpdateField(Guid storeId, Guid fieldId, UpsertProductFieldDefinitionRequest request, CancellationToken cancellationToken)
    {
        return Ok(await productService.UpdateFieldDefinitionAsync(CurrentUserId, storeId, fieldId, request, cancellationToken));
    }

    [HttpDelete("field-definitions/{fieldId:guid}")]
    public async Task<IActionResult> DeleteField(Guid storeId, Guid fieldId, CancellationToken cancellationToken)
    {
        await productService.DeleteFieldDefinitionAsync(CurrentUserId, storeId, fieldId, cancellationToken);
        return NoContent();
    }
}
