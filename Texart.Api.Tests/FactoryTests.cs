using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Texart.Api.Tests
{
    internal class FactoryTests
    {
        [Test]
        public void AllowsDelegateBehavior()
        {
            // sanity check to make sure that it can be used like a delegate
            TxFactory<int, Lazy<JToken>> factory = json => 5;
            int factoryResult = factory(new Lazy<JToken>(() => throw new NotImplementedException()));
            Assert.AreEqual(5, factoryResult);
        }

        [Test]
        public void AllowsJsonParsing()
        {
            // sanity check to make sure the API allows JSON passing
            TxFactory<int, Lazy<JToken>> factory = json =>
            {
                var serializer = new JsonSerializer();
                var data = json.Value.ToObject<AllowsJsonParsingSampleData>(serializer);
                return data.Int + data.Dict.Values.Sum();
            };
            var sampleJson = @"{
                ""Int"": 4,
                ""Dict"": {
                    ""key1"": 5,
                    ""key2"": 6
                }
            }";
            int factoryResult = factory(new Lazy<JToken>(() => JObject.Parse(sampleJson)));
            Assert.AreEqual(4 + 5 + 6, factoryResult);
        }

        private class AllowsJsonParsingSampleData
        {
            [JsonProperty(Required = Required.Always)]
            public int Int { get; set; }

            [JsonProperty(Required = Required.Always)]
            public IDictionary<string, int> Dict { get; set; }
        }
    }
}