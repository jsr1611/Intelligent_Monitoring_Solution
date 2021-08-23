using AirConDashboard.Properties;
using AirConData;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CommonClassLibrary;

namespace AirConDashboard
{
    public partial class Form1 : Form
    {

        GlobalVariables gloVar = new GlobalVariables();
        private DataSet ds = new DataSet();
        private AirData[] airData_List = null;
        private System.Threading.Thread updaterThread = null;
        public Form1()
        {
            InitializeComponent();
            // ini 읽기 //
            IniFile ini = new IniFile();
            ini.Load(AppInfo.StartupPath + "\\" + "Setting.ini");
            string program_title = ini["PROGRAM"]["TITLE"].ToString();
            this.Text = program_title;    // Program title on top-left side of the window
            string D_SERVERNAME = ini["DBSetting"]["SERVERNAME"].ToString();
            string D_NAME = ini["DBSetting"]["DBNAME"].ToString();
            string D_ID = ini["DBSetting"]["ID"].ToString();
            string D_PW = ini["DBSetting"]["PW"].ToString();


            Console.WriteLine($"############ {program_title} has started. ############");


            gloVar.dbName = D_NAME;
            gloVar.dataTable = gloVar.dbName[0] + "_DATATABLE";
            gloVar.dbUID = D_ID;
            gloVar.dbPWD = D_PW;
            gloVar.dbServerName = D_SERVERNAME;
            gloVar.sqlConn = new System.Data.SqlClient.SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=False";

            gloVar.ID_List = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            airData_List = new AirData[gloVar.ID_List.Length];
            for (int i = 0; i < gloVar.ID_List.Length; i++)
            {
                airData_List[i] = new AirData();
                airData_List[i].d_20device_number = gloVar.ID_List[i].ToString();
            }
            CheckForIllegalCrossThreadCalls = false;
            int screensize = tabControls.Bounds.Right;
            tabControls.ItemSize = new Size(screensize / gloVar.ID_List.Length, 50);




        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label_time.Text = "현재시간: " + DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
            //airData_List[0].s_32warning = 1;
            pictureBox_warning.BackgroundImage = airData_List[0].s_32warning == 0 ? Resources.air_notification_off_icon_127094 : Resources.air_warning_icon_127093;
            pictureBox_warning.BackColor = airData_List[0].s_32warning == 0 ? Color.LightGreen : Color.Red;
            label_tempMain.Text = airData_List[0].d_36current_temp + " C";
            label_tempStd.Text = airData_List[0].d_12default_temp + " C";

            label_humidMain.Text = airData_List[0].d_37current_humid + " %";
            label_humidStd.Text = airData_List[0].d_13default_humid + " %";

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label_time.Text = "현재시간: " + DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (updaterThread == null || !updaterThread.IsAlive)
            {
                updaterThread = new System.Threading.Thread(UpdateData);
                updaterThread.IsBackground = true;
                updaterThread.Start();
                Console.Write("\n\n\nTHREAD WAS STARTED AT " + DateTime.Now+"\n\n");
            }
            if(timer_forDataUpdate.Interval < 60000)
            {
                timer_forDataUpdate.Interval = 60000;
                Console.WriteLine("DataUpdate timer was set for: " + timer_forDataUpdate.Interval);
            }
                

        }

        private void UpdateData()
        {
            try
            {
                if (gloVar.sqlConn == null)
                {
                    gloVar.sqlConn = new System.Data.SqlClient.SqlConnection(gloVar.sqlConStr);
                    gloVar.sqlConn.Open();
                }
                else if (gloVar.sqlConn.State != ConnectionState.Open)
                {
                    if (gloVar.sqlConn.ConnectionString.Length == 0)
                        gloVar.sqlConn.ConnectionString = gloVar.sqlConStr;
                    gloVar.sqlConn.Open();
                }
                ds.Clear();
                for (int i = 0; i < gloVar.ID_List.Length; i++)
                {
                    gloVar.sqlCmdText = $"SELECT * FROM [{gloVar.dbName}].[dbo].[{gloVar.dataTable}] WHERE DateAndTime > DATEADD(MI, -1, GETDATE()) AND dID = {gloVar.ID_List[i]}";
                    using (gloVar.sqlCmd = new System.Data.SqlClient.SqlCommand(gloVar.sqlCmdText, gloVar.sqlConn))
                    {
                        using (gloVar.sqlDAptr = new System.Data.SqlClient.SqlDataAdapter(gloVar.sqlCmd))
                        {
                            gloVar.sqlDAptr.Fill(ds, gloVar.ID_List[i].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            SetData();

        }


        /// <summary>
        /// Update the data on the screen
        /// </summary>
        private void SetData()
        {
            int i = 0;
            try
            {
                if (tabControls.InvokeRequired)
                {
                    this.Invoke(new Action(delegate ()
                    {
                        i = tabControls.SelectedIndex;
                    }));

                }
                else
                {
                    i = tabControls.SelectedIndex;
                }
            }
            catch (Exception)
            {
            }

            Console.WriteLine("TabPage Index: " + i);

            //출처: https://ssscool.tistory.com/109 [시작]

            if (airData_List.Length <= i || ds.Tables.Count <= i || ds.Tables[i].Rows.Count == 0)
            {
                Console.WriteLine($"No data? {airData_List.Length} <= {i}, ds.Tables.Count={ds.Tables.Count} <= {i} or ds.Tables[i].Rows.Count == 0!");
                //return;
            }

            else
            {
                airData_List[i].s_01songpunggi_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_01songpunggi_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_02nengbang1_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_02nengbang1_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_03nengbang2_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_03nengbang2_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_04nanbang1_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_04nanbang1_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_05nanbang2_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_05nanbang2_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_06nanbang3_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_06nanbang3_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_07nanbang4_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_07nanbang4_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_08gasyb1_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_08gasyb1_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_09gasyb2_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_09gasyb2_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_10gybsu_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_10gybsu_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_11besu_run = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_11besu_run").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];

                airData_List[i].s_17songpunggi_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_17songpunggi_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_18nengbang1_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_18nengbang1_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_19nengbang2_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_19nengbang2_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_20nanbanggi_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_20nanbanggi_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_21gasybgi_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_21gasybgi_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_22sensor_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_22sensor_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_23nusu_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_23nusu_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_24gybsu_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_24gybsu_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_25besu_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_25besu_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_26lowTemp_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_26lowTemp_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_27highTemp_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_27highTemp_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_28lowHumid_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_28lowHumid_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_29highHumid_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_29highHumid_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];
                airData_List[i].s_30fire_warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_30fire_warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];

                airData_List[i].s_32warning = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "s_32warning").Select(x => Convert.ToInt32(x.Field<string>("dDataValue"))).ToArray()[0];



                #region
                // F01 : 16 bit Unsigned Integer Type
                airData_List[i].d_02inputPort = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_02inputPort").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_02inputPort = Convert.ToInt32(airData_List[i].d_02inputPort) >= 0 ? airData_List[i].d_02inputPort : (65536 + Convert.ToInt32(airData_List[i].d_02inputPort)).ToString();
                
                airData_List[i].d_03warningContent1 = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_03warningContent1").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_03warningContent1 = Convert.ToInt32(airData_List[i].d_03warningContent1) >= 0 ? airData_List[i].d_03warningContent1 : (65536 + Convert.ToInt32(airData_List[i].d_03warningContent1)).ToString();

                airData_List[i].d_04warningContent2 = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_04warningContent2").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_04warningContent2 = Convert.ToInt32(airData_List[i].d_04warningContent2) >= 0 ? airData_List[i].d_04warningContent2 : (65536 + Convert.ToInt32(airData_List[i].d_04warningContent2)).ToString();


                airData_List[i].d_05run_stop = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_05run_stop").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_05run_stop = Convert.ToInt32(airData_List[i].d_05run_stop) >= 0 ? airData_List[i].d_05run_stop : (65536 + Convert.ToInt32(airData_List[i].d_05run_stop)).ToString();
                
                airData_List[i].d_06nengbang_prop = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_06nengbang_prop").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_06nengbang_prop = Convert.ToInt32(airData_List[i].d_06nengbang_prop) >= 0 ? airData_List[i].d_06nengbang_prop : (65536 + Convert.ToInt32(airData_List[i].d_06nengbang_prop)).ToString();
                
                airData_List[i].d_07nanbang_prop = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_07nanbang_prop").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_07nanbang_prop = Convert.ToInt32(airData_List[i].d_07nanbang_prop) >= 0 ? airData_List[i].d_07nanbang_prop : (65536 + Convert.ToInt32(airData_List[i].d_07nanbang_prop)).ToString();
                
                airData_List[i].d_08gasyb_prop = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_08gasyb_prop").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_08gasyb_prop = Convert.ToInt32(airData_List[i].d_08gasyb_prop) >= 0 ? airData_List[i].d_08gasyb_prop : (65536 + Convert.ToInt32(airData_List[i].d_08gasyb_prop)).ToString();
                #endregion


                airData_List[i].d_11current_A = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_11current_A").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_12default_temp = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_12default_temp").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_13default_humid = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_13default_humid").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_14nengbang_dev = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_14nengbang_dev").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_15nanbang_dev = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_15nanbang_dev").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_16gasyb_dev = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_16gasyb_dev").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_17jesyb_dev = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_17jesyb_dev").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_18run_delay = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_18run_delay").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_19stop_delay = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_19stop_delay").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_20device_number = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_20device_number").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_21temp_cor = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_21temp_cor").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_22humid_cor = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_22humid_cor").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();

                airData_List[i].d_27set_highTemp = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_27set_highTemp").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_28set_lowTemp = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_28set_lowTemp").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_29set_highHumid = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_29set_highHumid").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_30set_lowHumid = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_30set_lowHumid").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();

                airData_List[i].d_36current_temp = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_36current_temp").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();
                airData_List[i].d_37current_humid = ds.Tables[$"{gloVar.ID_List[i]}"].AsEnumerable().Where(x => x.Field<string>("dCode") == "d_37current_humid").Select(x => x.Field<string>("dDataValue")).ToArray()[0].ToString();

            }

            Console.WriteLine($"Setting data for tabPage index: {i}");
            pictureBox_warning.BackgroundImage = airData_List[i].s_32warning == 0 ? Resources.air_notification_off_icon_127094 : Resources.air_warning_icon_127093;
            pictureBox_warning.BackColor = airData_List[i].s_32warning == 0 ? Color.LightGreen : Color.Red;
            //gaugeControl_warning.Gauges[0].AcceptOrder.

            label_tempMain.Text = airData_List[i].d_36current_temp + " C";
            label_tempStd.Text = airData_List[i].d_12default_temp + " C";

            label_humidMain.Text = airData_List[i].d_37current_humid + " %";
            label_humidStd.Text = airData_List[i].d_13default_humid + " %";

            warningsPage.SetData(panel_container, airData_List, i);

        }

        private void anyTabPage_Click(object sender, EventArgs e)
        {
            SetData();
        }

        private void pictureBox_warning_Click(object sender, EventArgs e)
        {
            this.Text += " - 경보 상세 내용";
            int i = 0;
            if (tabControls.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    i = tabControls.SelectedIndex;
                }));

            }
            else
            {
                i = tabControls.SelectedIndex;
            }

            //Console.WriteLine("TabPage Index: " + i);


            warningsPage.SetData(panel_container, airData_List, i);
            warningsPage.Visible = true;
        }
    }
}
