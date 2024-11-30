using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Models
{
    public class OrderMaster : BaseEntity<int>
    {
        [MaxLength(100)]
        public string OrderNo { get; set; }
        public int ClientId { get; set; }
        public virtual CustomUser? Client { get; set; }

        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        public int OrderStateId { get; set; }
        public virtual OrderState? OrderState { get; set; }
        [MaxLength(128)]
        public string? Notes { get; set; }
        public int? CouponId { get; set; }
        public virtual Coupon? Coupon { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
