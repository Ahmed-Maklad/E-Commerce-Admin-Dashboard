using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorImageDTOs
{
    public class GetPrdColorImgAPIWithLangDTO
    {
        public int PrdColorImageId { get; set; }
        public string? ImagePath { get; set; }
    }
}
