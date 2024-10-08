﻿using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? StockPrice { get; set; }

    public double? SellingPrice { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? Priority { get; set; }

    public double? TimeWork { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductPetService> ProductPetServices { get; set; } = new List<ProductPetService>();
}
