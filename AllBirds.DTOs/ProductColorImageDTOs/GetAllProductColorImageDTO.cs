using AllBirds.DTOs.ProductColorSizeDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorImageDTOs
{
    public class GetAllProductColorImageDTO
    {
        public int ProductColorId { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }
        public string Code { get; set; }
        public string? ImagePath { get; set; }

        public List<GetPCSDTO> ProductSizes { get; set; }
    }
}
