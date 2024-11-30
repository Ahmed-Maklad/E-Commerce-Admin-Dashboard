using AllBirds.DTOs.OrderDetailsDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.OrderMasterDTOs
{
    public class CreateOrderMasterDTO
    {
        //[MaxLength(100)]
        //public string? OrderNo { get; set; } // not needed in creation


        //public int OrderStateId { get; set; } = 1; //  not needed in creation

        //[MaxLength(128)]
        //public string? Notes { get; set; } // not needed in creation

        //public int? CouponId { get; set; } // not needed in creation
        public int Id { get; set; } = 0;

        public int ClientId { get; set; } = 0;

        [Range(10,15000)]
        public decimal Total { get; set; }

        public List<CreateOrderDetailDTO> ProductColorSizeId { get; set; }
    }
}
