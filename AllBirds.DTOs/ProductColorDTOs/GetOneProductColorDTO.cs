using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorDTOs
{
    public class GetOneProductColorDTO
    {
        public int ProductId { get; set; }
        public string ProductNo { get; set; }
        public string PNameAr { get; set; }
        public string PNameEn { get; set; }
        public string ColorNameEn { get; set; }
        public string ColorNameAr { get; set; }
        public int MainImageId { get; set; }
        public string? MainImagePath { get; set; }
        public string ColorCode { get; set; }

        public List<int> ProductColorImageId { get; set; }
        public List<string> Images { get; set; }
        public List<string> Sizes { get; set; }

    }
}
