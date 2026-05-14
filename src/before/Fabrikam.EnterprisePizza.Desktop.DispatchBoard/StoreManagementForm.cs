using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Fabrikam.EnterprisePizza.Business.StoreOps.Services;
using Fabrikam.EnterprisePizza.Shared.Contracts.StoreOps;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class StoreManagementForm : Form
    {
        private readonly StoreOperationsWorkbenchService _workbenchService;
        private readonly string _homeStoreNumber;
        private readonly string _terminalId;
        private readonly BindingList<StoreManagementRecord> _storeRows;
        private readonly BindingList<TerminalQueueRow> _terminalRows;
        private readonly DataGridView _storeGrid;
        private readonly DataGridView _terminalGrid;
        private readonly ListView _summaryListView;
        private readonly ListBox _alertsListBox;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripStatusLabel _refreshLabel;
        private ToolStripTextBox _filterTextBox;

        public StoreManagementForm(string storeNumber, string terminalId, StoreOperationsWorkbenchService workbenchService)
        {
            if (workbenchService == null)
            {
                throw new ArgumentNullException(nameof(workbenchService));
            }

            _workbenchService = workbenchService;
            _homeStoreNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            _terminalId = string.IsNullOrWhiteSpace(terminalId) ? "TERM-02" : terminalId.Trim().ToUpperInvariant();
            _storeRows = new BindingList<StoreManagementRecord>();
            _terminalRows = new BindingList<TerminalQueueRow>();

            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1080, 700);
            Width = 1220;
            Height = 780;
            Text = string.Format("Store Management - Home Store {0} [{1}]", _homeStoreNumber, _terminalId);

            var toolStrip = BuildToolStrip();
            _storeGrid = BuildStoreGrid();
            _terminalGrid = BuildTerminalGrid();
            _summaryListView = BuildSummaryListView();
            _alertsListBox = BuildAlertsListBox();

            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2,
                SplitterDistance = 790
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

            LoadStores("Store desk opened");
        }

        private ToolStrip BuildToolStrip()
        {
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            toolStrip.Items.Add(new ToolStripButton("Refresh", null, delegate { LoadStores("Manual refresh"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Filter"));

            _filterTextBox = new ToolStripTextBox
            {
                AutoSize = false,
                Width = 160
            };
            _filterTextBox.KeyDown += FilterTextBoxKeyDown;
            toolStrip.Items.Add(_filterTextBox);

            toolStrip.Items.Add(new ToolStripButton("Apply", null, delegate { LoadStores("Filter updated"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Home Store " + _homeStoreNumber));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Terminal " + _terminalId));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripButton("Close", null, delegate { Close(); }));

            return toolStrip;
        }

        private Control BuildWorkspace()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 345
            };

            var storesGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Store Desk"
            };
            storesGroup.Controls.Add(_storeGrid);

            var terminalsGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Terminal / Queue Detail"
            };
            terminalsGroup.Controls.Add(_terminalGrid);

            split.Panel1.Controls.Add(storesGroup);
            split.Panel2.Controls.Add(terminalsGroup);
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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 230F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var summaryGroup = new GroupBox
            {
                Text = "Store Summary",
                Dock = DockStyle.Fill
            };
            summaryGroup.Controls.Add(_summaryListView);

            var alertsGroup = new GroupBox
            {
                Text = "Supervisor Alerts",
                Dock = DockStyle.Fill
            };
            alertsGroup.Controls.Add(_alertsListBox);

            layout.Controls.Add(summaryGroup, 0, 0);
            layout.Controls.Add(alertsGroup, 0, 1);
            return layout;
        }

        private DataGridView BuildStoreGrid()
        {
            var grid = BuildReadOnlyGrid(_storeRows);
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Store",
                DataPropertyName = "StoreNumber",
                Width = 64
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Store Name",
                DataPropertyName = "StoreName",
                Width = 176
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "District",
                DataPropertyName = "District",
                Width = 108
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Manager",
                DataPropertyName = "ManagerOnDuty",
                Width = 136
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Board",
                DataPropertyName = "BoardMode",
                Width = 110
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Open Orders",
                DataPropertyName = "OpenOrderCount",
                Width = 88
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Drivers",
                DataPropertyName = "DriverCount",
                Width = 64
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Last Sync",
                DataPropertyName = "LastSyncTime",
                Width = 92
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "StoreStatus",
                Width = 110
            });
            grid.SelectionChanged += StoreGridSelectionChanged;
            return grid;
        }

        private DataGridView BuildTerminalGrid()
        {
            var grid = BuildReadOnlyGrid(_terminalRows);
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Terminal",
                DataPropertyName = "TerminalId",
                Width = 110
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Role",
                DataPropertyName = "Role",
                Width = 110
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Operator",
                DataPropertyName = "OperatorName",
                Width = 140
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Queue",
                DataPropertyName = "QueueState",
                Width = 140
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Last Refresh",
                DataPropertyName = "LastRefresh",
                Width = 92
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Supervisor Note",
                DataPropertyName = "SupervisorNote",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
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

        private ListBox BuildAlertsListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
        }

        private void LoadStores(string refreshReason)
        {
            var filter = (_filterTextBox == null ? string.Empty : _filterTextBox.Text).Trim();
            var stores = _workbenchService.GetStoreManagementRecords()
                .Where(store => MatchesFilter(store, filter))
                .OrderBy(store => store.StoreNumber)
                .ToList();

            ResetBinding(_storeRows, stores);
            PopulateSummary(stores);
            PopulateAlerts(stores);

            if (_storeGrid.Rows.Count > 0)
            {
                _storeGrid.Rows[0].Selected = true;
            }
            else
            {
                PopulateTerminalQueue(null);
            }

            _statusLabel.Text = string.Format("Store management refreshed ({0}).", refreshReason);
            _refreshLabel.Text = "Last refresh " + DateTime.Now.ToString("g");
        }

        private void StoreGridSelectionChanged(object sender, EventArgs e)
        {
            var selectedStore = _storeGrid.CurrentRow == null ? null : _storeGrid.CurrentRow.DataBoundItem as StoreManagementRecord;
            PopulateTerminalQueue(selectedStore);
            if (selectedStore == null)
            {
                return;
            }

            _statusLabel.Text = string.Format(
                "Store {0} is {1} with {2} open orders and {3} drivers.",
                selectedStore.StoreNumber,
                selectedStore.StoreStatus,
                selectedStore.OpenOrderCount,
                selectedStore.DriverCount);
        }

        private void PopulateTerminalQueue(StoreManagementRecord selectedStore)
        {
            var rows = new List<TerminalQueueRow>();

            if (selectedStore != null)
            {
                rows.Add(new TerminalQueueRow
                {
                    TerminalId = selectedStore.DispatchTerminalId,
                    Role = "Dispatch Board",
                    OperatorName = selectedStore.ManagerOnDuty,
                    QueueState = selectedStore.BoardMode,
                    LastRefresh = selectedStore.LastSyncTime,
                    SupervisorNote = selectedStore.StoreStatus == "Needs Follow-Up"
                        ? "Verify clock drift and printer queue before rush."
                        : "Terminal is holding the expected queue shape."
                });
                rows.Add(new TerminalQueueRow
                {
                    TerminalId = selectedStore.StoreNumber + "-COUNTER",
                    Role = "Carryout Counter",
                    OperatorName = "Front Counter",
                    QueueState = selectedStore.OpenOrderCount > 10 ? "Heavy" : "Normal",
                    LastRefresh = selectedStore.LastSyncTime,
                    SupervisorNote = selectedStore.OpenOrderCount > 10
                        ? "Stagger callback promises on delayed carryout tickets."
                        : "Counter queue is in line with current volume."
                });
            }

            ResetBinding(_terminalRows, rows);
        }

        private void PopulateSummary(IList<StoreManagementRecord> stores)
        {
            _summaryListView.BeginUpdate();
            _summaryListView.Items.Clear();
            AddSummaryItem("Visible Stores", stores.Count.ToString());
            AddSummaryItem("Needs Follow-Up", stores.Count(store => store.StoreStatus == "Needs Follow-Up").ToString());
            AddSummaryItem("High Volume", stores.Count(store => store.OpenOrderCount >= 10).ToString());
            AddSummaryItem("Home Store", _homeStoreNumber);
            AddSummaryItem("Terminal", _terminalId);
            AddSummaryItem("Supervisor", Environment.UserName);
            _summaryListView.EndUpdate();
        }

        private void PopulateAlerts(IList<StoreManagementRecord> stores)
        {
            _alertsListBox.BeginUpdate();
            _alertsListBox.Items.Clear();

            if (stores.Count == 0)
            {
                _alertsListBox.Items.Add("No stores match the current desk filter.");
                _alertsListBox.EndUpdate();
                return;
            }

            foreach (var store in stores.Where(store => store.StoreStatus == "Needs Follow-Up"))
            {
                _alertsListBox.Items.Add(string.Format("Store {0}: {1}", store.StoreNumber, store.EscalationNote));
            }

            foreach (var store in stores.Where(store => store.OpenOrderCount >= 10))
            {
                _alertsListBox.Items.Add(string.Format("Store {0}: high order volume, keep extra drivers near the hot rack.", store.StoreNumber));
            }

            if (_alertsListBox.Items.Count == 0)
            {
                _alertsListBox.Items.Add("No supervisor alerts are waiting right now.");
            }

            _alertsListBox.EndUpdate();
        }

        private void FilterTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadStores("Filter updated");
            }
        }

        private void AddSummaryItem(string label, string value)
        {
            var item = new ListViewItem(label);
            item.SubItems.Add(value);
            _summaryListView.Items.Add(item);
        }

        private static DataGridView BuildReadOnlyGrid(object dataSource)
        {
            return new DataGridView
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
                DataSource = dataSource
            };
        }

        private static void ResetBinding<T>(BindingList<T> bindingList, IList<T> rows)
        {
            bindingList.RaiseListChangedEvents = false;
            bindingList.Clear();
            foreach (var row in rows)
            {
                bindingList.Add(row);
            }

            bindingList.RaiseListChangedEvents = true;
            bindingList.ResetBindings();
        }

        private static bool MatchesFilter(StoreManagementRecord store, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return store.StoreNumber.Contains(filter)
                || store.StoreName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                || store.District.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private sealed class TerminalQueueRow
        {
            public string TerminalId { get; set; }

            public string Role { get; set; }

            public string OperatorName { get; set; }

            public string QueueState { get; set; }

            public string LastRefresh { get; set; }

            public string SupervisorNote { get; set; }
        }
    }
}
