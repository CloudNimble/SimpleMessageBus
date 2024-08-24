﻿using CloudNimble.SimpleMessageBus.Core;
using SimpleMessageBus.IndexedDb.Core;
using System;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Publish.IndexedDb
{

    /// <summary>
    /// Manages the process of publishing MessageBus messages to a browser-based IndexedDb database.
    /// </summary>
    public class IndexedDbMessagePublisher : IMessagePublisher
    {

        #region Private Members

        private readonly SimpleMessageBusDb _database;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public IndexedDbMessagePublisher(SimpleMessageBusDb database)
        {
            _database = database;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Publishes the specified <see cref="IMessage"/> to the specified incoming queue.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to wrap in a <see cref="MessageEnvelope"/> and publish to the queue.</param>
        /// <param name="isSystemGenerated">Specifies whether or not the event was generated by the system. (Not currently used).</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        public async Task PublishAsync(IMessage message, bool isSystemGenerated = false)
        {
            var envelope = new MessageEnvelope
            {
                Id = Guid.NewGuid(),
                DatePublished = DateTime.UtcNow,
            };
            await _database.Queue.AddAsync(envelope);
        }

        #endregion

    }

}