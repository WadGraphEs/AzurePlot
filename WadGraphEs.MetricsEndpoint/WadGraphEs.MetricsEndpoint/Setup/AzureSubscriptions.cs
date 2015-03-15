using Mono.Security.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AzureSubscriptions {
		internal static void StorePfxForSession(string sessionId,byte[] pfx,string password) {
			var dataContext = GetDataContext();

			var session = dataContext.AddAzureSubscriptionSessions.Find(sessionId);
			if(session == null) {
				session = new AddAzureSubscriptionSession {
					SessionId = sessionId
				};
				dataContext.AddAzureSubscriptionSessions.Add(session);
			}
			
			session.Pfx = pfx;
			session.Password = password;

			dataContext.SaveChanges();
		}

		private static DataContext GetDataContext() {
			return new DataContext();
		}

		internal static byte[] GetCertificateForSession(string sessionId) {
			var record = GetDataContext().AddAzureSubscriptionSessions.Find(sessionId);
			if(record==null) {
				throw new KeyNotFoundException("Session not found");
			}

			return X509Tools.GenerateCertificate.GetCertificateForBytes(record.Pfx,record.Password);
		}
	}
}