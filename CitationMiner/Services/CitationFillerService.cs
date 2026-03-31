using Newtonsoft.Json.Linq;

namespace CitationMiner.Services
{
    public class CitationFillerService
    {
        private readonly GoogleSheetsService _sheetsService;
        private readonly IConfiguration _configuration;
        private readonly string _sheetName;

        public CitationFillerService(GoogleSheetsService sheetsService, IConfiguration configuration, string sheetName = "Sheet1")
        {
            _sheetsService = sheetsService;
            _configuration = configuration;
            _sheetName = sheetName;
        }

        public async Task FillMissingCitationsAsync()
        {
            Console.WriteLine("Starting citation filling process...");

            try
            {
                // Read all data from the sheet (starting from row 2 since row 1 is header)
                var range = $"'{_sheetName}'!A2:O";
                Console.WriteLine($"Reading range: {range}");
                var rows = await _sheetsService.ReadSheetAsync(range);

                if (rows == null || rows.Count == 0)
                {
                    Console.WriteLine("No data found in the spreadsheet.");
                    return;
                }

                await ProcessRowsAsync(rows);
            }
catch (Exception ex)
{
    Console.WriteLine($"\n❌ ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner error: {ex.InnerException.Message}");
    }
    
    // Save full error to file
    var errorLog = $"ERROR LOG - {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
    errorLog += $"Message: {ex.Message}\n\n";
    errorLog += $"Type: {ex.GetType().FullName}\n\n";
    if (ex.InnerException != null)
    {
        errorLog += $"Inner Exception: {ex.InnerException.Message}\n";
        errorLog += $"Inner Type: {ex.InnerException.GetType().FullName}\n\n";
    }
    errorLog += $"Stack Trace:\n{ex.StackTrace}\n\n";
    errorLog += $"Full Exception:\n{ex}\n";
    
    File.WriteAllText("error_details.txt", errorLog);
    Console.WriteLine("\n📝 Full error details saved to: error_details.txt");
    
    throw;
}
        }

        private async Task ProcessRowsAsync(IList<IList<object>> rows)
        {
            // TEST LIMIT: Set to 0 for unlimited, or a number to limit processing
            const int TEST_LIMIT = 0;

            var updates = new Dictionary<string, string>();
            int processedCount = 0;
            int filledCount = 0;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                int rowNumber = i + 2; // +2 because we start from row 2 (row 1 is header)

                // Column C (index 2) = Paper Title
                // Column O (index 14) = Citation
                
                string paperTitle = row.Count > 2 ? row[2]?.ToString() ?? "" : "";
                string existingCitation = row.Count > 14 ? row[14]?.ToString() ?? "" : "";

                // Skip if title is empty or citation already exists (and is not just "#")
                if (string.IsNullOrWhiteSpace(paperTitle))
                {
                    continue;
                }

                bool isCitationMissing = string.IsNullOrWhiteSpace(existingCitation) || existingCitation.Trim() == "#";

                if (!isCitationMissing)
                {
                    Console.WriteLine($"Row {rowNumber}: Citation already exists, skipping...");
                    continue;
                }

                // Fetch citation from Google Scholar
                Console.WriteLine($"Row {rowNumber}: Fetching citation for '{paperTitle}'...");
                var newCitations = await FetchCitationFromScholar(paperTitle);
                
                if (!string.IsNullOrWhiteSpace(newCitations))
                {
                    // Merge with existing citations (incremental update)
                    string finalCitations = newCitations;
                    
                    if (!string.IsNullOrWhiteSpace(existingCitation) && existingCitation.Trim() != "#")
                    {
                        // Parse existing citations (ACEITA PONTO-E-VÍRGULA E QUEBRA DE LINHA)
                        var existingTitles = existingCitation.Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .Where(t => !string.IsNullOrWhiteSpace(t))
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);
                        
                        // Parse new citations (ACEITA PONTO-E-VÍRGULA E QUEBRA DE LINHA)
                        var newTitles = newCitations.Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .Where(t => !string.IsNullOrWhiteSpace(t));
                        
                        // Add only new titles (avoid duplicates)
                        foreach (var newTitle in newTitles)
                        {
                            existingTitles.Add(newTitle);
                        }
                        
                        // Rebuild the citation string (SALVA SEMPRE COM QUEBRA DE LINHA)
                        finalCitations = string.Join("\n", existingTitles);
                    }
                    
                    // Add to batch update
                    var cellRange = $"'{_sheetName}'!O{rowNumber}";
                    updates[cellRange] = finalCitations;
                    filledCount++;
                    Console.WriteLine($"Row {rowNumber}: Citations updated!");
                }
                else
                {
                    Console.WriteLine($"Row {rowNumber}: No citations found.");
                }

                processedCount++;

                // TEST LIMIT: Stop after processing TEST_LIMIT papers
                if (TEST_LIMIT > 0 && processedCount >= TEST_LIMIT)
                {
                    Console.WriteLine($"\n⚠️ TEST MODE: Stopped after processing {TEST_LIMIT} paper(s).");
                    break;
                }

                // Batch update every 10 rows to avoid rate limiting
                if (updates.Count >= 10)
                {
                    await _sheetsService.BatchUpdateCellsAsync(updates);
                    Console.WriteLine($"Updated {updates.Count} citations in the spreadsheet.");
                    updates.Clear();
                    
                    // Add delay to avoid Google Scholar rate limiting
                    await Task.Delay(2000);
                }
            }

            // Update remaining citations
            if (updates.Count > 0)
            {
                await _sheetsService.BatchUpdateCellsAsync(updates);
                Console.WriteLine($"Updated {updates.Count} citations in the spreadsheet.");
            }

            Console.WriteLine($"\nProcess completed!");
            Console.WriteLine($"Total rows processed: {processedCount}");
            Console.WriteLine($"Citations filled: {filledCount}");
        }



        private async Task<string?> FetchCitationFromScholar(string title)
        {
            try
            {
                var serpApiKey = _configuration["SerpApi:ApiKey"];
                if (string.IsNullOrEmpty(serpApiKey) || serpApiKey == "YOUR_SERPAPI_KEY_HERE")
                {
                    Console.WriteLine("⚠️  ERROR: SerpApi Key is missing in appsettings.json!");
                    return null;
                }

                var client = new HttpClient();
                
                // STEP 1: Search for the paper to get citation metadata
                var searchUrl = $"https://serpapi.com/search.json?engine=google_scholar&q={Uri.EscapeDataString(title)}&hl=pt-BR&api_key={serpApiKey}";
                
                var response = await client.GetAsync(searchUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"   SerpApi Search Error: {response.StatusCode} - {content}");
                    return null;
                }

                var json = JObject.Parse(content);
                
                // Check if there are organic results
                var organicResults = json["organic_results"];
                if (organicResults == null || !organicResults.Any())
                {
                    Console.WriteLine("   No results found on SerpApi.");
                    return null;
                }

                // Get the first result (most relevant)
                var firstResult = organicResults[0];
                
                // STEP 2: Check if the paper has citations using inline_links.cited_by
                var citedBy = firstResult["inline_links"]?["cited_by"];
                
                if (citedBy == null)
                {
                    Console.WriteLine("   Paper has no citations (inline_links.cited_by not present).");
                    return "#"; // Explicitly mark as "no citations"
                }

                var totalCitations = citedBy["total"]?.Value<int>() ?? 0;
                
                if (totalCitations == 0)
                {
                    Console.WriteLine("   Paper has 0 citations.");
                    return "#";
                }

                var citesId = citedBy["cites_id"]?.ToString();
                
                if (string.IsNullOrEmpty(citesId))
                {
                    Console.WriteLine("   Could not retrieve cites_id from SerpApi.");
                    return null;
                }

                Console.WriteLine($"   Found {totalCitations} citations. Fetching citing papers...");

                // STEP 3: Fetch the papers that cite this paper
                var citingPaperTitles = new List<string>();
                var currentPage = 0;
                var maxPages = 5; // Limit to avoid excessive API calls (5 pages = ~50 papers)
                
                while (currentPage < maxPages)
                {
                    var citesUrl = $"https://serpapi.com/search.json?engine=google_scholar&cites={citesId}&start={currentPage * 10}&api_key={serpApiKey}";
                    
                    // Add delay to be respectful to the API
                    if (currentPage > 0)
                    {
                        await Task.Delay(500);
                    }

                    response = await client.GetAsync(citesUrl);
                    content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   SerpApi Cites Error (page {currentPage}): {response.StatusCode}");
                        break;
                    }

                    var citesJson = JObject.Parse(content);
                    var citingResults = citesJson["organic_results"];

                    if (citingResults == null || !citingResults.Any())
                    {
                        // No more results
                        break;
                    }

                    // Extract titles and authors from citing papers
                    foreach (var citingResult in citingResults)
                    {
                        var citingTitle = citingResult["title"]?.ToString();
                        if (string.IsNullOrWhiteSpace(citingTitle))
                            continue;

                        // Try to extract author information from publication_info.summary
                        // This typically contains author names in the format "Author1, Author2 - Source, Year"
                        var publicationInfo = citingResult["publication_info"]?["summary"]?.ToString();
                        string authorPart = "";

                        if (!string.IsNullOrWhiteSpace(publicationInfo))
                        {
                            // Extract the part before the dash (usually contains authors)
                            var dashIndex = publicationInfo.IndexOf(" - ");
                            if (dashIndex > 0)
                            {
                                authorPart = publicationInfo.Substring(0, dashIndex).Trim();
                            }
                            else
                            {
                                // If no dash, try to get first author from inline authors
                                var authors = citingResult["publication_info"]?["authors"];
                                if (authors != null && authors.Any())
                                {
                                    var firstAuthor = authors[0]?["name"]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(firstAuthor))
                                    {
                                        authorPart = firstAuthor;
                                    }
                                }
                            }
                        }

                        // Format: "Author. Title." or just "Title." if no author found
                        string formattedCitation;
                        if (!string.IsNullOrWhiteSpace(authorPart))
                        {
                            formattedCitation = $"{authorPart}. {citingTitle.Trim()}.";
                        }
                        else
                        {
                            formattedCitation = $"{citingTitle.Trim()}.";
                        }

                        citingPaperTitles.Add(formattedCitation);
                    }

                    // Check if there's a next page
                    var pagination = citesJson["serpapi_pagination"];
                    if (pagination?["next"] == null)
                    {
                        // No more pages
                        break;
                    }

                    currentPage++;
                }

                if (citingPaperTitles.Count == 0)
                {
                    Console.WriteLine("   No citing paper titles found.");
                    return "#";
                }

                // Join titles with semicolon separator
                var result = string.Join("\n", citingPaperTitles);
                Console.WriteLine($"   Found {citingPaperTitles.Count} citing papers.");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching citations: {ex.Message}");
                return null;
            }
        }
    }
}
