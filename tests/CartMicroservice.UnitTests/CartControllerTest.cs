using CartMicroservice.Controllers;
using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartMicroservice.UnitTests;

public class CartControllerTest
{
    private readonly CartController _controller;
    private static readonly string UserId = "653e43b8c76b6b56a720803e";
    private static readonly string A54Id = "653e4410614d711b7fc953a7";
    private static readonly string A14Id = "253e4410614d711b7fc953a7";
    private readonly Dictionary<string, List<CartItem>> _carts = new()
    {
        {
            UserId,
            new()
            {
                new()
                {
                    CatalogItemId = A54Id,
                    Name = "Samsung Galaxy A54 5G",
                    Price = 500,
                    Quantity = 1
                },
                new()
                {
                    CatalogItemId = A14Id,
                    Name = "Samsung Galaxy A14 5G",
                    Price = 200,
                    Quantity = 2
                }
            }
        }
    };

    public CartControllerTest()
    {
        var mockRepo = new Mock<ICartRepository>();
        mockRepo.Setup(repo => repo.GetCartItems(It.IsAny<string>()))
            .Returns<string>(id => _carts[id]);
        mockRepo.Setup(repo => repo.InsertCartItem(It.IsAny<string>(), It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    items.Add(item);
                }
                else
                {
                    _carts.Add(userId, new List<CartItem> { item });
                }
            });
        mockRepo.Setup(repo => repo.UpdateCartItem(It.IsAny<string>(), It.IsAny<CartItem>()))
            .Callback<string, CartItem>((userId, item) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    var currentItem = items.FirstOrDefault(i => i.CatalogItemId == item.CatalogItemId);
                    if (currentItem != null)
                    {
                        currentItem.Name = item.Name;
                        currentItem.Price = item.Price;
                        currentItem.Quantity = item.Quantity;
                    }
                }
            });
        mockRepo.Setup(repo => repo.UpdateCatalogItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .Callback<string, string, decimal>((catalogItemId, name, price) =>
            {
                var cartItems = _carts
                .Values
                .Where(items => items.Any(i => i.CatalogItemId == catalogItemId))
                .SelectMany(items => items)
                .ToList();

                foreach (var cartItem in cartItems)
                {
                    cartItem.Name = name;
                    cartItem.Price = price;
                }
            });
        mockRepo.Setup(repo => repo.DeleteCartItem(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((userId, catalogItemId) =>
            {
                if (_carts.TryGetValue(userId, out var items))
                {
                    items.RemoveAll(i => i.CatalogItemId == catalogItemId);
                }
            });
        mockRepo.Setup(repo => repo.DeleteCatalogItem(It.IsAny<string>()))
            .Callback<string>((catalogItemId) =>
            {
                foreach (var cart in _carts)
                {
                    cart.Value.RemoveAll(i => i.CatalogItemId == catalogItemId);
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
                CatalogItemId = A54Id,
                Name = "Samsung Galaxy A54 5G",
                Price = 500,
                Quantity = 1
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        Assert.NotNull(_carts[UserId].FirstOrDefault(i => i.CatalogItemId == A54Id));
    }

    [Fact]
    public void UpdateCartItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            UserId,
            new CartItem
            {
                CatalogItemId = A54Id,
                Name = "Samsung Galaxy A54",
                Price = 550,
                Quantity = 2
            }
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _carts[UserId].FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.Name);
        Assert.Equal(550, catalogItem.Price);
        Assert.Equal(2, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCartItemTest()
    {
        var id = A14Id;
        var items = _carts[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(UserId, id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.Null(item);
    }

    [Fact]
    public void UpdateCatalogItemTest()
    {
        var catalogItemId = A54Id;
        var okObjectResult = _controller.Put(
            A54Id,
            "Samsung Galaxy A54",
            550
        );
        Assert.IsType<OkResult>(okObjectResult);
        var catalogItem = _carts[UserId].FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        Assert.NotNull(catalogItem);
        Assert.Equal("Samsung Galaxy A54", catalogItem.Name);
        Assert.Equal(550, catalogItem.Price);
        Assert.Equal(1, catalogItem.Quantity);
    }

    [Fact]
    public void DeleteCatalogItemTest()
    {
        var id = A14Id;
        var items = _carts[UserId];
        var item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.NotNull(item);
        var okObjectResult = _controller.Delete(id);
        Assert.IsType<OkResult>(okObjectResult);
        item = items.FirstOrDefault(i => i.CatalogItemId == id);
        Assert.Null(item);
    }
}