namespace LaBucaDeiDiavoli.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioinint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articolis", "Prezzo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articolis", "Prezzo", c => c.String());
        }
    }
}
