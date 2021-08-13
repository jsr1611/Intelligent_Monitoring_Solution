using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AirConDashboard
{
    public partial class warningsPage : UserControl
    {
        private List<Label> iport_list = null;
        private List<Label> wc_list = null;
        private List<Label> wc2_list = null;
        private List<Label> wc5_list = null;
        private List<Label> wc6_list = null;
        private List<Label> wc7_list = null;

        public warningsPage()
        {
            InitializeComponent();
            iport_list = new List<Label>() { ip_00, ip_01, ip_02, ip_03, ip_04, ip_05, ip_06, ip_07, ip_08, ip_09, ip_10, ip_11, ip_12, ip_13, ip_14, ip_15 };
            wc_list = new List<Label>() { wc_00, wc_01, wc_02, wc_03, wc_04, wc_05, wc_06, wc_07_sui, wc_08_gybsu, wc_09_pesu, wc_10_led6, wc_11_led5, wc_12_led4, wc_13_led3, wc_14_led2, wc_15_led1 };
            wc2_list = new List<Label>() { wc2_00, wc2_01, wc2_02, wc2_03, wc2_04, wc2_05, wc2_06, wc2_07, wc2_08, wc2_09, wc2_10, wc2_11, wc2_12, wc2_13, wc2_14, wc2_15 };

            wc5_list = new List<Label>() { wc5_00, wc5_08, wc5_09, wc5_10, wc5_11, wc5_12, wc5_13, wc5_14, wc5_15 };
            wc6_list = new List<Label>() { wc6_00, wc6_08, wc6_09, wc6_10, wc6_11, wc6_12, wc6_13, wc6_14, wc6_15 };
            wc7_list = new List<Label>() { wc7_00, wc7_08, wc7_09, wc7_10, wc7_11, wc7_12, wc7_13, wc7_14, wc7_15 };
        }


        public void SetData(Form activeForm, CommonClassLibrary.AirData[] airData_List, int i)
        {
            int btn_x = activeForm.Bounds.Right - 50;
            int btn_y = activeForm.Bounds.Top + 25;
            button_exit.SetBounds(btn_x, btn_y, button_exit.Bounds.Width, button_exit.Bounds.Height);

            pBox_s_01songpunggi_run.BackColor = airData_List[i].s_01songpunggi_run == 0 ? Color.WhiteSmoke : Color.Green;
            pBox_s_02nengbang1_run.BackColor = airData_List[i].s_02nengbang1_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox2_nengbang2.BackColor = airData_List[i].s_03nengbang2_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox3_nanbang1.BackColor = airData_List[i].s_04nanbang1_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox4_nanbang2.BackColor = airData_List[i].s_05nanbang2_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox5_nanbang3.BackColor = airData_List[i].s_06nanbang3_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox6_nanbang4.BackColor = airData_List[i].s_07nanbang4_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox7_gasyp1.BackColor = airData_List[i].s_08gasyb1_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox8_gasyp2.BackColor = airData_List[i].s_09gasyb2_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox9_gybsu.BackColor = airData_List[i].s_10gybsu_run == 0 ? Color.WhiteSmoke : Color.Green;
            pictureBox10_pesu.BackColor = airData_List[i].s_11besu_run == 0 ? Color.WhiteSmoke : Color.Green;

            pictureBox11_songpung_w.BackColor = airData_List[i].s_17songpunggi_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox12_nengbang1_w.BackColor = airData_List[i].s_18nengbang1_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox13__nengbang2_w.BackColor = airData_List[i].s_19nengbang2_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox14_nanbang_w.BackColor = airData_List[i].s_20nanbanggi_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox15_gasyb_w.BackColor = airData_List[i].s_21gasybgi_warning == 0 ? Color.LightGreen : Color.Red;

            pictureBox16_sensor_w.BackColor = airData_List[i].s_22sensor_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox17_nusu_w.BackColor = airData_List[i].s_23nusu_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox18_gybsu_w.BackColor = airData_List[i].s_24gybsu_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox19_pesu_w.BackColor = airData_List[i].s_25besu_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox20_lowTemp_w.BackColor = airData_List[i].s_26lowTemp_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox21_highTemp_w.BackColor = airData_List[i].s_27highTemp_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox22_lowHumid_w.BackColor = airData_List[i].s_28lowHumid_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox23_highHumid_w.BackColor = airData_List[i].s_29highHumid_warning == 0 ? Color.LightGreen : Color.Red;
            pictureBox24_fire_w.BackColor = airData_List[i].s_30fire_warning == 0 ? Color.LightGreen : Color.Red;

            pictureBox25_warning_w.BackColor = airData_List[i].s_32warning == 0 ? Color.LightGreen : Color.Red;


            lbl_11_currAmp.Text = airData_List[i].d_11current_A + " A";
            lbl_12_stdTemp.Text = airData_List[i].d_12default_temp + " C"; ;
            lbl_13_stdHumid.Text = airData_List[i].d_13default_humid + " %";
            lbl_14_nengbang.Text = airData_List[i].d_14nengbang_dev + " C";
            lbl_15_nanbang.Text = airData_List[i].d_15nanbang_dev + " C";
            lbl_16_gasyb.Text = airData_List[i].d_16gasyb_dev + " %";
            lbl_17_jesyb.Text = airData_List[i].d_17jesyb_dev + " %";
            lbl_18_runDelay.Text = airData_List[i].d_18run_delay + " 초";
            lbl_19_stopDelay.Text = airData_List[i].d_19stop_delay + " 초";
            lbl_20_dID.Text = airData_List[i].d_20device_number + " 번";
            lbl_21_corTemp.Text = airData_List[i].d_21temp_cor + " C";
            lbl_22_corHumid.Text = airData_List[i].d_22humid_cor + " %";

            lbl_27_stdHighTemp.Text = airData_List[i].d_27set_highTemp + " C";
            lbl_28_stdLowTemp.Text = airData_List[i].d_28set_lowTemp + " C";
            lbl_29_stdHighHumid.Text = airData_List[i].d_29set_highHumid + " %";
            lbl_30_stdLowHumid.Text = airData_List[i].d_30set_lowHumid + " %";

            lbl_36_Temp.Text = airData_List[i].d_36current_temp + " C";
            lbl_37_Humid.Text = airData_List[i].d_37current_humid + " %";

            //iport_list = 입력포트
            //wc_list =  경보내용1
            //wc2_list = 경보내용2
            //runStop = 운전정지
            //wc5_list = 냉방비례
            //wc6_list = 난방비례
            //wc7_list =가습비례

            //-------------------------- iport_list = 입력포트 -------------------------- 
            string binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_02inputPort), 2);
            string TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= iport_list.Count)
            {
                for (int a = 0; a < iport_list.Count; a++)
                {
                    iport_list[a].Text = TwosComplement[(TwosComplement.Length - 1) - a].ToString();
                }
            }
            //Console.Write($"Original input: {airData_List[i].d_02inputPort}, two's complement: {TwosComplement}.");

            //-------------------------- wc_list =  경보내용1 -------------------------- 
            int num = 0;
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_03warningContent1), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= wc_list.Count)
            {
                for (int a = 0; a < wc_list.Count; a++)
                {
                    int.TryParse(TwosComplement[(TwosComplement.Length - 1) - a].ToString(), out num);
                    wc_list[a].BackColor = num == 1 ? Color.Green : Color.Transparent;
                }
            }
            //Console.Write($"Original input: {airData_List[i].d_03warningContent1}, two's complement: {TwosComplement}.");

            //-------------------------- wc2_list = 경보내용2 -------------------------- 
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_04warningContent2), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= wc2_list.Count)
            {
                for (int a = 0; a < wc2_list.Count; a++)
                {
                    int.TryParse(TwosComplement[(TwosComplement.Length - 1) - a].ToString(), out num);
                    wc2_list[a].BackColor = num == 1 ? Color.Green : Color.Transparent;
                }
            }
            //Console.Write($"Original input: {airData_List[i].d_04warningContent2}, two's complement: {TwosComplement}.");

            //-------------------------- runStop = 운전정지 -------------------------- 
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_05run_stop), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= 15)
            {
                int.TryParse(TwosComplement[7].ToString().ToString(), out num);
                runStop08_delay.Text = num.ToString();
                runStop08_delay.BackColor = num == 1 ? Color.Green : Color.Transparent;

                int.TryParse(TwosComplement[15].ToString().ToString(), out num);
                runStop00_runStop.Text = num == 1 ? "운전": "정지";
                runStop00_runStop.BackColor = num == 1 ? Color.Green : Color.Transparent;
            }
            // Console.Write($"Original input: {airData_List[i].d_05run_stop}, two's complement: {TwosComplement}.");

            //-------------------------- wc5_list = 냉방비례 -------------------------- 
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_06nengbang_prop), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= wc5_list.Count)
            {
                for (int a = 1; a < wc5_list.Count; a++)
                {
                    int.TryParse(TwosComplement[(TwosComplement.Length - 8) - a].ToString(), out num);
                    wc5_list[a].Text = num.ToString();
                    wc5_list[a].BackColor = num == 1 ? Color.Green : Color.Transparent;
                }
            }
            //Console.Write($"Original input: {airData_List[i].d_06nengbang_prop}, two's complement: {TwosComplement}.");

            //-------------------------- wc6_list = 난방비례 -------------------------- 
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_07nanbang_prop), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= wc6_list.Count)
            {
                for (int a = 1; a < wc6_list.Count; a++)
                {
                    int.TryParse(TwosComplement[(TwosComplement.Length - 8) - a].ToString(), out num);
                    wc6_list[a].Text = num.ToString();
                    wc6_list[a].BackColor = num == 1 ? Color.Green : Color.Transparent;
                }
            }
            //Console.Write($"Original input: {airData_List[i].d_07nanbang_prop}, two's complement: {TwosComplement}.");

            //-------------------------- wc7_list = 가습비례 -------------------------- 
            binary = Convert.ToString(Convert.ToUInt16(airData_List[i].d_08gasyb_prop), 2);
            TwosComplement = "0" + GetTwosComplement(binary);
            TwosComplement = CheckLengthOfBits(TwosComplement);
            if (TwosComplement.Length >= wc7_list.Count)
            {
                for (int a = 1; a < wc7_list.Count; a++)
                {
                    int.TryParse(TwosComplement[(TwosComplement.Length - 8) - a].ToString(), out num);
                    wc7_list[a].Text = num.ToString();
                    wc7_list[a].BackColor = num == 1 ? Color.Green : Color.Transparent;
                }
            }
            // Console.Write($"Original input: {airData_List[i].d_08gasyb_prop}, two's complement: {TwosComplement}.");


            //--------------------------

            // input:         111000100000000
            //28928 => two's: 000111100000000


        }

        /// <summary>
        /// Complement the string with 0s if it is less than 16 in length
        /// </summary>
        /// <param name="twosComplement"></param>
        /// <returns></returns>
        private string CheckLengthOfBits(string twosComplement)
        {
            while (twosComplement.Length < 16)
            {
                twosComplement = "0" + twosComplement;
            }

            return twosComplement;
        }



        // Method to find two's complement
        public static string findTwoscomplement(StringBuilder str)
        {
            int n = str.Length;

            // Traverse the string to get
            // first '1' from the last of string
            int i;
            for (i = n - 1; i >= 0; i--)
            {
                if (str[i] == '1')
                {
                    break;
                }
            }

            // If there exists no '1' concat 1
            // at the starting of string
            if (i == -1)
            {
                return "1" + str;
            }

            // Continue traversal after the
            // position of first '1'
            for (int k = i - 1; k >= 0; k--)
            {
                // Just flip the values
                if (str[k] == '1')
                {
                    str.Remove(k, k + 1 - k).Insert(k, "0");
                }
                else
                {
                    str.Remove(k, k + 1 - k).Insert(k, "1");
                }
            }

            // return the modified string
            return str.ToString();
        }

        /// <summary>
        /// Get two's complement for a one's complement string input;
        /// </summary>
        /// <param name="onesComplement"></param>
        /// <returns></returns>
        private string GetTwosComplement(string onesComplement)
        {
            onesComplement = GetOnesComplement(onesComplement);
            string result = string.Empty;
            int carry = 1;
            int num = 0;
            if (onesComplement.Length > 1)
            {
                int.TryParse(onesComplement[onesComplement.Length - 1].ToString(), out num);
                if (num == 0)
                {
                    result = onesComplement.Substring(0, onesComplement.Length - 1) + "1";
                    //Console.WriteLine($"Input: {onesComplement}, One'sComplement: {result}");
                    //return result;
                }
                else
                {
                    for (int i = onesComplement.Length - 1; i >= 0; i--)
                    {
                        //Console.Write(" " + onesComplement[i] + "-> ");
                        int.TryParse(onesComplement[i].ToString(), out num);
                        if (carry == 0)
                        {
                            if (num == 1)
                            {
                                carry = 0;
                                result = "1" + result;
                            }
                            else
                            {
                                carry = 0;
                                result = "0" + result;
                            }
                        }
                        else
                        {
                            if (num == 1)
                            {
                                carry = 1;
                                result = "0" + result;
                            }
                            else
                            {
                                carry = 0;
                                result = "1" + result;
                            }
                        }
                        //Console.Write(result[0]);

                    }
                    //Console.WriteLine($"Input: {onesComplement}, Two'sComplement: {result}");
                }
            }
            return result;
        }
        /// <summary>
        /// Get one's complement for a binary string input;
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        private string GetOnesComplement(string binary)
        {
            string result = string.Empty;
            int num = 0;
            if (binary.Length > 1)
            {
                for (int i = 0; i < binary.Length; i++)
                {
                    //Console.Write(" " + binary[i] + "-> ");
                    int.TryParse(binary[i].ToString(), out num);
                    result += num == 1 ? "0" : "1";
                    //Console.Write(result[i]);
                }
                // Console.WriteLine($"Input: {binary}, One'sComplement: {result}");
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            int index = Form1.ActiveForm.Text.IndexOf("-");
            Form1.ActiveForm.Text = Form1.ActiveForm.Text.Substring(0, index);
        }
    }
}
