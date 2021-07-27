using CommonClassLibrary;
using System;

namespace AirConData
{
    public class AirData
    {

        // 경보 데이터 
        // ReadCoils
        public int s_01songpunggi_run { get; set; }
        public int s_02nengbang1_run { get; set; }
        public int s_03nengbang2_run { get; set; }
        public int s_04nanbang1_run { get; set; }
        public int s_05nanbang2_run { get; set; }
        public int s_06nanbang3_run { get; set; }
        public int s_07nanbang4_run { get; set; }
        public int s_08gasyb1_run { get; set; }
        public int s_09gasyb2_run { get; set; }
        public int s_10gybsu_run { get; set; }
        public int s_11besu_run { get; set; }

        public int s_17songpunggi_warning { get; set; }
        public int s_18nengbang1_warning { get; set; }
        public int s_19nengbang2_warning { get; set; }
        public int s_20nanbanggi_warning { get; set; }
        public int s_21gasybgi_warning { get; set; }
        public int s_22sensor_warning { get; set; }
        public int s_23nusu_warning { get; set; }
        public int s_24gybsu_warning { get; set; }
        public int s_25besu_warning { get; set; }
        public int s_26lowTemp_warning { get; set; }
        public int s_27highTemp_warning { get; set; }
        public int s_28lowHumid_warning { get; set; }
        public int s_29highHumid_warning { get; set; }
        public int s_30fire_warning { get; set; }

        public int s_32warning { get; set; }

        // 데이터 값
        // ReadHoldingRegister
        public string d_02inputPort { get; set; }
        public string d_03warningContent1 { get; set; }
        public string d_04warningContent2 { get; set; }
        public string d_05run_stop { get; set; }
        public string d_06nengbang_prop { get; set; } // proportion
        public string d_07nanbang_prop { get; set; }
        public string d_08gasyb_prop { get; set; }

        public string d_11current_A { get; set; }
        public string d_12default_temp { get; set; }
        public string d_13default_humid { get; set; }
        public string d_14nengbang_dev { get; set; } // deviation
        public string d_15nanbang_dev { get; set; }
        public string d_16gasyb_dev { get; set; }
        public string d_17jesyb_dev { get; set; }
        public string d_18run_delay { get; set; }
        public string d_19stop_delay { get; set; }
        public string d_20device_number { get; set; }
        public string d_21temp_cor { get; set; }  // correction
        public string d_22humid_cor { get; set; }

        public string d_27set_highTemp { get; set; }
        public string d_28set_lowTemp { get; set; }
        public string d_29set_highHumid { get; set; }
        public string d_30set_lowHumid { get; set; }

        public string d_36current_temp { get; set; }
        public string d_37current_humid { get; set; }



        private int[] stateVals { get; set; }
        private string[] stateNames { get; set; }

        private string[] non_stateVals { get; set; }
        private string[] non_stateNames { get; set; }

        


        /// <summary>
        /// Initialize variables with values from sensor data.
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        public void setData(bool[] d0, int[] d1)
        {
            s_01songpunggi_run = d0[0] == false ? 0 : 1;
            s_02nengbang1_run = d0[1] == false ? 0 : 1;
            s_03nengbang2_run = d0[2] == false ? 0 : 1;
            s_04nanbang1_run = d0[3] == false ? 0 : 1;
            s_05nanbang2_run = d0[4] == false ? 0 : 1;
            s_06nanbang3_run = d0[5] == false ? 0 : 1;
            s_07nanbang4_run = d0[6] == false ? 0 : 1;
            s_08gasyb1_run = d0[7] == false ? 0 : 1;
            s_09gasyb2_run = d0[8] == false ? 0 : 1;
            s_10gybsu_run = d0[9] == false ? 0 : 1;
            s_11besu_run = d0[10] == false ? 0 : 1;
            s_17songpunggi_warning = d0[16] == false ? 0 : 1;
            s_18nengbang1_warning = d0[17] == false ? 0 : 1;
            s_19nengbang2_warning = d0[18] == false ? 0 : 1;
            s_20nanbanggi_warning = d0[19] == false ? 0 : 1;
            s_21gasybgi_warning = d0[20] == false ? 0 : 1;
            s_22sensor_warning = d0[21] == false ? 0 : 1;
            s_23nusu_warning = d0[22] == false ? 0 : 1;
            s_24gybsu_warning = d0[23] == false ? 0 : 1;
            s_25besu_warning = d0[24] == false ? 0 : 1;
            s_26lowTemp_warning = d0[25] == false ? 0 : 1;
            s_27highTemp_warning = d0[26] == false ? 0 : 1;
            s_28lowHumid_warning = d0[27] == false ? 0 : 1;
            s_29highHumid_warning = d0[28] == false ? 0 : 1;
            s_30fire_warning = d0[29] == false ? 0 : 1;
            s_32warning = d0[31] == false ? 0 : 1;

            d_02inputPort = d1[1].ToString();
            d_03warningContent1 = d1[2].ToString();
            d_04warningContent2 = d1[3].ToString();
            d_05run_stop = d1[4].ToString();
            d_06nengbang_prop = d1[5].ToString();
            d_07nanbang_prop = d1[6].ToString();
            d_08gasyb_prop = d1[7].ToString();
            d_11current_A = String.Format("{0:0.0}", d1[10] / 10.0D);            // String.Format("{0:0.00}", non_stateVals[i] / 10.0D)
            d_12default_temp = String.Format("{0:0.0}", d1[11] / 10.0D);          // if((i >=7 && i <= 13) || (i>=17 && i<= 24))
            d_13default_humid = String.Format("{0:0.0}", d1[12] / 10.0D);             // i>=10 && i <= 16
            d_14nengbang_dev = String.Format("{0:0.0}", d1[13] / 10.0D);
            d_15nanbang_dev = String.Format("{0:0.0}", d1[14] / 10.0D);
            d_16gasyb_dev = String.Format("{0:0.0}", d1[15] / 10.0D);
            d_17jesyb_dev = String.Format("{0:0.0}", d1[16] / 10.0D);
            d_18run_delay = d1[17].ToString();
            d_19stop_delay = d1[18].ToString();
            d_20device_number = d1[19].ToString();
            d_21temp_cor = String.Format("{0:0.0}", d1[20] / 10.0D);
            d_22humid_cor = String.Format("{0:0.0}", d1[21] / 10.0D);
            d_27set_highTemp = String.Format("{0:0.0}", d1[26] / 10.0D);
            d_28set_lowTemp = String.Format("{0:0.0}", d1[27] / 10.0D);
            d_29set_highHumid = String.Format("{0:0.0}", d1[28] / 10.0D);
            d_30set_lowHumid = String.Format("{0:0.0}", d1[29] / 10.0D);
            d_36current_temp = String.Format("{0:0.0}", d1[35] / 10.0D);
            d_37current_humid = String.Format("{0:0.0}", d1[36] / 10.0D);
            
        }




        public bool insertIntoDB(GlobalVariables gloVar)
        {
            bool insert_OK = false;

            stateVals = getAllStateData();
            stateNames = getStateNames();

            non_stateVals = getAllNonStateData();
            non_stateNames = getNonStateNames();

            if(gloVar.sqlConn == null)
            {
                gloVar.sqlConn = new System.Data.SqlClient.SqlConnection();
            }

            if (gloVar.sqlConn.ConnectionString.Length == 0)
            {
                gloVar.sqlConn.ConnectionString = gloVar.sqlConStr;
                gloVar.sqlConn.Open();

            }
            else
            {
                if (gloVar.sqlConn.State != System.Data.ConnectionState.Open)
                {
                    gloVar.sqlConn.Open();
                }
            }

            if (gloVar.sqlCmd == null)
            {
                gloVar.sqlCmd = new System.Data.SqlClient.SqlCommand();
            }
             gloVar.transaction = gloVar.sqlConn.BeginTransaction();


            // VALUES( DateAndTime NVARCHAR(55) not null, dID int null, dCode NVARCHAR(250) null, dDataValue NVARCHAR(55) null, remarks NVARCHAR(255) null);
            // VALUES( 'datetime', deviceID, 'deviceCode', 'value', 'remarks');
            gloVar.sqlDefltCmdText = $"INSERT INTO {gloVar.dataTable} VALUES('{gloVar.DateAndTime}', {gloVar.dID}, ";
            gloVar.sqlCmdText = string.Empty;
            for (int i = 0; i < stateVals.Length; i++)
            {
                gloVar.sqlCmdText += gloVar.sqlDefltCmdText + $"'{stateNames[i]}', '{stateVals[i]}', '');";
            }

            for (int i = 0; i < non_stateVals.Length; i++)
            {
                gloVar.sqlCmdText += gloVar.sqlDefltCmdText + $"'{non_stateNames[i]}', '{non_stateVals[i]}', '');";
            }


            //Console.Write("\n" + sqlString + "\n");


            gloVar.sqlCmd.CommandText = gloVar.sqlCmdText;
            gloVar.sqlCmd.Connection = gloVar.sqlConn;
            gloVar.sqlCmd.Transaction = gloVar.transaction;
            try
            {
                gloVar.sqlCmd.ExecuteNonQuery();
                gloVar.transaction.Commit();
                insert_OK = true;
            }
            catch (Exception ex)
            {
                Console.Write($"DB Insertion Error: {ex.Message}. {ex.StackTrace}\n");
                try
                {
                    gloVar.transaction.Rollback();
                }
                catch (Exception)
                {
                    Console.Write(" transaction.Rollback() Error! ");

                }
            }
            finally
            {
                gloVar.transaction.Dispose();


            }

            return insert_OK;
        }






        /// <summary>
        /// Return all state data as an array of ints
        /// </summary>
        /// <returns></returns>
        public int[] getAllStateData()
        {
            return new int[] { s_01songpunggi_run, s_02nengbang1_run, s_03nengbang2_run, s_04nanbang1_run, s_05nanbang2_run, s_06nanbang3_run, s_07nanbang4_run, s_08gasyb1_run, s_09gasyb2_run, s_10gybsu_run, s_11besu_run, s_17songpunggi_warning, s_18nengbang1_warning, s_19nengbang2_warning, s_20nanbanggi_warning, s_21gasybgi_warning, s_22sensor_warning, s_23nusu_warning, s_24gybsu_warning, s_25besu_warning, s_26lowTemp_warning, s_27highTemp_warning, s_28lowHumid_warning, s_29highHumid_warning, s_30fire_warning, s_32warning };
        }


        /// <summary>
        /// Return all non-state values
        /// </summary>
        /// <returns></returns>
        public string[] getAllNonStateData()
        {
            return new string[] {
                d_02inputPort, d_03warningContent1, d_04warningContent2, d_05run_stop, d_06nengbang_prop, d_07nanbang_prop, d_08gasyb_prop,
                d_11current_A, d_12default_temp, d_13default_humid, d_14nengbang_dev, d_15nanbang_dev, d_16gasyb_dev, d_17jesyb_dev,
                d_18run_delay, d_19stop_delay, d_20device_number, d_21temp_cor, d_22humid_cor, d_27set_highTemp, d_28set_lowTemp, d_29set_highHumid,
                d_30set_lowHumid, d_36current_temp, d_37current_humid
            };
        }



        /// <summary>
        /// Return dCode for each data
        /// </summary>
        /// <returns></returns>
        internal string[] getNonStateNames()
        {
            return new string[] {
                "d_02inputPort", "d_03warningContent1", "d_04warningContent2", "d_05run_stop", "d_06nengbang_prop", "d_07nanbang_prop",
                "d_08gasyb_prop", "d_11current_A", "d_12default_temp", "d_13default_humid", "d_14nengbang_dev", "d_15nanbang_dev",
                "d_16gasyb_dev", "d_17jesyb_dev", "d_18run_delay","d_19stop_delay", "d_20device_number", "d_21temp_cor", "d_22humid_cor",
                "d_27set_highTemp", "d_28set_lowTemp", "d_29set_highHumid", "d_30set_lowHumid", "d_36current_temp", "d_37current_humid"
            };
        }



        /// <summary>
        /// Return dCode for each data
        /// </summary>
        /// <returns></returns>
        internal string[] getStateNames()
        {
            return new string[] {"s_01songpunggi_run", "s_02nengbang1_run" , "s_03nengbang2_run", "s_04nanbang1_run", "s_05nanbang2_run",
                "s_06nanbang3_run",  "s_07nanbang4_run", "s_08gasyb1_run", "s_09gasyb2_run", "s_10gybsu_run", "s_11besu_run", "s_17songpunggi_warning",
                "s_18nengbang1_warning", "s_19nengbang2_warning", "s_20nanbanggi_warning",  "s_21gasybgi_warning", "s_22sensor_warning", "s_23nusu_warning",
                "s_24gybsu_warning", "s_25besu_warning", "s_26lowTemp_warning", "s_27highTemp_warning", "s_28lowHumid_warning", "s_29highHumid_warning", "s_30fire_warning", "s_32warning" };
        }



        /// <summary>
        /// Return all data as txt
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $" 장비번호: {d_20device_number}번, 경보: {s_01songpunggi_run}, {s_02nengbang1_run}, {s_03nengbang2_run}, {s_04nanbang1_run}, " +
                $"{s_05nanbang2_run}, {s_06nanbang3_run},  {s_07nanbang4_run}, {s_08gasyb1_run}, {s_09gasyb2_run}, {s_10gybsu_run}, {s_11besu_run}, {s_17songpunggi_warning}" +
                $", {s_18nengbang1_warning}, {s_19nengbang2_warning}, {s_20nanbanggi_warning},  {s_21gasybgi_warning}, {s_22sensor_warning}, {s_23nusu_warning}" +
                $", {s_24gybsu_warning}, {s_25besu_warning}, {s_26lowTemp_warning}, {s_27highTemp_warning}, {s_28lowHumid_warning}, {s_29highHumid_warning}, {s_30fire_warning}, {s_32warning}" +
                $", \n출력값: {d_02inputPort}, {d_03warningContent1}, {d_04warningContent2}, {d_05run_stop}, {d_06nengbang_prop}, {d_07nanbang_prop}, {d_08gasyb_prop}" +
                $", {d_11current_A}A, {d_12default_temp}°C, {d_13default_humid}%, {d_14nengbang_dev}°C, {d_15nanbang_dev}°C, {d_16gasyb_dev}%, " +
                $"{d_17jesyb_dev}%, {d_18run_delay}초, {d_19stop_delay}초, {d_21temp_cor}°C, {d_22humid_cor}%, {d_27set_highTemp}°C, {d_28set_lowTemp}°C, {d_29set_highHumid}%, " +
                $"{d_30set_lowHumid}%, {d_36current_temp}°C, {d_37current_humid}%";
        }
    }
}
