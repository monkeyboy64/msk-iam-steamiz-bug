// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using KafkaNet.Suppliers;
using Streamiz.Kafka.Net.Kafka;
using Streamiz.Kafka.Net.Table;
using Microsoft.Extensions.Logging;

var configMSK = new StreamConfig<StringSerDes, StringSerDes>
{
    ApplicationId = "test-app-2",
    SessionTimeoutMs = 45000,
    BootstrapServers = "",
    SaslMechanism = SaslMechanism.OAuthBearer,
    SecurityProtocol = SecurityProtocol.SaslSsl,
    NumStreamThreads = 1,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    AllowAutoCreateTopics = true,
    StateDir = Path.Combine(".", Guid.NewGuid().ToString()),
    Logger = LoggerFactory.Create(b =>
    {
        b.SetMinimumLevel(LogLevel.Debug);
        b.AddConsole();
    }),
    Debug = "all",
};

StreamBuilder builder = new StreamBuilder();

builder
	.Table("streams-plaintext-input", InMemory.As<string, string>("table-store"))
	.ToStream()
	.Print(Printed<string, string>.ToOut());


Topology topology = builder.Build();

IKafkaSupplier supplier = new SupplierWithOAuth();

KafkaStream stream = new KafkaStream(topology, configMSK, supplier);

await stream.StartAsync();