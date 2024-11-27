using ClosedXML.Excel;
using Ganss.Excel;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TechTask.Dtos;
using TechTask.Persistence;

namespace TechTask.Services;

public class ItemService
{
    private readonly AppDbContext _dbContext;

    public ItemService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertItemInfo(List<ItemInfo> itemInfo)
    {
        await _dbContext.AddRangeAsync(itemInfo);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ParseDataFromExcel(IFormFile file, CancellationToken cancellationToken)
    {

        var data = new List<Dictionary<string, string>>();

        IWorkbook workbook;
        using (FileStream fileStream = new FileStream("Data/Temp" + file.FileName, FileMode.Open, FileAccess.Read))
        {
            workbook = new XSSFWorkbook(fileStream);

            await file.CopyToAsync(fileStream, cancellationToken);

            var products = new ExcelMapper(file.FileName).Fetch();

        }
    }

    public async Task CheckStatusOfItemInfo(ItemInfo itemInfo)
    {

    }

    public async Task GroupItemsByCostNotAbove200Euros()
    {

    }
}