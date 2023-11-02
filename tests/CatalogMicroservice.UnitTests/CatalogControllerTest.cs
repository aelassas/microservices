using CatalogMicroservice.Controllers;
using CatalogMicroservice.Model;
using CatalogMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CatalogMicroservice.UnitTests;

public class CatalogControllerTest
{
    private readonly CatalogController _controller;
    private static readonly string A54Id = "653e4410614d711b7fc953a7";
    private static readonly string A14Id = "253e4410614d711b7fc953a7";
    private readonly List<CatalogItem> _items = new()
    {
        new()
        {
            Id = A54Id,
            Name = "Samsung Galaxy A54 5G",
            Description = "Samsung Galaxy A54 5G mobile phone",
            Price = 500
        },
        new()
        {
            Id = A14Id,
            Name = "Samsung Galaxy A14 5G",
            Description = "Samsung Galaxy A14 5G mobile phone",
            Price = 200
        }
    };

    public CatalogControllerTest()
    {
        var mockRepo = new Mock<ICatalogRepository>();
        mockRepo.Setup(repo => repo.GetCatalogItems()).Returns(_items);
        mockRepo.Setup(repo => repo.GetCatalogItem(It.IsAny<string>()))
            .Returns<string>(id => _items.FirstOrDefault(i => i.Id == id));
        mockRepo.Setup(repo => repo.InsertCatalogItem(It.IsAny<CatalogItem>()))
            .Callback<CatalogItem>(_items.Add);
        mockRepo.Setup(repo => repo.UpdateCatalogItem(It.IsAny<CatalogItem>()))
            .Callback<CatalogItem>(i =>
            {
                var item = _items.FirstOrDefault(catalogItem => catalogItem.Id == i.Id);
                if (item != null)
                {
                    item.Name = i.Name;
                    item.Description = i.Description;
                    item.Price = i.Price;
                }
            });
        mockRepo.Setup(repo => repo.DeleteCatalogItem(It.IsAny<string>()))
            .Callback<string>(id => _items.RemoveAll(i => i.Id == id));
        _controller = new CatalogController(mockRepo.Object);
    }

    [Fact]
    public void GetCatalogItemsTest()
    {
        var okObjectResult = _controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var items = Assert.IsType<List<CatalogItem>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void GetCatalogItemTest()
    {
        var id = A54Id;
        var okObjectResult = _controller.Get(id);
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var item = Assert.IsType<CatalogItem>(okResult.Value);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public void InsertCatalogItemTest()
    {
        var createdResponse = _controller.Post(
            new CatalogItem
            {
                Id = "353e4410614d711b7fc953a7",
                Name = "iPhone 15",
                Description = "iPhone 15 mobile phone",
                Price = 1500
            }
        );
        var response = Assert.IsType<CreatedAtActionResult>(createdResponse);
        var item = Assert.IsType<CatalogItem>(response.Value);
        Assert.Equal("iPhone 15", item.Name);
    }

    [Fact]
    public void UpdateCatalogItemTest()
    {
        var id = A54Id;
        var okObjectResult = _controller.Put(
            new CatalogItem
            {
                Id = id,
                Name = "Samsung Galaxy S23 Ultra",
                Description = "Samsung Galaxy S23 Ultra mobile phone",
                Price = 1500
            });
        Assert.IsType<OkResult>(okObjectResult);
        var item = _items.FirstOrDefault(i => i.Id == id);
        Assert.NotNull(item);
        Assert.Equal("Samsung Galaxy S23 Ultra", item.Name);
        okObjectResult = _controller.Put(null);
        Assert.IsType<NoContentResult>(okObjectResult);
    }

    [Fact]
    public void DeleteCatalogItemTest()
    {
        var id = A54Id;
        var item = _items.FirstOrDefault(i => i.Id == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(id);
        Assert.IsType<OkResult>(okObjectResult);
        item = _items.FirstOrDefault(i => i.Id == id);
        Assert.Null(item);
    }
}