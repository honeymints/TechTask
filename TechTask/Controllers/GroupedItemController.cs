using Microsoft.AspNetCore.Mvc;
using TechTask.Services;

namespace TechTask.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupedItemController : ControllerBase
{
    private readonly GroupedItemService _groupedItemService;
    public GroupedItemController(GroupedItemService groupedItemService)
    {
        _groupedItemService = groupedItemService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var groupedItemDtos = await _groupedItemService.Get();

        return Ok(groupedItemDtos);
    }

    [HttpGet("Get/{id}")]
    public async Task<IActionResult> GetAll(int id)
    {
        var groupedItemDto = await  _groupedItemService.Get(id);
        
        return Ok(groupedItemDto);
    }
}