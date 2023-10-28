using CartMicroservice.Controllers;
using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartMicroservice.UnitTests;

public class CartControllerTest
{
    private readonly CartController _controller;
    private static readonly Guid UserId = new("ae4c07fc-fc81-4798-81ed-5a773787fe30");
    private static readonly Guid A54Id = new("ce2dbb82-6689-487b-9691-0a05ebabce4a");
    private static readonly Guid A14Id = new("ecf6bcac-9286-457b-b2e0-c1b1bc9849c6");
    private readonly Dictionary<Guid, List<CartItem>> _cartItems = new()
    {
        {
            UserId,
            new List<CartItem>
            {
                new()
                {
                    CatalogItemId = A54Id,
                    CatalogItemName = "Samsung Galaxy A54 5G",
                    CatalogItemPrice = 500,
                    Quantity = 1
                },
                new()
                {
                    CatalogItemId = A14Id,
                    CatalogItemName = "Samsung Galaxy A14 5G",
                    CatalogItemPrice = 200,
                    Quantity = 2
                }
            }
        }
    };

    public CartControllerTest()
    {
        var mockRepo = new Mock<ICartRepository>();
        mockRepo.Setup(repo => repo.GetCartItems(It.IsAny<Guid>()))
            .Returns<Guid>(id => _cartItems[id]);
        mockRepo.Setup(repo => repo.InsertCartItem(It.IsAny<Guid>(), It.IsAny<CartItem>()))
            .Callback<Guid, CartItem>((userId, item) =>
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
        mockRepo.Setup(repo => repo.UpdateCartItem(It.IsAny<Guid>(), It.IsAny<CartItem>()))
            .Callback<Guid, CartItem>((userId, item) =>
            {
                if (_cartItems.TryGetValue(userId, out var items))
                {
                    var currentItem = items.FirstOrDefault(i => i.CatalogItemId == item.CatalogItemId);
                    if (currentItem != null)
                    {
                        currentItem.CatalogItemName = item.CatalogItemName;
                        currentItem.CatalogItemPrice = item.CatalogItemPrice;
                        currentItem.Quantity = item.Quantity;
                    }
                }
            });
        mockRepo.Setup(repo => repo.DeleteCartItem(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Callback<Guid, Guid>((userId, catalogItemId) =>
            {
                if (_cartItems.TryGetValue(userId, out var items))
                {
                    items.RemoveAll(i => i.CatalogItemId == catalogItemId);
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
        var catalogItemId = Guid.NewGuid();
        var okObjectResult = _controller.Post(
            UserId,
            new CartItem
            {
                CatalogItemId = catalogItemId,
                CatalogItemName = "Samsung Galaxy S23 Ultra",
                CatalogItemPrice = 1500,
                Quantity = 1
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        Assert.NotNull(_cartItems[UserId].FirstOrDefault(i => i.CatalogItemId == catalogItemId));
    }

    [Fact]
    public void UpdateCartItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            UserId,
            new CartItem
            {
                CatalogItemId = catalogItemId,
                CatalogItemName = "Samsung Galaxy A54",
                CatalogItemPrice = 550,
                Quantity = 2
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _cartItems[UserId].FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.CatalogItemName);
        Assert.Equal(550, catalogItem.CatalogItemPrice);
        Assert.Equal(2, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCartItemTest()
    {
        var id = A14Id;
        var items = _cartItems[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(UserId, id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.Null(item);
    }
}