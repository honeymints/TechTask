using Microsoft.EntityFrameworkCore;
using TechTask.Dtos;
using TechTask.Persistence;

namespace TechTask.Services;

public class GroupedItemService
{
    private readonly AppDbContext _dbContext;
    public GroupedItemService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task GroupItems(float maxGroupPrice)
    {
        var items = _dbContext.Items.AsNoTracking();

        if (await items.AnyAsync(x => x.Status == ItemInfoStatusEnum.Processed))
        {
            throw new Exception("один из товаров уже был обработан!");
        }

        var groups = new List<List<GroupedItem>>();

        var sortedItems = items
        .OrderByDescending(item => item.Cost)
        .ToList();

        foreach (var item in sortedItems)
        {
            var remainingQuantity = item.Quantity;

            while (remainingQuantity > 0)
            {
                bool addedToGroup = false;

                foreach (var group in groups)
                {
                    var currentGroupPrice = group.Sum(g => g.Price * g.Quantity);
                    var spaceAvailable = maxGroupPrice - currentGroupPrice;

                    if (spaceAvailable >= item.Cost)
                    {
                        var quantityToAdd = (int)Math.Min(remainingQuantity, Math.Floor(spaceAvailable / item.Cost));
                        group.Add(new GroupedItem
                        {
                            Price = item.Cost,
                            Quantity = quantityToAdd,
                            Name = $"Группа {groups.Count + 1}"
                        });
                        remainingQuantity -= quantityToAdd;

                        item.Cost = remainingQuantity;

                        addedToGroup = true;
                        break;
                    }
                }

                if (!addedToGroup)
                {
                    var quantityToAdd = (int)Math.Min(remainingQuantity, Math.Floor(maxGroupPrice / item.Cost));
                    groups.Add(new List<GroupedItem>
                    {
                        new GroupedItem
                        {
                            Price = item.Cost,
                            Quantity = quantityToAdd,
                            Name = $"Группа {groups.Count + 1}"
                        }
                    });
                    remainingQuantity -= quantityToAdd;

                    item.Quantity = remainingQuantity;
                    item.Status = item.Quantity == 0 ? ItemInfoStatusEnum.Processed : ItemInfoStatusEnum.UnProcessed;
                }
            }
        }

        await Insert(groups.SelectMany(x => x).ToList());
    }

    public async Task Insert(List<GroupedItem> groupedItems)
    {
        await _dbContext.GroupedItems.AddRangeAsync(groupedItems);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<GroupedItem> Get(int id)
    {
        var item = await _dbContext.GroupedItems.FindAsync(id);

        if (item is null)
        {
            throw new Exception("Группа под данным идентификатором не существует!");
        }

        return item;
    }
    public List<GroupedItemDto> Get()
    {
        var groupedItemsQuery = _dbContext.GroupedItems
        .GroupBy(x => x.Name).AsNoTracking();

        var groupedItems = groupedItemsQuery
        .AsEnumerable()
        .Select(x => new GroupedItemDto
        {
            Name = x.Key,
            TotalPrice = x.Sum(y => y.Price * y.Quantity)
        }).ToList();

        return groupedItems;
    }
}
