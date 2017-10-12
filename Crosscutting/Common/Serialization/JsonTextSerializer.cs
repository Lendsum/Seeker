using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lendsum.Crosscutting.Common.Serialization
{
    /// <summary>
    /// Json object serializer
    /// </summary>
    public class JsonTextSerializer : ITextSerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        private JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTextSerializer"/> class.
        /// </summary>
        public JsonTextSerializer()
        {
            serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
        }

        /// <summary>
        /// Serializes a supplied object instance to JSON
        /// </summary>
        /// <param name="o">Instance to be serialized</param>
        /// <returns>
        /// Returns string of JSON serialization.
        /// </returns>
        public string Serialize(object o)
        {
            Check.NotNull(() => o);
            string output = JsonConvert.SerializeObject(o, Formatting.Indented, settings);
            return output;
        }

        /// <summary>
        /// Serializes a supplied object instance to JSON
        /// </summary>
        /// <typeparam name="T">Type of the object which is being serialized to JSON</typeparam>
        /// <param name="o">Instance to be serialized</param>
        /// <param name="typeNameHandlingAll">if set to <c>true</c> the types will be specified.</param>
        /// <returns>
        /// Returns string of JSON serialization.
        /// </returns>
        public string Serialize<T>(T o, bool typeNameHandlingAll = true)
        {
            Check.NotNull(() => o);

            if (typeNameHandlingAll)
            {
                var output = JsonConvert.SerializeObject(o, typeof(T), Formatting.Indented, settings);
                return output;
            }
            else
            {
                var output = JsonConvert.SerializeObject(o);
                return output;
            }
        }

        /// <summary>
        /// Deserializes a supplied object
        /// </summary>
        /// <param name="oSerialized">String value from which object is being deserialized</param>
        /// <returns>The deserialized object.</returns>
        public dynamic Deserialize(string oSerialized)
        {
            Check.NotNullOrEmpty(() => oSerialized);
            return JObject.Parse(oSerialized);
        }

        /// <summary>
        /// Deserializes a supplied object
        /// </summary>
        /// <param name="oSerialized">String value from which object is being deserialized</param>
        /// <param name="type">Type of the object which is being serialized</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(string oSerialized, Type type)
        {
            Check.NotNullOrEmpty(() => oSerialized);
            var result = JsonConvert.DeserializeObject(oSerialized, type, this.settings);
            return result;
        }

        /// <summary>
        /// Deserializes the specified o serialized.
        /// </summary>
        /// <typeparam name="T">Type of the object which is being serialized</typeparam>
        /// <param name="oSerialized">The o serialized.</param>
        /// <returns>The deserialized object.</returns>
        public T Deserialize<T>(string oSerialized)
        {
            Check.NotNullOrEmpty(() => oSerialized);
            var result = JsonConvert.DeserializeObject<T>(oSerialized, this.settings);
            return result;
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <param name="oSerialized">The o serialized.</param>
        /// <returns></returns>
        public string GetObjectType(string oSerialized)
        {
            Check.NotNullOrEmpty(() => oSerialized);
            var definition = new { AlgorithmType = "" };
            var deserialized = JsonConvert.DeserializeAnonymousType(oSerialized, definition);

            return deserialized.AlgorithmType;
        }

        /// <summary>
        /// Deserializes a collection.
        /// </summary>
        /// <typeparam name="T">Type of the object which is being serialized</typeparam>
        /// <param name="collectionSerialized">The collection serialized.</param>
        /// <returns>The collection deserialized.</returns>
        public IEnumerable<T> DeserializeCollection<T>(object collectionSerialized) where T : class
        {
            Check.NotNull(() => collectionSerialized);
            return this.Deserialize<IEnumerable<T>>(collectionSerialized.ToString());
        }
    }
}