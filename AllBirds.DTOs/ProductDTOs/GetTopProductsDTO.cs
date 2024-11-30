using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDTOs
{
    public record GetTopProductsDTO
    {
        public int Id { get; set; }
        public string? MainImagePath { get; set; }
        public decimal Price { get; set; }
        public string? ColorNameEn { get; set; }
        public string? ColorNameAr { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }

    }
}
