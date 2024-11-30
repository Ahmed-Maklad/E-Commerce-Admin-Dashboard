using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDetailDTOs
{
    public class GetPrdDetailsAPIWithLangDTO
    {
        public int PrdDetailId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
    }
}
