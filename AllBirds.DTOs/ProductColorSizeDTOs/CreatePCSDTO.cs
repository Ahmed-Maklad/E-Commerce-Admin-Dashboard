using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductColorSizeDTOs
{
    public class CreatePCSDTO
    {
        public int ProductColorId { get; set; }
        public int SizeId { get; set; }
        public int UnitsInStock { get; set; }
    }
}
