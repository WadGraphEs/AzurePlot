namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sqldatabases : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SQLDatabases",
                c => new
                    {
                        Servername = c.String(nullable: false, maxLength: 256),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Servername);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SQLDatabases");
        }
    }
}
