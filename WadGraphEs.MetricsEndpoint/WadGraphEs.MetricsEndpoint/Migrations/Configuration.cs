namespace AzurePlot.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataAccess.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //this is to be backwards compatible to the situation where the namespace was WadGraphEs.MetricsEndpoint instead of AzurePlot.Web
            this.ContextKey = "WadGraphEs.MetricsEndpoint.Migrations.Configuration";
        }

        protected override void Seed(DataAccess.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
