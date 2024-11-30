using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Models
{
    public class ProductColorSize : BaseEntity<int>
    {
        public int ProductColorId { get; set; }
        public virtual ProductColor? ProductColor { get; set; }
        public int SizeId { get; set; }
        public virtual Size? Size { get; set; }
        public int UnitsInStock { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
