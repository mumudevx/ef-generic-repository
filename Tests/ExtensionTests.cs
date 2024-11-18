using Xunit;
using Xunit.Abstractions;

namespace GenericRepository.Tests;

internal class TestEntity
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required float Price { get; set; }
    public required decimal Amount { get; set; }
}

public class ExtensionTests(ITestOutputHelper testOutputHelper)
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
    public void OrderByTest()
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

    [Fact]
    public void FilterByTest()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Name Equal B")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("B", result[0].Name);

        testOutputHelper.WriteLine("Filtered By Name Equal B");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    // Filter By integer property
    [Fact]
    public void FilterById()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Id GreaterThan 1")
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("B", result[0].Name);
        Assert.Equal("C", result[1].Name);

        testOutputHelper.WriteLine("Filtered By Id GreaterThan 1");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    // Filter By boolean property
    [Fact]
    public void FilterByIsActive()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("IsActive Equal true")
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("B", result[1].Name);

        testOutputHelper.WriteLine("Filtered By IsActive Equal true");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    // Filter By DateTime property
    [Fact]
    public void FilterByCreatedAt()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("CreatedAt GreaterThan 2022-01-01")
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);

        testOutputHelper.WriteLine("Filtered By CreatedAt GreaterThan 2022-01-01");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    // Filter By float property
    [Fact]
    public void FilterByPrice()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Price GreaterThan 20.5")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("C", result[0].Name);

        testOutputHelper.WriteLine("Filtered By Price GreaterThan 20.5");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    // Filter By decimal property
    [Fact]
    public void FilterByAmount()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Amount GreaterThan 20.5")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("C", result[0].Name);

        testOutputHelper.WriteLine("Filtered By Amount GreaterThan 20.5");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }

    [Fact]
    public void FilterByAndOrderByTest()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Name NotEqual B")
            .OrderBy("Name DESC")
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("C", result[0].Name);
        Assert.Equal("A", result[1].Name);

        testOutputHelper.WriteLine("Filtered By Name NotEqual B and Ordered By Name DESC");
        result.ForEach(x => testOutputHelper.WriteLine(x.Name));
    }
}