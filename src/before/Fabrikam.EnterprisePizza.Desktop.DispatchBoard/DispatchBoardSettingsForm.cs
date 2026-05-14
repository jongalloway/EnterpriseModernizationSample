using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fabrikam.EnterprisePizza.Desktop.DispatchBoard
{
    public class DispatchBoardSettingsForm : Form
    {
        private readonly TextBox _storeNumberTextBox;
        private readonly TextBox _terminalIdTextBox;
        private readonly NumericUpDown _autoRefreshSecondsControl;
        private readonly CheckBox _includeDriverNotesCheckBox;

        public DispatchBoardSettingsForm(DispatchBoardSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Dispatch Options";
            ClientSize = new Size(360, 205);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(9)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _storeNumberTextBox = new TextBox
            {
                Text = settings.StoreNumber,
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 8,
                Dock = DockStyle.Fill
            };

            _terminalIdTextBox = new TextBox
            {
                Text = settings.DispatchTerminalId,
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 16,
                Dock = DockStyle.Fill
            };

            _autoRefreshSecondsControl = new NumericUpDown
            {
                Minimum = DispatchBoardSettings.MinimumAutoRefreshSeconds,
                Maximum = DispatchBoardSettings.MaximumAutoRefreshSeconds,
                Value = DispatchBoardAppSettings.ClampAutoRefreshSeconds(settings.AutoRefreshSeconds),
                Dock = DockStyle.Left,
                Width = 90
            };

            _includeDriverNotesCheckBox = new CheckBox
            {
                Text = "Show driver notes and route reminders",
                Checked = settings.IncludeDriverNotes,
                AutoSize = true,
                Dock = DockStyle.Left
            };

            layout.Controls.Add(BuildLabel("Store Number"), 0, 0);
            layout.Controls.Add(_storeNumberTextBox, 1, 0);
            layout.Controls.Add(BuildLabel("Dispatch Terminal"), 0, 1);
            layout.Controls.Add(_terminalIdTextBox, 1, 1);
            layout.Controls.Add(BuildLabel("Auto Refresh"), 0, 2);
            layout.Controls.Add(_autoRefreshSecondsControl, 1, 2);
            layout.Controls.Add(BuildLabel("Board Options"), 0, 3);
            layout.Controls.Add(_includeDriverNotesCheckBox, 1, 3);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Width = 84
            };
            var cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 84
            };

            buttons.Controls.Add(okButton);
            buttons.Controls.Add(cancelButton);
            layout.Controls.Add(buttons, 1, 4);

            Controls.Add(layout);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        public DispatchBoardSettings Settings
        {
            get
            {
                return new DispatchBoardSettings
                {
                    StoreNumber = _storeNumberTextBox.Text.Trim().ToUpperInvariant(),
                    DispatchTerminalId = _terminalIdTextBox.Text.Trim().ToUpperInvariant(),
                    AutoRefreshSeconds = Decimal.ToInt32(_autoRefreshSecondsControl.Value),
                    IncludeDriverNotes = _includeDriverNotesCheckBox.Checked
                };
            }
        }

        private static Label BuildLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }
    }
}
