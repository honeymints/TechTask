
using Ganss.Excel;

namespace TechTask.Persistence;

public class ItemInfo
{
    public int Id { get; set; }
   
    public string Name { get; set; }

    public string UnitOfMeasurement { get; set; }

    public float Cost { get; set; }

    public float Quantity { get; set; }

    public ItemInfoStatusEnum Status { get; set; }

}

public enum ItemInfoStatusEnum
{
    Raw = 1,
    Processed = 2
}