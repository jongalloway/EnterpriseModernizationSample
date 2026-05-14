using System;
using System.Windows.Forms;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DispatchBoardForm());
        }
    }
}
