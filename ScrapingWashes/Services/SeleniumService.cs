using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScrapingWashes.Models;
using System.Globalization;

namespace ScrapingWashes.Services
{
    public class SeleniumService
    {
        private readonly EditionService _editionService;
        public List<dynamic> _allEditions = [];
        public List<dynamic> detailsArticles = [];

        public ChromeDriver _driver = new();

        public SeleniumService(EditionService editionService)
        {
            _editionService = editionService;
        }

        public void Init()
        {

            _driver.Navigate().GoToUrl("https://sol.sbc.org.br/index.php/washes/issue/archive");

            // Take editions 
            var editions = _driver.FindElement(By.ClassName("issues_archive")).FindElements(By.ClassName("obj_issue_summary"));

            foreach (var item in editions)
            {
                _allEditions.Add(new
                {
                    Title = item.FindElement(By.ClassName("title")).Text.Substring(6),
                    Link = item.FindElement(By.TagName("a")).GetAttribute("href"),
                    Date = item.FindElement(By.CssSelector("div.published")).Text.Replace("Publicado: ", "")
                });
            }

            _allEditions.Reverse();

            SaveEditions();
        }

        private void SaveEditions()
        {
            foreach (var item in _allEditions)
            {
                _driver.Navigate().GoToUrl(item.Link);


                var articles = _driver.FindElement(By.ClassName("sections"));


                if (!articles.FindElement(By.TagName("h2")).Text.Contains("ARTIGOS"))
                {
                    continue;
                }

                foreach (var article in articles.FindElements(By.ClassName("obj_article_summary")))
                {
                    detailsArticles.Add(new
                    {
                        Title = article.Text,
                        Link = article.FindElement(By.TagName("a")).GetAttribute("href")
                    });
                }

                // save editions
                var date = DateTime.ParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                _editionService.AddEdition(new Edition
                {
                    Year = date.Year,
                    Title = item.Title,
                    Location = "",
                    Date = date,
                    Proceedings = item.Link,
                });
            }
        }

        private void SaveEEditions()
        {
            foreach (var item in _allEditions)
            {
                _driver.Navigate().GoToUrl(item.Link);


            }
        }
    }
}
