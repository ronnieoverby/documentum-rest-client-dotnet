using Emc.Documentum.Rest.Http.Utility;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "d2-configurations", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class D2Configurations : Linkable , Executable // Available on the Repository object.
    {
        public string Title { get; set; }
        public SearchConfigurations getSearchConfigurations()
        {
            String searchConfigURI = LinkRelations.FindLinkAsString(Links, LinkRelations.SEARCH_CONFIGURATION.Rel);
            SearchConfigurations sConfigs = Client.Get<SearchConfigurations>(searchConfigURI, null);
            sConfigs.SetClient(Client);
            return sConfigs;
        }

        public ProfileConfigurations getProfileConfigurations()
        {
            String profileConfigURI = LinkRelations.FindLinkAsString(Links, LinkRelations.PROFILE_CONFIGURATION.Rel);
            ProfileConfigurations pConfigs= Client.Get<ProfileConfigurations>(profileConfigURI, null);
            pConfigs.SetClient(Client);
            return pConfigs;
        }

        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }


    }


}



