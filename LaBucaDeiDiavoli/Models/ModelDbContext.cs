﻿using System;
using System.Data.Entity;
using System.Linq;

namespace LaBucaDeiDiavoli.Models
{
    public class ModelDbContext : DbContext
    {
        // Il contesto è stato configurato per utilizzare una stringa di connessione 'ModelDbContext' dal file di configurazione 
        // dell'applicazione (App.config o Web.config). Per impostazione predefinita, la stringa di connessione è destinata al 
        // database 'LaBucaDeiDiavoli.Models.ModelDbContext' nell'istanza di LocalDb. 
        // 
        // Per destinarla a un database o un provider di database differente, modificare la stringa di connessione 'ModelDbContext' 
        // nel file di configurazione dell'applicazione.
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        // Aggiungere DbSet per ogni tipo di entità che si desidera includere nel modello. Per ulteriori informazioni 
        // sulla configurazione e sull'utilizzo di un modello Code, vedere http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Articoli> Articoli { get; set; }
        public virtual DbSet<DettagliOrdini> DettagliOrdini { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}