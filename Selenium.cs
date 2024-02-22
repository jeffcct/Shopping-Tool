using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools.V120.Overlay;
using CsvHelper.Configuration.Attributes;

class Item {
    public String Name {get; set;}
    public String Price {get; set;}
}

class Scraper {
    static void Main(string[] args) {
        ChromeOptions chromeOptions = new ChromeOptions();
        //chromeOptions.AddArguments("--headless=new");
        chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 0);
        var driver = new ChromeDriver(chromeOptions);

        int num_pages = getNumberPages(driver, "https://www.countdown.co.nz/shop/products/lowprice?page=1&inStockProductsOnly=true");
        Console.WriteLine("{0}", num_pages);
        List<String> returnedLinks = new List<String>();
        for (int i = 1; i <= 5; i++) {
            String link = String.Format("https://www.countdown.co.nz/shop/products/lowprice?page={0}&inStockProductsOnly=true", i);
            getItemLinks(driver, link, returnedLinks);
        }

        foreach (String link in returnedLinks) {
            Console.WriteLine(link);
        }
        Console.WriteLine("{0}", returnedLinks.Count);
        


        driver.Quit();
    }


    static int getNumberPages(ChromeDriver driver, String url) {
        driver.Navigate().GoToUrl(url);

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(c => c.FindElement(By.CssSelector("span.totalItemsCount")));
        String NumberPages = driver.FindElement(By.CssSelector("span.totalItemsCount")).Text.Split(" ")[0];
        return Int32.Parse(NumberPages);
    }

    static void getItemLinks(ChromeDriver driver, String url, List<String> returnLinks) {
        driver.Navigate().GoToUrl(url); 
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(c => c.FindElement(By.CssSelector("span.totalItemsCount"))); //waits untill the whole page loads.
        var links = driver.FindElements(By.CssSelector("a.product-entry"));

        foreach(var link in links) {
            returnLinks.Add(link.GetAttribute("href"));
        }
    }
}