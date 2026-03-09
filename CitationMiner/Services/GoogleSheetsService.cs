using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace CitationMiner.Services
{
    public class GoogleSheetsService
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string _applicationName = "CitationMiner";
        private SheetsService? _sheetsService;
        private readonly string _spreadsheetId;

        public GoogleSheetsService(string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;
        }

        public async Task<SheetsService> GetServiceAsync()
        {
            if (_sheetsService != null)
                return _sheetsService;

            UserCredential credential;
            var credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CitationMiner");

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });

            return _sheetsService;
        }

        public async Task<IList<IList<object>>> ReadSheetAsync(string range)
        {
            var service = await GetServiceAsync();
            var request = service.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = await request.ExecuteAsync();
            return response.Values ?? new List<IList<object>>();
        }

        public async Task<List<string>> GetSheetNamesAsync()
        {
            var service = await GetServiceAsync();
            var spreadsheet = await service.Spreadsheets.Get(_spreadsheetId).ExecuteAsync();
            return spreadsheet.Sheets.Select(s => s.Properties.Title).ToList();
        }

        public async Task BatchUpdateCellsAsync(Dictionary<string, string> updates)
        {
            var service = await GetServiceAsync();
            var data = new List<ValueRange>();

            foreach (var update in updates)
            {
                data.Add(new ValueRange
                {
                    Range = update.Key,
                    Values = new List<IList<object>> { new List<object> { update.Value } }
                });
            }

            var batchUpdateRequest = new BatchUpdateValuesRequest
            {
                Data = data,
                ValueInputOption = "USER_ENTERED"
            };

            var request = service.Spreadsheets.Values.BatchUpdate(batchUpdateRequest, _spreadsheetId);
            await request.ExecuteAsync();
        }
    }
}
