﻿using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public Guid? MasterCategoryId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<SupProduct> SupProducts { get; set; } = new List<SupProduct>();
}
