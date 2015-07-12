using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzurePlot.Lib.SQLDatabase;
using AzurePlot.Web.Logging;
using AzurePlot.Web.MVC.Commands;
using AzurePlot.Web.MVC.ViewModels;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.MVC.Controllers {
    public class HomeController : Controller{
		[HttpGet]
        public ActionResult Index() {
			var subscriptions = AzureSubscriptions.ListForOverview();
			var databases = AzureSQLDatabases.ListForOverview();
            return View(new OverviewViewModel{
				Services = subscriptions.Concat(databases).OrderBy(_=>_.Name).ToList()
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
			var pfx = WadGraphEs.X509Tools.GenerateCertificate.GeneratePfx(cmd.CertificateName,password);

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
			return View((object)Guid.NewGuid().ToString());
		}

		[HttpGet]
		public ActionResult AddAzureSQLDatabaseStep1(string sessionId) {
			return View((object)sessionId);
		}

		[HttpGet]
		public ActionResult AddAzureSQLDatabaseStep2(string sessionId) {
			return View(new CreateAzureSQLDatabaseCommand(){
				SessionId = sessionId
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddAzureSQLDatabaseStep2(CreateAzureSQLDatabaseCommand cmd, string sessionId) {
			if(!ModelState.IsValid) {
				return View(cmd);
			}

			if(AzureSQLDatabases.AlreadyHasServer(cmd.Servername)) {
				ModelState.AddModelError("servername", "This database is already configured");
				return View(cmd);
			}

			var testResult = AzureSQLDatabases.TestConnection(cmd);

			if(testResult.Failed) {
				ModelState.AddModelError("test-connection", testResult.ToString());
				return View(cmd);
			}

			AzureSQLDatabases.StoreInSession(sessionId, cmd);

			return RedirectToAction("AddAzureSQLDatabaseStep3", new {sessionId = sessionId });
		}

		[HttpGet]
		public ActionResult AddAzureSQLDatabaseStep3(string sessionId) {
			var finishCmd = AzureSQLDatabases.GetFinishCommandForSession(sessionId);
			return View(finishCmd);
		}

		[HttpPost]
		public ActionResult AddAzureSQLDatabaseStep3(string sessionId, string dummy) {
			AzureSQLDatabases.FinishSession(sessionId);
			return RedirectToRoute("Home");
		}

		[HttpGet]
		public ActionResult Dashboard() {
			return View(Setup.Dashboard.GetDashboardCharts());
		}

		[HttpPost]
		public ActionResult AddDashboardChart(string uri) {
			return Json(Setup.Dashboard.AddChart(uri));
			
		}

        [HttpPost]
        public ActionResult RemoveDashboardChart(int chartId) {
            Setup.Dashboard.RemoveChart(chartId);
            return Content("Ok");            
        }

        [HttpGet]
        public ActionResult GetChartInfoById(int id) {
            return Json(Setup.Dashboard.GetChartById(id),JsonRequestBehavior.AllowGet);
        }
    }
}