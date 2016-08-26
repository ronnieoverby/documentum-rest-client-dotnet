using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http.Headers;
using Emc.Documentum.Rest.Http.Utility;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Type resource model
    /// </summary>
    [DataContract(Name = "type", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class DmType : ExecLinkable
    {
        /// <summary>
        /// Type name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Type label
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Type category
        /// </summary>
        [DataMember(Name = "category")]
        public string Category { get; set; }

        /// <summary>
        /// Parent type
        /// </summary>
        [DataMember(Name = "parent")]
        public string Parent { get; set; }

        /// <summary>
        /// Shared type for lightweght type
        /// </summary>
        [DataMember(Name = "shared-parent")]
        public string SharedParent { get; set; }

        /// <summary>
        /// Type properties
        /// </summary>
        [DataMember(Name = "properties")]
        public List<TypeProperty> TypeProperties { get; set; }
    }

    /// <summary>
    /// Type property model
    /// </summary>
    [DataContract(Name = "property", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class TypeProperty 
    {
        /// <summary>
        /// Type property name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Type property repeating or not
        /// </summary>
        [DataMember(Name = "repeating")]
        public Boolean IsRepeating { get; set; }

        /// <summary>
        /// Type property value type
        /// </summary>
        [DataMember(Name = "type")]
        public string ValueType { get; set; }

        /// <summary>
        /// Type property value length
        /// </summary>
        [DataMember(Name = "length")]
        public Int32 ValueLength { get; set; }

        /// <summary>
        /// Type property label
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// Type property hidden or not
        /// </summary>
        [DataMember(Name = "hidden")]
        public Boolean IsHidden { get; set; }

        /// <summary>
        /// Type property required or not
        /// </summary>
        [DataMember(Name = "required")]
        public Boolean IsRequired { get; set; }

        /// <summary>
        /// Type property nullable or not
        /// </summary>
        [DataMember(Name = "notnull")]
        public Boolean IsNotNullable { get; set; }

        /// <summary>
        /// Type property readonly or not
        /// </summary>
        [DataMember(Name = "readonly")]
        public Boolean IsReadonly { get; set; }

        /// <summary>
        /// Type property searchable or not
        /// </summary>
        [DataMember(Name = "searchable")]
        public Boolean IsSearchable { get; set; }

        /// <summary>
        /// Type property default value from literal
        /// </summary>
        [DataMember(Name = "default-literal")]
        public string DefaultLiteral { get; set; }

        /// <summary>
        /// Type property default value from expression
        /// </summary>
        [DataMember(Name = "default-expression")]
        public string DefaultExpression { get; set; }

        /// <summary>
        /// Type property default values
        /// </summary>
        [DataMember(Name = "defaults")]
        public List<TypeDefaultValue> DefaultValues { get; set; }
    }

    /// <summary>
    /// Type property default value model
    /// </summary>
    [DataContract(Name = "default-value", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class TypeDefaultValue
    {
        /// <summary>
        /// Type property default value from literal
        /// </summary>
        [DataMember(Name = "default-literal")]
        public string DefaultLiteral { get; set; }

        /// <summary>
        /// Type property default value from expression
        /// </summary>
        [DataMember(Name = "default-expression")]
        public string DefaultExpression { get; set; }
    }
}
