using Broker.Clients.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Binders;

namespace Broker.Clients.Services
{
    public class Broadcaster : IBroadcaster
    {
        private IModel _channel;
        private Logger _logger;
        private IConnection _connection;
        private readonly BrokerConfig _brokerConfig;
        private static NLog.Logger Nlogger;

        public Broadcaster(Logger logger,
            IOptions<BrokerConfig> brokerConfig)
        {
            _logger = logger;
            _brokerConfig = brokerConfig.Value;
        }

        public void InitRabbitMQEventPublisher(IConnection connection)
        {

            // create channel
            _connection = connection;
            _channel = _connection.CreateModel();

            //declare exchange to be used for message delivery
            _channel.ExchangeDeclare(_brokerConfig.Server.Exchange, ExchangeType.Direct);

            //declare queue where message will be sent
            _channel.QueueDeclare(_brokerConfig.ClientQueue.QueueId, false, false, false, null);

            //Bind queue to use defined exchange
            _channel.QueueBind(_brokerConfig.ClientQueue.QueueId, _brokerConfig.Server.Exchange, _brokerConfig.ClientQueue.RoutingKey, null);
            _channel.BasicQos(0, _brokerConfig.ClientQueue.MaxQueueCount, false);

            //enable publisher confirm
            _channel.ConfirmSelect();

            //Event handler for payload published successfully
            _channel.BasicAcks += OnPayloadPublished;

            //Set Event to be triggered on connection shutdown
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        public Task PublishPayload(string payload, string eventId)
        {
            if (!String.IsNullOrEmpty(payload))
            {
                //Add tag (id) to content for easy tracking
                IBasicProperties basicProp = _channel.CreateBasicProperties();
                basicProp.Headers = new Dictionary<string, object>
                {
                    {
                        "eventId", eventId
                    }
                };

                var body = Encoding.UTF8.GetBytes(payload);
                //Publish message to Queue
                _channel.BasicPublish(exchange: _brokerConfig.Server.Exchange,
                                     routingKey: _brokerConfig.ClientQueue.RoutingKey,
                                     basicProperties: basicProp,
                                     body: body);

            }

            return Task.CompletedTask;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInfo($"connection to RabbitMQ shut down {e.ReplyText}");
        }

        private void OnPayloadPublished(object sender, BasicAckEventArgs e)
        {
            _logger.LogInfo($"RabbitMQ: Payload {e.DeliveryTag} published successfully");

        }

        public void DisposeConnection()
        {
            _logger.LogInfo("Closing connection...");

            //Disconnect from RabbitMQ
            _channel.Close();
            if (_connection.IsOpen)
            {
                _connection.Close();
            }
            
        }
    }
}
