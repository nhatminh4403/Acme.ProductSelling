﻿using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Categories
{
    public class GetProductsByCategoryInput : PagedAndSortedResultRequestDto
    {
        public Guid CategoryId { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "Name";

    }
}
