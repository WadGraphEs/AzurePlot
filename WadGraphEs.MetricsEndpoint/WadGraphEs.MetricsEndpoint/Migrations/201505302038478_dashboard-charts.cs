namespace WadGraphEs.MetricsEndpoint.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dashboardcharts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DashboardCharts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uri = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DashboardCharts");
        }
    }
}
