using Emc.Documentum.Rest.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Http.Utility
{
    /// <summary>
    /// Atom utility
    /// </summary>
    public class AtomUtil
    {
        /// <summary>
        /// atom id
        /// </summary>
        public static readonly string ID = "id";
        /// <summary>
        /// atom title
        /// </summary>
        public static readonly string TITLE = "title";
        /// <summary>
        /// atom summary
        /// </summary>
        public static readonly string SUMMARY = "summary";
        /// <summary>
        /// atom author
        /// </summary>
        public static readonly string AUTHOR = "author";
        /// <summary>
        /// atom updated
        /// </summary>
        public static readonly string UPDATED = "updated";
        /// <summary>
        /// atom published
        /// </summary>
        public static readonly string PUBLISHED = "published";
        /// <summary>
        /// atom entries
        /// </summary>
        public static readonly string ENTRIES = "entries";
        /// <summary>
        /// atom links
        /// </summary>
        public static readonly string LINKS = "links";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feed"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string FindEntryHref<T>(Feed<T> feed, string title)
        {
            string href = null;
            List<Entry<T>> entries = feed.Entries;
            if (entries != null)
            {
                foreach (Entry<T> entry in entries)
                {
                    if (title.Equals(entry.Title) && entry.Content is OutlineAtomContent)
                    {
                        OutlineAtomContent ct = entry.Content as OutlineAtomContent;
                        href = ct.Src;
                        if (href == null)
                        {
                            href = LinkRelations.FindLinkAsString(entry.Links, LinkRelations.SELF.Rel);
                        }
                        break;
                    }
                }
            }
            return href;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feed"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static T FindInlineEntry<T>(Feed<T> feed, string title)
        {
            T inlineContent = default (T);
            List<Entry<T>> entries = feed.Entries;
            if (entries != null)
            {
                foreach (Entry<T> entry in entries)
                {
                    if (title.Equals(entry.Title))
                    {
                        inlineContent = entry.Content;
                        break;
                    }
                }
            }
            return inlineContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feed"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static T FindInlineEntryBySummary<T>(Feed<T> feed, string title)
        {
            T inlineContent = default(T);
            List<Entry<T>> entries = feed.Entries;
            if (entries != null)
            {
                foreach (Entry<T> entry in entries)
                {
                    if (title.Equals(entry.Summary))
                    {
                        inlineContent = entry.Content;
                        break;
                    }
                }
            }
            return inlineContent;
        }
    }
}
