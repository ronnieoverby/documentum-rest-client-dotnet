using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;

namespace Emc.Documentum.Rest.DataModel
{

    [DataContract(Name = "peristentobject", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class AuditEntry
    {
        public string getEventName()
        {
            return getAttributeValue("event_name").ToString();
        }

        public string getEventSource()
        {
            return getAttributeValue("event_source").ToString();
        }

        public string getUserName()
        {
            return getAttributeValue("user_name").ToString();
        }
        public string getAuditedObjectId()
        {
            return getAttributeValue("audited_obj_id").ToString();
        }
        public string getTimeStamp()
        {
            return getAttributeValue("time_stamp").ToString();
        }
        public string getString1()
        {
            return getAttributeValue("string_1").ToString();
        }
        public string getString2()
        {
            return getAttributeValue("string_2").ToString();
        }
        public string getString3()
        {
            return getAttributeValue("string_3").ToString();
        }
        public string getString4()
        {
            return getAttributeValue("string_4").ToString();
        }
        public string getString5()
        {
            return getAttributeValue("string_5").ToString();
        }

        public string getId1()
        {
            return getAttributeValue("id_1").ToString();
        }
        public string getId2()
        {
            return getAttributeValue("id_2").ToString();
        }
        public string getId3()
        {
            return getAttributeValue("id_3").ToString();
        }
        public string getId4()
        {
            return getAttributeValue("id_4").ToString();
        }
        public string getId5()
        {
            return getAttributeValue("id_5").ToString();
        }
        public string getChronicleId()
        {
            return getAttributeValue("chronicle_id").ToString();
        }

        public string getObjectName()
        {
            return getAttributeValue("object_name").ToString();
        }
        public string getVersionLabel()
        {
            return getAttributeValue("version_label").ToString();
        }
        public string getObjecType()
        {
            return getAttributeValue("object_type").ToString();
        }
        public string getEventDescription()
        {
            return getAttributeValue("event_description").ToString();
        }
        public string getPolicyId()
        {
            return getAttributeValue("policy_id").ToString();
        }
        public string getCurrentState()
        {
            return getAttributeValue("current_state").ToString();
        }
        public string getWorkflowId()
        {
            return getAttributeValue("workflow_id").ToString();
        }
        public string getSessionId()
        {
            return getAttributeValue("session_id").ToString();
        }
        public string getUserId()
        {
            return getAttributeValue("user_id").ToString();
        }
        public string getOwnerName()
        {
            return getAttributeValue("owner_name").ToString();
        }
        public string getAclName()
        {
            return getAttributeValue("acl_name").ToString();
        }
        public string getAclDomain()
        {
            return getAttributeValue("acl_domain").ToString();
        }
        public string getApplicationCode()
        {
            return getAttributeValue("application_code").ToString();
        }
        public string getControllingApp()
        {
            return getAttributeValue("controlling_app").ToString();
        }
        public string getAttributeList()
        {
            return getAttributeValue("attribute_list").ToString();
        }
        public string getAttributeListId()
        {
            return getAttributeValue("attribute_list_id").ToString();
        }
        public string getAuditSignature()
        {
            return getAttributeValue("audit_signature").ToString();
        }
        public string getAuditVerion()
        {
            return getAttributeValue("audit_version").ToString();
        }
        public string getHostName()
        {
            return getAttributeValue("host_name").ToString();
        }
        public string getTimeStanpUTC()
        {
            return getAttributeValue("time_stamp_utc").ToString();
        }
        public string getAuditedObjectClass()
        {
            return getAttributeValue("i_audited_obj_class").ToString();
        }
        public string getRegistryId()
        {
            return getAttributeValue("registry_id").ToString();
        }
        public string isArchived()
        {
            return getAttributeValue("i_is_archived").ToString();
        }
        public string getAuditedVStamp()
        {
            return getAttributeValue("audited_obj_vstamp").ToString();
        }
        public string getAttributeListOld()
        {
            return getAttributeValue("attribute_list_old").ToString();
        }
        public string getAttributeListAspectId()
        {
            return getAttributeValue("attribute_list_aspect_id").ToString();
        }

        public string[] getFields()
        {
            return fields;
        }
        private string[] fields = {
            "event_name","event_source","r_gen_source","user_name","audited_obj_id","time_stamp",
            "string_1","string_2","string_3","string_4","string_5","id_1","id_2","id_3","id_4","id_5",
            "chronicle_id","object_name","version_label","object_type","event_description","policy_id",
            "current_state","workflow_id","session_id","user_id","owner_name","acl_name","acl_domain",
            "application_code","controlling_app","attribute_list","attribute_list_id","audit_signature",
            "audit_version","host_name","time_stamp_utc","i_audited_obj_class","registry_id","i_is_archived",
            "audited_obj_vstamp","attribute_list_old","attribute_list_aspect_id","r_object_sequence"
        };

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "definition")]
        public string Definition { get; set; }

        /// <summary>
        /// Dictionary value for Properteis
        /// </summary>
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        
        /// <summary>
        /// Datamember
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
        /// Dictionary for properties
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
            /*set
            {
                _properties = value;
            }*/
        }

        private Dictionary<string, object> _changedProperties;
        /// <summary>
        /// Set an attribute value. This should be extended to a custom model object that 
        /// has dictionary (available attribute) and business rules to avoid allowing 
        /// setting invalidate attributes/values.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public void setAttributeValue(String attributeName, Object attributeValue)
        {
            if (_changedProperties == null) _changedProperties = new Dictionary<string, object>();
            if (_properties.ContainsKey(attributeName))
            {
                _properties[attributeName] = attributeValue;
            }
            else
            {
                _properties.Add(attributeName, attributeValue);
            }

            if (_changedProperties.ContainsKey(attributeName))
            {
                _changedProperties[attributeName] = attributeValue;
            }
            else
            {
                _changedProperties.Add(attributeName, attributeValue);
            }
        }

        /// <summary>
        /// Get an attribute value
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public object getAttributeValue(string attributeName)
        {
            return _properties.ContainsKey(attributeName) ? _properties[attributeName] : null;
        }

        /// <summary>
        /// Gets value as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "{0}{{name: {1}, type: {2},  definition: {3}, properties[",
                this.GetType().Name,
                this.Name,
                this.Type,
                this.Definition);
            bool first = true;
            foreach (var property in this.Properties)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(", ");
                }
                builder.AppendFormat("{0}: {1}",
                    property.Key,
                    ToStringValue(property.Value));
            }
            builder.Append("]}}");
            return builder.ToString();
        }

        /// <summary>
        /// Gets a string value from the object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ToStringValue(object value)
        {
            var newv = value;
            if (value is Newtonsoft.Json.Linq.JArray)
            {
                JToken[] tokens = ((Newtonsoft.Json.Linq.JArray)value).ToArray();
                newv = new string[tokens.Length];
                for (int k = 0; k < tokens.Length; k++)
                {
                    ((string[])newv)[k] = tokens[k].ToString();
                }
            }
            if (newv != null && newv.GetType().IsArray)
            {
                StringBuilder b = new StringBuilder();
                b.Append("<");
                foreach (object item in (Array)newv)
                {
                    b.Append(item).Append(" ");
                }
                b.Append(">");
                return b.ToString();
            }
            else
            {
                return newv == null ? "" : newv.ToString();
            }
        }

        /// <summary>
        /// Gets a repeating value as a string
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object getRepeatingValue(string attribute, int index)
        {
            object[] values = getRepeatingValuesAsObject(attribute);
            return values == null ? null : values[index].ToString();
        }

        /// <summary>
        /// Gets a repeating value as a string
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getRepeatingString(string attribute, int index)
        {
            object ret = getRepeatingValue(attribute, index);
            return ret == null ? "" : ret.ToString();
        }

        /// <summary>
        /// Gets a repeating value as a string
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public object[] getRepeatingValuesAsObject(string attribute)
        {
            object[] values = null;
            var attr = getAttributeValue(attribute);
            if (attr == null) return values;
            if (attr is JArray)
            {
                values = ((JArray)getAttributeValue(attribute)).ToObject<Object[]>();
            }
            else
            {
                values = ((Object[])getAttributeValue(attribute));
            }
            return values;
        }

       /// <summary>
       /// Gets a repeating value as a string
       /// </summary>
       /// <param name="attribute"></param>
       /// <param name="seperator"></param>
       /// <returns></returns>
        public string getRepeatingValuesAsString(string attribute, string seperator)
        {
            StringBuilder strValues = new StringBuilder();
            object[] values = getRepeatingValuesAsObject(attribute);
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
                Console.WriteLine("Unable to get repeating values for " + getAttributeValue("r_object_id").ToString() + " attribute:" + attribute);
                Console.WriteLine(e.StackTrace);
            }
            return strValues.ToString();
        }
    }

}
