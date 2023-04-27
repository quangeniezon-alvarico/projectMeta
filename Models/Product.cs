using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ProjectMetaAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public int AvailableQuantity { get; set; }

    public string ProductStatus { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
public class ProductUpdateRequest
{
    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public int AvailableQuantity { get; set; }

    public string ProductStatus { get; set; } = null!;
}


