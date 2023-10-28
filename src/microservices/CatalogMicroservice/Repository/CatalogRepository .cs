using CatalogMicroservice.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace CatalogMicroservice.Repository;

public class CatalogRepository : ICatalogRepository
{
    private readonly IMongoDatabase _db;

    public CatalogRepository(IMongoDatabase db)
    {
        _db = db;
    }

    public IEnumerable<CatalogItem> GetCatalogItems()
    {
        var col = _db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        var catalogItems = col.Find(FilterDefinition<CatalogItem>.Empty).ToEnumerable();
        return catalogItems;
    }

    public CatalogItem? GetCatalogItem(Guid catalogItemId)
    {
        var col = _db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        var catalogItem = col.Find(c => c.Id == catalogItemId).FirstOrDefault();
        return catalogItem;
    }

    public void InsertCatalogItem(CatalogItem catalogItem)
    {
        var col = _db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        col.InsertOne(catalogItem);
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        var col = _db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        var update = Builders<CatalogItem>.Update
            .Set(c => c.Name, catalogItem.Name)
            .Set(c => c.Description, catalogItem.Description)
            .Set(c => c.Price, catalogItem.Price);
        col.UpdateOne(c => c.Id == catalogItem.Id, update);
    }

    public void DeleteCatalogItem(Guid catalogItemId)
    {
        var col = _db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        col.DeleteOne(c => c.Id == catalogItemId);
    }
}