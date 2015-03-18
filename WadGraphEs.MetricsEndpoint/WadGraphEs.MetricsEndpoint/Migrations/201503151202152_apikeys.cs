namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apikeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.APIKeyRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        APIKey = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.APIKey);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.APIKeyRecords", new[] { "APIKey" });
            DropTable("dbo.APIKeyRecords");
        }
    }
}
