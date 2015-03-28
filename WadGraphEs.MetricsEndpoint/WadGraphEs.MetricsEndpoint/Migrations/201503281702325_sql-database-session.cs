namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sqldatabasesession : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddSQLDatabaseSessions",
                c => new
                    {
                        SessionId = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        Servername = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.SessionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AddSQLDatabaseSessions");
        }
    }
}
