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
using System.Collections;
using System.Net.Sockets;

namespace LayoutTest
{
    [Activity(Label = "My Activity")]
    //游戏公正
    public class OutputActivity : Activity
    {
        public static OutputActivity instance;
        List<string> lineContent = new List<string>();
        ListView listView;

        Dictionary<int, string> cards1List = new Dictionary<int, string>();
        Dictionary<int, string> cards2List = new Dictionary<int, string>();
        const int pileList = 2;
        List<int>[] cardsRest = new List<int>[pileList];

        List<int>[] cardsSended = new List<int>[pileList];

        List<int>[] cardsOuted = new List<int>[pileList];


        public List<int> PlayerRequireCard(int pileIndex,int cardcount)//return json
        {
            List<string> cards1 = ((CardsInfoAdapter)(CardsInfoActivity.instance.listView1.Adapter)).GetCards();
            cards1List.Clear();
            for (int i = 0; i < cards1.Count; i++)
            {
                cards1List.Add(i, cards1[i]);
            }
            List<string> cards2 = ((CardsInfoAdapter)(CardsInfoActivity.instance.listView2.Adapter)).GetCards();
            cards2List.Clear();
            for (int i = 0; i < cards2.Count; i++)
            {
                cards2List.Add(i, cards2[i]);
            }


            //获得所有可发的牌
            //判断是否能循环发牌

            Sessions ses = LitJson.JsonMapper.ToObject<Sessions>(SessionsActivity.instance.GetSessionJsonData().ToJson());


            List<int> outcards = new List<int>();

            if (ses.循环发牌)
            {
                Random ran = new Random();
                for (int i = 0; i < cardcount; i++)
                {
                    if (cardsRest[pileIndex].Count == 0)
                    {
                        for (int j = 0; j < cardsOuted[pileIndex].Count; j++)
                        {
                            cardsRest[pileIndex].Add(cardsOuted[pileIndex][j]);
                        }
                        cardsOuted[pileIndex].Clear();
                    }
                    int cardindex = cardsRest[pileIndex][ran.Next(cardsRest[pileIndex].Count)];
                    outcards.Add(cardindex);
                    cardsSended[pileIndex].Add(cardindex);
                }
            }
            else
            {
                Random ran = new Random();
                for (int i = 0; i < cardcount; i++)
                {
                    if (cardsRest[pileIndex].Count == 0)
                    {
                        break;
                    }
                    int cardindex = cardsRest[pileIndex][ran.Next(cardsRest[pileIndex].Count)];
                    outcards.Add(cardindex);
                    cardsSended[pileIndex].Add(cardindex);
                }

            }
            return outcards;
        }
        public void NetworkMsg(Socket ts,string json)
        {
            Console.WriteLine(json);
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(json);
            if (((IDictionary)data).Contains("requirecards"))
            {
                if (((IDictionary)data).Contains("pileindex"))
                {
                    int pileindex = (int)data["pileindex"];
                    int cardscount = (int)data["requirecards"];
                    List<int> require = PlayerRequireCard(pileindex, cardscount);
                    string send = LitJson.JsonMapper.ToJson(require);
                    Console.WriteLine("send: "+send);
                    Activity1.instance.sockerServer.SendPackage(ts, send);
                }
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            instance = this;
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.console_output);

            listView = FindViewById<ListView>(Resource.Id.listView1);
            listView.Adapter = new OutputAdapter(this, lineContent);

            Button bt = FindViewById<Button>(Resource.Id.button1);
            //按下开始按钮
            bt.Click += (sender, e) =>
            {

                Push(DateTime.Now.ToString());
                
            };
            

        }
        public void Clear()
        {
            lineContent.Clear();
        }
        public void Push(string sz)
        {

            lineContent.Insert(0, sz);
            listView.Adapter = new OutputAdapter(this, lineContent);
        }
        public void Pop()
        {
            if (lineContent.Count > 0)
            {
                lineContent.RemoveAt(lineContent.Count - 1);
            }
            listView.Adapter = new OutputAdapter(this, lineContent);
        }
    }
}