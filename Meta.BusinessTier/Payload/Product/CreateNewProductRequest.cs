﻿using Meta.BusinessTier.Enums.Status;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Product
{
    public class CreateNewProductRequest
    {
        public string Name { get; set; }
        public double? StockPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public Guid CategoryId { get; set; }
        public double? TimeWork { get; set; }
        public int Priority { get; set; }
        public List<Guid>? supProductId { get; set; }
        public List<Image>? Image { get; set; } = new List<Image>();
    }
    public class Image
    {
        public string? ImageURL { get; set; }

    }
}
