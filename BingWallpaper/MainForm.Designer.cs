using System.ComponentModel;
using System.Windows.Forms;

namespace Kfstorm.BingWallpaper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.exitButton = new System.Windows.Forms.Button();
            this.homepageLabel = new System.Windows.Forms.LinkLabel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.runOnStartupCheckBox = new System.Windows.Forms.CheckBox();
            this.runOnStartupToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.titleLable = new System.Windows.Forms.LinkLabel();
            this.runningLabel = new System.Windows.Forms.Label();
            this.runningTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.noTitleTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.prePic = new System.Windows.Forms.Button();
            this.nextPic = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // prePic
            //
            prePic.Location = new System.Drawing.Point(285, 316);
            prePic.Name = "PrePic";
            prePic.Size = new System.Drawing.Size(75, 32);
            prePic.TabIndex = 7;
            prePic.Text = "上一个";
            prePic.UseVisualStyleBackColor = true;
            prePic.Click += new System.EventHandler(this.preButton_Click);
            // 
            // nextPic
            //
            nextPic.Location = new System.Drawing.Point(366, 316);
            nextPic.Name = "NextPic";
            nextPic.Size = new System.Drawing.Size(75, 32);
            nextPic.TabIndex = 8;
            nextPic.Text = "下一个";
            nextPic.UseVisualStyleBackColor = true;
            nextPic.Click += new System.EventHandler(this.nextButton_Click);
            //
            // exitButton
            // 
            this.exitButton.AutoSize = true;
            this.exitButton.Location = new System.Drawing.Point(447, 316);
            this.exitButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(150, 32);
            this.exitButton.TabIndex = 5;
            this.exitButton.Text = "退出";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // homepageLabel
            // 
            this.homepageLabel.AutoSize = true;
            this.homepageLabel.Location = new System.Drawing.Point(16, 127);
            this.homepageLabel.Name = "homepageLabel";
            this.homepageLabel.Size = new System.Drawing.Size(65, 17);
            this.homepageLabel.TabIndex = 2;
            this.homepageLabel.TabStop = true;
            this.homepageLabel.Text = "Copyright";
            this.homepageLabel.Click += new System.EventHandler(this.homepageLabel_Click);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.versionLabel.Location = new System.Drawing.Point(14, 83);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(159, 25);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "Product Version";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nameLabel.Location = new System.Drawing.Point(12, 26);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(219, 38);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Product Name";
            // 
            // runOnStartupCheckBox
            // 
            this.runOnStartupCheckBox.AutoSize = true;
            this.runOnStartupCheckBox.Location = new System.Drawing.Point(19, 200);
            this.runOnStartupCheckBox.Name = "runOnStartupCheckBox";
            this.runOnStartupCheckBox.Size = new System.Drawing.Size(99, 21);
            this.runOnStartupCheckBox.TabIndex = 3;
            this.runOnStartupCheckBox.Text = "开机时启动。";
            this.runOnStartupToolTip.SetToolTip(this.runOnStartupCheckBox, "开机启动时将不会显示窗口。");
            this.runOnStartupCheckBox.UseVisualStyleBackColor = true;
            // 
            // runOnStartupToolTip
            // 
            this.runOnStartupToolTip.ToolTipTitle = "后台运行";
            // 
            // titleLable
            // 
            this.titleLable.AutoSize = true;
            this.titleLable.Location = new System.Drawing.Point(16, 275);
            this.titleLable.Name = "titleLable";
            this.titleLable.Size = new System.Drawing.Size(135, 17);
            this.titleLable.TabIndex = 4;
            this.titleLable.TabStop = true;
            this.titleLable.Text = "Picture title (@Author)";
            this.noTitleTooltip.SetToolTip(this.titleLable, "在浏览器中显示壁纸。");
            this.titleLable.Click += new System.EventHandler(this.titleLable_Click);
            // 
            // runningLabel
            // 
            this.runningLabel.AutoSize = true;
            this.runningLabel.Location = new System.Drawing.Point(16, 324);
            this.runningLabel.Name = "runningLabel";
            this.runningLabel.Size = new System.Drawing.Size(58, 17);
            this.runningLabel.TabIndex = 6;
            this.runningLabel.Text = "Running.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 361);
            this.Controls.Add(this.prePic);
            this.Controls.Add(this.nextPic);
            this.Controls.Add(this.runningLabel);
            this.Controls.Add(this.titleLable);
            this.Controls.Add(this.runOnStartupCheckBox);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.homepageLabel);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button exitButton;
        private LinkLabel homepageLabel;
        private Label versionLabel;
        private Label nameLabel;
        private CheckBox runOnStartupCheckBox;
        private ToolTip runOnStartupToolTip;
        private LinkLabel titleLable;
        private Label runningLabel;
        private ToolTip runningTooltip;
        private ToolTip noTitleTooltip;
        private Button prePic;
        private Button nextPic;
    }
}