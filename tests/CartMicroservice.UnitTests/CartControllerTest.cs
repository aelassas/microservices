using CartMicroservice.Controllers;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Model;
using Moq;

namespace CartMicroservice.UnitTests;

public class CartControllerTest
{
    private readonly CartController _controller;
    private static readonly string UserId = "653e43b8c76b6b56a720803e";
    private static readonly string A54Id = "653e4410614d711b7fc953a7";
    private static readonly string A14Id = "253e4410614d711b7fc953a7";
    private readonly Dictionary<string, List<CartItem>> _cartItems = new()
    {
        {
            UserId,
            new List<CartItem>
            {
                new()
                {
                    CatalogItem = new()
                    {
                        Id = A54Id,
                        Name = "Samsung Galaxy A54 5G",
                        Price = 500,
                    },
                    Quantity = 1
                },
                new()
                {
                    CatalogItem = new()
                    {
                        Id = A14Id,
                        Name = "Samsung Galaxy A14 5G",
                        Price = 200,
                    },
                    Quantity = 2
                }
            }
        }
    };

    public CartControllerTest()
    {
        var mockRepo = new Mock<ICartRepository>();
        mockRepo.Setup(repo => repo.GetCartItems(It.IsAny<string>()))
            .Returns<string>(id => _cartItems[id]);
        mockRepo.Setup(repo => repo.InsertCartItem(It.IsAny<string>(), It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_cartItems.TryGetValue(userId, out var items))
                {
                    items.Add(item);
                }
                else
                {
                    _cartItems.Add(userId, new List<CartItem> { item });
                }
            });
        mockRepo.Setup(repo => repo.UpdateCartItem(It.IsAny<string>(), It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_cartItems.TryGetValue(userId, out var items))
                {
                    var currentItem = items.FirstOrDefault(i => i.CatalogItem?.Id == item.CatalogItem?.Id);
                    if (currentItem != null)
                    {
                        currentItem.CatalogItem!.Name = item.CatalogItem!.Name;
                        currentItem.CatalogItem.Price = item.CatalogItem!.Price;
                        currentItem.Quantity = item.Quantity;
                    }
                }
            });
        mockRepo.Setup(repo => repo.DeleteCartItem(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((userId, catalogItemId) =>
            {
                if (_cartItems.TryGetValue(userId, out var items))
                {
                    items.RemoveAll(i => i.CatalogItem?.Id == catalogItemId);
                }
            });
        _controller = new CartController(mockRepo.Object);
    }

    [Fact]
    public void GetCartItemsTest()
    {
        var okObjectResult = _controller.Get(UserId);
        var okResult = Assert.IsType<OkObjectResult>(okObjectResult);
        var items = Assert.IsType<List<CartItem>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void InsertCartItemTest()
    {
        var okObjectResult = _controller.Post(
            UserId,
            new CartItem
            {
                CatalogItem = new()
                {
                    Id = A54Id,
                    Name = "Samsung Galaxy A54 5G",
                    Price = 500,
                },
                Quantity = 1
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        Assert.NotNull(_cartItems[UserId].FirstOrDefault(i => i.CatalogItem?.Id == A54Id));
    }

    [Fact]
    public void UpdateCartItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            UserId,
            new CartItem
            {
                CatalogItem = new()
                {
                    Id = A54Id,
                    Name = "Samsung Galaxy A54",
                    Price = 550,
                },
                Quantity = 2
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _cartItems[UserId].FirstOrDefault(i => i.CatalogItem?.Id == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.CatalogItem!.Name);
        Assert.Equal(550, catalogItem.CatalogItem.Price);
        Assert.Equal(2, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCartItemTest()
    {
        var id = A14Id;
        var items = _cartItems[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItem?.Id == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(UserId, id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItem?.Id == id);
        Assert.Null(item);
    }
}