using Microsoft.AspNetCore.Mvc;

namespace TechTask.Controllers;

[ApiController]
[Route("[controller]")]
public class ExcelController : ControllerBase
{
    public ExcelController()
    {

    }

    // [HttpPost]
    // public async Task<IActionResult> ReadFile(IFormFile file)
    // {
    //     if (file == null || file.Length == 0)
    //     {
    //         return BadRequest("Файл пустой.");
    //     }


    // }
}