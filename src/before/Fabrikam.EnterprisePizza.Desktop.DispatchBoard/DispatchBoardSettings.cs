namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DispatchBoardSettings
    {
        public const int DefaultAutoRefreshSeconds = 45;

        public const int MinimumAutoRefreshSeconds = 15;

        public const int MaximumAutoRefreshSeconds = 300;

        public string StoreNumber { get; set; }

        public string DispatchTerminalId { get; set; }

        public int AutoRefreshSeconds { get; set; }

        public bool IncludeDriverNotes { get; set; }
    }
}
