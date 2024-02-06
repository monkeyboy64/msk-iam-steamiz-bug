using AWS.MSK.Auth;
using Confluent.Kafka;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.Metrics;

namespace KafkaNet.Suppliers;

class SupplierWithOAuth : IKafkaSupplier
{

    private AWSMSKAuthTokenGenerator mskAuthTokenGenerator = new AWSMSKAuthTokenGenerator();

    public StreamMetricsRegistry MetricsRegistry { get; set; }

    public IAdminClient GetAdmin(AdminClientConfig config)
    {
        return new AdminClientBuilder(config).SetOAuthBearerTokenRefreshHandler(OAuthHandler).Build();
    }

    public IConsumer<byte[], byte[]> GetConsumer(ConsumerConfig config, IConsumerRebalanceListener rebalanceListener)
    {
        ConsumerBuilder<byte[], byte[]> builder = new(config);
        if (rebalanceListener != null)
        {
            builder.SetPartitionsAssignedHandler(rebalanceListener.PartitionsAssigned);
            builder.SetPartitionsRevokedHandler(rebalanceListener.PartitionsRevoked);
            builder.SetPartitionsLostHandler(rebalanceListener.PartitionsLost);
        }
        
        return new ConsumerBuilder<byte[], byte[]>(config).SetOAuthBearerTokenRefreshHandler(OAuthHandler).Build();
    }

    public IConsumer<byte[], byte[]> GetGlobalConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<byte[], byte[]>(config).SetOAuthBearerTokenRefreshHandler(OAuthHandler).Build();
    }

    public IProducer<byte[], byte[]> GetProducer(ProducerConfig config)
    {
        return new ProducerBuilder<byte[], byte[]>(config).SetOAuthBearerTokenRefreshHandler(OAuthHandler).Build();
    }

    public IConsumer<byte[], byte[]> GetRestoreConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<byte[], byte[]>(config).SetOAuthBearerTokenRefreshHandler(OAuthHandler).Build();
    }

    private void OAuthHandler(IClient client, string cfg) 
    {
        var (token, expiryMs) = mskAuthTokenGenerator.GenerateAuthTokenAsync(Amazon.RegionEndpoint.USEast1).Result;
        
        client.OAuthBearerSetToken(token, expiryMs, "Dummy");
    }
}