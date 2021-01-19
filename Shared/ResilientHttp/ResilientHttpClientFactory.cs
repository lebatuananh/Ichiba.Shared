using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;

namespace Shared.ResilientHttp
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly int _exceptionsAllowedBeforeBreaking;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly int _retryCount;

        public ResilientHttpClientFactory(ILogger<ResilientHttpClient> logger, int exceptionsAllowedBeforeBreaking = 5,
            int retryCount = 6)
        {
            _logger = logger;
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _retryCount = retryCount;
        }


        public ResilientHttpClient CreateResilientHttpClient()
        {
            return new ResilientHttpClient(origin => CreatePolicies(), _logger);
        }

        private Policy[] CreatePolicies()
        {
            return new Policy[]
            {
                Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        // number of retries
                        _retryCount,
                        // exponential backofff
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        // on retry
                        (exception, timeSpan, retryCount, context) =>
                        {
                            var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                                      $"of {context.PolicyKey} " +
                                      $"at {context.OperationKey}, " +
                                      $"due to: {exception}.";
                            // _logger.LogWarning(msg);
                            // _logger.LogDebug(msg);
                        }),
                Policy.Handle<HttpRequestException>()
                    .CircuitBreakerAsync(
                        // number of exceptions before breaking circuit
                        _exceptionsAllowedBeforeBreaking,
                        // time circuit opened before retry
                        TimeSpan.FromMinutes(1),
                        (exception, duration) =>
                        {
                            // on circuit opened
                            // _logger.LogTrace("Circuit breaker opened");
                        },
                        () =>
                        {
                            // on circuit closed
                            // _logger.LogTrace("Circuit breaker reset");
                        })
            };
        }
    }
}