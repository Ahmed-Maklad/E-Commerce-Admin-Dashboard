using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.CategoryDTOs
{
    public class GetAllCategoryWithLangDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int Level { get; set; }
        public bool IsParentCategory { get; set; }
        public List<GetAllCategoryWithLangDTO>? Children { get; set; }
    }
}
