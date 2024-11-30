using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorDTOs
{
    public class GetPrdColorAPIWithLangDTO
    {
        public int PrdColorId { get; set; }
        public string? ColorName { get; set; }
        public string? ColorCode { get; set; }
        public int MainImageId { get; set; }
        public List<GetPrdColorImgAPIWithLangDTO>? PrdColorImages { get; set; }
        public List<GetPCSDTO>? PrdColorSizes { get; set; }
    }
}
