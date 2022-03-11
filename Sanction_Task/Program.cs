using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Sanction_Task
{
    internal class Program
    {
        HtmlAgilityPack.HtmlDocument? html;
       static List<string> postLinksForDetail = new List<string>();
       static List<string> postNames = new List<string>();
       static List<string> postPrices = new List<string>();

        static void Main(string[] args)
        {
            ShowInConsole();
        }  
        public void ReadDetailLinks(string link)
        {
            string pagesource;
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link); /*https://www.sahibinden.com/anasayfa-vitrin?viewType=Gallery*/
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, encoding: Encoding.UTF8);
                pagesource = reader.ReadToEnd();
            }
            using (WebClient client = new WebClient())
            {
                client.Proxy = new WebProxy("89.43.31.134");
                html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(pagesource);
                HtmlNode[] nodes = html.DocumentNode.SelectNodes(@"//*[@id=""container""]/ div[3]/div/div[3]/div[3]/ul/li/a").ToArray();
                foreach (var item in nodes)
                {
                    postLinksForDetail.Add(item.Attributes["href"].Value);
                    postNames.Add(item.InnerText);
                }
            }
        }
        public void ReadDetails(string link)
        {
            string pageSourceForDetails;
            for (int i = 0; i < postLinksForDetail.Count; i++)
            {
                HttpWebRequest requestForDetail = (HttpWebRequest)WebRequest.Create(link + postLinksForDetail[i].ToString());
                WebResponse responseForDetail = requestForDetail.GetResponse();
                using (Stream responseForDetailStream = responseForDetail.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseForDetailStream, encoding: Encoding.UTF8);
                    pageSourceForDetails = reader.ReadToEnd();
                }
                using (WebClient clientForDetail = new WebClient())
                {
                    clientForDetail.Proxy = new WebProxy("89.43.31.134"); //sahibinden ip ban atıyor onu engellemek için koydum.
                    html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(pageSourceForDetails);
                    var nodesForDetail = html.DocumentNode.SelectSingleNode(@"//*[@id=""favoriteClassifiedPrice""]"); //*[@id="favoriteClassifiedPrice"]
                    postPrices.Add(nodesForDetail.Attributes["value"].Value);
                    
                }
            }
        }
        public static void ShowInConsole()
        {
            for (int i = 0; i < postPrices.Count; i++)
            {
                Console.WriteLine(postNames[i] + ":" + postPrices[i]);
            }
            Console.ReadLine();
        }
        public void MeanPrice()
        {

        }
    }
    
}
