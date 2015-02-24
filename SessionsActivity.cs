using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LitJson;
using System.Collections;
using System.IO;


namespace LayoutTest
{
    [Activity(Label = "My Activity")]
    public class SessionsActivity : Activity
    {
        LinearLayout principalview;
        LinearLayout.LayoutParams parametros = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
        FileStream sessionFile;
        
        LitJson.JsonData sessionData;
        public static SessionsActivity instance;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            instance = this;
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.sessions);

            //TextView textview = new TextView(this);
            //textview.Text = "This is the My SessionsActivity tab";
            //SetContentView(textview);

            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(documents);
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++");

            FileInfo fi = new FileInfo(  Path.Combine(documents,  "sessions.txt"));

            sessionFile = fi.Open(FileMode.OpenOrCreate);
            using (StreamReader sr = new StreamReader(sessionFile))
            {
                string sessionJson = sr.ReadToEnd();
                if (string.IsNullOrEmpty(sessionJson))
                {
                    sessionJson = JsonMapper.ToJson(new Sessions());
                }
                sessionData = LitJson.JsonMapper.ToObject(sessionJson);
            }
           
            sessionFile.Close();

            sessionFile = fi.Open(FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(sessionFile))
            {
                sw.WriteLine(sessionData.ToJson());
            }
            sessionFile.Close();

            //
            //
            //
            principalview = FindViewById<LinearLayout>(Resource.Id.linearLayout1);

            foreach (string key in ((IDictionary)sessionData).Keys)
            {
                if (sessionData[key].IsInt)
                {
                    CreateNumberEditSession(key, (int)sessionData[key]);
                }
                if (sessionData[key].IsBoolean)
                {
                    CreateBoolSwitchSession(key, (bool)sessionData[key]);
                }
            }



        }
        public Sessions GetSessions()
        {
            return JsonMapper.ToObject<Sessions>(sessionData.ToJson());
        }
        void CreateNumberEditSession(string title, int defaultValue)
        {
            
            LinearLayout ll = new LinearLayout(this);
            ll.WeightSum = 100;
            ll.Orientation = Orientation.Horizontal;

            TextView tv = new TextView(this);
            tv.Text = title;
            ll.AddView(tv);

            EditText et = new EditText(this);
            et.Text = defaultValue.ToString();
            et.InputType = Android.Text.InputTypes.ClassNumber;
            et.TextChanged += (sender, e) => 
            {
                string change = "";
                foreach (var item in e.Text)
                {
                    change += item;
                }
                if (change!="")
                {
                    sessionData[title] = int.Parse(change);
                }
                else
                {
                    sessionData[title] = 0;
                }
                Save();
            };
            ll.AddView(et);

            principalview.AddView(ll, parametros);
            
        }
        void CreateBoolSwitchSession(string title, bool defaultValue)
        {
            Switch sw = new Switch(this);
            sw.Checked = defaultValue;
            sw.Text = title;
            sw.CheckedChange += (sender, e) =>
            {
                sessionData[title] = e.IsChecked;
                Save();
            };
            principalview.AddView(sw);
        }
        void Save()
        {
            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            FileInfo fi = new FileInfo(Path.Combine(documents, "sessions.txt"));
            sessionFile = fi.Open(FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(sessionFile))
            {
                sw.WriteLine(sessionData.ToJson());
            }
            sessionFile.Close();
        }
    }
}