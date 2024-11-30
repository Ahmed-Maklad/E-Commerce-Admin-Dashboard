using AllBirds.DTOs.ProductColorImageDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.ProductDTOs
{
    public class GetProductCardWithlangDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public List<GetAllProdcutColorImageWithlang> ProductColors { get; set; }
    }
}
