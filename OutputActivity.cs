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
using LitJson;

namespace LayoutTest
{
    [Activity(Label = "My Activity")]
    //游戏公正
    public class OutputActivity : Activity
    {
        public static OutputActivity instance;
        List<string> lineContent = new List<string>();
        ListView listView;


        List<CardInfo> cards = new List<CardInfo>();


        List<string>[] cardsName = new List<string>[2];
        List<CardInfo> cardsIdRest = new List<CardInfo>();

        List<CardInfo> cardsOuted = new List<CardInfo>();



        public List<CardInfo> PlayerRequireCard(int pileIndex, int cardcount)//return json
        {
            List<CardInfo> pileIndexRestCards = new List<CardInfo>();//
            foreach (var item in cardsIdRest)
            {
                if (item.pileIndex == pileIndex)
                {
                    if (CardsInfoActivity.instance.GetPile(pileIndex).GetCardChecked(item.id))
                    {
                        pileIndexRestCards.Add(item);
                    }
                }
            }

            //获得所有可发的牌
            //判断是否能循环发牌

            Sessions ses = LitJson.JsonMapper.ToObject<Sessions>(SessionsActivity.instance.GetSessionJsonData().ToJson());


            List<CardInfo> requirecards = new List<CardInfo>();

            if (ses.循环发牌)
            {
                Random ran = new Random();
                for (int i = 0; i < cardcount; i++)
                {
                    if (pileIndexRestCards.Count == 0)
                    {
                        for (int j = 0; j < cardsOuted.Count; j++)
                        {
                            if (cardsOuted[j].pileIndex == pileIndex)
                            {
                                pileIndexRestCards.Add(cardsOuted[j]);
                            }

                        }
                        cardsOuted.Clear();
                    }
                    CardInfo card = pileIndexRestCards[ran.Next(pileIndexRestCards.Count)];
                    requirecards.Add(card);

                }
            }
            else
            {
                Random ran = new Random();
                for (int i = 0; i < cardcount; i++)
                {
                    if (pileIndexRestCards.Count == 0)
                    {
                        break;
                    }
                    CardInfo card = pileIndexRestCards[ran.Next(pileIndexRestCards.Count)];
                    requirecards.Add(card);

                }

            }
            return requirecards;
        }
        public void NetworkMsg(Socket ts, string json)
        {
            Sessions ses = LitJson.JsonMapper.ToObject<Sessions>(SessionsActivity.instance.GetSessionJsonData().ToJson());

            Console.WriteLine(json);
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(json);
            if ((string)data["cmd"] == "requirecards")
            {
                if (((IDictionary)data).Contains("pileindex"))
                {
                    int pileindex = (int)data["pileindex"];
                    int cardscount = ses.发牌数;
                    List<CardInfo> require = PlayerRequireCard(pileindex, cardscount);
                    string send = LitJson.JsonMapper.ToJson(require);
                    Console.WriteLine("send: " + send);

                    LitJson.JsonData callback = new LitJson.JsonData();
                    callback["cmd"] = "requirecards_callback";
                    callback["cards"] = (JsonMapper.ToJson(require));
                    Activity1.instance.sockerServer.SendPackage(ts, callback.ToJson());
                }
            }
            else if ((string)data["cmd"] == "outcards")
            {
                List<CardInfo> ci = JsonMapper.ToObject<List<CardInfo>>((string)data["cards"]);
                foreach (var item in ci)
                {
                    cardsOuted.Add(item);
                }
                List<Socket> otherClients = Activity1.instance.sockerServer.socketList;

                LitJson.JsonData callback = new LitJson.JsonData();
                callback["cmd"] = "outcards_callback";
                callback["cards"] = (JsonMapper.ToJson(ci));
                foreach (var item in otherClients)
                {
                    Activity1.instance.sockerServer.SendPackage(item, callback.ToJson());
                }
            }
            else if ((string)data["cmd"] == "newgame")
            {
                //send to client
               

                LitJson.JsonData callback = new LitJson.JsonData();
                callback["cmd"] = "newgame_callback";
                callback["init"] = JsonMapper.ToJson(cards);


                List<CardInfo> require = PlayerRequireCard(0, ses.游戏开始手牌数量);
                callback["cards"] = (JsonMapper.ToJson(require));
                Activity1.instance.sockerServer.SendPackage(ts, callback.ToJson());

            }

        }
        public void NewGame()
        {
            Sessions ses = LitJson.JsonMapper.ToObject<Sessions>(SessionsActivity.instance.GetSessionJsonData().ToJson());

            cardsName[0] = ((CardsInfoAdapter)(CardsInfoActivity.instance.listView1.Adapter)).GetCards();
            cardsName[1] = ((CardsInfoAdapter)(CardsInfoActivity.instance.listView2.Adapter)).GetCards();


            cards.Clear();
            cardsIdRest.Clear();
            cardsOuted.Clear();

            for (int i = 0; i < cardsName[0].Count; i++)
            {
                CardInfo ci = new CardInfo() { pileIndex = 0, id = i, name = cardsName[0][i] };
                cards.Add(ci);
                cardsIdRest.Add(ci);
            }
            for (int i = 0; i < cardsName[1].Count; i++)
            {
                CardInfo ci = new CardInfo() { pileIndex = 0, id = i, name = cardsName[1][i] };
                cards.Add(ci);
                cardsIdRest.Add(ci);
            }

            List<Socket> clients = Activity1.instance.sockerServer.socketList;
            LitJson.JsonData childJ = new LitJson.JsonData();
            childJ["cmd"] = "newgame";
            childJ["sessions"] = JsonMapper.ToJson(ses);
            foreach (var item in clients)
            {
                Activity1.instance.sockerServer.SendPackage(item, childJ.ToJson());
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
                Clear();
                Push(DateTime.Now.ToString());
                NewGame();
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