using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraMap;
using System;
using System.Data;
using System.Windows.Forms;

namespace DXApplicationViewer
{
    public partial class ViewerForm1 : XtraForm
    {
        private int btnX, btnY;
        public ViewerForm1()
        {
            InitializeComponent();
            //dashboardViewer.Dashboard.Items[0].
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("value:" + dashboardViewer.Dashboard.Parameters["몇시간전"].Value);
            Console.WriteLine("zero? : " + (Convert.ToInt32(dashboardViewer.Dashboard.Parameters["몇시간전"].Value) == 0));
            if (Convert.ToInt32(dashboardViewer.Dashboard.Parameters["몇시간전"].Value) == 0)
            {
                timer1.Enabled = false;
                simpleButton1.Appearance.BackColor = System.Drawing.Color.Transparent;
            }
            else
            {
                dashboardViewer.ReloadData();
            }
            
            
        }

        private void ViewerForm1_MaximumSizeChanged(object sender, EventArgs e)
        {
            
        }

        private void ViewerForm1_MaximizedBoundsChanged(object sender, EventArgs e)
        {
            
        }

        private void ViewerForm1_Resize(object sender, EventArgs e)
        {
            btnX = dashboardViewer.Bounds.Right - 100;
            btnY = dashboardViewer.Bounds.Top +8;
            simpleButton1.SetBounds(btnX, btnY, simpleButton1.Bounds.Width, simpleButton1.Height);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (simpleButton1.Appearance.BackColor != System.Drawing.Color.Red)
            {
                //dashboardViewer.BeginUpdateParameters();
                dashboardViewer.Dashboard.Parameters["몇시간전"].Value = 1;
                //dashboardViewer.EndUpdateParameters();
                
                simpleButton1.Appearance.BackColor = System.Drawing.Color.Red;
                timer1.Enabled = true;
            }
            else
            {
                simpleButton1.Appearance.BackColor = System.Drawing.Color.Transparent;
                timer1.Enabled = false;
            }
                
        }

        private void ViewerForm1_Load(object sender, EventArgs e)
        {
            //timer1.Start();
            //dashboardViewer.BeginUpdateParameters();
            dashboardViewer.Dashboard.Parameters["시작날짜"].Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            dashboardViewer.Dashboard.Parameters["종료날짜"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //dashboardViewer.EndUpdateParameters();
            
            btnX = dashboardViewer.Bounds.Right - 100;
            btnY = dashboardViewer.Bounds.Top+8;
            simpleButton1.SetBounds(btnX, btnY, simpleButton1.Bounds.Width, simpleButton1.Height);
            
            DevExpress.DashboardCommon.DashboardParameterCollection items = dashboardViewer.Dashboard.Parameters;
            foreach (var item in items)
            {
                Console.WriteLine($"parameter: " + item.Name + " " + item.Value);
            }
        }
    }
}
