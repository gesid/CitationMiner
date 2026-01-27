# Quick Start Guide - Citation Filler

## What This Does
This application automatically fills missing citations in your Google Spreadsheet by:
1. Reading paper titles from **Column C**
2. Checking if **Column O** (citations) is empty
3. Searching Google Scholar for the citation
4. Filling in the APA citation in **Column O**

## Before First Run

### 1. Get OAuth Credentials (One-time setup)
Follow the detailed instructions in `SETUP_OAUTH.md` to:
- Create a Google Cloud project
- Enable Google Sheets API
- Download `credentials.json`
- Place it in `ScrapingWashes\ScrapingWashes\` folder

### 2. Update Sheet Name
Open `Program.cs` and update the sheet name if needed:
```csharp
var sheetName = "Sheet1"; // Change to your actual sheet tab name
```

## How to Run

```bash
cd ScrapingWashes
dotnet run
```

### First Time Running
- A browser will open
- Sign in with your Google account
- Grant permissions
- The app will remember your login for future runs

### Subsequent Runs
- Just run `dotnet run`
- No browser login needed (token is saved)

## What to Expect

The app will:
- ✅ Skip rows where citations already exist
- ✅ Process in batches of 10 to avoid rate limiting
- ✅ Add 2-second delays between batches
- ✅ Show progress in the console
- ✅ Display how many citations were filled

## Troubleshooting

### "credentials.json not found"
- Make sure the file is in `ScrapingWashes\ScrapingWashes\` directory

### "Spreadsheet not found"
- Verify you're signed in with the correct Google account
- Check that the spreadsheet ID is correct in `Program.cs`

### No citations found
- Check your Google Scholar cookie in `appsettings.json`
- The cookie may have expired

## Configuration

### Spreadsheet ID
Already configured: `1ogR7v57DApDMSz2lZfG5ajf6s7ekIer7BPjczNZaVSo`

### Google Scholar Cookie
Located in `appsettings.json` under `Variables:Cookie`
- This helps avoid Google Scholar rate limiting
- Update if you get blocked

## Notes
- The app will NOT overwrite existing citations
- It processes row by row, starting from row 2 (row 1 is headers)
- Press any key to exit when done
