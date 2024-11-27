using ClosedXML.Excel;
using Ganss.Excel;
using Microsoft.EntityFrameworkCore;
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

    public async Task InsertItemInfo(List<ItemInfoInput> itemInfoInputs)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var itemInfos = itemInfoInputs.Select(x => new ItemInfo
                {
                    Name = x.Name,

                    UnitOfMeasurement = x.UnitOfMeasurement,

                    Cost = x.Cost,

                    Quantity = x.Quantity,

                    Status = ItemInfoStatusEnum.Raw,
                }).ToList();

                await _dbContext.AddRangeAsync(itemInfos);

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();

            }
        }
    }

    public async Task<List<ItemInfoInput>> ParseDataFromExcel(IFormFile file, CancellationToken cancellationToken)
    {

        var items = new List<ItemInfoInput>() { };
        var filePath = Path.GetTempFileName();

        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        IWorkbook workbook;

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            workbook = new XSSFWorkbook(fileStream);

            var itemsInput = new ExcelMapper(filePath).Fetch<ItemInfoInput>().ToList();
            items.AddRange(itemsInput);
        }
        return items;
    }

    public async Task CheckStatusOfItemInfo(ItemInfo itemInfo)
    {

    }

    public string? GroupItemsByCostNotAbove200Euros()
    {
        var items = _dbContext.Items.AsNoTracking().AsEnumerable();


        var splitItems = items.SelectMany(item =>
        Enumerable.Range(1, item.Quantity).Select(_ => new
        {
            item.Cost,
            item.Name,
            Unit = 1
        }));

        var groupedItems = splitItems.GroupBy(x=>x.Name).ToList();

        var jsonSplitItems = JsonConvert.SerializeObject(groupedItems);
        
        return jsonSplitItems;
        // {

        // }
        // items.ForEach(x =>
        // {
        //     x.Cost
        // });
    }
}