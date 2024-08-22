using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestingProjectWPF.Models
{
    public class Balance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [ForeignKey("Account")]
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
        public decimal OpeningActive { get; set; }
        public decimal OpeningPassive { get; set; }
        public decimal TurnoverDebit { get; set; }
        public decimal TurnoverCredit { get; set; }

        [NotMapped]
        public decimal ClosingActive => OpeningActive + TurnoverDebit - TurnoverCredit;

        [NotMapped]
        public decimal ClosingPassive => OpeningPassive + TurnoverCredit - TurnoverDebit;

        [ForeignKey("UploadedFile")]
        public int FileId { get; set; }
        public virtual UploadedFile UploadedFile { get; set; }
    }
}
