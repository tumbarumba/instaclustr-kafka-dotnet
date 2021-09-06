using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Example.Domain.Sales;
using System;
using System.Threading;

namespace Example.Producer.Sales
{
    public class ExampleSalesProducer
    {
        static void Main(string[] args)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig { Url = "http://127.0.0.1:8081" };
            var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
            var producerConfig = new ProducerConfig { BootstrapServers = "0.0.0.0:9092" };
            var producerBuilder = new ProducerBuilder<long, Customer>(producerConfig)
                .SetValueSerializer(new AvroSerializer<Customer>(schemaRegistryClient).AsSyncOverAsync())
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"));
            var topic = "sales.customerupdates";

            using (var producer = producerBuilder.Build())
            {
                FakeDataGenerator generator = new FakeDataGenerator();
                while(true)
                {
                    var customer = generator.FakeCustomer();

                    Console.WriteLine($"Writing customer {customer.customerId} ({customer.name.givenName} {customer.name.familyName})...");
                    var message = new Message<long, Customer>() { Key = customer.customerId, Value = customer };
                    producer.ProduceAsync(topic, message)
                        .ContinueWith( task => {
                            if (task.IsFaulted)
                            {
                                Console.WriteLine($"-> Failed to write customer {customer.customerId}: {task.Result.Message}");
                            }
                            else
                            {
                                Console.WriteLine($"-> Wrote customer {customer.customerId} to [{task.Result.TopicPartitionOffset}]");
                            }
                        });

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
