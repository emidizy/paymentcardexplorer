# paymentcardexplorer
Web service for looking up credit and debit card meta data

Requirements: • Visual studio 2017 or later • .Net core 2.2 or later • MSSQL Server (local or cloud hosted) • RabbitMQ 3.7+ (local or cloud hosted)

Description: This project is made up of two applications; A payment card inquiry application (.Net Core Web Service) and a receiver application (Console application).

• This application (card inquiry microservice) looks up credit and debit card meta data from a third party API (https://binlist.net/) using a payment card’s Issuer Identification Number. This service returns the card details to the client and also publishes specific details of the returned card details to a RabbitMQ queue for the receiver application to consume.

• Card Inquiry records are persisted on a MsSQL database and this can be graphically accessed via an SQL Server Management Studio


• Tests on this application was done using xUnit, moq and Fluent Assertions.

• Documentation on endpoint(s) available on this microservice can be found on https://documenter.getpostman.com/view/5446757/TVmLCJFy or {{baseUrl}}/swagger 


Usage Guide 

Follow the steps below before you run this application; 

• Open the project’s appsettings file & ensure the database and RabbitMQ broker configurations are same as on your environment. 

• Open ‘’Package Manager console’’ CLI on visual studio while the project is open 

• Execute the following command on the CLI to create the application’s database

	update-database

Once done with the above steps, run this application (advised to keep the receiver application running at same time so that you can get real time Publish - Ack events). To trigger a publish to the queue, make a successful request to the “CardDetails/retrieve” endpoint provided on this service. The card inquiry history can be queried via the “CardDetails/inquiry/count” endpoint.

Assumptions:

All dependencies (runtime, database, RabbitMQ) are available and properly configured on your environment and the third party card inquiry API (https://binlist.net) is always available and returns card details for a given valid card IIN (Issuer Identification Number).

Contact: ediala94@gmail.com

