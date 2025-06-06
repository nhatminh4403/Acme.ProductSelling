﻿using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class RamSpecification : Entity<Guid>
    {
        public string RamType { get; set; } // e.g., "DDR5", "DDR4"
        public int Capacity { get; set; } // Total GB (e.g., 16, 32)
        public int Speed { get; set; } // MHz or MT/s
        public int ModuleCount { get; set; } // e.g., 2 (cho kit 2x8GB)
        public string Timing { get; set; } // e.g., "16-18-18-38"
        public float Voltage { get; set; } // V
        public bool HasRGB { get; set; }
    }
}
