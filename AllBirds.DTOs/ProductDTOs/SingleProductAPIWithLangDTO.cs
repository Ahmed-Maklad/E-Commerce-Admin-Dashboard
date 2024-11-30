using AllBirds.DTOs.ProductColorDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.ProductSpecificationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDTOs
{
    public class SingleProductAPIWithLangDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Highlights { get; set; }
        public string Sustainability { get; set; }
        public string SustainableMaterials { get; set; }
        public string ShippingAndReturns { get; set; }
        public string CareGuide { get; set; }
        public int Discount { get; set; }
        public bool FreeShipping { get; set; }
        public int MainColorId { get; set; }
        public int ReviewsCount { get; set; } = 0;
        public int TotalRate { get; set; } = 0;
        public List<GetPrdColorAPIWithLangDTO>? PrdColors { get; set; }
        public List<GetSpecAPIWithLangDTO>? Specifications { get; set; }
        public List<GetPrdDetailsAPIWithLangDTO>? Details { get; set; }





    }
}
