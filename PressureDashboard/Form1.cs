using System;
using System.Drawing;
using System.Windows.Forms;

namespace PressureDashboard
{
    public partial class Form1 : Form
    {
        private int btnX, btnY;
        private bool closeForm;
        public Form1()
        {
            InitializeComponent();
            try
            {
                dashboardViewer.DashboardSource = Application.StartupPath + "\\" + "ENGIONdashboard_pressure_0823.xml";
            }
            catch (Exception ex)
            {
                DialogResult dialogResult = MessageBox.Show("대시보드 xml 파일과 문제가 있다." + ex.Message, "에러 매시지", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    closeForm = true;
                }
            }
        }

        private void ViewerForm1_Resize(object sender, EventArgs e)
        {
            btnX = dashboardViewer.Bounds.Right - 100;
            btnY = dashboardViewer.Bounds.Top + 8;
            button1.SetBounds(btnX, btnY, button1.Bounds.Width, button1.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Appearance.BackColor == System.Drawing.Color.Transparent)
            {
                button1.Appearance.BackColor = System.Drawing.Color.Red;
                timer1.Enabled = true;
            }
            else
            {
                button1.Appearance.BackColor = System.Drawing.Color.Transparent;
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("value:" + dashboardViewer.Dashboard.Parameters["몇시간전"].Value);
            //Console.WriteLine("zero? : " + (Convert.ToInt32(dashboardViewer.Dashboard.Parameters["몇시간전"].Value) == 0));
            //if (Convert.ToInt32(dashboardViewer.Dashboard.Parameters["몇시간전"].Value) == 0)
            //{
            //    timer1.Enabled = false;
            //    button1.Appearance.BackColor = System.Drawing.Color.Transparent;
            //}
            //else
            //{
                dashboardViewer.ReloadData();
            //}
        }

        private void dashboardViewer_Load(object sender, EventArgs e)
        {
            
        }

        private void ViewerForm1_Load(object sender, EventArgs e)
        {
            if (closeForm)
                this.Close();
            btnX = dashboardViewer.Bounds.Right - 100;
            btnY = dashboardViewer.Bounds.Top + 8;
            button1.SetBounds(btnX, btnY, button1.Bounds.Width, button1.Height);
            //button1.Appearance.BackColor = System.Drawing.Color.Red;
        }
    }
}
