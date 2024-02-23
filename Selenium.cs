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


class Item {
    public String Name {get; set;}
    public String Price {get; set;}
    public String Link {get; set;}
    public String Shop {get; set;}
}

interface Scraper {
    int getNumberPages(ChromeDriver driver, String url);
    void getItems(ChromeDriver driver, List<Item> returnedLinks);
    void Export(List<Item> items);
}


class General {
    static void Main(string[] args) {

        CountdownScraper scraper = new CountdownScraper();

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
         driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        int num_pages = scraper.getNumberPages(driver, "https://www.countdown.co.nz/shop/browse/frozen?page=1&size=120&inStockProductsOnly=false");
        Console.WriteLine("{0}", num_pages);
        List<Item> returnedItems = new List<Item>();
        for (int i = 1; i <= 2; i++) { //System.Math.Ceiling(num_pages / 120.0)
            String link = String.Format("https://www.countdown.co.nz/shop/browse/frozen?page={0}&size=120&inStockProductsOnly=false", i);
            scraper.openurl(driver, link);
            scraper.getItems(driver, returnedItems);
        }

        foreach (Item item in returnedItems) {
            Console.WriteLine("{0}, {1}", item.Name, item.Price);
        }
        scraper.Export(returnedItems);
        


        driver.Quit();
    }
}
class CountdownScraper : Scraper {


    public int getNumberPages(ChromeDriver driver, String url) {
        driver.Navigate().GoToUrl(url);

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));
        wait.Until(c => c.FindElement(By.CssSelector("span.totalItemsCount")));
        String NumberPages = driver.FindElement(By.CssSelector("span.totalItemsCount")).Text.Split(" ")[0];
        return Int32.Parse(NumberPages);
    }

    public void openurl(ChromeDriver driver, String url) {
        driver.Navigate().GoToUrl(url); 
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(c => c.FindElement(By.CssSelector("span.totalItemsCount"))); //waits untill the whole page loads.
    }

    public void getItems(ChromeDriver driver, List<Item> returnedItems) {

        var links = driver.FindElements(By.CssSelector("a.product-entry"));

        foreach (var link in links) {
            Item item = new Item();
            IWebElement divElement = link.FindElement(By.CssSelector("h3"));
            item.Name = divElement.Text;

            divElement = link.FindElement(By.CssSelector("div.product-meta")).FindElement(By.CssSelector("product-price")).FindElement(By.CssSelector("h3"));
            item.Link = link.GetAttribute("href");
            var price = divElement.Text.Split("\n");
            item.Price = price[1] + "." + price[2];
            item.Shop = "Countdown";
            returnedItems.Add(item);

        }
    }

    public void Export(List<Item> items) {
        String filePath = "items.csv";
        using (StreamWriter writer = new StreamWriter(filePath))
        using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(items);
        }

        Console.WriteLine("Data exported to {filePath}");
    }
}