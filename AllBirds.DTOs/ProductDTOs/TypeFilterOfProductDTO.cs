using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDTOs
{
    public class TypeFilterOfProductDTO
    {
        public int categoryId { get; set; }
        public List<string> sizeNumber { get; set; } 
        public List<string> colorCode { get; set; }
        //public List<decimal> Price { get; set; }

    }
}
