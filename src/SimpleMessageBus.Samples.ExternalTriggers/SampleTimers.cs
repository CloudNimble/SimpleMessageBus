using CloudNimble.SimpleMessageBus.Publish;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SimpleMessageBus.Samples.Core;
using System;
using System.Threading.Tasks;

namespace SimpleMessageBus.Samples.ExternalTriggers
{

    /// <summary>
    /// This class demonstrates how to have <see cref="TimerTriggerAttribute">TimerTrigger</see> methods live in separate assemblies.
    /// </summary>
    public class SampleTimers
    {

        IMessagePublisher _publisher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        public SampleTimers(IMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        /// <summary>
        /// A sample function to show how to trigger jobs that run at specific times or specific frequencies.
        /// </summary>
        /// <param name="myTimer">The passed in <see cref="TimerInfo"/> object with details about the Timer event that triggered the function call.</param>
        /// <param name="log">The <see cref="ILogger"/> instance to write log entries to.</param>
        /// <remarks>
        /// The <see cref="FunctionNameAttribute"/> is required, along with a function to register dependencies in the DI container (even if that function does nothing),
        /// in order for Functions in separate assemblies to be located and called properly. The name you specify can be anything you like, and does not have to match the
        /// actual name of the method.
        /// </remarks>
        [FunctionName(nameof(Run))]
        public async Task Run([TimerTrigger("00:00:10", UseMonitor = true)]TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var message = new NewUserMessage()
            {
                Id = Guid.NewGuid(),
                Email = "timertest@simplemessagebus"
            };
            await _publisher.PublishAsync(message);
        }

    }

}