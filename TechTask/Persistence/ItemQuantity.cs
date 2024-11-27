namespace TechTask.Persistence;


public class ItemQuantity
{
    public int Id { get; set; }

    public int ItemInfoId { get; set; }

    public int Quantity { get; set; }

    public ItemInfo ItemInfo { get; set; }
}