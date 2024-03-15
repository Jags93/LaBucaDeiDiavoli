using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LaBucaDeiDiavoli.Models
{
    public class Articoli
    {
        [Key]
        public int IdArticolo { get; set; }
        public string Nome { get; set; }
        public string Immagine { get; set; }
        
        public decimal Prezzo { get; set; }
        public int TempoConsegna { get; set; }
    }
}