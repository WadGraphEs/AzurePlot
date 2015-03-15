using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    public class HomeController : Controller{
		[HttpGet]
        public ActionResult Index() {
            return View(new OverviewViewModel());
        }
		[HttpGet]
		public ActionResult AddAzureSubscription() {
			var sessionId = Guid.NewGuid().ToString();
			return View((object)sessionId);
		}

		[HttpGet]
		public ActionResult AddAzureSubscriptionStep1(string sessionId) {
			return View(new CreateManagementCertificateCommand {
				SessionId = sessionId
			});
		}

		[HttpPost]
		public ActionResult AddAzureSubscriptionStep1(CreateManagementCertificateCommand cmd, string sessionId) {
			if(!ModelState.IsValid) {
				return View(cmd);
			}

			var password = Guid.NewGuid().ToString();
			var pfx = X509Tools.GenerateCertificate.GeneratePfx(cmd.CertificateName,password);

			AzureSubscriptions.StorePfxForSession(sessionId,pfx,password);

			return RedirectToAction("AddAzureSubscriptionStep2", new { sessionId = sessionId });
		}

		[HttpGet]
		public ActionResult AddAzureSubscriptionStep2(string sessionId) {
			return View(new UploadCertificateViewModel { 
				SessionId = sessionId,
		
			});
		}

		[HttpGet]
		public ActionResult DownloadCertificate(string sessionId) {
			var cert = AzureSubscriptions.GetCertificateForSession(sessionId);

			return File(cert,"application/octet-stream","management.cer");
		}
    }
}