using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.DataModel;
using System.Collections;
using System.Collections.Generic;
using Emc.Documentum.Rest.Http.Utility;

namespace DroidBlankTest1
{
    [Activity(Label = "Documentum Testing", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private static readonly String ROOT = "0000000000000000";
        List<String> itemList = new List<String>();
        List<PersistentObject> currentObjectList;
        String currentId;
        List<String> previousIds = new List<String>();
        Repository repository;
        FeedGetOptions DEFAULT_OPTIONS { get { return new FeedGetOptions { Inline = true, Links = true }; } }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            PersistentObject obj = currentObjectList[position];
            PersistentObject currentObject = currentObjectList[position];
            String objectId = currentObject.getAttributeValue("r_object_id").ToString();
            if(currentObject.getAttributeValue("r_object_type").ToString().Equals("dm_document"))
            {
                //RestDocument doc = repository.getObjectById<RestDocument>(objectId);
                //ContentMeta contentMeta = doc.getContent();
                //Android.Net.Uri uri = Android.Net.Uri.Parse(contentMeta.getMediaUri());
                //Intent intent = new Intent(Intent.ActionView);
                //intent.SetDataAndType(uri, "application/octet-stream");
                //intent.SetFlags(ActivityFlags.ClearTop);
                //StartActivity(intent);
            } else
            {
                String dql = String.Format("select r_object_id, r_object_type, object_name from dm_sysobject where folder(ID('{0}'))",
                    objectId);
                itemList = new List<String>();
                currentObjectList = null;
                GetObjects(dql, DEFAULT_OPTIONS);

                Android.Widget.Toast.MakeText(this, "Navigating...", Android.Widget.ToastLength.Short).Show();

                UpdateListView();
                if (!currentId.Equals(ROOT)) previousIds.Add(currentId);
                currentId = objectId;
            }

            
        }

        public override void OnBackPressed()
        {
            if (currentId.Equals(ROOT)) base.OnBackPressed();
            String objectId = previousIds[previousIds.Count - 1];
            String dql = objectId.Equals(ROOT)? "select r_object_id, object_name, r_object_type from dm_cabinet" 
                    : String.Format("select r_object_id, r_object_type, object_name from dm_sysobject where folder(ID('{0}'))",
                    objectId);
            itemList = new List<String>();
            currentObjectList = null;
             // Exit the application as the previous page was before we entered the app.
            GetObjects(dql, DEFAULT_OPTIONS);

            Android.Widget.Toast.MakeText(this, "Navigating Back...", Android.Widget.ToastLength.Short).Show();

            UpdateListView();
            if(!objectId.Equals(ROOT)) previousIds.Remove(objectId);
            currentId = objectId;
            
        }

        private void WriteOutput(String msg)
        {
            Console.WriteLine(msg);
        }

        public void GetObjects(String dqlQuery, FeedGetOptions options)
        {
            Feed<PersistentObject> results = repository.ExecuteDQL<PersistentObject>(dqlQuery, options);

            if (results == null)
            {
                itemList.Add("NO RESULTS");
                return;
            }
            int resultCount = 0;
            currentObjectList = ObjectUtil.getFeedAsList<PersistentObject>(results, false);
            WriteOutput(String.Format("\t\t[API-11] Returning list of objects from query [{0}]", dqlQuery));
            foreach (PersistentObject obj in currentObjectList)
            {
                itemList.Add(obj.getAttributeValue("r_object_type").ToString() + ": " + obj.getAttributeValue("object_name").ToString());
                WriteOutput(String.Format("\t\t\tType: {0} Name: {1} ID: {2}",
                    obj.getAttributeValue("r_object_type").ToString(),
                    obj.getAttributeValue("object_name").ToString(),
                    obj.getAttributeValue("r_object_id").ToString()));
                resultCount++;
            }
            if (resultCount == 0) itemList.Add("NO RESULTS");
        }

        private void UpdateListView()
        {
            ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, itemList.ToArray());
            ExpandableListView dirList = FindViewById<ExpandableListView>(Resource.Id.listView);
        }

        protected override void OnCreate(Bundle bundle)
        {
            previousIds.Add(ROOT);
            currentId = ROOT;

            long testStart = DateTime.Now.Ticks;
            base.OnCreate(bundle);
            RestController client = new RestController("dmadmin", "xxxxxxx");
            RestService home = client.Get<RestService>("http://mm-d7.mccollough.int:8080/rest/services", null);
            if (home == null)
            {
                //WriteOutput("\nUnable to get Rest Service at: " + RestHomeUri + " check to see if the service is available.");
                return;
            }
            home.SetClient(client);
            ProductInfo productInfo = home.GetProductInfo();
            
            repository = home.GetRepository("McCollough");
            if (repository == null) throw new Exception("Unable to login to the repository, please see server logs for more details.");
            GetObjects(
                    "select r_object_id, object_name, r_object_type from dm_cabinet", DEFAULT_OPTIONS);

            WriteOutput("Took " + ((DateTime.Now.Ticks - testStart) / TimeSpan.TicksPerMillisecond) + "ms to get RestService and cabinet list");

            UpdateListView();
            
            // Set our view from the "main" layout resource
            // SetContentView(Resource.Layout.Main);

            // Get our UI controls from the loaded layout:
            //EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            //Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
            //Button callButton = FindViewById<Button>(Resource.Id.CallButton);



            //// Disable the "Call" button
            //callButton.Enabled = false;

            //// Add code to translate number
            //string translatedNumber = string.Empty;

            //translateButton.Click += (object sender, EventArgs e) =>
            //{
            //    // Translate user's alphanumeric phone number to numeric
            //    translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
            //    if (String.IsNullOrWhiteSpace(translatedNumber))
            //    {
            //        callButton.Text = "Call";
            //        callButton.Enabled = false;
            //    }
            //    else
            //    {
            //        callButton.Text = "Call " + translatedNumber;
            //        callButton.Enabled = true;
            //    }
            //};

            //callButton.Click += (object sender, EventArgs e) =>
            //{
            //    // On "Call" button click, try to dial phone number.
            //    var callDialog = new AlertDialog.Builder(this);
            //    callDialog.SetMessage("Call " + translatedNumber + "?");
            //    callDialog.SetNeutralButton("Call", delegate {
            //        // Create intent to dial phone
            //        var callIntent = new Intent(Intent.ActionCall);
            //        callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
            //        StartActivity(callIntent);
            //    });
            //    callDialog.SetNegativeButton("Cancel", delegate { });

            //    // Show the alert dialog to the user and wait for response.
            //    callDialog.Show();
            //};


        }
    }
}

