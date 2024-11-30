using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.CategoryDTOs  
{
   
    public class CUCategoryDTO
    {
        public int Id { get; set; }
        
        [StringLength(40, MinimumLength = 3)]
        public string NameAr { get; set; }
        
        [StringLength(40, MinimumLength = 3)]
        public string NameEn { get; set; }
        public int ParentCategoryId { get; set; } = 0;
        public int Level { get; set; } = 0;
        public bool IsParentCategory { get; set; } = false;
        public IFormFile? ImageData { get; set; }
        public string? ImagePath { get; set; }
    }
}
