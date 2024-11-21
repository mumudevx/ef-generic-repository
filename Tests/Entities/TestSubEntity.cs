namespace GenericRepository.Tests.Entities;

public class TestSubEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Age { get; set; }
    
    public TestEntity2? TestEntity2 { get; set; }
}