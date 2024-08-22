using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingProjectWPF.Models
{
    public class UploadedFile
    {
        [Key]
        public int FileId { get; set; }
        [Required]
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public virtual ICollection<Balance> Balances { get; set; }
    }
}
