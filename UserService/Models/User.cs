namespace UserService.Models;

public class User
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}