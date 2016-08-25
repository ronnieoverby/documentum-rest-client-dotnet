using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Net
{

    /// <summary>
    /// Options specific to search resource
    /// </summary>
    public class SearchOptions : FeedGetOptions
    {
        /// <summary>
        /// Query parameter 'q'
        /// </summary>
        public static readonly String PARAM_SEARCH_QUERY = "q";
        /// <summary>
        /// Query parameter 'locations'
        /// </summary>
        public static readonly String PARAM_LOCATIONS = "locations";
        /// <summary>
        /// Query parameter 'collections' 
        /// </summary>
        public static readonly String PARAM_COLLECTIONS = "collections";
        /// <summary>
        /// Query parameter 'facet'
        /// </summary>
        public static readonly String PARAM_FACET = "facet";
        /// <summary>
        /// Query parameter 'facet-value-constraints'
        /// </summary>
        public static readonly String PARAM_FACET_VALUE_CONTRAINTS = "facet-value-constraints";
        /// <summary>
        /// Query parameter 'timezone'
        /// </summary>
        public static readonly String PARAM_TIMEZONE = "timezone";
        /// <summary>
        /// Query parameter 'object-type'
        /// </summary>
        public static readonly String PARAM_OBJECT_TYPE = "object-type";

        /// <summary>
        /// Specifies full text search criterion when sending a GET request to the 
        /// Search resource or certain collection resources
        /// 
        /// Default: None
        /// </summary>
        public String SearchQuery
        {
            get
            {
                return pa[PARAM_SEARCH_QUERY].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_SEARCH_QUERY))
                {
                    pa.Add(PARAM_SEARCH_QUERY, value);
                }
                else { pa[PARAM_SEARCH_QUERY] = value; }
            }
        }

        /// <summary>
        /// Specifies a list of folder paths separated by comma (,) to which the 
        /// query is scoped.
        /// Default: None
        /// </summary>
        public String Locations
        {
            get
            {
                return pa[PARAM_LOCATIONS].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_LOCATIONS))
                {
                    pa.Add(PARAM_LOCATIONS, value);
                }
                else { pa[PARAM_LOCATIONS] = value; }
            }
        }

        /// <summary>
        /// Specifies a list of collections separated by comma (,). to which the 
        /// query is scoped. The collections must exist in xPlore.
        /// Default: None
        /// </summary>
        public String Collections
        {
            get
            {
                return pa[PARAM_COLLECTIONS].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_COLLECTIONS))
                {
                    pa.Add(PARAM_COLLECTIONS, value);
                }
                else { pa[PARAM_COLLECTIONS] = value; }
            }
        }

        /// <summary>
        /// Specifies the property to be used as facet
        /// Default: None
        /// </summary>
        public String Facet
        {
            get
            {
                return pa[PARAM_FACET].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_FACET))
                {
                    pa.Add(PARAM_FACET, value);
                }
                else { pa[PARAM_FACET] = value; }
            }
        }

        /// <summary>
        /// Specifies a property constraint expression. For example, when facet is set 
        /// to r_object_type and facet-value -constraints is set to dm_object, only 
        /// dm_object instances are returned. You can use boolean operators + for AND 
        /// and | for OR in a property constraint expression to define more complex 
        /// constraints. For example,the expression dm_object|dm _folder returns both 
        /// dm_object and dm_folder instances.
        /// Default: None
        /// </summary>
        public String FacetValueConstraints
        {
            get
            {
                return pa[PARAM_FACET_VALUE_CONTRAINTS].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_FACET_VALUE_CONTRAINTS))
                {
                    pa.Add(PARAM_FACET_VALUE_CONTRAINTS, value);
                }
                else { pa[PARAM_FACET_VALUE_CONTRAINTS] = value; }
            }
        }

        /// <summary>
        /// Indicates the time zone used to compute date facets.Valid time zones include 
        /// (case-insensitive):
        ///     Abbreviation, such as GMT and UTC 
        ///     Full names, such as America/Los _Angeles 
        ///     Custom time zones, such as GMT-8:00
        /// Default: GMT
        /// </summary>
        public String Timezone
        {
            get
            {
                return pa[PARAM_TIMEZONE].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_TIMEZONE))
                {
                    pa.Add(PARAM_TIMEZONE, value);
                }
                else { pa[PARAM_TIMEZONE] = value; }
            }
        }

        /// <summary>
        /// Specifies an object type. Only instances of the specified type 
        /// or its sub types are returned in the search result
        /// Default: None
        /// </summary>
        public String ObjectType
        {
            get
            {
                return pa[PARAM_OBJECT_TYPE].ToString();
            }
            set
            {
                if (!pa.ContainsKey(PARAM_OBJECT_TYPE))
                {
                    pa.Add(PARAM_OBJECT_TYPE, value);
                }
                else { pa[PARAM_OBJECT_TYPE] = value; }
            }
        }
    }
}
