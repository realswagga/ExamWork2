﻿using System;
using System.Collections.Generic;

namespace DataBaseLayer.Models;

public partial class ExamProduct
{
    public string ProductArticleNumber { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public string ProductCategory { get; set; } = null!;

    public string? ProductPhoto { get; set; }

    public string ProductManufacturer { get; set; } = null!;

    public decimal ProductCost { get; set; }

    public byte? ProductDiscountAmount { get; set; }

    public int ProductQuantityInStock { get; set; }

    public string ProductStatus { get; set; } = null!;

    public virtual ICollection<ExamOrderProduct> ExamOrderProducts { get; set; } = new List<ExamOrderProduct>();
}
