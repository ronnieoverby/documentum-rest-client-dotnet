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
    /// <summary>
    /// entryp point for Persistent object
    /// </summary>
    [DataContract(Name = "persistentobject", Namespace = "http://identifiers.emc.com/vocab/documentum")] 
    public class PersistentObject : Linkable, Executable
    {
        public PersistentObject()
        {

        }

        public PersistentObject(Dictionary<string, object> properties)
        {
            _properties = properties;
        }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "definition")]
        public string Definition { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();
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
        /// Dictionary of Properties 
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

        /// <summary>
        /// Dictionary of changed Properties
        /// </summary>
        private Dictionary<string, object> _changedProperties;
        /// <summary>
        /// Gets the changed properties
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
        /// Whether the object can be updated
        /// </summary>
        /// <returns>Returns Boolean value</returns>
        public bool CanUpdate()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.EDIT.Rel) != null;
        }


        /// <summary>
        /// Get the cabinet resource where this object locates at 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Cabinet GetCabinet(SingleGetOptions options)
        {
            return Client.GetSingleton<Cabinet>(
                this.Links,
                LinkRelations.CABINET.Rel,
                options);
        }

        /// <summary>
        /// Get the parent folder resource of this object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Folder GetParentFolder(SingleGetOptions options)
        {
            return Client.GetSingleton<Folder>(
                this.Links,
                LinkRelations.PARENT.Rel,
                options);
        }

        /// <summary>
        /// Get folder links feed of this object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<FolderLink> GetFolderLinks(FeedGetOptions options)
        {
            return Client.GetFeed<FolderLink>(
                this.Links,
                LinkRelations.PARENT_LINKS.Rel,
                options);
        }

        /// <summary>
        /// Link this object to a new folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public FolderLink LinkTo(Folder newObj, GenericOptions options)
        {
           
            return Client.Post<Folder, FolderLink>(
                this.Links,
                LinkRelations.PARENT_LINKS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Whether the object can be deleted
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.DELETE.Rel) != null;
        }
        /// <summary>
        /// Set an attribute value. This should be extended to a custom model object that 
        /// has dictionary (available attribute) and business rules to avoid allowing 
        /// setting invalidate attributes/values.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public void setAttributeValue(String attributeName, Object attributeValue)
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

        public object getAttributeValue(string attributeName)
        {
            return _properties.ContainsKey(attributeName) ? _properties[attributeName] : null;
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }

        /// <summary>
        /// Get value as string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>String</returns>
        //private string ToStringValue(object value)
        //{
        //    var newv = value;
        //    if (value is Newtonsoft.Json.Linq.JArray)
        //    {
        //        JToken[] tokens  = ((Newtonsoft.Json.Linq.JArray) value).ToArray();
        //        newv = new string[tokens.Length];
        //        for (int k = 0; k < tokens.Length; k++ )
        //        {
        //            ((string[])newv)[k] = tokens[k].ToString();
        //        }
        //    }
        //    if (newv != null && newv.GetType().IsArray)
        //    {
        //        StringBuilder b = new StringBuilder();
        //        b.Append("<");
        //        foreach(object item in (Array) newv)
        //        {
        //            b.Append(item).Append(" ");
        //        }
        //        b.Append(">");
        //        return b.ToString();
        //    }
        //    else{
        //        return newv == null ? "" : newv.ToString();
        //    }
        //}

        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        /// <summary>
        /// Rest controler client 
        /// </summary>
        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

        /// <summary>
        /// Get current persistent object resource
        /// </summary>
        /// <returns></returns>
        public T fetch<T>()
        {
            return Client.Self<T>(this.Links);
        }

        /// <summary>
        /// Paritially update the persistent object resource
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T Save<T>(T obj) where T : PersistentObject
        {
            PersistentObject tp = obj as PersistentObject;
          
            T updated = Client.Post<T>(selfLink(), obj);
            updated.Client = Client;
            return updated;
        }

        /// <summary>
        /// Save  object after properties update
        /// </summary>
        public void Save() 
        {
            
            Dictionary<string, object> tempProp = _properties;
            _properties = ChangedProperties;
            Client.Post(selfLink(), this);
            ChangedProperties = new Dictionary<string, object>();
            _properties = tempProp;
        }

        /// <summary>
        /// Completely update the persistent object resource
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T CompleteUpdate<T>(T obj) where T : PersistentObject
        {
            T updated = Client.Put<T>(selfLink(), obj);
            updated.Client = Client;
            return updated;
        }

        /// <summary>
        /// Delete the persistent object resource
        /// </summary>
        /// <param name="options"></param>
        public void Delete(GenericOptions options)
        {
            Client.Delete(selfLink(), options == null ? null : options.ToQueryList());
        }

        /// <summary>
        /// Get repeating value as object
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object getRepeatingValue(string attribute, int index) {
            object[] values = getRepeatingValuesAsObject(attribute);
            return values == null? null : values[index].ToString();
        }

       /// <summary>
       /// Get repeating string
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
       /// Get repeating value as object
       /// </summary>
       /// <param name="attribute"></param>
       /// <returns></returns>
        public object[] getRepeatingValuesAsObject(string attribute) {
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
        /// Get repeating values as string
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string getRepeatingValuesAsString(string attribute, string seperator) {
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

        /// <summary>
        /// Get a reference object representation of this resource with a href link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetHrefObject<T>() where T : PersistentObject
        {
            T hrefObj = (T)Activator.CreateInstance(typeof(T));
            hrefObj.Href = selfLink();
            hrefObj.SetClient(this.Client);
            return hrefObj;
        }

       /// <summary>
       /// Get a copy of an object
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
        public T GetCopy<T>() where T : PersistentObject
        {
            T hrefObj = (T)Activator.CreateInstance(typeof(T));
            hrefObj.Href = selfLink();
            hrefObj.SetClient(this.Client);
            return hrefObj;
        }

       /// <summary>
       /// Self link to the item 
       /// </summary>
       /// <returns></returns>
        private string selfLink()
        {
            string self = LinkRelations.FindLinkAsString(this.Links, LinkRelations.EDIT.Rel);
            if (self == null)
            {
                self = LinkRelations.FindLinkAsString(this.Links, LinkRelations.SELF.Rel);
            }
            return self;
        }
    }
}
