using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcDiscountCode.Data.Models
{
    [Table("DiscountCodes")]
    public class DiscountCode
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(8)]
        [Column(TypeName = "varchar(8)")]
        public string Code { get; set; } = default!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
