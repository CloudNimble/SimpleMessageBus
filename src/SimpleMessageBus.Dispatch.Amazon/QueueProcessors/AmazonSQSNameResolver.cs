using CloudNimble.SimpleMessageBus.Dispatch.Amazon;
using CloudNimble.WebJobs.Extensions.Amazon.SQS;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using System;

namespace CloudNimble.SimpleMessageBus.Dispatch
{

    /// <summary>
    /// A <see cref="INameResolver"/> for SimpleMessageBus instances backed by Amazon SQS.
    /// </summary>
    public class AmazonSQSNameResolver : INameResolver
    {

        #region Private Members

        private readonly AmazonSQSOptions _options;
        private readonly SQSNameResolver _baseResolver;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AmazonSQSNameResolver"/>.
        /// </summary>
        /// <param name="options">The <see cref="AmazonSQSOptions"/> to use for configuration.</param>
        /// <param name="baseResolver">The base SQS name resolver from WebJobs.Extensions.Amazon.</param>
        public AmazonSQSNameResolver(IOptions<AmazonSQSOptions> options, SQSNameResolver baseResolver)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(baseResolver);
            _options = options.Value;
            _baseResolver = baseResolver;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <param name="name">The name to resolve.</param>
        /// <returns>The resolved value.</returns>
        public string Resolve(string name)
        {
            return name switch
            {
                "QueueName" => _options.QueueName,
                "CompletedQueueName" => _options.CompletedQueueName,
                "PoisonQueueName" => _options.PoisonQueueName,
                _ => _baseResolver.Resolve(name)
            };
        }

        #endregion

    }

}