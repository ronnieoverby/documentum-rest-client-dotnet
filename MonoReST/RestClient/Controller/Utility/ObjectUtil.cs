using Emc.Documentum.Rest.controller.Utility;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Emc.Documentum.Rest.Http.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectUtil
    {
        /// <summary>
        /// Create a new random folder
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <param name="type"></param>
        /// <returns>Returns Folder</returns>
        public static Folder NewRandomFolder(string namePrefix, string type)
        {
            Folder obj = new Folder();
            obj.Name = "folder";
            obj.Type = type;
            obj.SetPropertyValue("object_name", namePrefix + System.Guid.NewGuid().ToString().Substring(0, 8));
            obj.SetPropertyValue("r_object_type", type);
            return obj;
        }

        /// <summary>
        /// Create a new random File
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Document NewRandomDocument(string namePrefix, string type)
        {
            Document obj = new Document();
            obj.Name = "document";
            obj.Type = type;
            obj.SetPropertyValue("object_name", NewRandomDocumentName(namePrefix));
            obj.SetPropertyValue("r_object_type", type);
            return obj;
        }

        /// <summary>
        /// Get a random document name with the given prefix
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <returns></returns>
        public static string NewRandomDocumentName(string namePrefix)
        {
            return namePrefix + System.Guid.NewGuid().ToString().Substring(0, 8);
        }


        /// <summary>
        /// Given a path /Cabinet/FolderParent/Child will split off the parent
        /// path /Cabinet/FolderParent as the first item in the list with the
        /// second item in the list being the folder name. Handle for Getting
        /// Folder by Qualification where: folder('{path}') and object_name =
        /// {object_name}
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns List object</returns>
        public static List<String>getPathAndFolderNameFromPath(string path) {
            List<String> ret = new List<String>();
            string[] fragments = path.Split( new string[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            int last = fragments.Length-1;
            StringBuilder parentPath = new StringBuilder();
            for (int i = 0; i < last; i++)
            {
                parentPath.Append("/"+fragments[i]);
            }
            // Add the parent path as the first item in the list
            ret.Add(parentPath.ToString());
            // The folder name is the second item in the list
            ret.Add(fragments[last]);
            return ret;
        }

        /// <summary>
        /// TODO: Really dislike this, needs to be stored in a global, read-only area to avoid
        /// creating this in memory multiple times. TODO: Move this to a configuration file and
        /// to a global load once threadlocal or similar.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,string> getFormatMappings()
        {
            // Using the dictionary allows for easier bi-directional lookup using linq queries
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("afp","AFP"); map.Add("ai","illustrator"); map.Add("aifc","aiff-c"); map.Add("aiff","aiff"); 
            map.Add("asf", "asf"); map.Add("asm", "ptcasm"); map.Add("asp", "asp");  map.Add("asx", "asx"); 
            map.Add("atd","atd"); map.Add("att","att"); map.Add("au","audio"); map.Add("avi","avi"); 
            map.Add("bas","vbbas"); map.Add("bin","bin"); map.Add("bk","mbook");
            map.Add("bmp","bmp"); map.Add("cfm","cfm"); map.Add("cfml","cfml"); map.Add("cgi","cgi");
            map.Add("cgm","cgm"); map.Add("chm","htmlhelp"); map.Add("class","class"); map.Add("cr2","cr2");
            map.Add("crw","crw"); map.Add("css","css"); map.Add("ctm","ctm"); map.Add("cvs","canvas");
            map.Add("daf","daf"); map.Add("dar","DAR"); map.Add("dcm","dicom"); map.Add("dec","dec");
            map.Add("dgn","ustn");map.Add("dib","dib"); map.Add("dita","dita"); map.Add("ditamap","ditamap");
            map.Add("dll","win32shrlib"); map.Add("dng","dng"); map.Add("doc","msw"); map.Add("docm","msw12me");
            map.Add("docx","msw12"); map.Add("dot","msw8template"); map.Add("dotm","msw12metemplate"); 
            map.Add("dotx","msw12template"); map.Add("drw","ptcdrw"); map.Add("dtd","dtd");
            map.Add("dv","dv"); map.Add("dwf","dwf"); map.Add("dwg","acad"); map.Add("dwt","mmdwt");
            map.Add("dxf","dxf"); map.Add("eap","eap"); map.Add("elm","elm"); map.Add("emcmf","emcmf");
            map.Add("emf","emf");map.Add("eml","eml"); map.Add("emz","emz"); map.Add("ent","ent");
            map.Add("eps","eps"); map.Add("f4v","f4v"); map.Add("fdf","fdf"); map.Add("fla","flash");
            map.Add("flv","flv"); map.Add("fm","filemakerpro");map.Add("fos","fos"); map.Add("fp3","filemakerpro3");
            map.Add("fp4","filemakerpro4"); map.Add("fpx","fpx"); map.Add("frm","vbfrm"); map.Add("gif","gif");
            map.Add("gxf","gxf"); map.Add("hdml","hdml"); map.Add("hhf","hhf");  map.Add("htm","html");
            map.Add("html","html"); map.Add("idl","idl"); map.Add("incd","incopy3"); map.Add("incx","incopy5");
            map.Add("indd","indesign7");map.Add("jar","jar"); map.Add("java","java"); map.Add("jhtml","jhtml");
            map.Add("jnt","jnt"); map.Add("jp2","jpeg2000"); map.Add("jpg","jpeg"); map.Add("jpeg","jpeg");
            map.Add("js","js"); map.Add("jsp","jsp"); map.Add("lbl","nicelabelv5"); map.Add("lxf","lxf");
            map.Add("mak","vbmak");map.Add("map","imagemap"); map.Add("mcr","mcr"); map.Add("mdb","ms_access");
            map.Add("mht", "mht"); map.Add("mhtm", "mht"); map.Add("mhtml", "mht");  map.Add("mif", "mif"); map.Add("mil", "cals1"); 
            map.Add("mmap", "mmap"); map.Add("mmas","mmas"); map.Add("mmat","mmat"); map.Add("mod","mod"); 
            map.Add("mov","quicktime"); map.Add("mp3","mp3"); map.Add("mp4","mpeg-4v"); map.Add("mpc","msproject_calendar");
            map.Add("mpg","mpeg"); map.Add("mpeg","mpeg"); map.Add("mpg2","mpeg2"); map.Add("mpeg2","mpeg2");
            map.Add("mpp","msproject");map.Add("mps","mps"); map.Add("mpt","mpt"); map.Add("mpv","msproject_view");
            map.Add("mpw","mpw"); map.Add("msg","msg"); map.Add("mss","mss"); map.Add("mxf","mxf");
            map.Add("odt","odt"); map.Add("oft","oft"); map.Add("one","one"); map.Add("onetoc","onetoc");
            map.Add("openformat","openformat"); map.Add("opml","opml"); map.Add("ov","cals2");
            map.Add("pcd","pcd");map.Add("pcl","pcl"); map.Add("pct","pict"); map.Add("pcx","pcx");
            map.Add("pdf","pdf");map.Add("pen","pen"); map.Add("pgm","pgm"); map.Add("php","php");
            map.Add("php3","php3"); map.Add("phtml","phtml"); map.Add("pm6","pagemaker");
            map.Add("png","png"); map.Add("pnm","pnm"); map.Add("pot","ppt8_template");
            map.Add("potm","ppt12metemplate"); map.Add("potx","ppt12template"); map.Add("ppm","ppm");
            map.Add("pps","pps"); map.Add("ppsm","ppt12meslideshow"); map.Add("ppsx","ppt12slideshow");
            map.Add("ppt","powerpoint"); map.Add("pptm","ppt12me"); map.Add("pptx","ppt12"); 
            map.Add("pre","freelance"); map.Add("pro","pro"); map.Add("properties","property");
            map.Add("prt","ptcprt"); map.Add("ps","ps"); map.Add("psd","photoshop7"); map.Add("ptd","ptd");
            map.Add("pub","pub"); map.Add("qxd","quark"); map.Add("ra","ra"); map.Add("ram","ram");
            map.Add("ras","ras");map.Add("rf10","rsrc10"); map.Add("rf9","rsrc9"); map.Add("rft","dca");
            map.Add("rle","rle"); map.Add("rls","rls"); map.Add("rlx","rlx"); map.Add("rm","rm");
            map.Add("rmh","rmh"); map.Add("rmm","rmm"); map.Add("rnx","rnx"); map.Add("rtf","rtf");
            map.Add("rv","rv"); map.Add("sam","amipro");map.Add("scm","scam"); map.Add("sct","sct");
            map.Add("sgi","sgi"); map.Add("sgm","sgml"); map.Add("shtml","shtml"); map.Add("sl","hpuxshrlib");
            map.Add("smil","smil");map.Add("so","linuxshrlib"); map.Add("soc","soc"); map.Add("spl","spl");
            map.Add("spml","spml"); map.Add("stm","stm"); map.Add("sun","sun"); map.Add("svg","svg");
            map.Add("swf","swf"); map.Add("tbr","tbr"); map.Add("tga","tga"); map.Add("tif","tiff");
            map.Add("txt","crtext"); map.Add("ump","ump"); map.Add("voc","voc"); map.Add("vrf","vrf");
            map.Add("vsd","vsd");map.Add("vss","vss"); map.Add("vst","vst"); map.Add("vsw","vsw");
            map.Add("wav","wave"); map.Add("wax","wax"); map.Add("wbmp","wbmp"); map.Add("wk4","123w");
            map.Add("wma","wma"); map.Add("wmf","wmf"); map.Add("wmv","wmv"); map.Add("wpd","wp10+");
            map.Add("wri","winwrite");map.Add("wrl","wrl"); map.Add("wsdl","wsdl"); map.Add("wvx","wvx");
            map.Add("xfdf","xfdf");map.Add("xfm","xfm"); map.Add("xls","excel"); map.Add("xlsb","excel12bbook"); 
            map.Add("xlsm","excel12mebook"); map.Add("xlsx","excel12book"); map.Add("xlt","excel8template"); 
            map.Add("xltm","excel12metemplate"); map.Add("xltx","excel12template"); map.Add("xlw","excel4book");
            map.Add("xmi","xmi");map.Add("xml","xml"); map.Add("xsd","xsd"); map.Add("xsf","xsf"); map.Add("xsl","xsl");
            map.Add("xsn","xsn"); map.Add("xtg","quark6_report"); map.Add("zip","zip"); map.Add("default","unknown");
            return map;
        }

        /// <summary>
        /// Gets the extension from a file for mapping into the Documentum system
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static String getDosExtensionFromFormat(string format)
        {
            Dictionary<string,string> map = getFormatMappings();
            string extension = (from k in map where k.Value.Equals(format) select k.Key).FirstOrDefault();
            return extension;
        }

        /// <summary>
        /// Get the documentum format string given a file extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static String getDocumentumFormatForFile(String extension)
        {
            if (extension.Contains('.'))
            { 
                extension = extension.Substring(extension.LastIndexOf('.')+1);
            }
            
            String format = "";

            Dictionary<string, string> map = getFormatMappings();
            format = (from k in map where k.Key.Equals(extension) select k.Value).FirstOrDefault();
            if (format == null)
            {
                return "unknown";
            }
            
            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static String getMimeTypeFromFileName(String fileName) {
           return CoreMimeMapping.GetMimeMapping(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T getRandomObjectFromArray<T>(T[] obj)
        {
            int index = new Random().Next(0, obj.Count());
            return obj[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T getRandomObjectFromList<T>(List<T> obj)
        {
            int index = new Random().Next(0, obj.Count);
            return obj[index];
        }

        /// <summary>
        /// Given a file path to a directory, will choose a random file from it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo getRandomFileFromDirectory(String path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = dirInfo.GetFiles();
            return fileInfo[new Random().Next(0, fileInfo.Length)];
        }

        /// <summary>
        /// Return a random file from a directory of a certain type
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static FileInfo getRandomFileFromDirectoryByExtension(String path, String extension)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = dirInfo.GetFiles("*." + extension);
            if (fileInfo.Any(x => x.Extension.Equals("." + extension)))
            {
                return fileInfo[new Random().Next(0, fileInfo.Length)];
            }
            else return null;
        }

        /// <summary>
        /// Create a file with random information for upload
        /// </summary>
        /// <param name="length"></param>
        /// <returns>Returns File object</returns>
        public static FileInfo getNewTextFileWithLength(int length)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            string path = Path.GetTempFileName();
            FileInfo file = new FileInfo(path);
            StreamWriter writer = file.CreateText();
            char[] texts = new char[length];
            for (int k = 0; k < length; k++ )
            {
                texts[k] = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            }
            writer.Write(texts);
            writer.Flush();
            writer.Dispose();
            return file;
        }


        public static EmailPackage getFeedAsEmailPackage(Feed<Document> feed)
        {
            RestController client = feed.Client;
            if (client == null) throw new Exception("CLIENTNULLEXCEPTION: the feed passed does not have its client set, no operations are possible without this.");
            long tStart = DateTime.Now.Ticks;
            SingleGetOptions options = new SingleGetOptions { Links = true };
            EmailPackage emailPkg = null; ;
            List<Entry<Document>> entries = feed.Entries;
            Entry<Document> emailEntry = (from k in entries where (k.Title != null && k.Title.Equals("EMAIL")) select k).FirstOrDefault();
            var attachmentEntry = (from a in entries where (a.Title != null && a.Title.Equals("ATTACHMENT")) select a.Content);

            if (emailEntry == null) return null;

            string objectId = emailEntry.Content.GetPropertyValue("r_object_id").ToString();
            Document document = null;
            //todo
                //client.getDocumentByQualification(
                //String.Format("dm_document where r_object_id = '{0}'", objectId), 
                //new FeedGetOptions { Inline = true, Links = true });

            emailPkg = new EmailPackage(document, feed.Title.StartsWith("DUPLICATE"));
            if (emailPkg.IsDuplicate)
            {
                if (feed.Title.Equals("DUPLICATE"))
                {
                    feed.Client.Logger.WriteToLog(Rest.Utility.LogLevel.WARNING, "EmailImport", "API on the server is out of date"
                    + "and will not search for duplicates in the System, only Folder.", new Exception("SERVERAPIOUTDATED"));
                }
                // Determine Duplicate Type and fill it in
                if (feed.Title.Equals("DUPLICATE-FOLDER"))
                {
                    emailPkg.DuplicateType = DuplicateType.FOLDER;
                }
                else emailPkg.DuplicateType = DuplicateType.SYSTEM;
            }
            
            //string extension = (from k in map where k.Value.Equals(format) select k.Key).FirstOrDefault();

            foreach (Document attachment in attachmentEntry)
            {
                string attachmentId = attachment.GetPropertyValue("r_object_id").ToString();
                Document attDocument = null;
                // todo
                // client.GetObjectById<Document>(attachmentId, options);
                emailPkg.Attachments.Add(attDocument);
            }
            client.Logger.WriteToLog(Rest.Utility.LogLevel.DEBUG, "EmailFeedToPackage", 
                "Conversion took " + ((DateTime.Now.Ticks - tStart)/TimeSpan.TicksPerMillisecond) + "ms", "TIMING");
            return emailPkg;
        }

        //private static T GetContentWithLinks<T>(RestController client, T content) where T : PersistentObject
        //{
        //    T fullContent = default(T);
        //    PersistentObject obj = (content as PersistentObject);
        //    string objectId = obj.getAttributeValue("r_object_id").ToString();
        //    SingleGetOptions options = new SingleGetOptions { Inline = true, Links = true };
        //    fullContent = client.GetObjectById<T>(objectId, options);
        //    if (content is Executable) (fullContent as Executable).SetClient(client);
        //    return fullContent;
        //}

        /// <summary>
        /// Gets the results of a request and converts them to a LIST as FULL Objects
        /// If you do not want to convert the objects in the list to full fetched
        /// objects, use getFeedAsList with the boolean fetchFullObject method argument.
        /// This method was made for backwards compatibility with former API releases.
        /// </summary>
        /// <typeparam name="T">The type of list to be returned</typeparam>
        /// <param name="fetchFullObject">Whether or not to fetch the full object(s) or just convert the feed to a list</param>
        /// <returns></returns>
        public static List<T> getFeedAsList<T>(Feed<T> feed) {
            return getFeedAsList(feed, true);
        }

        /// <summary>
        /// Gets the results of a request and converts them to a LIST
        /// </summary>
        /// <typeparam name="T">The type of list to be returned</typeparam>
        /// <param name="feed">The feed to be converted</param>
        /// <param name="fetchFullObject">Whether or not to fetch the full object(s) or just convert the feed to a list</param>
        /// <returns></returns>
        public static List<T> getFeedAsList<T>(Feed<T> feed, bool fetchFullObject)
        {
            List<T> list = new List<T>();
            List<Entry<T>> entries = feed.Entries;
            foreach (Entry<T> entry in entries)
            {
                T content = entry.Content;
                T fullContent = default(T);

                {
                    if (content is Linkable && fetchFullObject)
                    {
                        Linkable linkable = content as Linkable;
                        if (linkable.Links.Count > 0)
                        {
                            
                            Link self = LinkRelations.FindLink((content as Linkable).Links, LinkRelations.SELF.Rel);
                            if(self == null)
                            {
                                self = (content as Linkable).Links[0];
                            }
                            fullContent = feed.Client.Get<T>(self.Href, null);
                        }
                        else fullContent = content;
                    }
                    else
                    {
                        fullContent = content;
                    }
                }
                
                if(content is Executable) (fullContent as Executable).SetClient(feed.Client);
                list.Add(fullContent);
            }
            return list;
        }


        public static string getSafeFileName(string fileName)
        {
            fileName = fileName.Replace('/', '-');
            fileName = fileName.Replace('?', ' ');
            fileName = fileName.Replace('<', '[');
            fileName = fileName.Replace('>', ']');
            fileName = fileName.Replace('\\', '_');
            fileName = fileName.Replace(':', '-');
            fileName = fileName.Replace('*', '#');
            fileName = fileName.Replace('|', '#');
            fileName = fileName.Replace("\"", "'");
            return fileName;
        }
    }
}
