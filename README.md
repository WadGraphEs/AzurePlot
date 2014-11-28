wadgraphes-azure-metrics-wrapper
================================
This is an ASP.NET MVC project that outputs application metrics coming out of Windows Azure. 



## Getting started
To run this project you need to take a few step

1. Create a management certificate
2. Upload the management certificate in Windows Azure
3. Export the private key
4. Configure the application
5. Deploy the project to Azure
6. Test deployment (optional)

### 1. Create a management certificate
This project leverages the [Azure Service Management REST API](http://msdn.microsoft.com/en-us/library/azure/ee460799.aspx) using the management certificate method of authentication. 

To create a management certificate first run

`makecert -sky exchange -r -n "CN=<CertificateName>" -pe -a sha1 -len 2048 -ss My "<CertificateName>.cer"`

In a Visual Studio command prompt. This will install the certificate + key in your Personal Certificate (My) store, and output a .cer file that contains only the certificate.

For more info on create the certificate look at http://msdn.microsoft.com/en-us/library/azure/gg551722.aspx or http://msdn.microsoft.com/en-us/library/bfsktky3(v=vs.110).aspx for more details about makecert.

### 2. Upload the certificate to Azure
In the Azure management portal go to Settings -> Management Certificates -> Upload. Here you specify the certificate you just created.

### 3. Export the private key
In Windows, open the Certificate Manager (`certmgr.msc`) and go to Personal -> Certificates. Here you'll also find the certificate you just created. Open the certificate and goto Details -> Copy To File... Select "Yes, export the private key" when asked and on the next page select the PFX option. On the next page check "Password" and enter a password to your private key. Finally choose the location where you want to put the pfx.

### 4. Configure the application
In the `WadGraphEs.MetricsEndpoint` directory there is an example `metricsendpoint.config` file, `metricsendpoint.config.example`, copy this file to `metricsendpoint.config` and fill the values:
* `AuthenticationKey` this is used to authenticate users trying to access the metrics through the endpoint. You can use pwgen or something else to create this password.
* `AzureSubscriptionId` this is the Azure Subscription Id you uploaded the certificate for, you can find it (among others) in the Management Certificates overview in the portal.
* `CertFilename` The filename of the private key you created in step 3.
* `CertPassword` The password of the private key you created in step 4.

### 5. Deploy the `WadGraphEs.MetricsEndpoint` project to a web server
First, add both the PFX file and the `metricsendpoint.config` file to the `WadGraphEs.MetricsEndpoint` project in Visual Studio. This will make sure they are also deployed. Then deploy the project to a webserver (for example: an Azure Web Site). Make sure your PFX file has a pfx file extension, because we automatically deny access to that.

### 6. Test the deployment (optional)
This requires you to have curl installed. From the command prompt, type

`curl -H "Authorization: MetricsEndpoint-Key <AuthenticationKey>" http(s)://<yourhost>/usages`

With `AuthenticationKey` being the AuthenticationKey you specified in the `metricsendpoint.config` file.

This should result in some metrics JSON containing stats about your services being written.

Now you're done.

## Uses
This project can be used to feed [WadGraphEs](http://www.wadgraphes.com), a tool for monitoring you Azure powered websites.
