using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorSizeDTOs
{
    public class GetPCSDTO
    {
        public int ProductColorSizeId { get; set; }
        public string SizeNumber { get; set; }
        public int UnitsInStock { get; set; }
    }
}
