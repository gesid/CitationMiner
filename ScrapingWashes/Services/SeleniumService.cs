using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScrapingWashes.DTOs;
using ScrapingWashes.Models;
using ScrapingWashes.Repository;
using System.Globalization;

namespace ScrapingWashes.Services
{
    public class SeleniumService
    {
        private readonly BaseModelRepository<Edition> _editionRepository;
        private readonly BaseModelRepository<Paper> _paperRepository;
        private readonly BaseModelRepository<Author> _authorRepository;
        private readonly BaseModelRepository<AuthorPaper> _authorPaperRepository;
        public List<ScrapingDTO> _allEditions = [];
        public List<ScrapingDTO> _detailsArticles = [];

        public ChromeDriver _driver = new();

        public SeleniumService(BaseModelRepository<Edition> editionRepository, BaseModelRepository<Paper> paperRepository, BaseModelRepository<Author> authorRepository, BaseModelRepository<AuthorPaper> authorPaperRepository)
        {
            _editionRepository = editionRepository;
            _paperRepository = paperRepository;
            _authorRepository = authorRepository;
            _authorPaperRepository = authorPaperRepository;
        }

        public async Task<bool> Init()
        {

            _driver.Navigate().GoToUrl("https://sol.sbc.org.br/index.php/washes/issue/archive");

            // Take editions 
            var editions = _driver.FindElement(By.ClassName("issues_archive")).FindElements(By.ClassName("obj_issue_summary"));

            foreach (var item in editions)
            {
                _allEditions.Add(new ScrapingDTO
                {
                    Title = item.FindElement(By.ClassName("title")).Text.Substring(6),
                    Link = item.FindElement(By.TagName("a")).GetAttribute("href"),
                    Date = item.FindElement(By.CssSelector("div.published")).Text.Replace("Publicado: ", "")
                });
            }

            _allEditions.Reverse();

            await SaveEditions();
            _driver.Quit();
            return true;
        }

        private async Task SaveEditions()
        {
            foreach (var item in _allEditions)
            {
                // save editions
                var date = DateTime.ParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var edition = await _editionRepository.AddOrUpdateAsync(new Edition
                {
                    Year = date.Year,
                    Title = item.Title,
                    Location = "teste",
                    Date = date,
                    Proceedings = item.Link,
                }, where: x => x.Title == item.Title);

                _driver.Navigate().GoToUrl(item.Link);

                var articles = _driver.FindElement(By.ClassName("sections"));

                if (!articles.FindElement(By.TagName("h2")).Text.Contains("ARTIGOS"))
                {
                    continue;
                }

                foreach (var article in articles.FindElements(By.ClassName("obj_article_summary")))
                {
                    _detailsArticles.Add(new ScrapingDTO
                    {
                        Title = article.Text,
                        Link = article.FindElement(By.TagName("a")).GetAttribute("href"),
                        Year = edition.Year.ToString()
                    });
                }

                await SavePapers(edition.EditionId);
            }
        }

        private async Task SavePapers(int editionId)
        {
            foreach (var item in _detailsArticles)
            {
                _driver.Navigate().GoToUrl(item.Link);

                var paper = await _paperRepository.AddOrUpdateAsync(new Paper
                {
                    Title = item.Title,
                    Year = int.Parse(item.Year),
                    Abstract = "",
                    Summary = "",
                    Keywords = "",
                    Type = 0,
                    Link = "",
                    Citation = "",
                    References = "",
                    EditionId = editionId,
                }, where: x => x.Title == item.Title);

                await SaveAutors(paper.PaperId);
            }
        }

        private async Task SaveAutors(int paperId)
        {
            var authors = _driver.FindElement(By.ClassName("authors")).FindElements(By.TagName("span"));
            foreach (var author in authors)
            {
                var name = _driver.FindElement(By.ClassName("name")).Text;
                var authorSaved = await _authorRepository.AddOrUpdateAsync(new Author
                {
                    Name = name,
                    State = "teste",
                    Instituition = _driver.FindElement(By.ClassName("affiliation")).Text,
                    PaperId = paperId
                }, where: x => x.Name == name && x.PaperId == paperId);

                await _authorPaperRepository.AddOrUpdateAsync(new AuthorPaper
                {
                    AuthorId = authorSaved.AuthorId,
                    PaperId = paperId
                }, where: x => x.AuthorId == authorSaved.AuthorId && x.PaperId == paperId);
            }
        }
    }
}
