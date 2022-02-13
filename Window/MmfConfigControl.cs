using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Mmf;
using InfoReader.Mmf.Filters;
using InfoReader.Tools.I8n;

namespace InfoReader.Window
{
    public partial class MmfConfigControl : UserControl
    {
        private MmfBase _mmfBase;
        List<string> modes =
            new(new []
            {
            "Osu",
            "Taiko",
            "Catch",
            "Mania"
            });
        List<string> statuses = 
            new(new [] 
            {
            "SelectSong",
            "Playing",
            "Editing",
            "Rank",
            "MatchSetup",
            "Lobby",
            "Idle"
            });
        public MmfConfigControl(MmfBase mmf)
        {
            InitializeComponent();
            _mmfBase = mmf;
            lst_mode.DataSource = modes;
            lst_status.DataSource = statuses;
            lst_mode.Enabled = false;
            lst_status.Enabled = false;
            
            SetLocalization(LocalizationInfo.Current);
            SetInformation(mmf);
           
        }

        public void SetLocalization(LocalizationInfo localizationInfo)
        {
            var translations = localizationInfo.Translations;
            lb_mmfNameTag.Text = translations["LANG_WINDOW_MMF_NAME"];
            lb_modeTag.Text = translations["LANG_WINDOW_MMF_MODE"];
            lb_statusTag.Text = translations["LANG_WINDOW_MMF_STATUS"];
            lb_intervalTag.Text = translations["LANG_WINDOW_MMF_INTERVAL"];
            chkBox_enabled.Text = translations["LANG_WINDOW_MMF_ENABLED"];
            btn_save.Text = translations["LANG_WINDOW_MMF_SAVE"];
            btn_configFormat.Text = translations["LANG_WINDOW_MMF_CONFIG_FORMAT"];
        }


        string[] SplitEnum(string enumStr) => enumStr.Split(',').Select(s => s.Trim()).ToArray();
        static string CombineEnum(IEnumerable enumerable)
        {
            StringBuilder builder = new StringBuilder();
            var array = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            for (var i = 0; i < array.Length; i++)
            {
                builder.Append(array[i]);
                if (i < array.Length - 1)
                {
                    builder.Append(',');
                }
            }

            return builder.ToString();
        }

        void SetInformation(MmfBase mmf)
        {
            txBox_mmfName.Text = mmf.Name;
            txBox_interval.Text = mmf.UpdateInterval.ToString();
            lst_mode.ClearSelected();
            lst_status.ClearSelected();
            if (mmf is IModeMmf modeMmf)
            {
                lst_mode.Enabled = true;
                foreach (var s in SplitEnum(modeMmf.EnabledMode.ToString()))
                {
                    lst_mode.SetItemCheckState(modes.IndexOf(s), CheckState.Checked);
                }
            }
            if (mmf is IStatusMmf statusMmf)
            {
                lst_status.Enabled = true;
                foreach (var s in SplitEnum(statusMmf.EnabledStatus.ToString()))
                {
                    lst_status.SetItemCheckState(statuses.IndexOf(s), CheckState.Checked);
                }
            }

        }

       
        private void btn_save_Click(object sender, EventArgs e)
        {
            _mmfBase.Name = txBox_mmfName.Text;
            _mmfBase.Enabled = chkBox_enabled.Checked;
            _mmfBase.UpdateInterval = int.Parse(txBox_interval.Text);
            if (_mmfBase is IStatusMmf statusMmf)
            {
                statusMmf.EnabledStatus = StatusMmfFilter.StatusProcessor(CombineEnum(lst_status.SelectedItems));
            }
            if (_mmfBase is IModeMmf modeMmf)
            {
                modeMmf.EnabledMode = ModeMmfFilter.MulGameModeProcessor(CombineEnum(lst_mode.SelectedItems));
            }
        }

        private void txBox_interval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void lst_status_Leave(object sender, EventArgs e)
        {
            CheckedListBox checkedList = (CheckedListBox) sender;
            checkedList.ClearSelected();
        }

        private void lst_mode_Leave(object sender, EventArgs e)
        {
            CheckedListBox checkedList = (CheckedListBox)sender;
            checkedList.ClearSelected();
        }
    }
}
