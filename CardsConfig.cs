using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LayoutTest
{
    class CardsConfig
    {
        int maxListCount = 10;
        List<List<string>> cardsPiles = new List<List<string>>();

        public CardsConfig(Stream xml)
        {
            //string xmlFile = xml;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xml);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("urn", "urn:schemas-microsoft-com:office:spreadsheet");


            XmlNode Table = xmlDoc.SelectSingleNode("//urn:Worksheet/urn:Table", nsMgr);
            if (Table == null)
            {
                Console.WriteLine("null");
            }
            else
            {
                for (int i = 0; i < maxListCount; i++)//最大列数量
                {
                    List<string> tmp = new List<string>();
                    cardsPiles.Add(tmp);
                }


                XmlNodeList xnl = Table.ChildNodes;
                for (int i = 0; i < xnl.Count; i++)
                {
                    XmlNode row = xnl[i];
                    XmlNodeList cells = row.ChildNodes;
                    for (int j = 0; j < cells.Count; j++)
                    {
                        XmlElement xe = (XmlElement)cells.Item(j);
                        if ( string.IsNullOrEmpty(xe.GetAttribute("ss:Index"))==false)
                        {
                            cardsPiles[int.Parse(xe.GetAttribute("ss:Index"))-1].Add(cells[j].InnerText);
                        }
                        else
                        {
                            cardsPiles[j].Add(cells[j].InnerText);
                        }
                    }
                }
            }
        }

        public int GetPilesCount()
        {
            int piles = 0;
            foreach (var item in cardsPiles)
            {
                if (item.Count > 0)
                {
                    piles++;
                }
            }
            return piles;
        }
        public List<string> GetPile(int index)
        {
            try
            {
                return cardsPiles[index];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }


    }
}
