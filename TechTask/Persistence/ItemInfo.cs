
using System.ComponentModel.DataAnnotations;
using Ganss.Excel;

namespace TechTask.Persistence;

public class ItemInfo
{
    [Key]
    public int Id { get; set; }
   
    public string Name { get; set; }

    public string UnitOfMeasurement { get; set; }

    public float Cost { get; set; }

    public int Quantity { get; set; }

    public ItemInfoStatusEnum Status { get; set; }

}

public enum ItemInfoStatusEnum
{
    UnProcessed = 1,
    Processed = 2
}