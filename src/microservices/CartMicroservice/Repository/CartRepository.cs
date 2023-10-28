using CartMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartMicroservice.Repository;

public class CartRepository : ICartRepository
{
    private readonly IMongoCollection<Cart> _col;

    public CartRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<Cart>(Cart.DocumentName);
    }

    public IEnumerable<CartItem> GetCartItems(Guid userId) =>
        _col.Find(c => c.UserId == userId).FirstOrDefault()?.CartItems ?? new List<CartItem>();


    public void InsertCartItem(Guid userId, CartItem cartItem)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CartItems = new List<CartItem> { cartItem } };
            _col.InsertOne(cart);
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
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void UpdateCartItem(Guid userId, CartItem cartItem)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == cartItem.CatalogItemId);
            cart.CartItems.Add(cartItem);
            var update = Builders<Cart>
                .Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }

    public void DeleteCartItem(Guid userId, Guid catalogItemId)
    {
        var cart = _col.Find(c => c.UserId == userId).FirstOrDefault();
        if (cart != null)
        {
            cart.CartItems.RemoveAll(ci => ci.CatalogItemId == catalogItemId);
            var update = Builders<Cart>
                .Update
                .Set(c => c.CartItems, cart.CartItems);
            _col.UpdateOne(c => c.UserId == userId, update);
        }
    }
}