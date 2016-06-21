using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcWebApp.Models
{
    [Table("Anexos")]
    public class AnexoInfo
    {
        [Key]
        public Guid IdAnexo { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long ContentLength { get; set; }

        public string ServerPath { get; set; }
    }
}