using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace Emc.Documentum.Rest.Http.Utility
{
    /// <summary>
    /// Utility class for working with URI links
    /// </summary>
    public class UriUtil

    {
        /// <summary>
        /// Build a URI from a base path and query
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string BuildUri(string baseUri, List<KeyValuePair<string, object>> query)
        {
            
            UriBuilder builder = new UriBuilder(baseUri);

            // So stupid, you cannot do String query = builder.Query, then builder.Query = query;
            // Since it could be a "fix" in the future from .NET, we shoudl check if first characer is ?
            String existingQuery = builder.Query.StartsWith("?") ? builder.Query.Substring(1) : builder.Query;
            StringBuilder baseQueryString = (existingQuery == null || existingQuery.Trim().Equals(""))
                ? new StringBuilder()
                : new StringBuilder(existingQuery);

            if (query != null && query.Count != 0)
            {
                foreach (KeyValuePair<string, object> pair in query) 
                {
                    if (pair.Value != null)
                    {
                        if (baseQueryString.ToString().Equals("")) {
                            baseQueryString.Append(pair.Key).Append("=").Append(pair.Value);
                        } else
                        {
                            baseQueryString.Append("&").Append(pair.Key).Append("=").Append(pair.Value);
                        }
                    }                  
                }
            }
            builder.Query = baseQueryString.ToString();
            return builder.ToString();
        }

        /// <summary>
        /// Get a parameter value from a given URI
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string getParameterFromUri(Uri uri, string parameterName)
        {
            try
            {
				var baseQuery = HttpUtility.ParseQueryString(uri.ToString());
                if (baseQuery != null)
                {
                    return (String)baseQuery[parameterName];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GETPARAMETERFROMURI EXCEPTION: " + e.StackTrace);
            }
            return null;
        }
    }
}
