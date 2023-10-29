using CatalogMicroservice.Model;
using System.Collections.Generic;

namespace CatalogMicroservice.Repository;

public interface ICatalogRepository
{
    IList<CatalogItem> GetCatalogItems();
    CatalogItem? GetCatalogItem(string catalogItemId);
    void InsertCatalogItem(CatalogItem catalogItem);
    void UpdateCatalogItem(CatalogItem catalogItem);
    void DeleteCatalogItem(string catagItemId);
}