using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.OrderDetailsDTOs
{
    public class GetAllClientOrderDetailsDTO
    {
        public int Id { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public int ProductColorSizeId { get; set; } = 0;
        public string? ProductName { get; set; }
        public string? ProductImagePath { get; set; }
        public string? ColorName { get; set; }
        public string? SizeNumber { get; set; }
        public decimal Price { get; set; } = 0;
        public decimal DetailPrice { get; set; } = 0;
        public int Quantity { get; set; } = 0;
    }
}
