namespace GenericRepository.Tests.Entities;

public class TestEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required float Price { get; set; }
    public required decimal Amount { get; set; }

    public required TestSubEntity SubEntity { get; set; }
}