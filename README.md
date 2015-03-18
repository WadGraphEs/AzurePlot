# WadGraphEs Azure Dashboard
This is an open source project for monitoring your Azure based applications. It's goal is to provide a better tool for monitoring your Azure applications by providing both graphs of your metrics as well as exposing them through an API.

## Current project status
* Connect to the Azure Subscription REST API to read portal metrics for Azure Cloud Services, VMs and Web Sites.
* Connect to the Windows Azure Web Sites API to read Web Site specific metrics.
* Bootstrap based GUI to setup the application and the services.
* Expose metrics through an API.
* Integrated API test tool.
* Integrated log viewer to quickly diagnose problems.
* Can run on any IIS web server including Windows Azure Web Sites

## How to use?
Simply clone the project from GitHub, open the solution in Visual Studio and build and run the solution. THis will start a web application through which you can setup the application.

## What's next
We're working on:
* Azure SQL Database support
* A dashboard system to quickly show your application state

## Acknowledgments
We heavily rely on other people's work, specifically:
* [Bootstrap](http://getbootstrap.com/)
* [Mono.Security](http://www.mono-project.com/archived/cryptography/)
And lots more. Thank you!

## Uses
This project can be used to feed [WadGraphEs](http://www.wadgraphes.com), a tool for monitoring you Azure powered websites.
