using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestingProjectWPF.Models
{
    public class Account
    {
        [Key]
        [Column("Id")]
        [StringLength(10)]
        public string Id { get; set; }

        [Required]
        [Column("Class")]
        public string Class { get; set; }

        public virtual Balance Balance { get; set; }
    }
}
