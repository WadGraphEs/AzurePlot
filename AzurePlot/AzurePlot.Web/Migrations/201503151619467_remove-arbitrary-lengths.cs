namespace AzurePlot.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removearbitrarylengths : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AzureSubscriptions", "Name", c => c.String());
            AlterColumn("dbo.AzureSubscriptions", "FromSessionId", c => c.String());
            AlterColumn("dbo.AzureSubscriptions", "AzureSubscriptionId", c => c.String());
            AlterColumn("dbo.AzureSubscriptions", "Password", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AzureSubscriptions", "Password", c => c.String(maxLength: 4000));
            AlterColumn("dbo.AzureSubscriptions", "AzureSubscriptionId", c => c.String(maxLength: 4000));
            AlterColumn("dbo.AzureSubscriptions", "FromSessionId", c => c.String(maxLength: 4000));
            AlterColumn("dbo.AzureSubscriptions", "Name", c => c.String(maxLength: 4000));
        }
    }
}
