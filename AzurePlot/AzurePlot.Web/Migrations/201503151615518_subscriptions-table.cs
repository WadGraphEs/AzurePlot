namespace AzurePlot.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AzureSubscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        FromSessionId = c.String(maxLength: 4000),
                        AzureSubscriptionId = c.String(maxLength: 4000),
                        AddedOnUtc = c.DateTime(nullable: false),
                        Pfx = c.Binary(),
                        Password = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AzureSubscriptions");
        }
    }
}
