using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllBirds.Models
{
    public class Product : BaseEntity<int>
    {
        [MaxLength(100)]
        public string ProductNo { get; set; }
        [MaxLength(150)]
        public string NameAr { get; set; }

        [MaxLength(150)]
        public string NameEn { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? HighlightsAr { get; set; }
        
        [MaxLength(500)]
        public string? HighlightsEn { get; set; }

        [MaxLength(500)]
        public string? SustainabilityAr { get; set; }
        
        [MaxLength(500)]
        public string? SustainabilityEn { get; set; }

        [MaxLength(1000)]
        public string? SustainableMaterialsAr { get; set; }

        [MaxLength(1000)]
        public string? SustainableMaterialsEn { get; set; }

        [MaxLength(600)]
        public string? ShippingAndReturnsAr { get; set; }

        [MaxLength(600)]
        public string? ShippingAndReturnsEn { get; set; }

        [MaxLength(1000)]
        public string? CareGuideAr { get; set; }

        [MaxLength(1000)]
        public string? CareGuideEn { get; set; }

        public int Discount { get; set; }

        public bool FreeShipping { get; set; } = false;
        public int MainColorId { get; set; }

        
        public virtual ICollection<CategoryProduct>? Categories { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductColor>? AvailableColors { get; set; }
        public virtual ICollection<ProductReview>? Reviews { get; set; }
        public virtual ICollection<ClientFavorite>? ClientsFavoriteIt { get; set; }
        public virtual ICollection<ProductSpecification>? Specifications { get; set; }
        public virtual ICollection<ProductDetail>? Details { get; set; }
    }
}
