using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDTOs
{
    public class ProductCardDTO
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal Price { get; set; }

        public List<GetAllProductColorImageDTO> ProductColors { get; set; }

    }
}
