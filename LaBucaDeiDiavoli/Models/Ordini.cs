using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LaBucaDeiDiavoli.Models
{
    public class Ordini
    {
        [Key]
        public int IdOrdine { get; set; }

        [ForeignKey("Utenti")]
        public int IdUtente { get; set; }

        [Required]
        [StringLength(50)]
        public string Indirizzo { get; set; }

        public bool IsEvaso { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        public virtual Utenti Utenti { get; set; }

       
        public virtual ICollection<DettagliOrdini> DettagliOrdini { get; set; }
        public DateTime DataOrdine { get; internal set; }
    }
}