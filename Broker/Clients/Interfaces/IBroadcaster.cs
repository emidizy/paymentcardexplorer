using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Broker.Clients.Interfaces
{
    public interface IBroadcaster
    {
        void InitRabbitMQEventPublisher(IConnection connection);
        Task PublishPayload(string payload, string eventId);
        void DisposeConnection();
    }
}
