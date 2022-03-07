using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using InfoReader.Tools.I8n;

namespace InfoReader.Window
{
    public partial class FormatEditor : Form
    {
        private static bool _syntaxInitialized;
        private readonly string _configPath;
        public FormatEditor(string path)
        {
            InitializeComponent();
            _configPath = path;
            btn_save.Text = LocalizationInfo.Current.Translations["LANG_UI_BTN_SAVE"];
            if (!_syntaxInitialized)
            {
                FileSyntaxModeProvider syntaxModeProvider = new FileSyntaxModeProvider(".\\CodeSyntax\\");
                HighlightingManager.Manager.AddSyntaxModeFileProvider(syntaxModeProvider);
                _syntaxInitialized = true; 
            }
            txBox_formatEditor.LoadFile(path, true, true);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            string content = txBox_formatEditor.Document.TextContent;
            File.WriteAllText(_configPath, content);
        }
    }
}
