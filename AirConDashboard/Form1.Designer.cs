
namespace AirConDashboard
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.tabControls = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label_time = new System.Windows.Forms.Label();
            this.timer_forTime = new System.Windows.Forms.Timer(this.components);
            this.timer_forDataUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel_timeContainer = new System.Windows.Forms.Panel();
            this.panel_container = new System.Windows.Forms.Panel();
            this.pictureBox_warning = new System.Windows.Forms.PictureBox();
            this.label_humidMain = new System.Windows.Forms.Label();
            this.label_tempMain = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label_humidStd = new System.Windows.Forms.Label();
            this.label_tempStd = new System.Windows.Forms.Label();
            this.warningsPage = new AirConDashboard.warningsPage();
            this.tabControls.SuspendLayout();
            this.panel_timeContainer.SuspendLayout();
            this.panel_container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_warning)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControls
            // 
            this.tabControls.Controls.Add(this.tabPage1);
            this.tabControls.Controls.Add(this.tabPage2);
            this.tabControls.Controls.Add(this.tabPage3);
            this.tabControls.Controls.Add(this.tabPage4);
            this.tabControls.Controls.Add(this.tabPage5);
            this.tabControls.Controls.Add(this.tabPage6);
            this.tabControls.Controls.Add(this.tabPage7);
            this.tabControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControls.ItemSize = new System.Drawing.Size(100, 50);
            this.tabControls.Location = new System.Drawing.Point(0, 0);
            this.tabControls.Name = "tabControls";
            this.tabControls.SelectedIndex = 0;
            this.tabControls.Size = new System.Drawing.Size(1820, 52);
            this.tabControls.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControls.TabIndex = 0;
            this.tabControls.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 54);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1812, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "공조 1호기";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 54);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1812, 0);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "공조 2호기";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 54);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1812, 0);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "공조3호기";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 54);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1812, 0);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "공조4호기";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 54);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1812, 0);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "공조5호기";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 54);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1812, 0);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "공조6호기";
            this.tabPage6.UseVisualStyleBackColor = true;
            this.tabPage6.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.Location = new System.Drawing.Point(4, 54);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1812, 0);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "공조7호기";
            this.tabPage7.UseVisualStyleBackColor = true;
            this.tabPage7.Click += new System.EventHandler(this.anyTabPage_Click);
            // 
            // label_time
            // 
            this.label_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_time.Font = new System.Drawing.Font("Gulim", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_time.Location = new System.Drawing.Point(0, 0);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(1820, 29);
            this.label_time.TabIndex = 0;
            this.label_time.Text = "날짜:";
            this.label_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer_forTime
            // 
            this.timer_forTime.Enabled = true;
            this.timer_forTime.Interval = 1000;
            this.timer_forTime.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer_forDataUpdate
            // 
            this.timer_forDataUpdate.Enabled = true;
            this.timer_forDataUpdate.Interval = 1000;
            this.timer_forDataUpdate.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // panel_timeContainer
            // 
            this.panel_timeContainer.Controls.Add(this.label_time);
            this.panel_timeContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_timeContainer.Location = new System.Drawing.Point(0, 52);
            this.panel_timeContainer.Name = "panel_timeContainer";
            this.panel_timeContainer.Size = new System.Drawing.Size(1820, 29);
            this.panel_timeContainer.TabIndex = 10;
            // 
            // panel_container
            // 
            this.panel_container.Controls.Add(this.warningsPage);
            this.panel_container.Controls.Add(this.pictureBox_warning);
            this.panel_container.Controls.Add(this.label_humidMain);
            this.panel_container.Controls.Add(this.label_tempMain);
            this.panel_container.Controls.Add(this.label6);
            this.panel_container.Controls.Add(this.label_humidStd);
            this.panel_container.Controls.Add(this.label_tempStd);
            this.panel_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_container.Location = new System.Drawing.Point(0, 81);
            this.panel_container.Name = "panel_container";
            this.panel_container.Size = new System.Drawing.Size(1820, 656);
            this.panel_container.TabIndex = 11;
            // 
            // pictureBox_warning
            // 
            this.pictureBox_warning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_warning.Location = new System.Drawing.Point(941, 191);
            this.pictureBox_warning.Name = "pictureBox_warning";
            this.pictureBox_warning.Size = new System.Drawing.Size(150, 150);
            this.pictureBox_warning.TabIndex = 15;
            this.pictureBox_warning.TabStop = false;
            this.pictureBox_warning.Click += new System.EventHandler(this.pictureBox_warning_Click);
            // 
            // label_humidMain
            // 
            this.label_humidMain.Font = new System.Drawing.Font("Gulim", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_humidMain.Location = new System.Drawing.Point(530, 191);
            this.label_humidMain.Name = "label_humidMain";
            this.label_humidMain.Size = new System.Drawing.Size(200, 150);
            this.label_humidMain.TabIndex = 14;
            this.label_humidMain.Text = "%";
            this.label_humidMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_tempMain
            // 
            this.label_tempMain.Font = new System.Drawing.Font("Gulim", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_tempMain.Location = new System.Drawing.Point(324, 191);
            this.label_tempMain.Name = "label_tempMain";
            this.label_tempMain.Size = new System.Drawing.Size(200, 150);
            this.label_tempMain.TabIndex = 13;
            this.label_tempMain.Text = "C";
            this.label_tempMain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Gulim", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(969, 368);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "통합경보";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_humidStd
            // 
            this.label_humidStd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_humidStd.Font = new System.Drawing.Font("Gulim", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_humidStd.Location = new System.Drawing.Point(584, 368);
            this.label_humidStd.Name = "label_humidStd";
            this.label_humidStd.Size = new System.Drawing.Size(80, 20);
            this.label_humidStd.TabIndex = 11;
            this.label_humidStd.Text = "설정 습도(%)";
            this.label_humidStd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_tempStd
            // 
            this.label_tempStd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempStd.Font = new System.Drawing.Font("Gulim", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_tempStd.Location = new System.Drawing.Point(382, 368);
            this.label_tempStd.Name = "label_tempStd";
            this.label_tempStd.Size = new System.Drawing.Size(80, 20);
            this.label_tempStd.TabIndex = 10;
            this.label_tempStd.Text = "설전 온도(C)";
            this.label_tempStd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // warningsPage
            // 
            this.warningsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warningsPage.Location = new System.Drawing.Point(0, 0);
            this.warningsPage.Name = "warningsPage";
            this.warningsPage.Size = new System.Drawing.Size(1820, 656);
            this.warningsPage.TabIndex = 16;
            this.warningsPage.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1820, 737);
            this.Controls.Add(this.panel_container);
            this.Controls.Add(this.panel_timeContainer);
            this.Controls.Add(this.tabControls);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControls.ResumeLayout(false);
            this.panel_timeContainer.ResumeLayout(false);
            this.panel_container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_warning)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControls;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Timer timer_forTime;
        private System.Windows.Forms.Timer timer_forDataUpdate;
        private System.Windows.Forms.Panel panel_timeContainer;
        private System.Windows.Forms.Panel panel_container;
        private System.Windows.Forms.PictureBox pictureBox_warning;
        private System.Windows.Forms.Label label_humidMain;
        private System.Windows.Forms.Label label_tempMain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_humidStd;
        private System.Windows.Forms.Label label_tempStd;
        private warningsPage warningsPage;
    }
}

