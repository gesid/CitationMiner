# Google Sheets OAuth Setup Guide

## Prerequisites
Before running the application, you need to set up Google OAuth credentials.

## Step-by-Step Setup

### 1. Create a Google Cloud Project
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Click on the project dropdown at the top
3. Click "New Project"
4. Enter a project name (e.g., "ScrapingWashes")
5. Click "Create"

### 2. Enable Google Sheets API
1. In your project, go to "APIs & Services" > "Library"
2. Search for "Google Sheets API"
3. Click on it and press "Enable"

### 3. Create OAuth Credentials
1. Go to "APIs & Services" > "Credentials"
2. Click "Create Credentials" > "OAuth client ID"
3. If prompted, configure the OAuth consent screen:
   - Choose "External" user type
   - Fill in the required fields (App name, User support email, Developer contact)
   - Click "Save and Continue"
   - Skip the "Scopes" section (click "Save and Continue")
   - Add your email as a test user
   - Click "Save and Continue"
4. Back to creating OAuth client ID:
   - Application type: "Desktop app"
   - Name: "ScrapingWashes Desktop"
   - Click "Create"
5. Download the JSON file (click the download icon)
6. Rename it to `credentials.json`
7. **Place it in the same folder as the executable** (`ScrapingWashes\ScrapingWashes\`)

### 4. Update Configuration

#### Update Sheet Name
In `Program.cs`, update the sheet name to match your Google Sheets tab name:
```csharp
var sheetName = "Sheet1"; // Change this to your actual sheet tab name
```

#### Verify Spreadsheet ID
The spreadsheet ID is already configured:
```
1ogR7v57DApDMSz2lZfG5ajf6s7ekIer7BPjczNZaVSo
```

### 5. First Run
1. Run the application: `dotnet run`
2. A browser window will open asking you to sign in to Google
3. Sign in with your Google account
4. Grant the requested permissions
5. The application will save the authentication token for future runs

## Troubleshooting

### "credentials.json not found"
- Make sure the `credentials.json` file is in the `ScrapingWashes\ScrapingWashes\` directory

### "Access denied" or "403 Forbidden"
- Make sure you've added your email as a test user in the OAuth consent screen
- Make sure the Google Sheets API is enabled

### "Spreadsheet not found"
- Verify the spreadsheet ID in `Program.cs`
- Make sure you're signed in with the Google account that has access to the spreadsheet

## Column Mapping
- **Column C**: Paper titles (used for searching)
- **Column O**: Citations (will be filled if empty)

## Notes
- The application will skip rows where Column O already has data
- It processes in batches of 10 to avoid rate limiting
- There's a 2-second delay between batches to respect Google Scholar's rate limits
