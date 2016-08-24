using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Net
{
    public class Search
    {
        private string _view;
        private bool _viewSpecified = false;
        public string View
        {
            get { return _view; }
            set { _viewSpecified = true; _view = value; }
        }

        private string _query;
        private bool _querySpecified = false;
        public string Query
        {
            get { return _query; }
            set { _querySpecified = true; _query = value; }
        }

        private string _locations;
        private bool _locationsSpecified = false;
        public string Locations
        {
            get { return _locations; }
            set { _locationsSpecified = true; _locations = value; }
        }

        private string _collections;
        private bool _collectionsSpecified = false;
        public string Collections
        {
            get { return _collections; }
            set { _collectionsSpecified = true; _collections = value; }
        }

        private string _facet;
        private bool _facetSpecified = false;
        public string Facet
        {
            get { return _facet; }
            set { _facetSpecified = true; _facet = value; }
        }

        private string _facetValueContraints;
        private bool _facetValueContraintsSpecified = false;
        public string FacetConstraints
        {
            get { return _facetValueContraints; }
            set { _facetValueContraintsSpecified = true; _facetValueContraints = value; }
        }

        /// <summary>
        /// TODO: Make this an enum of allowable values
        /// </summary>
        private string _timezone;
        private bool _timezoneSpecified = false;
        public string TimeZone
        {
            get { return _timezone; }
            set 
            {
                try
                {
                    TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(value);
                    if (info != null)
                    {
                        _timezoneSpecified = true;
                        _timezone = value;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unknown timezone id " + value + " used. Use SearchOptions.getValidTimeZonesgetValidTimeZones() for a list of timezone ids. Setting to TimeZone.Local=" + TimeZoneInfo.Local.Id, e);
                    TimeZone =  TimeZoneInfo.Local.Id;
                }
            }
        }

        public List<string> getValidTimeZones()
        {
            List<string> tzList = new List<string>();
            ReadOnlyCollection<TimeZoneInfo> tzs = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo info in tzs)
            {
                tzList.Add(info.Id);
            }
            return tzList;
        }

        private string _objectType;
        private bool _objectTypeSpecified = false;
        public string ObjectType
        {
            get { return _objectType; }
            set { _objectTypeSpecified = true; _objectType = value; }
        }

        private bool _inline = true;
        private bool _inlineSpecified = true;
        public bool Inline
        {
            get { return _inline; }
            set { _inlineSpecified = true; _inline = value; }
        }

        private int _page = 1;
        private bool _pageSpecified = true;
        public int PageNumber
        {
            get { return _page; }
            set { _pageSpecified = true; _page = value; }
        }

        private int _itemsPerPage = 10;
        private bool _itemsPerPageSpecified = true;
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPageSpecified = true; _itemsPerPage = value; }
        }

        private bool _includeTotal = true;
        private bool _includeTotalSpecified = true;
        public bool IncludeTotal
        {
            get { return _includeTotal; }
            set { _includeTotalSpecified = true; _includeTotal = value; }
        }

        private string _sort;
        private bool _sortSpecified = false;
        public string Sort
        {
            get { return _sort; }
            set { _sortSpecified = true; _sort = value; }
        }

        public bool Raw { get; set; }

        public List<KeyValuePair<string, object>> ToQueryList()
        {
            List<KeyValuePair<string, object>> pa = new List<KeyValuePair<string, object>>();

            if (_viewSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("view", _view));
            }
            if (_querySpecified)
            {
                pa.Add(new KeyValuePair<string, object>("q", _query));
            }
            if (_locationsSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("locations", _locations));
            }
            if (_collectionsSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("collections", _collections));
            }
            if (_facetSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("facet", _facet));
            }
            if (_facetValueContraintsSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("facet-value-contraints", _facetValueContraints));
            }
            if (_timezoneSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("timezone", _timezone));
            }
            //else
            //{
            //    pa.Add(new KeyValuePair<string, object>("timezone", TimeZoneInfo.Local.StandardName));
            //}
            if (_objectTypeSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("object-type", _objectType));
            }
            if (_inlineSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("inline", _inline));
            }
            if (_pageSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("page", _page));
            }
            if (_itemsPerPageSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("items-per-page", _itemsPerPage));
            }
            if (_includeTotalSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("include-total", _includeTotal));
            }
            if (_sortSpecified)
            {
                pa.Add(new KeyValuePair<string, object>("sort", _sort));
            }

            return pa;
        }
    }
}
