using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Configuration;
namespace Sanction_Task
{
    internal class Program
    {

        static Dictionary<string, string> combinedList = new Dictionary<string, string>();
        static HtmlAgilityPack.HtmlDocument? html;
        static List<string> postLinksForDetail = new List<string>();
        static List<string> postNames = new List<string>();
        static List<string> postPrices = new List<string>();
        static List<int> pricesForAverage = new List<int>();

        static void Main(string[] args)
        {
            ReadDetailLinks("https://www.sahibinden.com");
            ReadDetails("https://www.sahibinden.com");
            ShowInConsole();
            SaveToFile(@"C:");
        }
        public static void ReadDetailLinks(string link)
        {
            string pageSource;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Timeout = 3000;
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, encoding: Encoding.UTF8);
                pageSource = reader.ReadToEnd();
            }
            using (WebClient client = new WebClient())
            {
                //client.Headers.Add("user-agent", Guid.NewGuid().ToString());
                //client.Proxy = new WebProxy("89.43.31.134");
                html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(pageSource);
                HtmlNode[] nodes = html.DocumentNode.SelectNodes(@"//*[@id=""container""]/ div[3]/div/div[3]/div[3]/ul/li/a").ToArray();
                foreach (var item in nodes)
                {
                    if (item.Attributes["href"].Value.Contains("/ilan"))
                    {
                        postLinksForDetail.Add(item.Attributes["href"].Value);
                        postNames.Add(item.InnerText);
                    }
                    else   // ilan olmayan 3 adet girdiyi burada eliyorum bu 3 veri reklam oluyor.
                    {
                        Console.WriteLine("burası reklam olan satır");
                    }
                }
            }
        }
        public static void ReadDetails(string link)
        {
            string pageSourceForDetails;
            for (int i = 0; i < postLinksForDetail.Count; i++)
            {
                HttpWebRequest requestForDetail = (HttpWebRequest)WebRequest.Create(link + postLinksForDetail[i].ToString()); //linki detay linki ile birleştirdim.
                requestForDetail.Timeout = 3000;
                WebResponse responseForDetail = requestForDetail.GetResponse();
                using (Stream responseForDetailStream = responseForDetail.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseForDetailStream, encoding: Encoding.UTF8);
                    pageSourceForDetails = reader.ReadToEnd();
                }
                using (WebClient clientForDetail = new WebClient())
                {
                    //clientForDetail.Headers.Add("user-agent", Guid.NewGuid().ToString());
                    //clientForDetail.Proxy = new WebProxy("89.43.31.134");
                    html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(pageSourceForDetails);
                    var nodesForDetail = html.DocumentNode.SelectSingleNode(@"//*[@id=""favoriteClassifiedPrice""]"); //*[@id="favoriteClassifiedPrice"]
                    if (nodesForDetail == null)
                    {
                        Console.WriteLine("Burası Reklam Satırı");
                    }
                    else
                    {
                        postPrices.Add(nodesForDetail.Attributes["value"].Value);
                        postPrices[i] = postPrices[i].Replace("TL", "");
                        postPrices[i] = postPrices[i].Replace(".", "");
                    }
                }
            }
            // Not: vitrinde 56 veri var ve 3 adedi reklam olduğu için 53 adet veri geliyor. Fakat Sahibinden.com veri çektikten belli bir süre 
            // Sonra ip ban atıyor. 3-4 veri ile denediğimde oldu fakat 56 verinin tamamı ile deneyince System.Net.WebException: 'The remote server returned an error:(429)
            // hatası veriyor. Bu kısmı çözmek için timeout, proxy, useragent gibi yollar denedim fakat yapamadım. Değerlendirmeniz için belirtmek istedim.
        }
        public static void ShowInConsole()
        {
            for (int i = 0; i < postPrices.Count; i++)
            {
                Console.WriteLine(postNames[i] + ":" + postPrices[i]);
                combinedList.Add(postNames[i], postPrices[i]);
            }
            if (postPrices.Count > 0)
            {
                foreach (var item in postPrices)
                {
                    pricesForAverage.Add(Convert.ToInt32(item));
                }
            }
            Console.WriteLine("Average:"+pricesForAverage.Average().ToString() + "TL");
        }
        public static void SaveToFile(string path)
        {
            FileStream fs = File.Create(path + "\\" + "sahibinden.txt");
            StreamWriter sw = new StreamWriter(fs);
            foreach (var item in combinedList)
            {
                sw.WriteLine(item.Key.Trim() + ":" + item.Value.Trim());
            }
            sw.WriteLine("Average:" + pricesForAverage.Average().ToString() + "TL"); 
            sw.Close();
            fs.Close();
        }
    }
}
