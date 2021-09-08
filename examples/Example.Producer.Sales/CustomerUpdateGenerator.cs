using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Example.Domain.Sales;
using System;
using System.Threading;

namespace Example.Producer.Sales
{
    public class CustomerUpdateGenerator
    {
        // tag::Run[]
        public void Run()
        {
            using (var producer = BuildProducer())
            {
                while(true)
                {
                    ProduceFakeCustomer(producer);
                    Thread.Sleep(1000);
                }
            }
        }
        // end::Run[]

        // tag::BuildProducer[]
        private IProducer<string, Customer> BuildProducer()
        {
            var registryConfig = new SchemaRegistryConfig { 
                Url = "http://127.0.0.1:8081" 
            };
            var registryClient = new CachedSchemaRegistryClient(
                registryConfig);
            var customerSerializer = new AvroSerializer<Customer>(
                registryClient);
            var producerConfig = new ProducerConfig { 
                BootstrapServers = "0.0.0.0:9092" 
            };

            return new ProducerBuilder<string, Customer>(producerConfig)
                .SetValueSerializer(customerSerializer.AsSyncOverAsync())
                .SetErrorHandler((_, e) => {
                    Console.WriteLine($"Error: {e.Reason}");
                })
                .Build();
        }
        // end::BuildProducer[]

        // tag::ProduceFakeCustomer[]
        private void ProduceFakeCustomer(IProducer<string, Customer> producer)
        {
            var faker = new FakeDataGenerator();
            var fake = faker.FakeCustomer();

            Console.WriteLine($"Writing customer {fake.customerId} " +
                $"({fake.name.givenName} {fake.name.familyName})...");

            var message = new Message<string, Customer>() { 
                Key = fake.customerId,
                Value = fake 
            };

            var topic = "Sales.CustomerUpdates";
            producer.ProduceAsync(topic, message)
                .ContinueWith( task => {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine($"-> Failed to write customer " +
                            $"{fake.customerId}: {task.Result.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"-> Wrote customer {fake.customerId} " +
                            $"to [{task.Result.TopicPartitionOffset}]");
                    }
                });
        }
        // end::ProduceFakeCustomer[]
    }
}
