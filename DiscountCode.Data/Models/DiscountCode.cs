using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscountCode.Data.Models
{
    public class DiscountCode
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(8)]
        [Column(TypeName = "varchar(8)")]
        public string Code { get; set; } = default!;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
