using CatalogMicroservice.Model;
using CatalogMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CatalogMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepository;

    public CatalogController(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    // GET: api/<CatalogController>
    [HttpGet]
    public IActionResult Get()
    {
        var catalogItems = _catalogRepository.GetCatalogItems();
        return Ok(catalogItems);
    }

    // GET api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var catalogItem = _catalogRepository.GetCatalogItem(id);
        return Ok(catalogItem);
    }

    // POST api/<CatalogController>
    [HttpPost]
    public IActionResult Post([FromBody] CatalogItem catalogItem)
    {
        _catalogRepository.InsertCatalogItem(catalogItem);
        return CreatedAtAction(nameof(Get), new { id = catalogItem.Id }, catalogItem);
    }

    // PUT api/<CatalogController>
    [HttpPut]
    public IActionResult Put([FromBody] CatalogItem? catalogItem)
    {
        if (catalogItem != null)
        {
            _catalogRepository.UpdateCatalogItem(catalogItem);
            return Ok();
        }
        return new NoContentResult();
    }

    // DELETE api/<CatalogController>/653e4410614d711b7fc953a7
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        _catalogRepository.DeleteCatalogItem(id);
        return Ok();
    }
}