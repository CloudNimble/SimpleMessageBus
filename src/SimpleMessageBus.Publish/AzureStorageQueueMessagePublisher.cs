﻿using CloudNimble.SimpleMessageBus.Core;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace CloudNimble.SimpleMessageBus.Publish
{

    /// <summary>
    /// Manages the process of publishing MessageBus messages to Azure Queue Storage.
    /// </summary>
    public class AzureStorageQueueMessagePublisher : IMessagePublisher
    {

        #region Private Members

        private readonly AzureStorageQueueOptions _options;
        private CloudQueue _queue;

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="CloudQueue"/> instance this publisher will write to. 
        /// </summary>
        internal CloudQueue Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = GetQueue(_options.QueueName, _options.StorageConnectionString);
                }
                return _queue;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AzureStorageQueueMessagePublisher"/>.
        /// </summary>
        /// <param name="options">The <see cref="AzureStorageQueueOptions"/> instance to use to configure the Azure Storage Queue. Should be registered with your Dependency Injection container.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/>.QueueName is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The connection string you specified was not found in the ConnectionStrings collection.</exception>
        public AzureStorageQueueMessagePublisher(IOptions<AzureStorageQueueOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "Please register an AzureQueueOptions instance with your DI container.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.QueueName))
            {
                throw new ArgumentNullException(nameof(options.Value.QueueName), "Please specify an Azure Storage Queue name.");
            }
            if (string.IsNullOrWhiteSpace(options.Value.StorageConnectionString))
            {
                throw new ArgumentNullException(nameof(options.Value.StorageConnectionString), "Please specify the name of the Azure Storage Connection String.");
            }
            _options = options.Value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Publishes the specified <see cref="IMessage"/> to a queue.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to wrap in a <see cref="MessageEnvelope"/> and publish to the queue.</param>
        /// <param name="isSystemGenerated">Specifies whether or not the event was generated by the system. (Not currently used).</param>
        /// <returns>A <see cref="Task"/> reference for the asynchronous function.</returns>
        public async Task PublishAsync(IMessage message, bool isSystemGenerated = false)
        {
            if (Queue == null)
            {
                throw new ArgumentNullException(nameof(Queue), "The Queue has not been initialized. Pass a 'queueName' into the constructor and try again.");
            }

            //RWM: Wrap the entity in a MessageEnvelope.
            var envelope = new MessageEnvelope(message)
            {
                Id = Guid.NewGuid(),
                DatePublished = DateTimeOffset.UtcNow
            };

            //RWM: Push it onto the queue.
            await Queue.AddMessageAsync(envelope.ToCloudQueueMessage()).ConfigureAwait(false);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Connects to a given Azure Cloud Storage account to get a given Queue.
        /// </summary>
        /// <param name="queueName">The name of the CloudQueue instance to get.</param>
        /// <param name="connectionString">The connection string for the Storage Account.</param>
        internal static CloudQueue GetQueue(string queueName, string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            try
            {
                queue.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AzureQueueMessagePublisher Error: " + ex.Message);
                //TODO: RWM: Add telemetry.
                //_telemetryClient.TrackException(ex);
                throw;
            }

            return queue;
        }

        #endregion

    }

}