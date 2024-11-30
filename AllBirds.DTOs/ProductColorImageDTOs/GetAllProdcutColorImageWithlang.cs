using AllBirds.DTOs.ProductColorSizeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorImageDTOs
{
    public class GetAllProdcutColorImageWithlang
    {
        public int ProductColorId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? ImagePath { get; set; }

        public List<GetPCSDTO> ProductSizes { get; set; }
    }
}
