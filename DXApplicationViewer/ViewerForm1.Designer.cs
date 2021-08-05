
namespace DXApplicationViewer
{
    partial class ViewerForm1
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
            this.dashboardViewer = new DevExpress.DashboardWin.DashboardViewer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.dashboardViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // dashboardViewer
            // 
            this.dashboardViewer.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dashboardViewer.Appearance.BackColor2 = System.Drawing.Color.Black;
            this.dashboardViewer.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dashboardViewer.Appearance.Options.UseBackColor = true;
            this.dashboardViewer.Appearance.Options.UseForeColor = true;
            this.dashboardViewer.AsyncMode = true;
            this.dashboardViewer.DashboardSource = new System.Uri("C:\\DLIT_Files\\엔지온_last.xml", System.UriKind.Absolute);
            this.dashboardViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardViewer.Location = new System.Drawing.Point(0, 0);
            this.dashboardViewer.Name = "dashboardViewer";
            this.dashboardViewer.Size = new System.Drawing.Size(1150, 676);
            this.dashboardViewer.TabIndex = 0;
            this.dashboardViewer.UseNeutralFilterMode = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.simpleButton1.Appearance.Options.UseBackColor = true;
            this.simpleButton1.Location = new System.Drawing.Point(997, 0);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(40, 25);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Live";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // ViewerForm1
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 676);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.dashboardViewer);
            this.Name = "ViewerForm1";
            this.Text = "Dashboard Viewer";
            this.MaximizedBoundsChanged += new System.EventHandler(this.ViewerForm1_MaximizedBoundsChanged);
            this.MaximumSizeChanged += new System.EventHandler(this.ViewerForm1_MaximumSizeChanged);
            this.Load += new System.EventHandler(this.ViewerForm1_Load);
            this.Resize += new System.EventHandler(this.ViewerForm1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dashboardViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.DashboardWin.DashboardViewer dashboardViewer;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}

