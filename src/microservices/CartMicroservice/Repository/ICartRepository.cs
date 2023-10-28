using CartMicroservice.Model;
using System;
using System.Collections.Generic;

namespace CartMicroservice.Repository;

public interface ICartRepository
{
    IList<CartItem> GetCartItems(Guid userId);
    void InsertCartItem(Guid userId, CartItem cartItem);
    void UpdateCartItem(Guid userId, CartItem cartItem);
    void DeleteCartItem(Guid userId, Guid cartItemId);
}