using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Models
{
    public class OrderState : BaseEntity<int>
    {
        [MaxLength(16)]
        public string StateAr { get; set; }
        
        [MaxLength(16)]
        public string StateEn { get; set; }
        public virtual ICollection<OrderMaster>? Orders { get; set; }
    }
}
