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

    public void GroupItems(float maxGroupPrice)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        try
        {
            var items = _dbContext.Items
            .AsNoTracking()
            .Where(x => x.Status == ItemInfoStatusEnum.UnProcessed);

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
                            });
                            remainingQuantity -= quantityToAdd;

                            item.Quantity = remainingQuantity;

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
                        }
                    });

                        remainingQuantity -= quantityToAdd;
                    }

                    item.Quantity = remainingQuantity;
                    item.Status = item.Quantity == 0 ? ItemInfoStatusEnum.Processed : ItemInfoStatusEnum.UnProcessed;

                    _dbContext.Entry(item).State = EntityState.Modified;
                }
                _dbContext.SaveChanges();
            }

            foreach (var (index, group) in groups.Select((item, index) => (index, item)))
            {
                group.ForEach(x => x.Name = $"Группа {index + 1}");
            }

            _dbContext.SaveChanges();
            Insert(groups.SelectMany(x => x).ToList());
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw e;
        }
    }

    public void Insert(List<GroupedItem> groupedItems)
    {
        _dbContext.GroupedItems.AddRange(groupedItems);

        _dbContext.SaveChanges();
    }

    public async Task<GroupedItemDto> Get(int id)
    {
        var item = await _dbContext.GroupedItems.FindAsync(id);

        if (item is null)
        {
            throw new Exception("Группа под данным идентификатором не существует!");
        }

        var itemDto = new GroupedItemDto
        {
            Name = item.Name,
            TotalPrice = item.Quantity * item.Price
        };

        return itemDto;
    }
    public async Task<List<GroupedItemDto>> Get()
    {
        var groupedItemsQuery = await _dbContext.GroupedItems
        .GroupBy(x => x.Name)
        .AsNoTracking()
        .ToListAsync();

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
