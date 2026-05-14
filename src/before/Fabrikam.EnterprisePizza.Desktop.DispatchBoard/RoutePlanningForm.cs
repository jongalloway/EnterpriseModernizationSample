using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class RoutePlanningForm : Form
    {
        private readonly Func<IList<DispatchTicket>> _ticketProvider;
        private readonly string _storeNumber;
        private readonly string _terminalId;
        private readonly BindingList<RoutePlanRow> _routeRows;
        private readonly BindingList<RouteStopRow> _routeStopRows;
        private readonly DataGridView _routeGrid;
        private readonly DataGridView _stopGrid;
        private readonly ListView _summaryListView;
        private readonly ListBox _plannerNotesListBox;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripStatusLabel _refreshLabel;

        public RoutePlanningForm(string storeNumber, string terminalId, Func<IList<DispatchTicket>> ticketProvider)
        {
            if (ticketProvider == null)
            {
                throw new ArgumentNullException(nameof(ticketProvider));
            }

            _ticketProvider = ticketProvider;
            _storeNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            _terminalId = string.IsNullOrWhiteSpace(terminalId) ? "TERM-02" : terminalId.Trim().ToUpperInvariant();
            _routeRows = new BindingList<RoutePlanRow>();
            _routeStopRows = new BindingList<RouteStopRow>();

            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(940, 620);
            Width = 1080;
            Height = 700;
            Text = string.Format("Route Planning - Store {0} [{1}]", _storeNumber, _terminalId);

            var toolStrip = BuildToolStrip();
            _routeGrid = BuildRouteGrid();
            _stopGrid = BuildStopGrid();
            _summaryListView = BuildSummaryListView();
            _plannerNotesListBox = BuildPlannerNotesListBox();

            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2,
                SplitterDistance = 690
            };
            mainSplit.Panel1.Controls.Add(BuildPlannerWorkspace());
            mainSplit.Panel2.Controls.Add(BuildSidebar());

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

            Controls.Add(mainSplit);
            Controls.Add(toolStrip);
            Controls.Add(_statusStrip);

            LoadRoutePlans("Route desk opened");
        }

        private ToolStrip BuildToolStrip()
        {
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            toolStrip.Items.Add(new ToolStripButton("Refresh", null, delegate { LoadRoutePlans("Planner refresh"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Store " + _storeNumber));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Terminal " + _terminalId));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Print Worksheet", null, PrintWorksheet));
            toolStrip.Items.Add(new ToolStripButton("Close", null, delegate { Close(); }));

            return toolStrip;
        }

        private Control BuildPlannerWorkspace()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 290
            };

            var routesGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Route Waves"
            };
            routesGroup.Controls.Add(_routeGrid);

            var stopsGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Selected Route Stops"
            };
            stopsGroup.Controls.Add(_stopGrid);

            split.Panel1.Controls.Add(routesGroup);
            split.Panel2.Controls.Add(stopsGroup);
            return split;
        }

        private Control BuildSidebar()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                Padding = new Padding(6)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var summaryGroup = new GroupBox
            {
                Text = "Planner Summary",
                Dock = DockStyle.Fill
            };
            summaryGroup.Controls.Add(_summaryListView);

            var notesGroup = new GroupBox
            {
                Text = "Planner Notes",
                Dock = DockStyle.Fill
            };
            notesGroup.Controls.Add(_plannerNotesListBox);

            layout.Controls.Add(summaryGroup, 0, 0);
            layout.Controls.Add(notesGroup, 0, 1);

            return layout;
        }

        private DataGridView BuildRouteGrid()
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
                DataSource = _routeRows
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Wave",
                DataPropertyName = "DispatchWave",
                Width = 72
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Route Zone",
                DataPropertyName = "RouteZone",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tickets",
                DataPropertyName = "TicketCount",
                Width = 60
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Drivers",
                DataPropertyName = "DriverCount",
                Width = 60
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Mileage",
                DataPropertyName = "EstimatedMileage",
                Width = 74
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ready By",
                DataPropertyName = "ReadyWindow",
                Width = 86
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Planner Note",
                DataPropertyName = "PlannerNote",
                Width = 160
            });

            grid.SelectionChanged += RouteGridSelectionChanged;
            return grid;
        }

        private DataGridView BuildStopGrid()
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
                DataSource = _routeStopRows
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ticket",
                DataPropertyName = "TicketId",
                Width = 66
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Driver",
                DataPropertyName = "DriverCode",
                Width = 82
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Area",
                DataPropertyName = "CustomerArea",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ETA",
                DataPropertyName = "QuotedEta",
                Width = 62
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Load Status",
                DataPropertyName = "LoadStatus",
                Width = 92
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Reminder",
                DataPropertyName = "Reminder",
                Width = 180
            });

            return grid;
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

            listView.Columns.Add("Metric", 130);
            listView.Columns.Add("Value", 120);
            return listView;
        }

        private ListBox BuildPlannerNotesListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
        }

        private void LoadRoutePlans(string refreshReason)
        {
            var tickets = _ticketProvider() ?? new List<DispatchTicket>();
            var routePlans = tickets
                .GroupBy(ticket => ticket.RouteZone)
                .Select(group => CreateRoutePlanRow(group.Key, group.ToList()))
                .OrderBy(row => row.DispatchWave)
                .ThenBy(row => row.RouteZone)
                .ToList();

            _routeRows.RaiseListChangedEvents = false;
            _routeRows.Clear();
            foreach (var routePlan in routePlans)
            {
                _routeRows.Add(routePlan);
            }

            _routeRows.RaiseListChangedEvents = true;
            _routeRows.ResetBindings();

            PopulateSummary(routePlans, tickets);
            PopulatePlannerNotes(routePlans, tickets);

            if (_routeGrid.Rows.Count > 0)
            {
                _routeGrid.Rows[0].Selected = true;
            }
            else
            {
                PopulateStops(null);
            }

            _statusLabel.Text = string.Format("Route planning refreshed for store {0} ({1}).", _storeNumber, refreshReason);
            _refreshLabel.Text = "Last refresh " + DateTime.Now.ToString("g");
        }

        private void RouteGridSelectionChanged(object sender, EventArgs e)
        {
            var row = _routeGrid.CurrentRow == null ? null : _routeGrid.CurrentRow.DataBoundItem as RoutePlanRow;
            PopulateStops(row);
            if (row == null)
            {
                return;
            }

            _statusLabel.Text = string.Format(
                "Wave {0} covers {1} with {2} tickets and {3} drivers.",
                row.DispatchWave,
                row.RouteZone,
                row.TicketCount,
                row.DriverCount);
        }

        private void PopulateStops(RoutePlanRow routePlan)
        {
            _routeStopRows.RaiseListChangedEvents = false;
            _routeStopRows.Clear();

            if (routePlan != null)
            {
                foreach (var stop in routePlan.Stops)
                {
                    _routeStopRows.Add(stop);
                }
            }

            _routeStopRows.RaiseListChangedEvents = true;
            _routeStopRows.ResetBindings();
        }

        private void PopulateSummary(IList<RoutePlanRow> routePlans, IList<DispatchTicket> tickets)
        {
            _summaryListView.BeginUpdate();
            _summaryListView.Items.Clear();

            var lateWaveCount = routePlans.Count(row => row.DispatchWave == "Wave 3");
            var crossoverCount = tickets.Count(ticket => ticket.TicketId % 2 == 0);

            AddSummaryItem("Store", _storeNumber);
            AddSummaryItem("Terminal", _terminalId);
            AddSummaryItem("Route Waves", routePlans.Count.ToString());
            AddSummaryItem("Tickets", tickets.Count.ToString());
            AddSummaryItem("Crossovers", crossoverCount.ToString());
            AddSummaryItem("Late Waves", lateWaveCount.ToString());
            AddSummaryItem("Planner", Environment.UserName);

            _summaryListView.EndUpdate();
        }

        private void PopulatePlannerNotes(IList<RoutePlanRow> routePlans, IList<DispatchTicket> tickets)
        {
            _plannerNotesListBox.BeginUpdate();
            _plannerNotesListBox.Items.Clear();

            if (routePlans.Count == 0)
            {
                _plannerNotesListBox.Items.Add("No active routes are waiting on the desk.");
                _plannerNotesListBox.EndUpdate();
                return;
            }

            foreach (var routePlan in routePlans)
            {
                _plannerNotesListBox.Items.Add(
                    string.Format(
                        "{0}: hold {1} dispatch to keep {2} together.",
                        routePlan.DispatchWave,
                        routePlan.DriverCount > 1 ? "front half of" : "last ticket in",
                        routePlan.RouteZone));
            }

            if (tickets.Count(ticket => ticket.TicketId % 2 == 0) > 0)
            {
                _plannerNotesListBox.Items.Add("Counter hold tickets should stay near the hot rack until the matching wave leaves.");
            }

            _plannerNotesListBox.EndUpdate();
        }

        private void PrintWorksheet(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Legacy worksheet output is staged for the back-office printer queue.",
                "Route Worksheet",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private RoutePlanRow CreateRoutePlanRow(string routeZone, IList<DispatchTicket> tickets)
        {
            var firstTicket = tickets.OrderBy(ticket => ticket.TicketId).First();
            var driverCount = tickets.Select(ticket => ticket.DriverCode).Distinct().Count();
            var waveNumber = Math.Abs(firstTicket.TicketId % 3) + 1;
            var mileage = 4 + tickets.Count * 3 + waveNumber;
            var readyMinute = 10 + (waveNumber * 6);
            var stops = tickets
                .OrderBy(ticket => ticket.TicketId)
                .Select(ticket => new RouteStopRow
                {
                    TicketId = ticket.TicketId,
                    DriverCode = ticket.DriverCode,
                    CustomerArea = ticket.RouteZone,
                    QuotedEta = (18 + (ticket.TicketId % 4) * 4) + " min",
                    LoadStatus = ticket.TicketId % 2 == 0 ? "Hold for Pair" : "Stage Now",
                    Reminder = ticket.TicketId % 2 == 0
                        ? "Pair with the lobby callback ticket before release."
                        : "Keep garlic butter cups with the insulated bag."
                })
                .ToList();

            return new RoutePlanRow
            {
                DispatchWave = "Wave " + waveNumber,
                RouteZone = routeZone,
                TicketCount = tickets.Count,
                DriverCount = driverCount,
                EstimatedMileage = mileage + " mi",
                ReadyWindow = "5:" + readyMinute.ToString("00"),
                PlannerNote = driverCount > 1 ? "Merge close stops before send." : "Single driver route.",
                Stops = stops
            };
        }

        private void AddSummaryItem(string label, string value)
        {
            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            _summaryListView.Items.Add(item);
        }

        private sealed class RoutePlanRow
        {
            public RoutePlanRow()
            {
                Stops = new List<RouteStopRow>();
            }

            public string DispatchWave { get; set; }

            public string RouteZone { get; set; }

            public int TicketCount { get; set; }

            public int DriverCount { get; set; }

            public string EstimatedMileage { get; set; }

            public string ReadyWindow { get; set; }

            public string PlannerNote { get; set; }

            public IList<RouteStopRow> Stops { get; set; }
        }

        private sealed class RouteStopRow
        {
            public int TicketId { get; set; }

            public string DriverCode { get; set; }

            public string CustomerArea { get; set; }

            public string QuotedEta { get; set; }

            public string LoadStatus { get; set; }

            public string Reminder { get; set; }
        }
    }
}
