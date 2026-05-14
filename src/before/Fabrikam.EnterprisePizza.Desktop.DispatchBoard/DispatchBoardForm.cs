using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DispatchBoardForm : Form
    {
        private readonly DispatchCoordinator _dispatchCoordinator;
        private readonly StoreOperationsWorkbenchService _workbenchService;
        private readonly Timer _refreshTimer;
        private readonly DataGridView _ticketGrid;
        private readonly ListBox _driverNotesListBox;
        private readonly ListView _summaryListView;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripStatusLabel _refreshLabel;
        private readonly BindingList<DispatchTicketRow> _ticketRows;
        private ToolStripTextBox _storeNumberTextBox;
        private ToolStripLabel _terminalLabel;
        private DispatchBoardSettings _settings;

        public DispatchBoardForm()
        {
            _dispatchCoordinator = new DispatchCoordinator();
            _workbenchService = new StoreOperationsWorkbenchService(_dispatchCoordinator);
            _settings = DispatchBoardAppSettings.Load();
            _ticketRows = new BindingList<DispatchTicketRow>();
            _refreshTimer = new Timer();
            _refreshTimer.Tick += RefreshTimerTick;

            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(960, 640);
            Width = 1140;
            Height = 760;
            Text = "Fabrikam Enterprise Pizza Dispatch Board";

            var menuStrip = BuildMenu();
            var toolStrip = BuildToolStrip();
            _ticketGrid = BuildTicketGrid();
            _summaryListView = BuildSummaryListView();
            _driverNotesListBox = BuildDriverNotesListBox();

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2,
                SplitterDistance = 730
            };
            splitContainer.Panel1.Controls.Add(_ticketGrid);
            splitContainer.Panel2.Controls.Add(BuildSidebar());

            _statusLabel = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _refreshLabel = new ToolStripStatusLabel
            {
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip = new StatusStrip();
            _statusStrip.Items.Add(_statusLabel);
            _statusStrip.Items.Add(_refreshLabel);

            Controls.Add(splitContainer);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            Controls.Add(_statusStrip);

            MainMenuStrip = menuStrip;

            ApplySettingsToShell();
            LoadDispatchBoard("Initial desktop load");
        }

        private MenuStrip BuildMenu()
        {
            var menuStrip = new MenuStrip();

            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add(BuildMenuItem("&Refresh Board", delegate { LoadDispatchBoard("Manual refresh"); }, Keys.F5));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(BuildMenuItem("E&xit", delegate { Close(); }, Keys.Alt | Keys.F4));

            var workspaceMenu = new ToolStripMenuItem("&Workspace");
            workspaceMenu.DropDownItems.Add(BuildMenuItem("&Order Lookup...", OpenOrderLookup, Keys.Control | Keys.L));
            workspaceMenu.DropDownItems.Add(BuildMenuItem("&Store Management...", OpenStoreManagement, Keys.Control | Keys.M));
            workspaceMenu.DropDownItems.Add(new ToolStripSeparator());
            workspaceMenu.DropDownItems.Add(BuildMenuItem("Dispatch &Options...", OpenSettingsDialog, Keys.Control | Keys.O));

            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add(BuildMenuItem("&About", ShowAboutDialog, Keys.None));

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(workspaceMenu);
            menuStrip.Items.Add(helpMenu);
            return menuStrip;
        }

        private ToolStrip BuildToolStrip()
        {
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            toolStrip.Items.Add(new ToolStripButton("Refresh", null, delegate { LoadDispatchBoard("Toolbar refresh"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Store"));

            _storeNumberTextBox = new ToolStripTextBox
            {
                AutoSize = false,
                CharacterCasing = CharacterCasing.Upper,
                Text = _settings.StoreNumber,
                Width = 58
            };
            _storeNumberTextBox.KeyDown += StoreNumberTextBoxKeyDown;
            toolStrip.Items.Add(_storeNumberTextBox);

            toolStrip.Items.Add(new ToolStripButton("Apply", null, ApplyStoreNumber));
            toolStrip.Items.Add(new ToolStripSeparator());

            _terminalLabel = new ToolStripLabel();
            toolStrip.Items.Add(_terminalLabel);

            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Orders", null, OpenOrderLookup));
            toolStrip.Items.Add(new ToolStripButton("Stores", null, OpenStoreManagement));
            toolStrip.Items.Add(new ToolStripButton("Options", null, OpenSettingsDialog));

            return toolStrip;
        }

        private DataGridView BuildTicketGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D,
                DataSource = _ticketRows
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ticket",
                DataPropertyName = "TicketId",
                Width = 70
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Driver",
                DataPropertyName = "DriverCode",
                Width = 85
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Route Zone",
                DataPropertyName = "RouteZone",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Wave",
                DataPropertyName = "DispatchWave",
                Width = 85
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ETA",
                DataPropertyName = "QuotedEta",
                Width = 65
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "BoardStatus",
                Width = 115
            });

            grid.SelectionChanged += TicketGridSelectionChanged;
            return grid;
        }

        private Control BuildSidebar()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                Padding = new Padding(6)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 205F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var summaryGroup = new GroupBox
            {
                Text = "Terminal Summary",
                Dock = DockStyle.Fill
            };
            summaryGroup.Controls.Add(_summaryListView);

            var notesGroup = new GroupBox
            {
                Text = "Driver Notes / Route Reminders",
                Dock = DockStyle.Fill
            };
            notesGroup.Controls.Add(_driverNotesListBox);

            layout.Controls.Add(summaryGroup, 0, 0);
            layout.Controls.Add(notesGroup, 0, 1);
            return layout;
        }

        private ListView BuildSummaryListView()
        {
            var listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                FullRowSelect = true,
                GridLines = true
            };

            listView.Columns.Add("Metric", 135);
            listView.Columns.Add("Value", 120);
            return listView;
        }

        private ListBox BuildDriverNotesListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
        }

        private void ApplyStoreNumber(object sender, EventArgs e)
        {
            _settings.StoreNumber = NormalizeStoreNumber(_storeNumberTextBox.Text);
            LoadDispatchBoard("Store changed");
        }

        private void StoreNumberTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ApplyStoreNumber(sender, e);
            }
        }

        private void OpenOrderLookup(object sender, EventArgs e)
        {
            using (var dialog = new OrderLookupForm(_settings.StoreNumber, _settings.DispatchTerminalId, _workbenchService))
            {
                dialog.ShowDialog(this);
            }
        }

        private void OpenStoreManagement(object sender, EventArgs e)
        {
            using (var dialog = new StoreManagementForm(_settings.StoreNumber, _settings.DispatchTerminalId, _workbenchService))
            {
                dialog.ShowDialog(this);
            }
        }

        private void OpenSettingsDialog(object sender, EventArgs e)
        {
            using (var dialog = new DispatchBoardSettingsForm(_settings))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                _settings = dialog.Settings;
                _settings.StoreNumber = NormalizeStoreNumber(_settings.StoreNumber);
                _settings.DispatchTerminalId = NormalizeTerminalId(_settings.DispatchTerminalId);
                DispatchBoardAppSettings.Save(_settings);
                ApplySettingsToShell();
                LoadDispatchBoard("Options updated");
            }
        }

        private void ShowAboutDialog(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Fabrikam Enterprise Pizza Dispatch Board\r\nLegacy desktop shell for dispatch, order lookup, and store operations follow-up.",
                "About Dispatch Board",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void LoadDispatchBoard(string refreshReason)
        {
            var storeNumber = NormalizeStoreNumber(_settings.StoreNumber);
            _settings.StoreNumber = storeNumber;
            var tickets = LoadTicketsForCurrentStore();
            var ticketRows = tickets.Select(CreateRow).ToList();

            _ticketRows.RaiseListChangedEvents = false;
            _ticketRows.Clear();
            foreach (var row in ticketRows)
            {
                _ticketRows.Add(row);
            }

            _ticketRows.RaiseListChangedEvents = true;
            _ticketRows.ResetBindings();

            PopulateSummary(ticketRows);
            PopulateDriverNotes(ticketRows);

            _statusLabel.Text = string.Format("Store {0} board refreshed ({1}).", storeNumber, refreshReason);
            _refreshLabel.Text = "Last refresh " + DateTime.Now.ToString("g");

            if (_ticketGrid.Rows.Count > 0)
            {
                _ticketGrid.Rows[0].Selected = true;
            }
        }

        private IList<DispatchTicket> LoadTicketsForCurrentStore()
        {
            return _dispatchCoordinator.GetActiveTickets(NormalizeStoreNumber(_settings.StoreNumber)) ?? new List<DispatchTicket>();
        }

        private void PopulateSummary(IList<DispatchTicketRow> ticketRows)
        {
            var lookupOrders = _workbenchService.GetOrderLookupRecords(_settings.StoreNumber);
            var visibleHoldCount = ticketRows.Count(row => row.BoardStatus == "Counter Hold");

            _summaryListView.BeginUpdate();
            _summaryListView.Items.Clear();
            AddSummaryItem("Store", _settings.StoreNumber);
            AddSummaryItem("Terminal", _settings.DispatchTerminalId);
            AddSummaryItem("Auto Refresh", _settings.AutoRefreshSeconds + " sec");
            AddSummaryItem("Active Tickets", ticketRows.Count.ToString());
            AddSummaryItem("Lookup Orders", lookupOrders.Count.ToString());
            AddSummaryItem("Counter Holds", visibleHoldCount.ToString());
            AddSummaryItem("Operator", Environment.UserName);
            _summaryListView.EndUpdate();
        }

        private void PopulateDriverNotes(IList<DispatchTicketRow> ticketRows)
        {
            _driverNotesListBox.BeginUpdate();
            _driverNotesListBox.Items.Clear();

            if (!_settings.IncludeDriverNotes)
            {
                _driverNotesListBox.Items.Add("Driver notes are hidden for this terminal.");
                _driverNotesListBox.EndUpdate();
                return;
            }

            if (ticketRows.Count == 0)
            {
                _driverNotesListBox.Items.Add("No active runs on the board.");
                _driverNotesListBox.EndUpdate();
                return;
            }

            foreach (var row in ticketRows)
            {
                _driverNotesListBox.Items.Add(string.Format("{0}: {1}", row.DriverCode, row.DriverReminder));
            }

            _driverNotesListBox.Items.Add("Order Lookup tracks counter callbacks and carryout promise times.");
            _driverNotesListBox.Items.Add("Store Management keeps terminal drift and staffing issues on one desk.");
            _driverNotesListBox.EndUpdate();
        }

        private void TicketGridSelectionChanged(object sender, EventArgs e)
        {
            if (_ticketGrid.CurrentRow == null)
            {
                return;
            }

            var row = _ticketGrid.CurrentRow.DataBoundItem as DispatchTicketRow;
            if (row == null)
            {
                return;
            }

            _statusLabel.Text = string.Format(
                "Ticket {0} assigned to {1} covering {2}. {3}",
                row.TicketId,
                row.DriverCode,
                row.RouteZone,
                row.BoardStatus);
        }

        private void ApplySettingsToShell()
        {
            _settings.StoreNumber = NormalizeStoreNumber(_settings.StoreNumber);
            _settings.DispatchTerminalId = NormalizeTerminalId(_settings.DispatchTerminalId);
            _storeNumberTextBox.Text = _settings.StoreNumber;
            _terminalLabel.Text = "Terminal " + _settings.DispatchTerminalId;
            _refreshTimer.Interval = _settings.AutoRefreshSeconds * 1000;
            _refreshTimer.Enabled = true;
            Text = string.Format(
                "Fabrikam Enterprise Pizza Dispatch Board - Store {0} [{1}]",
                _settings.StoreNumber,
                _settings.DispatchTerminalId);
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            LoadDispatchBoard("Auto refresh");
        }

        private static DispatchTicketRow CreateRow(DispatchTicket ticket)
        {
            var waveNumber = Math.Abs(ticket.TicketId % 3) + 1;
            var etaMinutes = 18 + (ticket.TicketId % 4) * 4;
            var holdStatus = ticket.TicketId % 2 == 0 ? "Counter Hold" : "Out for Run";

            return new DispatchTicketRow
            {
                TicketId = ticket.TicketId,
                DriverCode = ticket.DriverCode,
                RouteZone = ticket.RouteZone,
                DispatchWave = "Wave " + waveNumber,
                QuotedEta = etaMinutes + " min",
                BoardStatus = holdStatus,
                DriverReminder = holdStatus == "Counter Hold"
                    ? "Confirm breadsticks and callback slip before sending."
                    : "Carry extra ranch cups for pickup crossover."
            };
        }

        private static ToolStripMenuItem BuildMenuItem(string text, EventHandler handler, Keys shortcutKeys)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += handler;

            if (shortcutKeys != Keys.None)
            {
                item.ShortcutKeys = shortcutKeys;
            }

            return item;
        }

        private void AddSummaryItem(string label, string value)
        {
            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            _summaryListView.Items.Add(item);
        }

        private static string NormalizeStoreNumber(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "014" : value.Trim().ToUpperInvariant();
        }

        private static string NormalizeTerminalId(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "TERM-02" : value.Trim().ToUpperInvariant();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refreshTimer.Dispose();
            base.OnFormClosed(e);
        }

        private sealed class DispatchTicketRow
        {
            public int TicketId { get; set; }

            public string DriverCode { get; set; }

            public string RouteZone { get; set; }

            public string DispatchWave { get; set; }

            public string QuotedEta { get; set; }

            public string BoardStatus { get; set; }

            public string DriverReminder { get; set; }
        }
    }
}
