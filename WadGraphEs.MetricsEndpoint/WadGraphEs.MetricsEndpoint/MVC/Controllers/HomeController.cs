using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.Logging;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    public class HomeController : Controller{
		[HttpGet]
        public ActionResult Index() {
			var services = AzureSubscriptions.ListForOverview();
            return View(new OverviewViewModel{
				Services = services
			});
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
		public ActionResult AddAzureSubscriptionStep3(string sessionId) {
			return View(new EnterSubscriptionIdCommand{  SessionId = sessionId});
		}


		[HttpPost]
		public ActionResult AddAzureSubscriptionStep3(EnterSubscriptionIdCommand cmd) {
			if(!ModelState.IsValid) {
				return View(cmd);
			}

			var exceptions = AzureSubscriptions.TestConnection(cmd.SessionId,cmd.AzureSubscriptionId);

			if(exceptions.Any()) {
				foreach(var exc in exceptions) {
					ModelState.AddModelError("test-configuration",exc);
				}

				return View(cmd);
			}

			AzureSubscriptions.AddAzureSubscriptionIdForSession(cmd.SessionId,cmd.AzureSubscriptionId);

			return RedirectToAction("AddAzureSubscriptionStep4", new { sessionId=cmd.SessionId });
		}

		[HttpGet]
		public ActionResult AddAzureSubscriptionStep4(string sessionId) {
			var finishCommand = AzureSubscriptions.CreateFinishCommandForSession(sessionId);
			return View(finishCommand);
		}

		[HttpPost]
		public ActionResult AddAzureSubscriptionStep4(FinishAddingAzureSubscription cmd) {
			AzureSubscriptions.Handle(cmd);
			return RedirectToRoute("Home");
		}

		[HttpGet]
		public ActionResult DownloadCertificate(string sessionId) {
			var cert = AzureSubscriptions.GetCertificateForSession(sessionId);

			return File(cert,"application/octet-stream","management.cer");
		}

        [HttpGet]
        public ActionResult ApiSettings() {
            var settings = APIEndpoint.GetApiSettings();

            return View(settings);
        }

        [HttpGet]
        public ActionResult TestApi() {
            var apiSettings = APIEndpoint.GetApiSettings();

            return View(new TestAPIViewModel {
                Command = new TestAPICommand {
                    APIKey = apiSettings.APIKey,
                    EndpointUrl = apiSettings.EndpointUrl,
                    Accept = "application/json"
                }
            });
        }

        [HttpPost]
        public ActionResult TestApi(TestAPICommand cmd) {
            try {
                var result = APIEndpoint.TestEndpoint(cmd);
                return View(new TestAPIViewModel {
                    Command = cmd,
                    Result = result
                });
            }
            catch(Exception e) {
                return View(new TestAPIViewModel {
                    Command = cmd,
                    Result = TestAPIResult.FromException(e)
                });    
            }
        }

		[HttpGet]
		public ActionResult Logs() {
			return View(ViewableLogTarget.GetLatestMessages());
		}

		[HttpGet]
		public ActionResult AddAzureSQLDatabase() {
			return View();
		}

		[HttpGet]
		public ActionResult AddAzureSQLDatabaseStep1() {
			return View();
		}
    }
}