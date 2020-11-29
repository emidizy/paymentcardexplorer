using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Binders;
using Broker.Clients.Interfaces;

namespace Broker
{
    public class MessageClient
    {
        private IModel _clientChannel;
        private readonly Logger _logger;
        private IConnection _connection;
        private readonly IServiceProvider _brokerSvcProvider;
        private readonly IBroadcaster _eventPublisher;
        private readonly BrokerConfig _brokerConfig;

        public MessageClient(ILogger<BrokerDaemon> loggerFactory, 
            IServiceProvider serviceProvider,
            IOptions<BrokerConfig> brokerConfig,
            IBroadcaster eventPublisher,
            Logger logger)
        {
            _logger = logger;
            _brokerSvcProvider = serviceProvider;
            _brokerConfig = brokerConfig.Value;
            _eventPublisher = eventPublisher;

            var connection = CreateConnectionToRabbitMQ();
            RegisterEventPublisher(connection);
        }

       
        private IConnection CreateConnectionToRabbitMQ()
        {
            var server = _brokerConfig.Server;
            var factory = new ConnectionFactory();
            if (server.isLocal)
            {
                factory = new ConnectionFactory { HostName = server.IP };
            }
            else
            {
                factory = new ConnectionFactory { HostName = server.IP, UserName = server.Username, Password = server.Password };
            }
            // create connection to Rabbitmq server
            var connection = factory.CreateConnection();

            return connection;
        }
        

        private void RegisterEventPublisher(IConnection connection)
        {
            _eventPublisher.InitRabbitMQEventPublisher(connection);
        }

    }
}
