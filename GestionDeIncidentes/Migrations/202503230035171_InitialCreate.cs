namespace GestionDeIncidentes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Incidencias", "UsuarioReportaId", "dbo.Usuarios");
            CreateTable(
                "dbo.Tecnicoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Incidencias", "Estado", c => c.String());
            AlterColumn("dbo.Incidencias", "Prioridad", c => c.String());
            AddForeignKey("dbo.Incidencias", "UsuarioReportaId", "dbo.Usuarios", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Incidencias", "UsuarioReportaId", "dbo.Usuarios");
            AlterColumn("dbo.Incidencias", "Prioridad", c => c.Int(nullable: false));
            AlterColumn("dbo.Incidencias", "Estado", c => c.Int(nullable: false));
            DropTable("dbo.Tecnicoes");
            AddForeignKey("dbo.Incidencias", "UsuarioReportaId", "dbo.Usuarios", "Id", cascadeDelete: true);
        }
    }
}
