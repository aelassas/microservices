using System;

namespace CatalogMicroservice.Model;

public class CatalogItem
{
    public static readonly string DocumentName = "catalogItems";

    public Guid Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}