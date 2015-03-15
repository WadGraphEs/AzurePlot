using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.DataAccess {
	public class DataContext : IdentityDbContext<ProxyUser>{
		public DataContext() : base("wadgraphes-proxy"){
			Database.SetInitializer<DataContext>(null);
		}

		public DbSet<APIKeyRecord> APIKeys{get;set;}
		public DbSet<AddAzureSubscriptionSession> AddAzureSubscriptionSessions{get;set;}

		public DbSet<AzureSubscription> AzureSubscriptions{get;set;}
	}
}