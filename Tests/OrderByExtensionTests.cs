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
            Amount = 10.5m,
            SubEntity = new TestSubEntity
            {
                Id = 1,
                Name = "Sub C",
                Age = 22,
                TestEntity2 = new TestEntity2
                {
                    Id = 1,
                    Name = "SubSub C",
                    CreatedAt = DateTime.Now,
                    Password = "12fewfX!@#"
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
                Name = "Sub A",
                Age = 24
            }
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

    [Fact]
    public void OrderByNestedProperty()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .OrderBy("SubEntity.Name DESC")
            .ToList();

        // Assert
        Assert.Equal("Sub C", result[0].SubEntity.Name);
        Assert.Equal("Sub B", result[1].SubEntity.Name);
        Assert.Equal("Sub A", result[2].SubEntity.Name);

        testOutputHelper.WriteLine("Ordered By SubEntity.Name DESC");
        result.ForEach(x => testOutputHelper.WriteLine($"{x.Name} - {x.SubEntity.Name}"));
    }

    [Fact]
    public void OrderBy2LevelNestedProperty()
    {
        // Arrange
        var queryable = _collection.AsQueryable();

        // Act
        var result = queryable
            .OrderBy("SubEntity.TestEntity2.Name DESC")
            .ToList();

        // Assert
        Assert.Equal("SubSub C", result[0].SubEntity.TestEntity2?.Name);

        testOutputHelper.WriteLine("Ordered By SubEntity.TestEntity2.Name DESC");
        result.ForEach(x =>
            testOutputHelper.WriteLine($"{x.Name} - {x.SubEntity.Name} - {x.SubEntity.TestEntity2?.Name}"));
    }
}