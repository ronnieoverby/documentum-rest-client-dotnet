using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// A base model for any persistent object resource
    /// </summary>
    [DataContract(Name = "persistentobject", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class PersistentObject : ExecLinkable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PersistentObject()
        {

        }

        /// <summary>
        /// Constructor with input properties
        /// </summary>
        /// <param name="properties"></param>
        public PersistentObject(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// Href link on behalf of the object, used in copy, etc.
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Resource name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Persistent object type name
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Persistent object type resource URI
        /// </summary>
        [DataMember(Name = "definition")]
        public string Definition { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>
        /// Properties of the persistent object
        /// </summary>
        [DataMember(Name = "properties")]
        private Dictionary<string, object> PropertiesToBinding
        {
            get
            {
                return Properties == null || Properties.Count == 0 ? null : Properties;
            }
            set
            {
                _properties = value;
            }
        }
        /// <summary>
        /// Properties of the persistent object in dictionary
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<string, object>();
                }
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        private Dictionary<string, object> _changedProperties;

        /// <summary>
        /// Changed properties of the persistent object
        /// </summary>
        public Dictionary<string,object> ChangedProperties
        {
            get
            {
                if (_changedProperties == null)
                {
                    _changedProperties = new Dictionary<string, object>();
                }
                return _changedProperties;
            }

            set
            {
                _changedProperties = value;
            }
        }

        /// <summary>
        /// Set an attribute value. This should be extended to a custom model object that 
        /// has dictionary (available attribute) and business rules to avoid allowing 
        /// setting invalidate attributes/values.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public void SetPropertyValue(String attributeName, Object attributeValue)
        {
            if (_properties.ContainsKey(attributeName))
            {
                _properties[attributeName] = attributeValue;
            }
            else
            {
                _properties.Add(attributeName, attributeValue);
            }

            if (ChangedProperties.ContainsKey(attributeName))
            {
                ChangedProperties[attributeName] = attributeValue;
            }
            else
            {
                ChangedProperties.Add(attributeName, attributeValue);
            }
        }

        /// <summary>
        /// Get property value by name
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string attributeName)
        {
            return _properties.ContainsKey(attributeName) ? _properties[attributeName] : null;
        }

        /// <summary>
        /// Get property value as string by name
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetPropertyString(string attributeName)
        {
            object value = _properties.ContainsKey(attributeName) ? _properties[attributeName] : null;
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// Get property repeating value as object on the specified index
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object GetRepeatingValue(string attribute, int index)
        {
            object[] values = GetRepeatingValuesAsObject(attribute);
            return values == null ? null : values[index].ToString();
        }

        /// <summary>
        /// Get property repeating value as string on the specified index
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetRepeatingString(string attribute, int index)
        {
            object ret = GetRepeatingValue(attribute, index);
            return ret == null ? "" : ret.ToString();
        }


        /// <summary>
        /// Get property repeating value as object array
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public object[] GetRepeatingValuesAsObject(string attribute)
        {
            object[] values = null;
            var attr = GetPropertyValue(attribute);
            if (attr == null) return values;
            if (attr is JArray)
            {
                values = ((JArray)GetPropertyValue(attribute)).ToObject<Object[]>();
            }
            else
            {
                values = ((Object[])GetPropertyValue(attribute));
            }
            return values;
        }

        /// <summary>
        /// Get repeating values as string with the specified string separator
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string GetRepeatingValuesAsString(string attribute, string seperator)
        {
            StringBuilder strValues = new StringBuilder();
            object[] values = GetRepeatingValuesAsObject(attribute);
            try
            {
                bool first = true;
                foreach (object value in values)
                {
                    if (first)
                    {
                        first = false;
                        strValues.Append(value.ToString());
                    }
                    else
                    {
                        strValues.Append(seperator + value.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get repeating values for " + GetPropertyValue("r_object_id").ToString() + " attribute:" + attribute);
                Console.WriteLine(e.StackTrace);
            }
            return strValues.ToString();
        }
    }
}
