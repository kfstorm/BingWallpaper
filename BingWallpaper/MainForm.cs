using System;
using System.Reflection;
using System.Windows.Forms;

namespace Kfstorm.BingWallpaper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Text = Application.ProductName;
            nameLabel.Text = Application.ProductName;
            versionLabel.Text = Application.ProductVersion;
            homepageLabel.Text =
                ((AssemblyCopyrightAttribute)
                 (Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), true)[0]))
                    .Copyright;
            homepageLabel.Tag = @"http://www.kfstorm.com/blog";
            noTitleTooltip.SetToolTip(homepageLabel, (string) homepageLabel.Tag);
            exitButton.Text = string.Format("退出{0}", Application.ProductName);
            noTitleTooltip.SetToolTip(exitButton, string.Format("使{0}停止运行。", Application.ProductName));
            runOnStartupCheckBox.Text = string.Format("开机时启动{0}。", Application.ProductName);
            runningLabel.Text = string.Format("{0}正在工作。", Application.ProductName);
            runningTooltip.ToolTipTitle = string.Format("{0}正在后台运行", Application.ProductName);
            runningTooltip.SetToolTip(runningLabel,
                                      string.Format("您可以关闭本窗口，{0}将继续运行。\n点击“{1}”按钮即可使其停止运行。", Application.ProductName,
                                                    exitButton.Text));

            runOnStartupCheckBox.Checked = Program.GetRunOnStartup();
            runOnStartupCheckBox.CheckedChanged += runOnStartupCheckBox_CheckedChanged;
        }

        public void UpdateState(State state)
        {
            if (state == null) throw new ArgumentNullException("state");
            titleLable.Text = state.Copyright;
            titleLable.Tag = state.PictureUrl;
        }

        private bool _showWithoutActivation;

        public void SetShowWithoutActivation(bool showWithoutActivation)
        {
            _showWithoutActivation = showWithoutActivation;
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return _showWithoutActivation;
            }
        }

        private void homepageLabel_Click(object sender, EventArgs e)
        {
            if (homepageLabel.Tag != null) UrlHelper.OpenLink((string)homepageLabel.Tag);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Program.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                Program.Exit();
            }
        }

        private void runOnStartupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Program.SetRunOnStartup(runOnStartupCheckBox.Checked);
        }

        private void titleLable_Click(object sender, EventArgs e)
        {
            if (titleLable.Tag != null) UrlHelper.OpenLink((string)titleLable.Tag);
        }

    }
}