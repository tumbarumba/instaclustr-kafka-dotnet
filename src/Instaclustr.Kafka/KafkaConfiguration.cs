// Copyright 2021 Instaclustr Pty Ltd
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
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

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
            this.configurationRoot = LoadConfiguration(args);
        }

        /// <summary>
        ///     Retrieve a value from the configuration.
        /// </summary>
        public T GetValue<T>(string key) =>
            configurationRoot.GetValue<T>(key);

        /// <summary>
        ///     Retrieve a section from the configuration.
        /// </summary>
        public IConfigurationSection GetSection(string key) =>
            configurationRoot.GetSection(key);

        /// <summary>
        ///     Loads the section of the specified key into a new ProducerConfig.
        /// </summary>
        public ProducerConfig GetProducerConfig(string key) =>
            BindNewConfig<ProducerConfig>(key);

        /// <summary>
        ///     Loads the section of the specified key into a new ConsumerConfig.
        /// </summary>
        public ConsumerConfig GetConsumerConfig(string key) =>
            BindNewConfig<ConsumerConfig>(key);

        /// <summary>
        ///     Loads the section of the specified key into a new SchemaRegistryConfig.
        /// </summary>
        public SchemaRegistryConfig GetSchemaRegistryConfig(string key) =>
            BindNewConfig<SchemaRegistryConfig>(key);

        private T BindNewConfig<T>(string key) where T : new()
        {
            var config = new T();
            configurationRoot.GetSection(key).Bind(config);
            return config;
        }

        private static IConfigurationRoot LoadConfiguration(string[] args)
        {
            IConfigurationRoot root = null;
            Action<IConfigurationRoot> setRoot = (newRoot) => root = newRoot;

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
                        
                    setRoot(configuration.Build());
                }).Build();

            return root;
        }
    }
}
