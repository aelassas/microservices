using CatalogMicroservice.Model;
using System;
using System.Collections.Generic;

namespace CatalogMicroservice.Repository;

public interface ICatalogRepository
{
    IList<CatalogItem> GetCatalogItems();
    CatalogItem? GetCatalogItem(Guid catagItemId);
    void InsertCatalogItem(CatalogItem catalogItem);
    void UpdateCatalogItem(CatalogItem catalogItem);
    void DeleteCatalogItem(Guid catagItemId);
}