using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Mmf;
using osuTools.Game;

namespace InfoReader.Window
{
    public partial class MmfSelectWindow : Form
    {
        public MmfSelectWindow()
        {
            InitializeComponent();
        }

        MmfBase CreateMmf(string name, int interval, string type, Dictionary<string,object> extraInfo)
        {
            return type switch
            {
                "Status" => new StatusMmf(name, (OsuGameStatus)extraInfo["status"])
                {
                    Enabled = true, FormatFile = $"Format\\{name}.status.ifrfmt", UpdateInterval = interval
                },
                "Mode" => new ModeMmf(name, (MmfGameMode)extraInfo["mode"])
                {
                    Enabled = true, FormatFile = $"Format\\{name}.mode.ifrfmt", UpdateInterval = interval
                },
                "StatusMode" => new StatusModeMmf(name, (MmfGameMode)extraInfo["mode"], (OsuGameStatus)extraInfo["status"])
                {
                    Enabled = true, FormatFile = $"Format\\{name}.statusmode.ifrfmt", UpdateInterval = interval
                },
                _ => new NormalMmf(name)
                {
                    Enabled = true, FormatFile = $"Format\\{name}.normal.ifrfmt", UpdateInterval = interval
                }
            };
        }

        private MmfBase? _createdMmf;
        public MmfBase? GetCreatedMmf() => _createdMmf;

        private void btn_add_Click(object sender, EventArgs e)
        {
            Show();
            string type = "Default";
            if (radioBtn_statusMmf.Checked)
            {
                type = "Status";
            }
            if (radioBtn_modeMmf.Checked)
            {
                type = "Mode";
            }
            if (radioBtn_statusModeMmf.Checked)
            {
                type = "StatusMode";
            }

            if (!string.IsNullOrEmpty(txBox_mmfName.Text) && int.TryParse(txBox_interval.Text, out var interval))
            {
                //_createdMmf = CreateMmf(txBox_mmfName.Text, interval, type);
                Close();
            }
            else
            {
                MessageBox.Show("Name or interval is invalid.");
            }
        }
    }
}
