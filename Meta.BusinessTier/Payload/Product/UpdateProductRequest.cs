﻿using Meta.BusinessTier.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Product
{
    public class UpdateProductRequest
    {
        public string Name { get; set; }

        public double? UnitPrice { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public Guid CategoryId { get; set; }
        public int Priority { get; set; }
    }
}
