using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fabrikam.EnterprisePizza.Shared.Contracts.Routing;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DriverAssignmentForm : Form
    {
        private readonly Func<IList<DispatchTicket>> _ticketProvider;
        private readonly string _storeNumber;
        private readonly string _terminalId;
        private readonly BindingList<DriverAssignmentRow> _driverRows;
        private readonly BindingList<PendingTicketRow> _pendingTicketRows;
        private readonly DataGridView _driverGrid;
        private readonly DataGridView _pendingTicketGrid;
        private readonly ListView _summaryListView;
        private readonly ListBox _assignmentNotesListBox;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripStatusLabel _refreshLabel;

        public DriverAssignmentForm(string storeNumber, string terminalId, Func<IList<DispatchTicket>> ticketProvider)
        {
            if (ticketProvider == null)
            {
                throw new ArgumentNullException(nameof(ticketProvider));
            }

            _ticketProvider = ticketProvider;
            _storeNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            _terminalId = string.IsNullOrWhiteSpace(terminalId) ? "TERM-02" : terminalId.Trim().ToUpperInvariant();
            _driverRows = new BindingList<DriverAssignmentRow>();
            _pendingTicketRows = new BindingList<PendingTicketRow>();

            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(940, 620);
            Width = 1060;
            Height = 690;
            Text = string.Format("Driver Assignments - Store {0} [{1}]", _storeNumber, _terminalId);

            var toolStrip = BuildToolStrip();
            _driverGrid = BuildDriverGrid();
            _pendingTicketGrid = BuildPendingTicketGrid();
            _summaryListView = BuildSummaryListView();
            _assignmentNotesListBox = BuildAssignmentNotesListBox();

            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2,
                SplitterDistance = 690
            };
            mainSplit.Panel1.Controls.Add(BuildWorkspace());
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

            LoadAssignments("Assignment desk opened");
        }

        private ToolStrip BuildToolStrip()
        {
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            toolStrip.Items.Add(new ToolStripButton("Refresh", null, delegate { LoadAssignments("Assignment refresh"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Store " + _storeNumber));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Terminal " + _terminalId));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Assign Highlighted", null, AssignHighlightedTicket));
            toolStrip.Items.Add(new ToolStripButton("Balance Board", null, BalanceBoard));
            toolStrip.Items.Add(new ToolStripButton("Close", null, delegate { Close(); }));

            return toolStrip;
        }

        private Control BuildWorkspace()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };

            var driversGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Drivers On Shift"
            };
            driversGroup.Controls.Add(_driverGrid);

            var pendingGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Pending Ticket Queue"
            };
            pendingGroup.Controls.Add(_pendingTicketGrid);

            split.Panel1.Controls.Add(driversGroup);
            split.Panel2.Controls.Add(pendingGroup);
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
                Text = "Assignment Summary",
                Dock = DockStyle.Fill
            };
            summaryGroup.Controls.Add(_summaryListView);

            var notesGroup = new GroupBox
            {
                Text = "Dispatcher Notes",
                Dock = DockStyle.Fill
            };
            notesGroup.Controls.Add(_assignmentNotesListBox);

            layout.Controls.Add(summaryGroup, 0, 0);
            layout.Controls.Add(notesGroup, 0, 1);
            return layout;
        }

        private DataGridView BuildDriverGrid()
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
                DataSource = _driverRows
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Driver",
                DataPropertyName = "DriverCode",
                Width = 80
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Home Zone",
                DataPropertyName = "HomeZone",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Runs",
                DataPropertyName = "CurrentRunCount",
                Width = 52
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Target",
                DataPropertyName = "SuggestedLoad",
                Width = 58
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Next Wave",
                DataPropertyName = "NextDispatchWave",
                Width = 78
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "ShiftStatus",
                Width = 88
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Last Drop",
                DataPropertyName = "LastDropTime",
                Width = 78
            });

            grid.SelectionChanged += DriverGridSelectionChanged;
            return grid;
        }

        private DataGridView BuildPendingTicketGrid()
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
                DataSource = _pendingTicketRows
            };

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ticket",
                DataPropertyName = "TicketId",
                Width = 68
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Route Zone",
                DataPropertyName = "RouteZone",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ETA",
                DataPropertyName = "QuotedEta",
                Width = 64
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Priority",
                DataPropertyName = "Priority",
                Width = 72
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Suggested Driver",
                DataPropertyName = "SuggestedDriver",
                Width = 102
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Queue Status",
                DataPropertyName = "QueueStatus",
                Width = 104
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

        private ListBox BuildAssignmentNotesListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
        }

        private void LoadAssignments(string refreshReason)
        {
            var tickets = (_ticketProvider() ?? new List<DispatchTicket>()).OrderBy(ticket => ticket.TicketId).ToList();
            var driverRows = tickets
                .GroupBy(ticket => ticket.DriverCode)
                .Select(group => CreateDriverAssignmentRow(group.Key, group.ToList()))
                .OrderBy(row => row.DriverCode)
                .ToList();
            var pendingRows = tickets
                .OrderByDescending(ticket => ticket.TicketId % 2 == 0)
                .ThenBy(ticket => ticket.RouteZone)
                .Select(CreatePendingTicketRow)
                .ToList();

            _driverRows.RaiseListChangedEvents = false;
            _driverRows.Clear();
            foreach (var driverRow in driverRows)
            {
                _driverRows.Add(driverRow);
            }

            _driverRows.RaiseListChangedEvents = true;
            _driverRows.ResetBindings();

            _pendingTicketRows.RaiseListChangedEvents = false;
            _pendingTicketRows.Clear();
            foreach (var pendingRow in pendingRows)
            {
                _pendingTicketRows.Add(pendingRow);
            }

            _pendingTicketRows.RaiseListChangedEvents = true;
            _pendingTicketRows.ResetBindings();

            PopulateSummary(driverRows, pendingRows);
            PopulateNotes(driverRows, pendingRows);

            if (_driverGrid.Rows.Count > 0)
            {
                _driverGrid.Rows[0].Selected = true;
            }

            if (_pendingTicketGrid.Rows.Count > 0)
            {
                _pendingTicketGrid.Rows[0].Selected = true;
            }

            _statusLabel.Text = string.Format("Driver board refreshed for store {0} ({1}).", _storeNumber, refreshReason);
            _refreshLabel.Text = "Last refresh " + DateTime.Now.ToString("g");
        }

        private void DriverGridSelectionChanged(object sender, EventArgs e)
        {
            var driver = _driverGrid.CurrentRow == null ? null : _driverGrid.CurrentRow.DataBoundItem as DriverAssignmentRow;
            if (driver == null)
            {
                return;
            }

            _statusLabel.Text = string.Format(
                "{0} is holding {1} run(s) for {2}.",
                driver.DriverCode,
                driver.CurrentRunCount,
                driver.HomeZone);
        }

        private void AssignHighlightedTicket(object sender, EventArgs e)
        {
            var driver = _driverGrid.CurrentRow == null ? null : _driverGrid.CurrentRow.DataBoundItem as DriverAssignmentRow;
            var ticket = _pendingTicketGrid.CurrentRow == null ? null : _pendingTicketGrid.CurrentRow.DataBoundItem as PendingTicketRow;

            if (driver == null || ticket == null)
            {
                MessageBox.Show(
                    this,
                    "Select one driver and one pending ticket before staging an assignment.",
                    "Driver Assignments",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var stagedWave = driver.NextDispatchWave;
            StageTicketToDriver(ticket, driver, "Staged on " + stagedWave);

            _pendingTicketRows.ResetBindings();
            _driverRows.ResetBindings();

            _statusLabel.Text = string.Format(
                "Ticket {0} is staged to {1} for terminal review.",
                ticket.TicketId,
                driver.DriverCode);
        }

        private void BalanceBoard(object sender, EventArgs e)
        {
            var balancedCount = 0;
            foreach (var ticket in _pendingTicketRows.Where(row => string.IsNullOrWhiteSpace(row.SuggestedDriver)))
            {
                var bestDriver = _driverRows.OrderBy(row => row.CurrentRunCount).ThenBy(row => row.DriverCode).FirstOrDefault();
                if (bestDriver == null)
                {
                    break;
                }

                var balancedWave = bestDriver.NextDispatchWave;
                StageTicketToDriver(ticket, bestDriver, "Balanced on " + balancedWave);
                balancedCount += 1;
            }

            _pendingTicketRows.ResetBindings();
            _driverRows.ResetBindings();
            _statusLabel.Text = balancedCount == 0
                ? "Pending tickets already have staged drivers."
                : "The board spread pending tickets across the lightest loads.";
        }

        private void PopulateSummary(IList<DriverAssignmentRow> drivers, IList<PendingTicketRow> pendingTickets)
        {
            _summaryListView.BeginUpdate();
            _summaryListView.Items.Clear();

            AddSummaryItem("Store", _storeNumber);
            AddSummaryItem("Terminal", _terminalId);
            AddSummaryItem("Drivers On Shift", drivers.Count.ToString());
            AddSummaryItem("Pending Tickets", pendingTickets.Count.ToString());
            AddSummaryItem("Counter Holds", pendingTickets.Count(row => row.Priority == "Counter Hold").ToString());
            AddSummaryItem("Balanced Queue", pendingTickets.Count(row => row.QueueStatus == "Balanced").ToString());
            AddSummaryItem("Dispatcher", Environment.UserName);

            _summaryListView.EndUpdate();
        }

        private void PopulateNotes(IList<DriverAssignmentRow> drivers, IList<PendingTicketRow> pendingTickets)
        {
            _assignmentNotesListBox.BeginUpdate();
            _assignmentNotesListBox.Items.Clear();

            if (drivers.Count == 0)
            {
                _assignmentNotesListBox.Items.Add("No drivers are clocked in for this dispatch terminal.");
                _assignmentNotesListBox.EndUpdate();
                return;
            }

            foreach (var driver in drivers)
            {
                _assignmentNotesListBox.Items.Add(
                    string.Format(
                        "{0}: hold near {1}; next send is {2}.",
                        driver.DriverCode,
                        driver.HomeZone,
                        driver.NextDispatchWave));
            }

            if (pendingTickets.Any(row => row.Priority == "Counter Hold"))
            {
                _assignmentNotesListBox.Items.Add("Counter hold tickets should be paired before the next board send.");
            }

            _assignmentNotesListBox.EndUpdate();
        }

        private DriverAssignmentRow CreateDriverAssignmentRow(string driverCode, IList<DispatchTicket> tickets)
        {
            var firstTicket = tickets.OrderBy(ticket => ticket.TicketId).First();
            var currentRunCount = tickets.Count;
            return new DriverAssignmentRow
            {
                DriverCode = driverCode,
                HomeZone = firstTicket.RouteZone,
                CurrentRunCount = currentRunCount,
                SuggestedLoad = currentRunCount + 1,
                NextDispatchWave = "Wave " + Math.Min(currentRunCount + 1, 4),
                ShiftStatus = currentRunCount > 1 ? "Rolling" : "Ready",
                LastDropTime = "5:" + (12 + currentRunCount * 5).ToString("00")
            };
        }

        private PendingTicketRow CreatePendingTicketRow(DispatchTicket ticket)
        {
            return new PendingTicketRow
            {
                TicketId = ticket.TicketId,
                RouteZone = ticket.RouteZone,
                QuotedEta = (18 + (ticket.TicketId % 4) * 4) + " min",
                Priority = ticket.TicketId % 2 == 0 ? "Counter Hold" : "Ready",
                SuggestedDriver = string.Empty,
                QueueStatus = ticket.TicketId % 2 == 0 ? "Hold for Pair" : "Ready to Send"
            };
        }

        private static void StageTicketToDriver(PendingTicketRow ticket, DriverAssignmentRow driver, string queueStatus)
        {
            ticket.SuggestedDriver = driver.DriverCode;
            ticket.QueueStatus = queueStatus;
            driver.CurrentRunCount += 1;
            driver.SuggestedLoad = driver.CurrentRunCount + 1;
            driver.NextDispatchWave = BuildWaveLabel(driver.CurrentRunCount);
            driver.ShiftStatus = "Queued";
        }

        private static string BuildWaveLabel(int currentRunCount)
        {
            return "Wave " + Math.Min(currentRunCount + 1, 4);
        }

        private void AddSummaryItem(string label, string value)
        {
            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            _summaryListView.Items.Add(item);
        }

        private sealed class DriverAssignmentRow
        {
            public string DriverCode { get; set; }

            public string HomeZone { get; set; }

            public int CurrentRunCount { get; set; }

            public int SuggestedLoad { get; set; }

            public string NextDispatchWave { get; set; }

            public string ShiftStatus { get; set; }

            public string LastDropTime { get; set; }
        }

        private sealed class PendingTicketRow
        {
            public int TicketId { get; set; }

            public string RouteZone { get; set; }

            public string QuotedEta { get; set; }

            public string Priority { get; set; }

            public string SuggestedDriver { get; set; }

            public string QueueStatus { get; set; }
        }
    }
}
