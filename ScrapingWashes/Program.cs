using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


var driver = new ChromeDriver();

driver.Navigate().GoToUrl("https://sol.sbc.org.br/index.php/washes/issue/archive");

// Take editions 
var editions = driver.FindElement(By.ClassName("issues_archive")).FindElements(By.ClassName("obj_issue_summary"));

var detailsEditions = new List<dynamic>();

try
{
    foreach (var item in editions)
    {
        detailsEditions.Add(new
        {
            Title = item.FindElement(By.ClassName("title")).Text,
            Link = item.FindElement(By.TagName("a")).GetAttribute("href"),
            Date = item.FindElement(By.CssSelector("div.published")).Text
        });
    }

    // open editions
    foreach (var item in detailsEditions)
    {
        driver.Navigate().GoToUrl(item.Link);

        var articles = driver.FindElement(By.ClassName("sections"));//.FindElements(By.ClassName("obj_article_summary"));
        

        if (!articles.FindElement(By.TagName("h2")).Text.Contains("ARTIGOS"))
        {
            continue;
        }

        var detailsArticles = new List<dynamic>();

        foreach (var article in articles.FindElements(By.ClassName("obj_article_summary")))
        {
            detailsArticles.Add(new
            {
                Title = article.Text,
                Link = article.FindElement(By.TagName("a")).GetAttribute("href")
            });
        }

        // take references
        foreach (var article in detailsArticles)
        {
            try
            {
                driver.Navigate().GoToUrl(article.Link);

                if (!driver.PageSource.Contains("Referências"))
                {
                    continue;
                }

                var references = driver.FindElement(By.ClassName("references")).FindElement(By.ClassName("value")).Text;


                Console.WriteLine($"Referecia: \n {article.Title}");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(references);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception)
            {

                throw;
            }
            

        }
    }
}
catch (Exception)
{

    throw;
} finally
{
    driver.Quit();
}

