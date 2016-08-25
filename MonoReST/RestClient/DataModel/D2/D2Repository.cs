using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;


namespace Emc.Documentum.Rest.DataModel.D2
{
    /// <summary>
    /// D2 Repository resource model
    /// </summary>
    [DataContract(Name = "repository", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class D2Repository : Repository
    {
        public D2Configurations GetD2Configurations(SingleGetOptions options)
        {
            return Client.GetSingleton<D2Configurations>(
                this.Links,
                LinkRelations.D2_CONFIGURATION.Rel,
                options);
        }

        public Feed<D2Task> GetD2TaskList()
        {
            FeedGetOptions options = new FeedGetOptions();
            options.Inline = true;
            return Client.GetFeed<D2Task>(
                this.Links, 
                LinkRelations.TASK_LIST.Rel, 
                options);
        }

        public bool isD2Rest()
        {
            string d2ConfigLink = LinkRelations.FindLinkAsString(
                Links,
                LinkRelations.D2_CONFIGURATION.Rel);
            return d2ConfigLink != null;
        }

        public D2Document ImportNewD2Document(FileInfo file, string documentName, string repositoryPath, D2Configuration d2config)
        {
            //if (!repositoryPath.StartsWith("/")) throw new Exception("Repository path " + repositoryPath + " is not valid."
            //     + " The path must be a fully qualified path");
            //Folder importFolder = getOrCreateFolderByPath(repositoryPath);
            //if (importFolder == null) throw new Exception("Unable to fetch or create folder by path: " + repositoryPath);

            D2Document newDocument = new D2Document();
            newDocument.SetPropertyValue("object_name", documentName);
            newDocument.SetPropertyValue("r_object_type", "dm_document");
            if (d2config != null) newDocument.Configuration = d2config;
            GenericOptions importOptions = new GenericOptions();
            importOptions.SetQuery("format", ObjectUtil.getDocumentumFormatForFile(file.Extension));
            D2Document created = ImportD2DocumentWithContent(newDocument, file.OpenRead(), ObjectUtil.getMimeTypeFromFileName(file.Name), importOptions);

            return created;
        }

        public D2Document ImportD2DocumentWithContent(D2Document newObj, Stream otherPartStream, string otherPartMime, GenericOptions options)
        {
            Dictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(otherPartStream, otherPartMime);
            return Client.Post<D2Document>(
                this.Links,
                LinkRelations.OBJECT_CREATION.Rel,
                newObj,
                otherParts,
                options);
        }


    } // End D2Repository Class
}
