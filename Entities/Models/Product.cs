namespace Entities.Models;

//Каждый продукт должен иметь атрибуты,
//такие как идентификатор (ID), название, описание, цена, доступность,
//ID пользователя создавшего товар, дата создания и т. д.

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public bool IsVisible { get; set; }
    public DateTime CreationTime { get; set; }
    
    public User User { get; set; }
    public Guid UserId { get; set; }
}