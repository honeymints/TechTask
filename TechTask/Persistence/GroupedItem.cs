using System.ComponentModel.DataAnnotations;

namespace TechTask.Persistence;


public class GroupedItem
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public float Price { get; set; }

    public int Quantity { get; set; }
}