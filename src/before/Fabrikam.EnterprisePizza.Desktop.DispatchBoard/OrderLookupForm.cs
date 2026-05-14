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
    public class OrderLookupForm : Form
    {
        private readonly StoreOperationsWorkbenchService _workbenchService;
        private readonly string _storeNumber;
        private readonly string _terminalId;
        private readonly BindingList<OrderLookupRecord> _orderRows;
        private readonly BindingList<OrderWorkflowRow> _workflowRows;
        private readonly DataGridView _orderGrid;
        private readonly DataGridView _workflowGrid;
        private readonly ListView _summaryListView;
        private readonly ListBox _followUpListBox;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly ToolStripStatusLabel _refreshLabel;
        private ToolStripTextBox _searchTextBox;
        private ToolStripComboBox _statusComboBox;

        public OrderLookupForm(string storeNumber, string terminalId, StoreOperationsWorkbenchService workbenchService)
        {
            if (workbenchService == null)
            {
                throw new ArgumentNullException(nameof(workbenchService));
            }

            _workbenchService = workbenchService;
            _storeNumber = string.IsNullOrWhiteSpace(storeNumber) ? "014" : storeNumber.Trim().ToUpperInvariant();
            _terminalId = string.IsNullOrWhiteSpace(terminalId) ? "TERM-02" : terminalId.Trim().ToUpperInvariant();
            _orderRows = new BindingList<OrderLookupRecord>();
            _workflowRows = new BindingList<OrderWorkflowRow>();

            Font = SystemFonts.MessageBoxFont;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1040, 680);
            Width = 1180;
            Height = 760;
            Text = string.Format("Order Lookup - Store {0} [{1}]", _storeNumber, _terminalId);

            var toolStrip = BuildToolStrip();
            _orderGrid = BuildOrderGrid();
            _workflowGrid = BuildWorkflowGrid();
            _summaryListView = BuildSummaryListView();
            _followUpListBox = BuildFollowUpListBox();

            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel2,
                SplitterDistance = 760
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

            LoadOrders("Order desk opened");
        }

        private ToolStrip BuildToolStrip()
        {
            var toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            toolStrip.Items.Add(new ToolStripButton("Refresh", null, delegate { LoadOrders("Manual refresh"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Find"));

            _searchTextBox = new ToolStripTextBox
            {
                AutoSize = false,
                Width = 160
            };
            _searchTextBox.KeyDown += SearchTextBoxKeyDown;
            toolStrip.Items.Add(_searchTextBox);

            toolStrip.Items.Add(new ToolStripButton("Apply", null, delegate { LoadOrders("Lookup updated"); }));
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Status"));

            _statusComboBox = new ToolStripComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _statusComboBox.Items.AddRange(new object[] { "All", "Ready", "Exception", "Carryout Hold" });
            _statusComboBox.SelectedIndex = 0;
            _statusComboBox.SelectedIndexChanged += delegate { LoadOrders("Filter updated"); };
            toolStrip.Items.Add(_statusComboBox);

            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(new ToolStripLabel("Store " + _storeNumber));
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
                SplitterDistance = 355
            };

            var ordersGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Order Ledger"
            };
            ordersGroup.Controls.Add(_orderGrid);

            var workflowGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Selected Order Workflow"
            };
            workflowGroup.Controls.Add(_workflowGrid);

            split.Panel1.Controls.Add(ordersGroup);
            split.Panel2.Controls.Add(workflowGroup);
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
                Text = "Lookup Summary",
                Dock = DockStyle.Fill
            };
            summaryGroup.Controls.Add(_summaryListView);

            var notesGroup = new GroupBox
            {
                Text = "Counter Follow-Up",
                Dock = DockStyle.Fill
            };
            notesGroup.Controls.Add(_followUpListBox);

            layout.Controls.Add(summaryGroup, 0, 0);
            layout.Controls.Add(notesGroup, 0, 1);
            return layout;
        }

        private DataGridView BuildOrderGrid()
        {
            var grid = BuildReadOnlyGrid(_orderRows);
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Order",
                DataPropertyName = "OrderNumber",
                Width = 72
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Channel",
                DataPropertyName = "Channel",
                Width = 92
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Customer",
                DataPropertyName = "CustomerName",
                Width = 150
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Service",
                DataPropertyName = "ServiceMode",
                Width = 84
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Promise",
                DataPropertyName = "PromiseWindow",
                Width = 88
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "TicketTotalDisplay",
                Width = 68
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Kitchen",
                DataPropertyName = "KitchenStatus",
                Width = 98
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Dispatch",
                DataPropertyName = "DispatchStatus",
                Width = 102
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Payment",
                DataPropertyName = "PaymentStatus",
                Width = 86
            });
            grid.SelectionChanged += OrderGridSelectionChanged;
            return grid;
        }

        private DataGridView BuildWorkflowGrid()
        {
            var grid = BuildReadOnlyGrid(_workflowRows);
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Step",
                DataPropertyName = "Step",
                Width = 160
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Station",
                DataPropertyName = "Station",
                Width = 124
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 110
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Note",
                DataPropertyName = "Note",
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

        private ListBox BuildFollowUpListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
        }

        private void LoadOrders(string refreshReason)
        {
            var orders = _workbenchService.GetOrderLookupRecords(_storeNumber);
            var search = (_searchTextBox == null ? string.Empty : _searchTextBox.Text).Trim();
            var status = _statusComboBox == null || _statusComboBox.SelectedItem == null
                ? "All"
                : _statusComboBox.SelectedItem.ToString();

            var filteredOrders = orders
                .Where(order => MatchesSearch(order, search))
                .Where(order => MatchesStatus(order, status))
                .OrderBy(order => order.PromiseWindow)
                .ThenBy(order => order.OrderNumber)
                .ToList();

            ResetBinding(_orderRows, filteredOrders);
            PopulateSummary(filteredOrders);
            PopulateFollowUps(filteredOrders);

            if (_orderGrid.Rows.Count > 0)
            {
                _orderGrid.Rows[0].Selected = true;
            }
            else
            {
                PopulateWorkflow(null);
            }

            _statusLabel.Text = string.Format("Order lookup refreshed for store {0} ({1}).", _storeNumber, refreshReason);
            _refreshLabel.Text = "Last refresh " + DateTime.Now.ToString("g");
        }

        private void OrderGridSelectionChanged(object sender, EventArgs e)
        {
            var selectedOrder = _orderGrid.CurrentRow == null ? null : _orderGrid.CurrentRow.DataBoundItem as OrderLookupRecord;
            PopulateWorkflow(selectedOrder);
            if (selectedOrder == null)
            {
                return;
            }

            _statusLabel.Text = string.Format(
                "Order {0} for {1} is {2} / {3}.",
                selectedOrder.OrderNumber,
                selectedOrder.CustomerName,
                selectedOrder.KitchenStatus,
                selectedOrder.DispatchStatus);
        }

        private void PopulateWorkflow(OrderLookupRecord selectedOrder)
        {
            var rows = new List<OrderWorkflowRow>();

            if (selectedOrder != null)
            {
                rows.Add(new OrderWorkflowRow
                {
                    Step = "Order Entry",
                    Station = "Counter",
                    Status = selectedOrder.PaymentStatus,
                    Note = selectedOrder.Channel + " order confirmed for " + selectedOrder.CustomerName + "."
                });
                rows.Add(new OrderWorkflowRow
                {
                    Step = "Kitchen Queue",
                    Station = "Makeline",
                    Status = selectedOrder.KitchenStatus,
                    Note = selectedOrder.ServiceMode == "Delivery"
                        ? "Bag with hot hold tag before dispatch."
                        : "Keep staged near the carryout shelf."
                });
                rows.Add(new OrderWorkflowRow
                {
                    Step = "Release",
                    Station = selectedOrder.ServiceMode == "Delivery" ? "Dispatch" : "Front Counter",
                    Status = selectedOrder.DispatchStatus,
                    Note = selectedOrder.DispatchStatus == "Carryout Hold"
                        ? "Call guest before bumping promise window."
                        : "Release on promise window " + selectedOrder.PromiseWindow + "."
                });
            }

            ResetBinding(_workflowRows, rows);
        }

        private void PopulateSummary(IList<OrderLookupRecord> orders)
        {
            _summaryListView.BeginUpdate();
            _summaryListView.Items.Clear();
            AddSummaryItem("Store", _storeNumber);
            AddSummaryItem("Terminal", _terminalId);
            AddSummaryItem("Visible Orders", orders.Count.ToString());
            AddSummaryItem("Delivery", orders.Count(order => order.ServiceMode == "Delivery").ToString());
            AddSummaryItem("Carryout", orders.Count(order => order.ServiceMode != "Delivery").ToString());
            AddSummaryItem("Exceptions", orders.Count(order => order.KitchenStatus == "Exception" || order.DispatchStatus == "Carryout Hold").ToString());
            AddSummaryItem("Clerk", Environment.UserName);
            _summaryListView.EndUpdate();
        }

        private void PopulateFollowUps(IList<OrderLookupRecord> orders)
        {
            _followUpListBox.BeginUpdate();
            _followUpListBox.Items.Clear();

            if (orders.Count == 0)
            {
                _followUpListBox.Items.Add("No orders match the current lookup.");
                _followUpListBox.EndUpdate();
                return;
            }

            foreach (var order in orders.Where(order => order.DispatchStatus == "Carryout Hold"))
            {
                _followUpListBox.Items.Add(string.Format("Order {0}: call {1} on the carryout callback list.", order.OrderNumber, order.CustomerName));
            }

            foreach (var order in orders.Where(order => order.KitchenStatus == "Exception"))
            {
                _followUpListBox.Items.Add(string.Format("Order {0}: verify remake note before releasing to {1}.", order.OrderNumber, order.ServiceMode.ToLowerInvariant()));
            }

            if (_followUpListBox.Items.Count == 0)
            {
                _followUpListBox.Items.Add("No counter follow-up items are waiting right now.");
            }

            _followUpListBox.EndUpdate();
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadOrders("Lookup updated");
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

        private static bool MatchesSearch(OrderLookupRecord order, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return true;
            }

            return order.OrderNumber.ToString().Contains(search)
                || order.CustomerName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                || order.Channel.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool MatchesStatus(OrderLookupRecord order, string status)
        {
            if (string.IsNullOrWhiteSpace(status) || status == "All")
            {
                return true;
            }

            if (status == "Ready")
            {
                return order.KitchenStatus == "Ready" && order.DispatchStatus == "Ready";
            }

            if (status == "Exception")
            {
                return order.KitchenStatus == "Exception";
            }

            return order.DispatchStatus == status;
        }

        private sealed class OrderWorkflowRow
        {
            public string Step { get; set; }

            public string Station { get; set; }

            public string Status { get; set; }

            public string Note { get; set; }
        }
    }
}
