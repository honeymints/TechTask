using Microsoft.AspNetCore.Mvc;
using TechTask.Services;

namespace TechTask.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly ItemService _itemService;
    public ItemController(ItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost("Insert")]
    public async Task<IActionResult> Create(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл пустой.");
        }

        await _itemService.ParseDataFromExcel(file, cancellationToken);

        return Created();
    }

    // [HttpPost]
}