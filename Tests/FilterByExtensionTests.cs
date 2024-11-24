using GenericRepository.Tests.Entities;
using Xunit;
using Xunit.Abstractions;

namespace GenericRepository.Tests;

public class FilterByExtensionTests(ITestOutputHelper testOutputHelper)
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
            Amount = 10.5m,
            SubEntity = new TestSubEntity
            {
                Id = 1,
                Name = "Sub A",
                Age = 22,
                TestEntity2 = new TestEntity2
                {
                    Id = 1,
                    Name = "Sub A2",
                    CreatedAt = DateTime.Now,
                    Password = "12fewfX!@#",
                    SubEntity = new TestSubEntity
                    {
                        Id = 1,
                        Name = "Sub Sub A3",
                        Age = 22
                    }
                }
            }
        },
        new()
        {
            Id = 2,
            Name = "B",
            IsActive = true,
            CreatedAt = DateTime.Now,
            Price = 20.5f,
            Amount = 20.5m,
            SubEntity = new TestSubEntity
            {
                Id = 2,
                Name = "Sub B",
                Age = 23
            }
        },
        new()
        {
            Id = 3,
            Name = "C",
            IsActive = false,
            CreatedAt = new DateTime(2022, 1, 1),
            Price = 30.5f,
            Amount = 30.5m,
            SubEntity = new TestSubEntity
            {
                Id = 3,
                Name = "Sub C",
                Age = 24
            }
        }
    ];

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

    // Filter By string property
    [Fact]
    public void FilterByName()
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

    // Filter by nested property
    [Fact]
    public void FilterByNestedProperty()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("SubEntity.Age GreaterThan 23")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Sub C", result[0].SubEntity.Name);

        testOutputHelper.WriteLine("Filtered By SubEntity.Age GreaterThan 23");
        result.ForEach(x => testOutputHelper.WriteLine($"{x.Name} - {x.SubEntity.Name}"));
    }

    // Filter by 2-level nested property
    [Fact]
    public void FilterBy2LevelNestedProperty()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("SubEntity.TestEntity2.Password Equal 12fewfX!@#")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);

        testOutputHelper.WriteLine("Filtered By SubEntity.TestEntity2.Password Equal 12fewfX!@#");

        result.ForEach(x =>
            testOutputHelper.WriteLine($"{x.Name} - {x.SubEntity.Name} - {x.SubEntity.TestEntity2?.Name}"));
    }

    // Filter by 3-level nested property
    [Fact]
    public void FilterBy3LevelNestedProperty()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("SubEntity.TestEntity2.SubEntity.Name Equal Sub Sub A3")
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);

        testOutputHelper.WriteLine("Filtered By SubEntity.TestEntity2.SubEntity.Name Equal Sub Sub A3");

        result.ForEach(x =>
            testOutputHelper.WriteLine($"{x.Name} - {x.SubEntity.Name} - {x.SubEntity.TestEntity2?.Name} - " +
                                       $"{x.SubEntity.TestEntity2?.SubEntity?.Name}"));
    }

    // Filter multiple selection for integer values
    [Fact]
    public void FilterByMultipleIdSelection()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy("Id Contains [2,3]")
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].Id);
        Assert.Equal(3, result[1].Id);

        testOutputHelper.WriteLine("Filtered by Id Contains [2,3]");

        result.ForEach(x => testOutputHelper.WriteLine($"{x.Id} - {x.Name}"));
    }

    // Filter by multiple params
    [Fact]
    public void FilterByMultipleParams()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .FilterBy(
                "Id Contains [1,2]",
                "SubEntity.Age Equal 22",
                "IsActive Equal true"
            )
            .ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
        Assert.Equal(22, result[0].SubEntity.Age);
        Assert.True(result[0].IsActive);

        testOutputHelper.WriteLine("Filtered By Multiple Params");
        result.ForEach(x => testOutputHelper.WriteLine(
            $"{x.Name} - ID: {x.Id} - Age: {x.SubEntity.Age} - IsActive: {x.IsActive}"));
    }
}