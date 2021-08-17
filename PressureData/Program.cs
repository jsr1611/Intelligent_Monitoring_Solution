using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Linq;
using CommonClassLibrary;
using EasyModbus;

namespace PressureData
{
    class Program
    {

        private static GlobalVariables gv = null;
        private static GlobalMethods gm = null;


        /// <summary>
        /// 메인함수
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            ////////////////////////////////////////////////////////


            // ini 읽기 //////
            IniFile ini = new IniFile();

            ini.Load(AppInfo.StartupPath + "\\" + "Setting.ini");

            string D_IP = ini["DBSetting"]["IP"].ToString();
            string D_SERVERNAME = ini["DBSetting"]["SERVERNAME"].ToString();
            string D_NAME = ini["DBSetting"]["DBNAME"].ToString();
            string D_ID = ini["DBSetting"]["ID"].ToString();
            string D_PW = ini["DBSetting"]["PW"].ToString();


            ////////////////////////////////////////////////////////


            Console.Title = "Pressure Data Collection App";

            GlobalVariables globalVariables = new GlobalVariables();
            GlobalMethods globalMethods = new GlobalMethods();
            gm = globalMethods;
            gv = globalVariables;

            string program_title = ini["PROGRAM"]["TITLE"].ToString();
            Console.Title = program_title; //"IMS Pressure Data Collector App";     // Program title on top left side of the window

            gv.dbServerName = D_SERVERNAME;
            gv.dbUID = D_ID;
            gv.dbPWD = D_PW;
            gv.dbName = D_NAME;
            gv.deviceTable = gv.dbName[0] + "_DEVICES_p";
            gv.sanghanHahanTable = gv.dbName[0] + "_SanghanHahan";
            gv.dataTable = gv.dbName[0] + "_DATATABLE_p";

            gv.sqlConStr = $@"Data Source={gv.dbServerName};Initial Catalog={gv.dbName};User id={gv.dbUID};Password={gv.dbPWD};Integrated Security=False";

            Console.WriteLine($"############ {program_title} has started. ############");


            gv.sqlConn = new SqlConnection(gv.sqlConStr);
            try
            {
                if (gv.sqlConn.State != ConnectionState.Open) { gv.sqlConn.Open(); }
            }
            catch
            {
                gv.sqlConStr = $@"Data Source={gv.dbServerName};Initial Catalog={gv.dbName};Integrated Security=True";
                gv.sqlConn = new SqlConnection(gv.sqlConStr);
                try
                {
                    if (gv.sqlConn.State != ConnectionState.Open) { gv.sqlConn.Open(); }
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"SQL Connection error: {ex2.Message}. {ex2.StackTrace}");
                }

            }


            gv.DevTbColumns = gm.GetTableColumnNames(gv, gv.deviceTable).ToArray();
            //g.UsageTableColumn_p = GetTableColumnNames(g.UsageTable_p);
            gv.ID_List = gm.GetSensorIDs(gv).ToArray();

            ValueTuple<string, int, int, int> S_TimeoutSettings_p = GetTimeSettings();
            //g.S_RETRYCOUNTER = S_TimeoutSettings_p.Item2;
            gv.COMPort_List = new string[] { S_TimeoutSettings_p.Item1 };
            //g.S_ConTimeOut = S_TimeoutSettings_p.Item3;
            //g.S_DelayTime = S_TimeoutSettings_p.Item4;

            //Console.WriteLine($"Time and other Settings:\nRetryCounter: {g.S_RETRYCOUNTER}, \nCOM port: {g.COM_PORT}, \nConTimeout:{g.S_ConTimeOut}, \nDelay:{g.S_DelayTime}, \nSqlConstr:{g.sqlConStr}");



            //List<int> slaveId_list = GetSensorIDs(); // new List<int>() { 51, 52, 53 };
            //Console.WriteLine("Sensor IDs ");
            for (int id_index = 0; id_index < gv.ID_List.Length; id_index++)
            {
                Console.WriteLine("ID: " + gv.ID_List[id_index]);
            }


            string[] portNames = SerialPort.GetPortNames();

            ModbusClient modbusSlave = new ModbusClient();
            modbusSlave.SerialPort = gv.COMPort_List[0];
            modbusSlave.Parity = System.IO.Ports.Parity.None;
            modbusSlave.StopBits = System.IO.Ports.StopBits.One;
            try
            {
                modbusSlave.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("ModbusClient 연결 시 에러 발생: " + e.Message + e.StackTrace);
            }
            bool res;
            int[] d1 = null;
            Dictionary<int, DateTime> resultCounter = new Dictionary<int, DateTime>();
            while (true && modbusSlave.Connected)
            {
                resultCounter.Clear();
                res = false;
                for (int i = 0; i < gv.ID_List.Length; i++)
                {
                    d1 = null;
                    try
                    {
                        modbusSlave.UnitIdentifier = (byte)gv.ID_List[i];
                        d1 = modbusSlave.ReadHoldingRegisters(60, 8);

                        res = prepareDataForStoring(d1, gv.ID_List[i]);
                        if (res)
                            resultCounter.Add(gv.ID_List[i], DateTime.Now);
                        else
                            Console.WriteLine("Data collection unsuccessful");


                    }
                    catch (Exception ex3)
                    {
                        Console.WriteLine($"??Error Message: {ex3.Message} 차압센서 데이터 수집이 안됨. slaveId:{gv.ID_List[i]}");

                    }

                    System.Threading.Thread.Sleep(100);
                }

                System.Threading.Thread.Sleep(1000);




                if (resultCounter.Count == gv.ID_List.Length)
                {
                    Console.WriteLine("\n--------------------DATA COLLECTION SUCCESSFUL-----------------------");
                    while (DateTime.Now.Second != 0)
                    {
                        System.Threading.Thread.Sleep(500);
                    }

                }


            }
            Console.WriteLine($"\n############ {program_title} has stopped due to the above exception. ############");
            Console.WriteLine("\nApplication window will be closed at " + DateTime.Now.AddMinutes(1) + ".\nPlease, check if you have connected the devices properly and you have the right settings in 'Settings.ini' file, and then run the application again. \nThank you!");
            System.Threading.Thread.Sleep(60000);
        }

        /// <summary>
        /// Time settings and COM Port for Pressure sensor data collection
        ///  g.S_RETRYCOUNTER = S_TimeoutSettings_p.Item2;
        ///  g.COM_PORT = S_TimeoutSettings_p.Item1;
        ///  g.S_ConTimeOut = S_TimeoutSettings_p.Item3;
        ///  g.S_DelayTime = S_TimeoutSettings_p.Item4;
        /// </summary>
        /// <returns></returns>
        private static ValueTuple<string, int, int, int> GetTimeSettings()
        {
            ValueTuple<string, int, int, int> values = new ValueTuple<string, int, int, int>();
            string tbName = gv.dbName[0] + "_TimeSettings";
            string sqlGet = $"IF EXISTS(SELECT * FROM sysobjects " +
                                                $" WHERE name = '{tbName}' AND xtype = 'U') SELECT settingName, settingValue FROM [{gv.dbName}].[dbo].[{tbName}] WHERE sensorCategory = 'pressure' AND settingCategory = 'collection'";
            //string sqlquery = $"SELECT * FROM [{dbName}].[dbo].[{dbName[0]}_TimeSettings] WHERE sensorCategory = 'particle' AND settingCategory = 'chart';";
            using (SqlConnection con = new SqlConnection(gv.sqlConStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sqlGet, con))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        int i = 0;
                        while (r.Read())
                        {
                            if (r["settingName"].ToString().Contains("1"))
                                values = (r.GetValue(1).ToString(), values.Item2, values.Item3, values.Item4);
                            else if (r["settingName"].ToString().Contains("2"))
                                values = (values.Item1, Convert.ToInt32(r.GetValue(1)), values.Item3, values.Item4);
                            else if (r["settingName"].ToString().Contains("3"))
                                values = (values.Item1, values.Item2, Convert.ToInt32(r.GetValue(1)), values.Item4);
                            else if (r["settingName"].ToString().Contains("4"))
                                values = (values.Item1, values.Item2, values.Item3, Convert.ToInt32(r.GetValue(1)));
                            i += 1;
                            if (i == 4)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return values;
        }

        private static bool connect(ModbusClient modbusSlave, string port)
        {
            bool res = false;
            modbusSlave.SerialPort = port;
            try
            {
                modbusSlave.Connect();
                Console.WriteLine($"Connected using COM Port: {port} ");
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nCouldn't connect to Modbus slave using port: {port}! Error Message: {ex.Message}. {ex.StackTrace}");
                modbusSlave.SerialPort = gv.COMPort_List[0];

                try
                {
                    modbusSlave.Connect();
                    Console.WriteLine($"Connected using port {gv.COMPort_List[0]}");
                    res = true;
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"\n\nCouldn't connect to Modbus slave using port: {gv.COMPort_List[0]}! Error Message: {ex2.Message}. {ex2.StackTrace}");
                }
            }
            return res;
        }



        /// <summary>
        /// 차압 센서 데이터 수집 프로그램 (무한 반복 함수)
        /// </summary>
        private static bool prepareDataForStoring(int[] d1, int sensorId)
        {
            bool dbInsertOK;
            // -- dp (diff pressure) data object initialization.
            DpData dpData = new DpData();

            string timestamp_p = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            dpData.sID = sensorId;
            dpData.pcTimestamp = timestamp_p;
            dpData.s_pa = d1[0].ToString();
            dpData.s_mbar = d1[1].ToString();
            dpData.s_kpa = d1[2].ToString();
            dpData.s_hpa = d1[3].ToString();
            dpData.s_mmH2o = String.Format("{0:0.00}", d1[4] / 100d);
            dpData.s_inchH2O = d1[5].ToString();
            dpData.s_mmHg = d1[6].ToString();
            dpData.s_inchHg = d1[7].ToString();

            dpData = SetData(dpData, "pressure");
            //Console.WriteLine("Checking usage is ok");


            Tuple<bool, string> resFromDb = StoreDataToDB(dpData);
            dbInsertOK = resFromDb.Item1;
            if (dbInsertOK)
            {
                Console.Write("\n" + resFromDb.Item2 + dpData.ToString() + " dbInsert_OK " + dbInsertOK);
            }
            else
            {
                Console.WriteLine("Error: " + resFromDb.Item2);
            }

            return dbInsertOK;
        }


        /// <summary>
        /// 수집된 데이터를 DB에 저장해주는 함수
        /// </summary>
        /// <param name="data">수집 데이터</param>
        /// <returns>수집이 잘 되면 true반환함</returns>
        private static Tuple<bool, string> StoreDataToDB(DpData data)
        {
            bool res = false;
            string txt = string.Empty;
            using (SqlConnection con = new SqlConnection(gv.sqlConStr))
            {
                if (gv.sqlConStr.Length > 0 && gv.ID_List.Contains(data.sID))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        SqlCommand insertCmd = new SqlCommand();
                        insertCmd.Connection = con;
                        string sqlInsertData = string.Empty;
                        //Console.Write($" Inserting into db table: {g.dbName} + {g.s_DATATABLE_p}"  );


                        //---------hpa 저장-------------------->
                        if (data.check_hpaOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'hpa'" + // sensorCode
                                        $", '{data.s_hpa}'" +
                                        $", '');";

                        //---------inchH2o 저장-------------------->
                        if (data.check_inchh2oOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'inchh2o'" + // sensorCode
                                        $", '{data.s_inchH2O}'" +
                                        $", '');";
                        //---------inchHg  저장-------------------->
                        if (data.check_inchhgOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'inchh2o'" + // sensorCode
                                        $", '{data.s_inchHg}'" +
                                        $", '');";

                        //---------kpa 저장-------------------->
                        if (data.check_kpaOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'kpa'" + // sensorCode
                                        $", '{data.s_kpa}'" +
                                        $", '');";


                        //---------mbar 저장-------------------->
                        if (data.check_mbarOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'mbar'" + // sensorCode
                                        $", '{data.s_mbar}'" +
                                        $", '');";

                        //---------mmH2O(Aqua) 저장-------------------->
                        if (data.check_mmh2oOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'mmh2o'" + // sensorCode
                                        $", '{data.s_mmH2o}'" +
                                        $", '');";

                        //---------mmHg 저장-------------------->
                        if (data.check_mmhgOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'mmhg'" + // sensorCode
                                        $", '{data.s_mmHg}'" +
                                        $", '');";


                        //---------pa 저장-------------------->
                        if (data.check_paOn)
                            sqlInsertData += $"INSERT INTO {gv.dataTable} VALUES(" +
                                        $"'{data.pcTimestamp}'" +
                                        $", {data.sID}" +
                                        $", 'pa'" + // sensorCode
                                        $", '{data.s_pa}'" +
                                        $", '');";


                        // -----------------DB Insert실행 부분------------->

                        insertCmd.CommandType = CommandType.Text;
                        insertCmd.CommandText = sqlInsertData;
                        //mutex_lock.WaitOne();
                        if (insertCmd.CommandText.Length > 0)
                        {
                            insertCmd.Transaction = transaction;
                            /*if (!DataAlreadyExists(data.GetID(), data.GetTimestamp()))
                            {*/
                            insertCmd.ExecuteNonQuery();
                            transaction.Commit();
                            res = true;
                            //}
                            /*else
                            {
                                txt = " 중복 ";
                            }*/
                        }
                        //mutex_lock.ReleaseMutex();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n" + ex.Message + ex.StackTrace);
                        try
                        {
                            transaction.Rollback();
                            txt = "DB Insert Failed";
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("\n" + ex2.Message + ex2.StackTrace);
                        }

                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }

            }
            return new Tuple<bool, string>(res, txt);
        }

        /// <summary>
        /// 수집여부 확인 및 Data클레스에 적용 후 데이터 반환
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorCategory"></param>
        /// <returns></returns>
        private static DpData SetData(DpData data, string sensorCategory)
        {

            List<string> tbColumns = gm.GetTableColumnNames(gv, gv.sanghanHahanTable);

            if (tbColumns.Count == 0)
            {
                Console.WriteLine($"{gv.sanghanHahanTable} table 존재하지 않거나 다른 에러가 발생했습니다.");
                return data;
            }
            string sql_select = $"SELECT {tbColumns[2]}, {tbColumns[7]} FROM [{gv.dbName}].[dbo].[{gv.sanghanHahanTable}] WHERE {tbColumns[0]} = '{sensorCategory}' AND {tbColumns[1]} = {data.sID}";

            using (SqlConnection con = new SqlConnection(gv.sqlConStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql_select, con))
                {
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (r[tbColumns[2]].Equals("hpa"))
                                data.check_hpaOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("inchh2o"))
                                data.check_inchh2oOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("inchhg"))
                                data.check_inchhgOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("kpa"))
                                data.check_kpaOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("mbar"))
                                data.check_mbarOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("mmh2o"))
                                data.check_mmh2oOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("mmhg"))
                                data.check_mmhgOn = r.GetString(1).Equals("Yes");
                            else if (r[tbColumns[2]].Equals("pa"))
                                data.check_paOn = r.GetString(1).Equals("Yes");
                        }
                    }
                }
            }

            return data;
        }




    }
}