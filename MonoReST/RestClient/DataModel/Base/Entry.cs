using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Entry is used in the Feed. It is a container for any serializable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "entry", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Entry<T>: Linkable
    {
        /// <summary>
        /// ID of the feed entry
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }
        /// <summary>
        /// Title of the feed entry
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }
        /// <summary>
        /// Updated date of the feed entry
        /// </summary>
        [DataMember(Name = "updated")]
        public string Updated { get; set; }

        /// <summary>
        /// Summary of the feed entry
        /// </summary>
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        private List<Author> _authors = new List<Author>();

        /// <summary>
        /// Authors of the feed entry
        /// </summary>
        [DataMember(Name = "author")]
        public List<Author> Authors 
        { 
            get 
            {
                if (_authors == null)
                {
                    _authors = new List<Author>();
                }
                return _authors;
            }
            set
            {
                _authors = value;
            } 
        }
        /// <summary>
        /// The object contained within the feed entry
        /// </summary>
        [DataMember(Name = "content", IsRequired = false)]
        public T Content { get; set; }

        /// <summary>
        /// The search score
        /// </summary>
        [DataMember(Name = "score")]
        public float Score { get; set; }

        /// <summary>
        /// The terms that match, can be used to highlight in the summary
        /// </summary>
        [DataMember(Name = "terms")]
        public string[] Terms { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Entry{{id: {0}, title: {1}, summary: {2}, updated: {3}, ",
                this.Id,
                this.Title,
                this.Summary,
                this.Updated);
            
            if(this.Authors != null)
            {
                builder.Append("authors<");
            
                foreach (Author author in this.Authors)
                {
                    builder.Append(author.Name).Append(" ");
                }
                builder.Append(">, ");
            }
            
            builder.Append("content: ").Append(this.Content.ToString());
            builder.Append("}");
            return builder.ToString();
        }
    }

    /// <summary>
    /// Used to broker the object to an atom feed content object with src link (not inline object)
    /// </summary>
    [DataContract(Name = "content", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class OutlineAtomContent 
    {
        /// <summary>
        /// The rest link to the object
        /// </summary>
        [DataMember(Name = "src")]
        public string Src { get; set; }
        /// <summary>
        /// The content type of the object (xml/json, etc)
        /// </summary>
        [DataMember(Name = "content-type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Convenience method to override to string to give a simple src/content json representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Content{{src: {0}, content-type: {1}}}",
                this.Src,
                this.ContentType);
            return builder.ToString();
        }
    }
}
