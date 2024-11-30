using AllBirds.DTOs.OrderDetailsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.OrderMasterDTOs
{
    public class GetUserCartCheckoutDTO
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int ClientId { get; set; }
        public decimal Total { get; set; }
        public int OrderStateId { get; set; } 
        //public string? OrderStateNameAr { get; set; }
        //public string? OrderStateNameEn { get; set; }
        public string? Notes { get; set; }
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
        public string? DiscountAmount { get; set; }
        public string? DiscountPerctnage { get; set; }
        public List<GetAllCartCheckoutDetailsDTO> OrderDetails { get; set; }
    }
}
