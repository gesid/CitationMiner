using HtmlAgilityPack;
using ScrapingWashes.DTOs;
using ScrapingWashes.Models;
using ScrapingWashes.Repository;
using System.Globalization;

namespace ScrapingWashes.Services
{
    public class ScrapingWashesService
    {
        private readonly BaseModelRepository<Edition> _editionRepository;
        private readonly BaseModelRepository<Paper> _paperRepository;
        private readonly BaseModelRepository<Author> _authorRepository;
        private readonly BaseModelRepository<AuthorPaper> _authorPaperRepository;
        public List<ScrapingDTO> _allEditions = [];
        public List<ScrapingDTO> _detailsArticles = [];

        public HtmlWeb _driver = new();

        public ScrapingWashesService(BaseModelRepository<Edition> editionRepository, BaseModelRepository<Paper> paperRepository, BaseModelRepository<Author> authorRepository, BaseModelRepository<AuthorPaper> authorPaperRepository)
        {
            _editionRepository = editionRepository;
            _paperRepository = paperRepository;
            _authorRepository = authorRepository;
            _authorPaperRepository = authorPaperRepository;
        }

        public async Task<bool> Init()
        {

            var document = _driver.Load("https://sol.sbc.org.br/index.php/washes/issue/archive");

            // Take editions 
            var editions = document.DocumentNode.SelectSingleNode("//*[@id=\"pkp_content_main\"]/div/ul").SelectNodes("//*[@id=\"pkp_content_main\"]/div/ul/li/div");

            foreach (var item in editions)
            {
                _allEditions.Add(new ScrapingDTO
                {
                    Title = item.SelectSingleNode(".//a").InnerText.Trim().Substring(6),
                    Link = item.SelectSingleNode(".//a").GetAttributeValue("href", "").Trim(),
                    Date = item.SelectSingleNode(".//div[1]/span[2]").InnerText.Trim()
                });
            }

            _allEditions.Reverse();

            await SaveEditions();
            await SavePapers();

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
                    Location = $"teste prod {DateTime.UtcNow}",
                    Date = date,
                    Proceedings = item.Link,
                }, where: x => x.Title == item.Title);

                var document = _driver.Load(item.Link);
                var articles = document.DocumentNode.SelectSingleNode("//*[@id=\"pkp_content_main\"]/div/div/div[2]").SelectNodes("//*[@id=\"pkp_content_main\"]/div/div/div[2]/div/ul/li");


                if (articles is null)
                {
                    continue;
                }

                foreach (var article in articles)
                {
                    _detailsArticles.Add(new ScrapingDTO
                    {
                        Title = article.SelectSingleNode(".//div[1]/div[1]/a").InnerText.Trim(),
                        Link = article.SelectSingleNode(".//div[1]/div[1]/a").GetAttributeValue("href", "").Trim(),
                        Year = edition.Year,
                        EditionId = edition.EditionId
                    });
                }
            }
        }

        private async Task SavePapers()
        {
            foreach (var item in _detailsArticles)
            {
                var document = _driver.Load(item.Link);

                var paper = await _paperRepository.AddOrUpdateAsync(new Paper
                {
                    Title = item.Title,
                    Year = item.Year,
                    Abstract = "",
                    Summary = "",
                    Keywords = "",
                    Type = 0,
                    Link = "",
                    Citation = "",
                    References = "",
                    EditionId = item.EditionId,
                }, where: x => x.Title == item.Title);

                await SaveAutors(document, paper.PaperId);
            }
        }

        private async Task SaveAutors(HtmlDocument document, int paperId)
        {
            var authors = document.DocumentNode.SelectNodes("//*[@id=\"pkp_content_main\"]/div/article/div/div[1]/ul");
            foreach (var author in authors)
            {
                var name = author.SelectSingleNode(".//li[1]/span[1]").InnerText.Trim();
                var instituition = author.SelectSingleNode(".//li[1]/span[2]");

                var authorSaved = await _authorRepository.AddOrUpdateAsync(new Author
                {
                    Name = name,
                    State = "teste",
                    Instituition = instituition is not null ? instituition.InnerText.Trim() : null,
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
