using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleMessageBus.IndexedDb.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Dispatch.IndexedDb
{

    /// <summary>
    /// Processes queue items stored in an IndexedDB database and dispatches them to all <see cref="IMessageHandler">IMessageHandlers</see> registered with the DI container.
    /// </summary>
    public class IndexedDbQueueProcessor : IQueueProcessor, IDisposable
    {

        #region Private Members

        private readonly SimpleMessageBusDb _database;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;
        private readonly BlockingCollection<MessageEnvelope> _queue;
        private bool disposedValue;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dispatcher"></param>
        /// <param name="serviceProvider"></param>
        public IndexedDbQueueProcessor(SimpleMessageBusDb database, IMessageDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _database = database;
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
            _queue = [];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task LoadQueueItems()
        {
            var items = await _database.Queue.GetAllAsync<MessageEnvelope>();
            foreach (var item in items)
            {
                _queue.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Start(CancellationToken cancellationToken = default)
        {
            await LoadQueueItems();
            foreach (var item in _queue.GetConsumingEnumerable(cancellationToken))
            {
                await ProcessQueue(item);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The method calling this method should be responsible for getting the message from the database.
        /// </remarks>
        internal async Task ProcessQueue(MessageEnvelope messageEnvelope, ILogger logger = null)
        {
            try
            {
                using var lifetimeScope = _serviceProvider.CreateScope();
                messageEnvelope.AttemptsCount++;
                messageEnvelope.ProcessLog = logger;
                messageEnvelope.ServiceScope = lifetimeScope;
                await _dispatcher.Dispatch(messageEnvelope);
                await _database.Completed.AddAsync(messageEnvelope);
            }
            catch (Exception ex)
            {
                logger?.LogCritical(ex, "An error occurred dispatching the MessageEnvelope with ID {0}", messageEnvelope?.Id);

                if (messageEnvelope.AttemptsCount < 5)
                {
                    await _database.Queue.AddAsync(messageEnvelope);
                }
                else
                {
                    await _database.Failed.AddAsync(messageEnvelope);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _queue.CompleteAdding();
                }

                disposedValue = true;
            }
        }

        #endregion

    }

}
