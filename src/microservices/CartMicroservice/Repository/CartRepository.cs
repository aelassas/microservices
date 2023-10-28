using CartMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartMicroservice.Repository;

public class CartRepository : ICartRepository
{
    private readonly IMongoDatabase _db;

    public CartRepository(IMongoDatabase db)
    {
        _db = db;
    }

    public IEnumerable<CartItem> GetCartItems(Guid userId)
    {
        var col = _db.GetCollection<Cart>(Cart.DocumentName);
        var cart = col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            return cart.CartItems;
        }
        return new List<CartItem>();
    }

    public void InsertCartItem(Guid userId, CartItem cartItem)
    {
        var col = _db.GetCollection<Cart>(Cart.DocumentName);
        var cart = col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CartItems = new List<CartItem> { cartItem } };
            col.InsertOne(cart);
        }
        else
        {
            var cartItemFromDb = cart.CartItems.FirstOrDefault(ci => ci.CatalogItemId == cartItem.CatalogItemId);
            if (cartItemFromDb == null)
            {
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItemFromDb.Quantity++;
            }
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void UpdateCartItem(Guid userId, CartItem cartItem)
    {
        var col = _db.GetCollection<Cart>(Cart.DocumentName);
        var cart = col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == cartItem.CatalogItemId);
            cart.CartItems.Add(cartItem);
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void DeleteCartItem(Guid userId, Guid catalogItemId)
    {
        var col = _db.GetCollection<Cart>(Cart.DocumentName);
        var cart = col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == catalogItemId);
            var update = Builders<Cart>.Update
                .Set(c => c.CartItems, cart.CartItems);
            col.UpdateOne(c => c.UserId == userId, update);
        }
    }
}