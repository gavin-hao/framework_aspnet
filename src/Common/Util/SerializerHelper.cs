using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Dongbo.Common.Util
{
    public class SerializeHelper
    {
        #region Xml Serialize

        public static string SerializeIt(object ToBeSerialized)
        {
            XmlSerializer serializer = new XmlSerializer(ToBeSerialized.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, ToBeSerialized);
            byte[] storeit = stream.ToArray();
            string result = null;
            result = System.Text.Encoding.UTF8.GetString(storeit);
            stream.Close();
            stream.Dispose();
            return result;
        }

        public static T DeserializeIt<T>(string ToBeDeserialized, Type t)
        {

            XmlSerializer serializer = new XmlSerializer(t);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ToBeDeserialized);
            MemoryStream stream = new MemoryStream(buffer);
            return (T)serializer.Deserialize(stream);
        }

        public static T DeserializeXml<T>(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(buffer);

            return (T)serializer.Deserialize(stream);
        }

        #endregion

        #region Json Serialize

        public static string SerializeJson(object obj)
        {
            
            
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);

        }

        #endregion
    }


    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public SerializableDictionary()
            : base()
        {
        }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }
        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }
        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 从对象的XML表示形式生成该对象
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item"); 

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadEndElement();

                this.Add(key, value);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// 将对象转换为其XML表示形式
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }

}
