using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorDTOs
{
    public record GetALlProductColorDTO
    {
        public int Id { get; set; }

        public string ProductNo { get; set; }
        public string PNameAr { get; set; }
        public string PNameEn { get; set; }
        public string ColorNameEn { get; set; }
        public string ColorNameAr { get; set; }
        public string ColorCode { get; set; }
        public bool IsDeleted { get; set; }


        public int MainImageId { get; set; }
        public string? MainImagePath { get; set; }

    }
}
