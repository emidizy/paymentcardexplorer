using AppCore.Application.Interfaces;
using AppCore.Application.Services;
using AppCore.Shared.Interfaces;
using AppCore.Shared.Services;
using Broker;
using Broker.Clients.Interfaces;
using Broker.Clients.Services;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repository.Interfaces;
using Persistence.Repository.Services;
using Persistence.UnitOfWork.Interfaces;
using Persistence.UnitOfWork.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace PaymentCardExplorer.ServiceRegistry
{
    public static class AppServiceCollection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<Logger>();

            //Register Db services
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ICardInquiryRepository, CardInquiryRepository>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            //Register Core services
            services.AddTransient<IRestResponse, RestResponse>();
            services.AddTransient<IHttpClient, HttpClient>();
            services.AddTransient<IBinListService, BinListService>();
            services.AddTransient<IInquiryCountService, InquiryCountService>();
            services.AddTransient<IResponseHandler, ResponseHandler>();

            //Register Broker services
            services.AddSingleton<IBroadcaster, Broadcaster>();
            services.AddHostedService<BrokerDaemon>();


            return services;
        }
    }
}
