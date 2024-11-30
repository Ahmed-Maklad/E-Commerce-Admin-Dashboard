using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.OrderMasterDTOs
{
    public record GetAllOrderMastersDTO
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }

        public decimal Total { get; set; }
        public int OrderStateId { get; set; }
        public string? OrderStateName { get; set; }

        public string? Notes { get; set; }
        public int? CouponId { get; set; }  //coupon quantity
        public string? DiscountAmount { get; set; }  //coupon quantity
        public string? DiscountPerctnage { get; set; }  //coupon quantity

    }
}
