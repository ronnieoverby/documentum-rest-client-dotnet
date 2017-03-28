//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Threading.Tasks;

//namespace Emc.Documentum.Rest.Net
//{
//    /// <summary>
//    /// .NET built-in DataContract JSON serializer
//    /// </summary>
//    public class DefaultDataContractJsonSerializer : AbstractJsonSerializer
//    {
//        //private MediaTypeWithQualityHeaderValue JSON_GENERIC_MEDIA_TYPE;
//        private MediaTypeWithQualityHeaderValue JSON_VND_MEDIA_TYPE;
//        private DataContractJsonSerializerSettings JSON_SER_SETTINGS;

//        /// <summary>
//        /// DefaultDataContractJsonSerializer with default setting
//        /// </summary>
//        public DefaultDataContractJsonSerializer()
//        {
//          //  JSON_GENERIC_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/*+json");
//            JSON_VND_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/vnd.emc.documentum+json");
//            JSON_SER_SETTINGS = new DataContractJsonSerializerSettings();
//            JSON_SER_SETTINGS.UseSimpleDictionaryFormat = true;
//            JSON_SER_SETTINGS.DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
//        }

//        /// <summary>
//        /// Read resource data object from JSON message stream
//        /// </summary>
//        /// <typeparam name="T">Resource data model type</typeparam>
//        /// <param name="input">JSON input stream</param>
//        /// <returns>Resource data model object</returns>
//        public override T ReadObject<T>(Stream input)
//        {
//            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), JSON_SER_SETTINGS);
//            T obj = (T)ser.ReadObject(input);
//            return obj;
//        }

//        /// <summary>
//        /// Write resource data object to JSON message stream
//        /// </summary>
//        /// <typeparam name="T">Resource data model type</typeparam>
//        /// <param name="output">JSON output stream</param>
//        /// <param name="obj">Resource data model object</param>
//        public override void WriteObject<T>(Stream output, T obj)
//        {
//            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), JSON_SER_SETTINGS);
//            ser.WriteObject(output, obj);
//        }

//        /// <summary>
//        /// Serialize resource data object to JSON string
//        /// </summary>
//        /// <typeparam name="T">Resource data model type</typeparam>
//        /// <param name="obj">Resource data model object</param>
//        /// <returns>String JSON message</returns>
//        public String Serialize<T>(T obj)
//        {
//            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), JSON_SER_SETTINGS);
//            String json = "";
//            using (MemoryStream ms = new MemoryStream()) 
//            {
                
//                ser.WriteObject(ms, obj);
//                json = Encoding.UTF8.GetString(ms.ToArray());
                
//                return json;
//            }
            
//        }
//    }
//}
