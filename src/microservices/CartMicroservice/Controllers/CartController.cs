using CartMicroservice.Model;
using CartMicroservice.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(ICartRepository cartRepository) : ControllerBase
{
    // GET: api/<CartController>
    [HttpGet]
    [Authorize]
    public IActionResult Get([FromQuery(Name = "u")] string userId)
    {
        var cartItems = cartRepository.GetCartItems(userId);
        return Ok(cartItems);
    }

    // POST api/<CartController>
    [HttpPost]
    [Authorize]
    public IActionResult Post([FromQuery(Name = "u")] string userId, [FromBody] CartItem cartItem)
    {
        cartRepository.InsertCartItem(userId, cartItem);
        return Ok();
    }

    // PUT api/<CartController>
    [HttpPut]
    [Authorize]
    public IActionResult Put([FromQuery(Name = "u")] string userId, [FromBody] CartItem cartItem)
    {
        cartRepository.UpdateCartItem(userId, cartItem);
        return Ok();
    }

    // DELETE api/<CartController>
    [HttpDelete]
    [Authorize]
    public IActionResult Delete([FromQuery(Name = "u")] string userId, [FromQuery(Name = "ci")] string cartItemId)
    {
        cartRepository.DeleteCartItem(userId, cartItemId);
        return Ok();
    }

    // PUT api/<CartController>/update-catalog-item
    [HttpPut("update-catalog-item")]
    [Authorize]
    public IActionResult Put([FromQuery(Name = "ci")] string catalogItemId, [FromQuery(Name = "n")] string name, [FromQuery(Name = "p")] decimal price)
    {
        cartRepository.UpdateCatalogItem(catalogItemId, name, price);
        return Ok();
    }

    // DELETE api/<CartController>/delete-catalog-item
    [HttpDelete("delete-catalog-item")]
    [Authorize]
    public IActionResult Delete([FromQuery(Name = "ci")] string catalogItemId)
    {
        cartRepository.DeleteCatalogItem(catalogItemId);
        return Ok();
    }
}