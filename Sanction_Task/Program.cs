using System;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Sanction_Task
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            string pagesource;
            List<string> postLinksForDetail = new List<string>();
            List<string> postNames = new List<string>();
            List<int> postPrices = new List<int>();
            HtmlAgilityPack.HtmlDocument html;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.sahibinden.com/");
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, encoding: Encoding.UTF8);
                pagesource = reader.ReadToEnd();
            }
            using (WebClient client = new WebClient())
            {
               
                html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(pagesource);
                HtmlNode[] nodes = html.DocumentNode.SelectNodes(@"//*[@id=""container""]/ div[3]/div/div[3]/div[3]/ul/li/a").ToArray();
                foreach (var item in nodes)
                {
                   postLinksForDetail.Add(item.Attributes["href"].Value);
                   postNames.Add(item.InnerText);
                   
                }
            }
            //for (int i = 0; i < postLinksForDetail.Count; i++)
            //{
            //    HttpWebRequest requestForDetail = (HttpWebRequest)WebRequest.Create($"https://www.sahibinden.com/ + {postLinksForDetail[i]}");
            //}
            Console.ReadLine();


        }
        
        
    }
    
}
