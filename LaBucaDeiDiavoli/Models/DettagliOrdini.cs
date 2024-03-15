using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LaBucaDeiDiavoli.Models
{
    public class DettagliOrdini
    {
        [Key]
        public int IdDettagliOrdine { get; set; }

        [ForeignKey("Ordini")]
        public int IdOrdine { get; set; }

        [ForeignKey("Articoli")]
        public int IdArticolo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore di 0")]
        public int Quantita { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal PrezzoTotale { get; set; }

        public virtual Ordini Ordini { get; set; }
        public virtual Articoli Articoli { get; set; }

      
    }
}