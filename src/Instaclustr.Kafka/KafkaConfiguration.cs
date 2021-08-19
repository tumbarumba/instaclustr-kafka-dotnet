// Copyright 2021 Instaclustr.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Refer to LICENSE for more information.

using Confluent.Kafka;
// using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
// using System.Text.Json;
// using NLog;
// using NLog.Extensions.Logging;

namespace Instaclustr.Kafka
{
    /// <summary>
    ///     Utility to load configuration information from files, the environment,
    ///     and the command line.
    /// </summary>
    public class KafkaConfiguration
    {
        private IConfigurationRoot configurationRoot = null;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public KafkaConfiguration(string[] args = null)
        {
            InitConfiguration(null);
        }

        /// <summary>
        ///     Retrieve a value from the configuration.
        /// </summary>
        public T GetValue<T>(string key)
        {
            return configurationRoot.GetValue<T>(key);
        }

        /// <summary>
        ///     Loads the section of the specified key into a new ProducerConfig.
        /// </summary>
        public ProducerConfig GetProducerConfig(string key)
        {
            var producerConfig = new ProducerConfig();
            configurationRoot.GetSection(key).Bind(producerConfig);
            return producerConfig;
        }

        private void InitConfiguration(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    configuration.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
                    configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                    configuration.AddEnvironmentVariables();
                    if (args != null)
                    {
                        configuration.AddCommandLine(args);
                    }
                        
                    configurationRoot = configuration.Build();
                }).Build();

/*        
        //
        // Summary:
        //     NUnit logger instance.
        private Logger logger = null;
        //
        // Summary:
        //     Represents the root of an Microsoft.Extensions.Configuration.IConfiguration hierarchy.
        private IConfigurationRoot configurationRoot = null;

        //
        // Summary:
        //     This class is an interface to map user defined
        //     configurations to the needed action in order to connect to Kafka and Schema Registry.
        //
        // Parameters:
        //   args:
        //     String arguments used to override the configurations from appsettings.json
        //     or environment variables.
        public KafkaConfiguration(string[] args = null) 
        {
            // Sets up the configuration 
            InitConfiguration(args);

            // Load NLog configuration section
            LogManager.Configuration = new NLogLoggingConfiguration(configurationRoot.GetSection("NLog"));
            this.logger = LogManager.GetCurrentClassLogger();
        }

        public string GetValue(string key)
        {
            var value = configurationRoot.GetValue<string>($"{key}");
            this.logger.Debug($"Retrieving value for key '{key}'. Result is '{value}'.");

            return value;
        }

        // Topic list
        // You can override the configuration settings by defining system environment variables:
        //     * Kafka__Topics__{topicKey}
        public string GetTopic(string topicKey)
        {
            var topicName = configurationRoot.GetValue<string>($"Kafka:Topics:{topicKey}");
            this.logger.Debug($"Retrieving topic key '{topicKey}'. Result is '{topicName}'.");

            return topicName;
        }

        //
        // Summary:
        //     Returns Producer configuration properties
        //
        // Parameters:
        //   producerKey:
        //     The producer key define in appsettings.json
        public ProducerConfig GetProducerConfig(string producerKey)
        {
            // Producer Config
            // You can override the configuration settings by defining system environment variables:
            //     * Kafka__{producerKey}__BootstrapServers
            //     * Kafka__{producerKey}__SecurityProtocol
            //     * Kafka__{producerKey}__SaslMechanism
            //     * Kafka__{producerKey}__SaslUsername
            //     * Kafka__{producerKey}__SaslPassword
            var producerConfig = new ProducerConfig();
            configurationRoot.GetSection($"Kafka:{producerKey}").Bind(producerConfig);
            logger.Debug($"Load producer settings for key {producerKey}");
            return producerConfig;
        }

        //
        // Summary:
        //     Returns Consumer configuration properties
        //
        // Parameters:
        //   consumerKey:
        //     The consumer key define in appsettings.json
        public ConsumerConfig GetConsumerConfig(string consumerKey)
        {
            // Producer Config
            // You can override the configuration settings by defining system environment variables:
            //     * Kafka__{consumerKey}__BootstrapServers
            //     * Kafka__{consumerKey}__GroupId
            //     * Kafka__{consumerKey}__AutoOffsetReset
            //     * Kafka__{consumerKey}__SecurityProtocol
            //     * Kafka__{consumerKey}__SaslMechanism
            //     * Kafka__{consumerKey}__SaslUsername
            //     * Kafka__{consumerKey}__SaslPassword
            var consumerConfig = new ConsumerConfig();
            configurationRoot.GetSection($"Kafka:{consumerKey}").Bind(consumerConfig);
            logger.Debug($"Load consumer settings for key {consumerKey}");
            return consumerConfig;
        }


        //
        // Summary:
        //     Returns Schema Registry configuration properties
        //
        // Parameters:
        //   schemaRegistryKey:
        //     The Schema Registry key define in appsettings.json
        public SchemaRegistryConfig GetSchemaRegistryConfig(string schemaRegistryKey)
        {
            // SchemaRegistry Config
            // You can override the configuration settings by defining system environment variables:
            //     * Kafka__{schemaRegistryKey}__Url
            //     * Kafka__{schemaRegistryKey}__BasicAuthCredentialsSource
            //     * Kafka__{schemaRegistryKey}__BasicAuthUserInfo
            //     * Kafka__{schemaRegistryKey}__MaxCachedSchemas
            var schemaRegistryConfig = new SchemaRegistryConfig();
            configurationRoot.GetSection($"Kafka:{schemaRegistryKey}").Bind(schemaRegistryConfig);
            logger.Debug($"Load schema registry settings for key {schemaRegistryKey}");
            return schemaRegistryConfig;
        }
*/    
    }
}
