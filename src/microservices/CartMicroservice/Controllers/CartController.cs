using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CartMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    // GET: api/<CartController>
    [HttpGet]
    public IActionResult Get([FromQuery(Name = "u")] Guid userId)
    {
        var cartItems = _cartRepository.GetCartItems(userId);
        return new OkObjectResult(cartItems);
    }

    // POST api/<CartController>
    [HttpPost]
    public IActionResult Post([FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
    {
        _cartRepository.InsertCartItem(userId, cartItem);
        return new OkResult();
    }

    // PUT api/<CartController>
    [HttpPut]
    public IActionResult Put([FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
    {
        _cartRepository.UpdateCartItem(userId, cartItem);
        return new OkResult();
    }

    // DELETE api/<CartController>
    [HttpDelete]
    public IActionResult Delete([FromQuery(Name = "u")] Guid userId, [FromQuery(Name = "ci")] Guid cartItemId)
    {
        _cartRepository.DeleteCartItem(userId, cartItemId);
        return new OkResult();
    }
}