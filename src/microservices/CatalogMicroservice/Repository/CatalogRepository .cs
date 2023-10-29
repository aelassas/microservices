using Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace CatalogMicroservice.Repository;

public class CatalogRepository : ICatalogRepository
{
    private readonly IMongoCollection<CatalogItem> _col;
    private readonly IMongoCollection<Cart> _cartCol;

    public CatalogRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<CatalogItem>(CatalogItem.DocumentName);
        _cartCol = db.GetCollection<Cart>(Cart.DocumentName);
    }

    public IList<CatalogItem> GetCatalogItems() =>
        _col.Find(FilterDefinition<CatalogItem>.Empty).ToList();

    public CatalogItem GetCatalogItem(string catalogItemId) =>
        _col.Find(c => c.Id == catalogItemId).FirstOrDefault();

    public void InsertCatalogItem(CatalogItem catalogItem) =>
        _col.InsertOne(catalogItem);

    public void UpdateCatalogItem(CatalogItem catalogItem) =>
        _col.UpdateOne(c => c.Id == catalogItem.Id, Builders<CatalogItem>.Update
            .Set(c => c.Name, catalogItem.Name)
            .Set(c => c.Description, catalogItem.Description)
            .Set(c => c.Price, catalogItem.Price));

    public void DeleteCatalogItem(string catalogItemId)
    {
        // Delete catalog item
        _col.DeleteOne(c => c.Id == catalogItemId);

        // Delete catalog item references from carts
        var carts = _cartCol.Find(c => c.CartItems.Any(i => i.CatalogItem!.Id == catalogItemId)).ToList();
        foreach (var cart in carts)
        {
            cart.CartItems.RemoveAll(i => i.CatalogItem!.Id == catalogItemId);
            _cartCol.UpdateOne(c => c.Id == cart.Id, Builders<Cart>.Update.Set(c => c.CartItems, cart.CartItems));
        }
    }
}