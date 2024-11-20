using GenericRepository.Tests.Entities;
using Xunit;
using Xunit.Abstractions;

namespace GenericRepository.Tests;

public class ExcludePropertiesExtensionTests(ITestOutputHelper testOutputHelper)
{
    private readonly List<TestEntity2> _collection2 =
    [
        new()
        {
            Id = 1,
            Name = "A",
            CreatedAt = DateTime.Now,
            Password = "123456"
        },
        new()
        {
            Id = 2,
            Name = "B",
            CreatedAt = DateTime.Now,
            Password = "fhsdf@#%$"
        },
        new()
        {
            Id = 3,
            Name = "C",
            CreatedAt = DateTime.Now,
            Password = "abcdefxyz123!"
        }
    ];

    // Exclude Properties
    [Fact]
    public void ExcludePropertiesTest()
    {
        // Arrange
        var queryable = _collection2.AsQueryable();

        // Act
        var result = queryable
            .ExcludeProperties("CreatedAt", "Password")
            .ToList();

        // Assert
        Assert.Equal(3, result.Count);

        testOutputHelper.WriteLine("Excluded CreatedAt and Password");
        result.ForEach(x => testOutputHelper.WriteLine($"{x.Id} {x.Name} {x.CreatedAt} {x.Password}"));
    }
}