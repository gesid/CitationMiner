using HtmlAgilityPack;
using ScrapingWashes.DTOs;
using ScrapingWashes.Enums;
using ScrapingWashes.Models;
using ScrapingWashes.Repository;
using System.Globalization;

namespace ScrapingWashes.Services
{
    public class ScrapingWashesService(BaseModelRepository<Edition> editionRepository,
        BaseModelRepository<Paper> paperRepository, BaseModelRepository<Author> authorRepository,
        BaseModelRepository<AuthorPaper> authorPaperRepository, IConfiguration configuration)
    {
        private readonly BaseModelRepository<Edition> _editionRepository = editionRepository;
        private readonly BaseModelRepository<Paper> _paperRepository = paperRepository;
        private readonly BaseModelRepository<Author> _authorRepository = authorRepository;
        private readonly BaseModelRepository<AuthorPaper> _authorPaperRepository = authorPaperRepository;
        private readonly IConfiguration _configuration = configuration;
        public List<ScrapingDTO> _allEditions = [];
        public List<ScrapingDTO> _detailsArticles = [];

        public HtmlWeb _driver = new();

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
                var document = _driver.Load(item.Link);

                item.Date = document.DocumentNode.SelectSingleNode("//div[@class='published']/span[@class='value']").InnerText.Trim();

                var date = DateTime.ParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var edition = await _editionRepository.AddOrUpdateAsync(new Edition
                {
                    Year = date.Year,
                    Title = item.Title,
                    Location = "",
                    Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                    Proceedings = item.Link,
                }, where: x => x.Title == item.Title || x.Proceedings == item.Link);

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

                var summary = document.DocumentNode.SelectSingleNode("//div[@class='item abstract']/p") ??
                    document.DocumentNode.SelectSingleNode("//div[@class='item abstract']");
                var keywords = document.DocumentNode.SelectSingleNode("//div[@class='item keywords']//span[@class='value']");
                var references = document.DocumentNode.SelectSingleNode("//div[@class='item references']//div[@class='value']");
                var citation = await TakeCitation(item.Title);
                var type = document.DocumentNode.SelectSingleNode("//nav[@class='cmp_breadcrumbs']//li[@class='current']");



                var paper = await _paperRepository.AddOrUpdateAsync(new Paper
                {
                    Title = item.Title,
                    Year = item.Year,
                    Abstract = null,
                    Summary = summary?.InnerText.Replace("Resumo", "").Trim(),
                    Keywords = keywords is not null ? string.Join(", ", keywords.InnerText.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))) : null,
                    Type = type is not null ? (type.InnerText.Trim() == "Posters" ? ETypePaper.Poster : type.InnerText.Trim() == "Artigos Curtos" ? ETypePaper.ShortPaper : type.InnerText.Trim() == "Artigos Completos" ? ETypePaper.FullPaper : null) : null,
                    Link = item.Link,
                    Citation = citation,
                    References = references?.InnerText.Trim(),
                    EditionId = item.EditionId,
                }, where: x => x.Title == item.Title);

                await SaveAutors(document, paper.PaperId);
            }
        }

        private async Task SaveAutors(HtmlDocument document, int paperId)
        {
            var authors = document.DocumentNode.SelectNodes("//*[@id=\"pkp_content_main\"]/div/article/div/div[1]/ul/li");
            foreach (var author in authors)
            {
                var name = author.SelectSingleNode(".//span[1]").InnerText.Trim();
                var instituition = author.SelectSingleNode(".//span[2]");

                var authorSaved = await _authorRepository.AddOrUpdateAsync(new Author
                {
                    Name = name,
                    State = "teste",
                    Instituition = instituition?.InnerText.Trim(),
                    PaperId = paperId
                }, where: x => x.Name == name && x.PaperId == paperId);

                await _authorPaperRepository.AddOrUpdateAsync(new AuthorPaper
                {
                    AuthorId = authorSaved.AuthorId,
                    PaperId = paperId
                }, where: x => x.AuthorId == authorSaved.AuthorId && x.PaperId == paperId);
            }
        }

        private async Task<string?> TakeCitation(string title)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", _configuration["Variables:Cookie"]);
            var response = await client.GetAsync("https://scholar.google.com/scholar?hl=pt-BR&as_sdt=0%2C5&q=" + title.Replace(" ", "+") + "&btnG=");

            var document = new HtmlDocument();
            document.LoadHtml(await response.Content.ReadAsStringAsync());

            var citar = document.DocumentNode.SelectSingleNode("/html/body/div/div[10]/div[2]/div[3]/div[2]/div");

            if (citar is not null)
            {
                response = await client.GetAsync("https://scholar.google.com/scholar?q=info:" + citar.GetAttributeValue("data-cid", "") + ":scholar.google.com/&output=cite&scirp=0&hl=pt-BR");
                document.LoadHtml(await response.Content.ReadAsStringAsync());
            }

            var citation = document.DocumentNode.SelectSingleNode("//div[@id='gs_citt']//th[contains(text(), 'APA')]/following-sibling::td//div");

            return citation is not null ? citation.InnerText.Trim() : null;
        }
    }
}
