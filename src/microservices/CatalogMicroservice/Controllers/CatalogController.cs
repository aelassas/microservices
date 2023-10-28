using CatalogMicroservice.Model;
using CatalogMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using System;

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
        return new OkObjectResult(catalogItems);
    }

    // GET api/<CatalogController>/110ec627-2f05-4a7e-9a95-7a91e8005da8
    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var catalogItem = _catalogRepository.GetCatalogItem(id);
        return new OkObjectResult(catalogItem);
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
            return new OkResult();
        }
        return new NoContentResult();
    }

    // DELETE api/<CatalogController>/110ec627-2f05-4a7e-9a95-7a91e8005da8
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _catalogRepository.DeleteCatalogItem(id);
        return new OkResult();
    }
}