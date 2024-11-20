using GenericRepository.Tests.Entities;
using Xunit;
using Xunit.Abstractions;

namespace GenericRepository.Tests;

public class OrderByExtensionTests(ITestOutputHelper testOutputHelper)
{
    private readonly List<TestEntity> _collection =
    [
        new()
        {
            Id = 1,
            Name = "A",
            IsActive = true,
            CreatedAt = DateTime.Now,
            Price = 10.5f,
            Amount = 10.5m
        },
        new()
        {
            Id = 2,
            Name = "B",
            IsActive = true,
            CreatedAt = DateTime.Now,
            Price = 20.5f,
            Amount = 20.5m
        },
        new()
        {
            Id = 3,
            Name = "C",
            IsActive = false,
            CreatedAt = new DateTime(2022, 1, 1),
            Price = 30.5f,
            Amount = 30.5m
        }
    ];
    
    [Fact]
    public void OrderByNameAsc()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .OrderBy("Name")
            .ToList();

        // Assert
        Assert.Equal("A", result[0].Name);
        Assert.Equal("B", result[1].Name);
        Assert.Equal("C", result[2].Name);

        testOutputHelper.WriteLine("Ordered By Name ASC");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }
    
    [Fact]
    public void OrderByNameDesc()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .OrderBy("Name DESC")
            .ToList();

        // Assert
        Assert.Equal("C", result[0].Name);
        Assert.Equal("B", result[1].Name);
        Assert.Equal("A", result[2].Name);

        testOutputHelper.WriteLine("Ordered By Name DESC");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }
}