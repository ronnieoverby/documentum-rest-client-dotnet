using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// Feed related query parameters
    /// </summary>
    public class FeedGetOptions : SingleGetOptions
    {
        /// <summary>
        /// Query parameter 'inline'
        /// </summary>
        public static readonly string PARAM_INLINE = "inline";
        /// <summary>
        /// Query parameter 'recursive'
        /// </summary>
        public static readonly String PARAM_RECURSIVE = "recursive";
        /// <summary>
        /// Query parameter 'sort'
        /// </summary>
        public static readonly String PARAM_SORT = "sort";
        /// <summary>
        /// Query parameter 'filter'
        /// </summary>
        public static readonly String PARAM_FILTER = "filter";
        /// <summary>
        /// Query parameter 'page'
        /// </summary>
        public static readonly String PARAM_PAGE = "page";
        /// <summary>
        /// Query parameter 'items-per-page'
        /// </summary>
        public static readonly String PARAM_ITEMS_PER_PAGE = "items-per-page";
        /// <summary>
        /// Query parameter 'include-total'
        /// </summary>
        public static readonly String PARAM_INCLUDE_TOTAL = "include-total";
        /// <summary>
        /// Query parameter 'raw'
        /// </summary>
        public static readonly String PARAM_RAW = "raw";

        /// <summary>
        /// Default constructor
        /// </summary>
        public FeedGetOptions() : base() {
            Inline = true;
            PageNumber = 1;
            IncludeTotal = false;
        }

        /// <summary>
        /// Specifies whether to inline objects in a feed.
        /// </summary>
        public Boolean Inline
        {
            get
            {
                return (Boolean)pa[PARAM_INLINE];
            }
            set
            {
                if (!pa.ContainsKey(PARAM_INLINE))
                {
                    pa.Add(PARAM_INLINE, value);
                }
                else { pa[PARAM_INLINE] = value; }
            }
        }

        /// <summary>
        /// Specifies the object properties to retrieve. This parameter 
        /// works only when inline is set to true so if set to true
        /// this method will also set inline to true;
        /// </summary>
        public override String View
        {
            get
            {
                return base.View;
            }
            set
            {
                if (!String.IsNullOrEmpty(View))
                {
                    Inline = true;
                }
                base.View = value;
            }
        }

        /// <summary>
        /// Determines whether or not to return link relations in the object 
        /// representation This parameter works only when inline is set to true.
        /// true - return link relations
        /// false - do not return link relations
        /// Default: true
        /// </summary>
        public override Boolean Links
        {
            get
            {
                return base.Links;
            }
            set
            {
                if (value) Inline = true;
                base.Links = value;
            }
        }

        /// <summary>
        /// Specifies the sort spec, e.g. "object_name asc"
        /// </summary>
        public String Sort
        {
            get { return pa[PARAM_SORT].ToString();}
            set {
                if (!pa.ContainsKey(PARAM_SORT)) {
                    pa.Add(PARAM_SORT, value);
                } else { pa[PARAM_SORT] = value; }
            }
        }

        /// <summary>
        /// Specifies the filter, e.g. "starts-with(object_name, 'Readme')"
        /// </summary>
        public String Filter
        {
            get { return pa[PARAM_FILTER].ToString(); }
            set
            {
                if (!pa.ContainsKey(PARAM_FILTER))
                {
                    pa.Add(PARAM_FILTER, value);
                }
                else { pa[PARAM_FILTER] = value; }
            }
        }

        /// <summary>
        /// Specifies the page number of the page to return. If you set 
        /// items-per-page to 200, and page to 2, the operation returns 
        /// items 201 to 400.
        /// Default: 1
        /// </summary>
        public Int32 PageNumber 
        {
            get { return (Int32)pa[PARAM_PAGE];}
            set
            {
                if (!pa.ContainsKey(PARAM_PAGE))
                {
                    pa.Add(PARAM_PAGE, value);
                }
                else { pa[PARAM_PAGE] = value; }
            } 
        }

        /// <summary>
        /// Specifies the number of items to be rendered on a page
        /// default: 100
        /// </summary>
        public Int32 ItemsPerPage
        {
            get { return (Int32)pa[PARAM_ITEMS_PER_PAGE]; }
            set
            {
                if (!pa.ContainsKey(PARAM_ITEMS_PER_PAGE))
                {
                    pa.Add(PARAM_ITEMS_PER_PAGE, value);
                }
                else { pa[PARAM_ITEMS_PER_PAGE] = value; }
            }
        }

        /// <summary>
        /// Specifies whether or not to return the total number of objects. 
        /// For paged feeds, objects in all pages are counted.
        /// true - return the total number of objects
        /// false - do not return the total number of objects
        /// Default: false
        /// </summary>
        public Boolean IncludeTotal
        {
            get { return (Boolean)pa[PARAM_INCLUDE_TOTAL]; }
            set
            {
                if (!pa.ContainsKey(PARAM_INCLUDE_TOTAL))
                {
                    pa.Add(PARAM_INCLUDE_TOTAL, value);
                }
                else { pa[PARAM_INCLUDE_TOTAL] = value; }
            }
        }

        /// <summary>
        ///  Used on DQL query resource only.
        ///  Indicates whether to return the DQL results as raw property bag or with links generated.
        ///  When this parameter is set to false, the REST server generates link relations and/or
        ///  including thumbnail links if possible.
        /// </summary>
        public Boolean Raw
        {
            get { return (Boolean)pa[PARAM_RAW]; }
            set
            {
                if (!pa.ContainsKey(PARAM_RAW))
                {
                    pa.Add(PARAM_RAW, value);
                }
                else { pa[PARAM_RAW] = value; }
            }
        }


        /// <summary>
        /// Specifies whether or not to return all indirect children recursively when a 
        /// request tries to get the children of an object.
        /// true -  return all indirect children recursively when a request tries to get 
        ///         the children of an object.
        /// false - Only return direct children when a request tries to get the children 
        ///         of an object.
        /// Default: false
        /// </summary>
        public Boolean Recursive
        {
            get { return (Boolean)pa[PARAM_RECURSIVE]; }
            set
            {
                if (!pa.ContainsKey(PARAM_RECURSIVE))
                {
                    pa.Add(PARAM_RECURSIVE, value);
                }
                else { pa[PARAM_RECURSIVE] = value; }
            }
        }
    }
}
