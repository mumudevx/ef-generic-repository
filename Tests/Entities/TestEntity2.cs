namespace GenericRepository.Tests.Entities;

public class TestEntity2
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required string Password { get; set; }
    
    public TestSubEntity? SubEntity { get; set; }
}