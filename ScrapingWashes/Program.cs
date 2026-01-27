using ScrapingWashes.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Get configuration
var configuration = app.Configuration;

// Your spreadsheet ID from configuration
var spreadsheetId = configuration["Logging:SpreadsheetId"] ?? "1VBYf5yWvbkUoTdCi9jX4LBteTpYg3EdSF8y20K1AwLQ";

// Sheet name from configuration
var sheetName = configuration["Logging:SheetName"] ?? "dataset";

Console.WriteLine("=== ScrapingWashes - Citation Filler ===");
Console.WriteLine($"Spreadsheet ID: {spreadsheetId}");
Console.WriteLine($"Sheet Name: {sheetName}");
Console.WriteLine();

try
{
    // Initialize services
    var sheetsService = new GoogleSheetsService(spreadsheetId);
    
    // Get available sheet names
    Console.WriteLine("Fetching available sheets...");
    var availableSheets = await sheetsService.GetSheetNamesAsync();
    
    Console.WriteLine("Available Tabs:");
    foreach (var sheet in availableSheets)
    {
        Console.WriteLine($" - {sheet}");
    }
    Console.WriteLine();

    // Check if configured sheet name exists
    if (!availableSheets.Contains(sheetName))
    {
        Console.WriteLine($"⚠️ WARNING: The configured sheet name '{sheetName}' was not found.");
        Console.WriteLine($"Defaulting to the first available sheet: '{availableSheets.First()}'");
        sheetName = availableSheets.First();
    }

    var citationFiller = new CitationFillerService(sheetsService, configuration, sheetName);

    // Run the citation filling process
    await citationFiller.FillMissingCitationsAsync();

    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}
catch (FileNotFoundException)
{
    Console.WriteLine("ERROR: credentials.json file not found!");
    Console.WriteLine("Please follow the setup instructions to create OAuth credentials.");
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine($"\nStack trace: {ex.StackTrace}");
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}