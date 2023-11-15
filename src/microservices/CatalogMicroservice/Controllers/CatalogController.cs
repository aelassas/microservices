using CatalogMicroservice.Model;
using CatalogMicroservice.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController(ICatalogRepository catalogRepository) : ControllerBase
{
    // GET: api/<CatalogController>
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var catalogItems = catalogRepository.GetCatalogItems();
        return Ok(catalogItems);
    }

    // GET api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult Get(string id)
    {
        var catalogItem = catalogRepository.GetCatalogItem(id);
        return Ok(catalogItem);
    }

    // POST api/<CatalogController>
    [HttpPost]
    [Authorize]
    public IActionResult Post([FromBody] CatalogItem catalogItem)
    {
        catalogRepository.InsertCatalogItem(catalogItem);
        return CreatedAtAction(nameof(Get), new { id = catalogItem.Id }, catalogItem);
    }

    // PUT api/<CatalogController>
    [HttpPut]
    [Authorize]
    public IActionResult Put([FromBody] CatalogItem? catalogItem)
    {
        if (catalogItem != null)
        {
            catalogRepository.UpdateCatalogItem(catalogItem);
            return Ok();
        }
        return new NoContentResult();
    }

    // DELETE api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(string id)
    {
        catalogRepository.DeleteCatalogItem(id);
        return Ok();
    }
}