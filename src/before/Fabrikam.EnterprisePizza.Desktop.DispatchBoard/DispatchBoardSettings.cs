namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DispatchBoardSettings
    {
        public string StoreNumber { get; set; }

        public string DispatchTerminalId { get; set; }

        public int AutoRefreshSeconds { get; set; }

        public bool IncludeDriverNotes { get; set; }
    }
}
