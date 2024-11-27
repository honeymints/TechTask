using Ganss.Excel;

namespace TechTask.Dtos;


public class ItemInfoInput
{
    [Column("Наименование")]
    public string Name { get; set; }

    [Column("Единица измерения")]
    public string UnitOfMeasurement { get; set; }

    [Column("Цена за единицу, евро")]
    public string Cost { get; set; }

    [Column("Количество, шт.")]
    public string Quantity { get; set; }
}