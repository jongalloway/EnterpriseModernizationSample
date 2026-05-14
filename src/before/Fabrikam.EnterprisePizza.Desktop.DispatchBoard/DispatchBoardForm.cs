using System.Windows.Forms;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DispatchBoardForm : Form
    {
        public DispatchBoardForm()
        {
            Text = "Dispatch Board - Store 014 (DockPanel Suite shell)";
            Width = 640;
            Height = 360;

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill
            };

            var coordinator = new DispatchCoordinator();
            foreach (var ticket in coordinator.GetActiveTickets("014"))
            {
                listBox.Items.Add(ticket.TicketId + " / " + ticket.DriverCode + " / " + ticket.RouteZone);
            }

            Controls.Add(listBox);
        }
    }
}
