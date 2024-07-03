using CloudNimble.SimpleMessageBus.Core;
using CloudNimble.SimpleMessageBus.IndexedDb.Core;
using Microsoft.Extensions.DependencyInjection;
using SimpleMessageBus.IndexedDb.Core;
using System;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Publish.IndexedDb
{

    /// <summary>
    /// 
    /// </summary>
    public class IndexedDbMessagePublisher : IMessagePublisher
    {

        private readonly SimpleMessageBusDb _smbDb;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smbDb"></param>
        public IndexedDbMessagePublisher([FromKeyedServices(IndexedDbConstants.Queue)] SimpleMessageBusDb smbDb)
        {
            _smbDb = smbDb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isSystemGenerated"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task PublishAsync(IMessage message, bool isSystemGenerated = false)
        {
            var envelope = new MessageEnvelope
            {
                DatePublished = DateTime.UtcNow,
            };
            await _smbDb.Queue.AddAsync(envelope);
        }

    }

}
