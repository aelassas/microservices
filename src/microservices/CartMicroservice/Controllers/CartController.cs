using CartMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Model;

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
    public IActionResult Get([FromQuery(Name = "u")] string userId)
    {
        var cartItems = _cartRepository.GetCartItems(userId);
        return Ok(cartItems);
    }

    // POST api/<CartController>
    [HttpPost]
    public IActionResult Post([FromQuery(Name = "u")] string userId, [FromBody] CartItem cartItem)
    {
        _cartRepository.InsertCartItem(userId, cartItem);
        return Ok();
    }

    // PUT api/<CartController>
    [HttpPut]
    public IActionResult Put([FromQuery(Name = "u")] string userId, [FromBody] CartItem cartItem)
    {
        _cartRepository.UpdateCartItem(userId, cartItem);
        return Ok();
    }

    // DELETE api/<CartController>
    [HttpDelete]
    public IActionResult Delete([FromQuery(Name = "u")] string userId, [FromQuery(Name = "ci")] string cartItemId)
    {
        _cartRepository.DeleteCartItem(userId, cartItemId);
        return Ok();
    }
}