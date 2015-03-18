namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addazuresubscriptionsession : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddAzureSubscriptionSessions",
                c => new
                    {
                        SessionId = c.String(nullable: false, maxLength: 128),
                        Password = c.String(maxLength: 4000),
                        Pfx = c.Binary(),
                    })
                .PrimaryKey(t => t.SessionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AddAzureSubscriptionSessions");
        }
    }
}
