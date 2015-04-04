namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sqldatabaseversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SQLDatabases", "Version", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SQLDatabases", "Version");
        }
    }
}
