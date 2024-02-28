using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools.V120.Overlay;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using OpenQA.Selenium.DevTools.V120.Network;
using System.ComponentModel;
using System.Threading.Tasks;


class Item {
    public String Name {get; set;}
    public String Price {get; set;}
    public String Category {get; set;}
    public String Shop {get; set;}
    public String Link {get; set;}
    public String Image {get; set;}

}

interface Scraper {
    int getNumberPages(ChromeDriver driver);
    void openurl(ChromeDriver driver, String url);
    void getItems(ChromeDriver driver, List<Item> returnedLinks);
    List<string> getCategories(ChromeDriver driver);

}


class General {
    static void Main(string[] args) {

        CountdownScraper scraper = new CountdownScraper();

        ChromeDriver driver = constructDriver();

        scraper.openurl(driver, "https://www.countdown.co.nz/");
        List<String> categories = scraper.getCategories(driver);
        List<Item> returnedItems = new List<Item>();

        Parallel.ForEach(categories, category => {
            using (ChromeDriver driver = constructDriver()) {
                String url = String.Format(category, 1);
                scraper.openurl(driver, url);
                int numItems = scraper.getNumberPages(driver);
                for (int i = 1; i <= Math.Min(1000, Math.Ceiling(numItems / 120.0)); i++) { // Min is used hear simpliy so that I can reduce the scope for small tests as opposed to doing a full scrape each time.
                    url = String.Format(category, i);
                    scraper.openurl(driver, url);
                    scraper.getItems(driver, returnedItems);
                }
            }
            
        });

        Export(returnedItems);
        

        driver.Quit();
    }

    static ChromeDriver constructDriver() {
        ChromeOptions chromeOptions = new ChromeOptions();
        //chromeOptions.AddArguments("--headless=new");
        chromeOptions.AddArgument("--disable-gpu");
        chromeOptions.AddArgument("--disable-extensions");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-software-rasterizer");
        chromeOptions.AddArgument("--disable-translate");
        chromeOptions.AddArgument("--disable-features=site-per-process");

        chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);
        var driver = new ChromeDriver(chromeOptions);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
        return driver;
    }

    static void Export(List<Item> items) {
        String filePath = "../data/items.csv";
        using (StreamWriter writer = new StreamWriter(filePath))
        using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(items);
        }

        Console.WriteLine("Data exported to {0}", filePath);
    }
}
class CountdownScraper : Scraper {
    public List<String> getCategories(ChromeDriver driver) {
        List<String> returnCategories = new List<String>();
        var categories = driver.FindElements(By.CssSelector("a.link.ng-star-inserted"));
        foreach (var category in categories) {
            String output = category.GetAttribute("href") + "?page={0}&size=120&inStockProductsOnly=false";
            returnCategories.Add(output);
            Console.WriteLine(output);
        }

        return returnCategories;
    }

    public int getNumberPages(ChromeDriver driver) {
        String NumberPages = driver.FindElement(By.CssSelector("span.totalItemsCount")).Text.Split(" ")[0];
        return Int32.Parse(NumberPages);
    }

    public void openurl(ChromeDriver driver, String url) {
        driver.Navigate().GoToUrl(url); 
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void getItems(ChromeDriver driver, List<Item> returnedItems) {

        String category = driver.FindElement(By.CssSelector("div.headingContainer")).FindElement(By.CssSelector("h1")).Text;
        var links = driver.FindElements(By.CssSelector("a.product-entry"));

        foreach (var link in links) {
            Item item = new Item();
            item.Shop = "Countdown";
            item.Category = category;
            
            

            item.Name = link.FindElement(By.CssSelector("h3")).Text;

            var divElement = link.FindElement(By.CssSelector("div.product-meta")).FindElement(By.CssSelector("product-price")).FindElement(By.CssSelector("h3"));
            item.Link = link.GetAttribute("href");

            item.Price = divElement.Text.Replace("\n", " ");

            String image = link.FindElement(By.CssSelector("img")).GetAttribute("src");
            item.Image = image;   

            returnedItems.Add(item);

        }
    }
}