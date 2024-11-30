using AllBirds.DTOs.OrderDetailsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.DTOs.OrderMasterDTOs
{
    public class GetAllClientOrderMasterDTO
    {
        public int Id { get; set; } = 0;
        public string OrderNo { get; set; }
        public int ClientId { get; set; } = 0;
        public string? ClientName { get; set; }
        public string? ClientAddress { get; set; }
        public decimal Total { get; set; } = 0;
        public int OrderStateId { get; set; } = 0;
        public string? OrderStateName { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public int DiscountPercentage { get; set; } = 0;
        public string? DateOrdered { get; set; }
        public List<GetAllClientOrderDetailsDTO>? Details { get; set; }
    }
}
