using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Persistence.Entities
{
    [Table("CardInquiryLog")]
    public class CardInquiryLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string IIN { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int NoOfHit { get; set; }

        [Required]
        public DateTime InquiryDate { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }
    }
}
