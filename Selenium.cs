using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools.V120.Overlay;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using System.Security.Cryptography.X509Certificates;


class Item {
    public String Name {get; set;}
    public String Price {get; set;}
}

interface Scraper {
    int getNumberPages(ChromeDriver driver, String url);
    void getItemLinks(ChromeDriver driver, String url, List<String> returnedLinks);
}


class general {
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

        int num_pages = scraper.getNumberPages(driver, "https://www.countdown.co.nz/shop/products/lowprice?page=1&size=120&inStockProductsOnly=true");
        Console.WriteLine("{0}", num_pages);
        List<String> returnedLinks = new List<String>();
        for (int i = 1; i <= System.Math.Ceiling(num_pages / 120.0); i++) {
            String link = String.Format("https://www.countdown.co.nz/shop/products/lowprice?page={0}&size=120&inStockProductsOnly=true", i);
            scraper.getItemLinks(driver, link, returnedLinks);
        }

        foreach (String link in returnedLinks) {
            Console.WriteLine(link);
        }
        Console.WriteLine("{0}", returnedLinks.Count);
        


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

    public void getItemLinks(ChromeDriver driver, String url, List<String> returnLinks) {
        driver.Navigate().GoToUrl(url); 
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(c => c.FindElement(By.CssSelector("span.totalItemsCount"))); //waits untill the whole page loads.
        var links = driver.FindElements(By.CssSelector("a.product-entry"));

        foreach(var link in links) {
            returnLinks.Add(link.GetAttribute("href"));
        }
    }
}