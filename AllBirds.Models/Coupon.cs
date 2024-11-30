using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Models
{
    public class Coupon : BaseEntity<int>
    {
        [MaxLength(8)]
        public string Code { get; set; }
        public int Discount { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<OrderMaster>? Orders { get; set; }
    }
}
