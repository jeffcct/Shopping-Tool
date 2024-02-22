using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CsvHelper;
using HtmlAgilityPack;

namespace MyProgram {
    class Scraper {
        static void Main(string[] args) {
            String url = "http://books.toscrape.com/catalogue/category/books/mystery_3/index.html";
            List<String> links = GetItemLinks(url);
            foreach (String link in links) {
                Console.WriteLine("{0}", link);
            } 

            //List<Item> items = GetItemInformation(links);
            //Console.WriteLine("There were {0} items", links.Count);
            //exportToCSV(books);
        }

        static HtmlDocument GetDocument(string url) {
            HtmlWeb web = new HtmlWeb();
            
            HtmlDocument doc = web.Load(url);
            Console.WriteLine("hi");
            return doc;
        }

        static List<String> GetItemLinks(String url) {
            var ItemLinks = new List<String>();
            HtmlDocument doc = GetDocument(url);
            String query = "\\a[contains(@class,\"product-entry\")]";
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes(query);
            int total = 0;
            foreach (var link in linkNodes) {
                ItemLinks.Add(link.Attributes["href"].Value);
                Console.WriteLine(total);
                total++;
            }
            return ItemLinks; 
        }
    }
}

