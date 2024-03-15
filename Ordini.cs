using System.ComponentModel.DataAnnotations.Schema;

[Table("NomeCorrettoTabella")]
public class Ordini
{
    [Key]
    public int IdOrdine { get; set; }
    [Required]
    [StringLength(50)]
    public string Indirizzo { get; set; }
    public bool IsEvaso { get; set; }
    [StringLength(200)]
    public string Note { get; set; }
    public virtual Utenti Utenti { get; set; }
    public virtual ICollection<DettagliOrdini> DettagliOrdini { get; set; }

    // le proprietà del modello vanno qui
}
