namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscriptionidinsession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AddAzureSubscriptionSessions", "AzureSubscriptionId", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AddAzureSubscriptionSessions", "AzureSubscriptionId");
        }
    }
}
