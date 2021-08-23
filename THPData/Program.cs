using CommonClassLibrary;
using EasyModbus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;

namespace THPData
{
    class Program
    {

        private static string program_title = string.Empty;
        /// <summary>
        /// 데이터 수집이 안될때 기존데이터를 쓰기 위해서 기다리는 (delay)시간.
        /// </summary>
        private static int DELAYTIME = 40;
        private static string folderPath = AppInfo.StartupPath + "\\Log_" + DateTime.Now.ToString("yyyyMM") + ".txt";


        static bool exitSystem = false;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            // LOG FILE에 저장함.
            try
            {
                File.AppendAllLines(folderPath, new string[] { $"프로그램을 종료했습니다. {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} 사유: 'Exiting system due to external CTRL - C, or process kill, or shutdown' " });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex.Message + ex.StackTrace + " ");
            }
            Console.WriteLine($"\n############ {program_title} has stopped due to the above exception or error. ############");
            Console.WriteLine("\nApplication window will be closed at " + DateTime.Now.AddMinutes(1) + ".\nPlease, check if you have connected the devices properly and you have the right settings in 'Settings.ini' file, and then run the application again. \nThank you!");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion



        static void Main(string[] args)
        {
            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            GlobalVariables gloVar = new GlobalVariables();

            // ini 읽기 //////
            IniFile ini = new IniFile();
            ini.Load(AppInfo.StartupPath + "\\" + "Setting.ini");
            program_title = ini["PROGRAM"]["TITLE"].ToString();
            Console.Title = program_title; //"IMS Particle Data Collector App";     // Program title on top left side of the window
            string D_SERVERNAME = ini["DBSetting"]["SERVERNAME"].ToString();
            string D_NAME = ini["DBSetting"]["DBNAME"].ToString();
            string D_ID = ini["DBSetting"]["ID"].ToString();
            string D_PW = ini["DBSetting"]["PW"].ToString();

            
            Console.WriteLine($"############ {program_title} has started. ############");
            
            // LOG FILE에 저장함.
            try
            {
                File.AppendAllLines(folderPath, new string[] { $"프로그램을 시작했습니다. {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} " });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex.Message + ex.StackTrace + " ");
            }


            gloVar.dbName = D_NAME;
            gloVar.dataTable = gloVar.dbName[0] + "_DATATABLE";
            gloVar.deviceTable = gloVar.dbName[0] + "_DEVICES";
            gloVar.dbUID = D_ID;
            gloVar.dbPWD = D_PW;
            gloVar.dbServerName = D_SERVERNAME;
            gloVar.sanghanHahanTable = gloVar.dbName[0] + "_SanghanHahan";


            gloVar.sqlConn = new SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=False; MultipleActiveResultSets=True";

            GlobalMethods.printDataBaseDetails(gloVar);
            SpcData spcData = new SpcData();



            gloVar.DevTbColumns = GlobalMethods.GetTableColumnNames(gloVar, gloVar.deviceTable).ToArray();
            if (gloVar.DevTbColumns != null && gloVar.DevTbColumns.Length > 0)
            {
                gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[0]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
                gloVar.ID_List = GlobalMethods.GetSensorIDs(gloVar).ToArray();  //Array.ConvertAll(GlobalMethods.GetColumnDataAsList(gloVar, "int", "sID", gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
                gloVar.S_DTColumns = GlobalMethods.GetTableColumnNames(gloVar, gloVar.dataTable).ToArray();
                gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[5]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
                gloVar.IPAddress_List = GlobalMethods.GetColumnDataAsList(gloVar, "string", gloVar.DevTbColumns[5], gloVar.sqlCmdText).ToArray();
                gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[6]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
                gloVar.Port_List = Array.ConvertAll(GlobalMethods.GetColumnDataAsList(gloVar, "string", gloVar.DevTbColumns[6], gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
                
                gloVar.modbusClient_List = new ModbusClient[gloVar.ID_List.Length];
                gloVar.bad_clientFlag_List = new bool[gloVar.ID_List.Length];
                System.Threading.Thread[] thread_List = new System.Threading.Thread[gloVar.ID_List.Length];

                //ErrorMessage 정보를 저장하는 DB테이블이 존재하는지 확인하는 부분.
                string errorMsgTableName = gloVar.dbName[0] + "_ERROR_MESSAGES";
                gloVar.ErrorMsgTable = new ValueTuple<string, bool, List<string>>(errorMsgTableName, GlobalMethods.IfTableExists(gloVar.sqlConStr, errorMsgTableName), new List<string>());
                if (!gloVar.ErrorMsgTable.Item2 || gloVar.ErrorMsgTable.Item3.Count == 0)
                {
                    GlobalMethods.ErrorMsgCollect(gloVar, errorMsgTableName);
                }


                string THREADINFO = string.Empty;

                int i = 0;
                while (gloVar.ID_List.Length > 0)
                {
                    while (i < gloVar.ID_List.Length)
                    {
                        try
                        {
                            if (thread_List[i] == null || !thread_List[i].IsAlive || thread_List[i].ThreadState == System.Threading.ThreadState.Aborted) // thread_List[i].ThreadState != System.Threading.ThreadState.Running || 
                            {

                                if (thread_List[i] == null)
                                    THREADINFO += "|\t" + i + "." + "THREAD " + i + " IS NULL! \t|";
                                else if (!thread_List[i].IsAlive)
                                    THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS NOT ALIVE! \t|";
                                else if (thread_List[i].ThreadState == System.Threading.ThreadState.Aborted)
                                    THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS ABORTED! \t|";
                                else
                                {
                                    THREADINFO += "|\t" + i + "." + thread_List[i].Name + "'s STATE IS SOMETHING ELSE, BUT NOT ALIVE! \t|";
                                }
                                SpcData spcData1 = new SpcData();
                                spcData1.setSqlFields(gloVar.sqlConn);
                                thread_List[i] = new System.Threading.Thread(() => StartProgram(spcData1, gloVar, i));

                                if (thread_List[i].Name == null || thread_List[i].Name.Length == 0)
                                    thread_List[i].Name = "THREAD " + i;
                                thread_List[i].IsBackground = true;
                                thread_List[i].Start();
                            }
                            else
                            {
                                //THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS ALIVE! \t|";
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                        catch (Exception e)
                        {
                            // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                            try
                            {
                                
                                    File.AppendAllLines(folderPath, new string[] { "THREADING " + e.Message + e.StackTrace });
                                
                                //GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e, "THREADING", "Thread Creation", "THREAD 생성 시 에러 발생. ", gloVar.ID_List[i], gloVar.modbusClient_List[i].IPAddress, gloVar.Port_List[i].ToString(), "");
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                            }
                            //Console.WriteLine(e.ToString());
                        }
                        i += 1;
                    }
                    i = 0;
                    //Console.WriteLine(" " + THREADINFO + " ");
                    System.Threading.Thread.Sleep(60000);
                    Console.WriteLine($"---------------------- Local Time: {DateTime.Now} ----------------------");
                    THREADINFO = string.Empty;

                }
            }

            Console.WriteLine($"\n############ {program_title} has stopped due to the above exception or error. ############");
            Console.WriteLine("\nApplication window will be closed at " + DateTime.Now.AddMinutes(1) + ".\nPlease, check if you have connected the devices properly and you have the right settings in 'Settings.ini' file, and then run the application again. \nThank you!");
            // LOG FILE에 저장함.
            try
            {
                File.AppendAllLines(folderPath, new string[] { $"프로그램을 종료했습니다. {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} 사유: '{gloVar.ErrorMsgTable.Item1}' DB 테이블 최신 에러로그 참고하세요." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex.Message + ex.StackTrace + " ");
            }
            System.Threading.Thread.Sleep(60000);

        }

        private static void StartProgram(SpcData spcData, GlobalVariables gloVar, int index)
        {
            if (gloVar.ID_List == null || gloVar.ID_List.Length <= index)
                return;

            spcData.consolePrintText += $"THD {index} started. ";
            bool connected;
            int connRetryCounter = 0;
            int readRetryCounter = 0;
            bool oldDataExists = false;

            SpcData oldData = new SpcData();
            spcData.dID = gloVar.ID_List[index];
            oldData.dID = gloVar.ID_List[index];
            spcData.SetData(gloVar, spcData.dID, "particle");
            oldData.SetData(gloVar, spcData.dID, "particle");
            //spcData.consolePrintText += spcData.printUsage() + ".\n";
            spcData.DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            try
            {
                while (true)
                {
                    spcData.timeNow = DateTime.Now;
                    if (spcData.consolePrintText.Length != 0)
                        spcData.consolePrintText = string.Empty;
                    spcData.data_OK = false;
                    connected = false;
                    connected = connect(gloVar, index);

                    if (connected)
                    {
                        while (spcData.d == null || spcData.d.Length < 38 || spcData.d[22] != -1)
                        {
                            if (spcData.d != null)
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                            readRetryCounter += 1;
                            if (!gloVar.modbusClient_List[index].Connected || readRetryCounter > 5)
                            {
                                if (readRetryCounter > 5)
                                {
                                    gloVar.bad_clientFlag_List[index] = true;
                                    readRetryCounter = 0;
                                }
                                connected = connect(gloVar, index);
                            }
                            try
                            {

                                spcData.d = gloVar.modbusClient_List[index].ReadInputRegisters(0, 38);
                                
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("ERROR Reading Data" + e.Message + e.StackTrace);
                                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                                
                                try
                                {
                                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e, "Modbus", "데이터 수집", "데이터 수집 시 에러 발생. ", oldData.dID, gloVar.modbusClient_List[index].IPAddress, gloVar.Port_List[index].ToString(), "");
                                }
                                catch (Exception)
                                {
                                    File.AppendAllLines(folderPath, new string[] { "ErrorLog저장 시 에러 발생 에러메시지 원본:" + e.Message + e.StackTrace });
                                }
                                gloVar.bad_clientFlag_List[index] = true;
                                readRetryCounter = 0;
                            }

                            if ((DateTime.Now - spcData.timeNow).TotalSeconds >= DELAYTIME && oldDataExists)
                            {
                                Console.WriteLine($"\n + BREAK LOOP for sID: {spcData.dID} + \n");
                                break;
                            }

                        }

                        if (spcData.d != null && spcData.d.Length >= 38 && spcData.d[22] == -1)
                        {
                            spcData.DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            spcData.sTime = $"20{spcData.d[12].ToString("D2")}-{spcData.d[13].ToString("D2")}-{spcData.d[14].ToString("D2")} {spcData.d[15].ToString("D2")}:{spcData.d[16].ToString("D2")}:{spcData.d[17].ToString("D2")}.000";

                            //데이터가 중복인지 확인하고, 중복 확인 시, 다음 (cycle) 측정시간 + 3초까지 기다리고, 나머지 코드를 skip함.
                            // check if data is duplicated, and if yes, wait until the next cycle and skip the remaining code
                            if (oldDataExists && (Convert.ToDateTime(spcData.DateAndTime) - Convert.ToDateTime(oldData.DateAndTime)).TotalSeconds < 60 && Convert.ToDateTime(oldData.sTime) == Convert.ToDateTime(spcData.sTime))
                            {
                                // check if data is duplicated, and if yes, skip to next cycle
                                // LOG FILE에 저장함.
                                try
                                {
                                    File.AppendAllLines(folderPath, new string[] { $"중복 데이터 발견했다! {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}, ID: {gloVar.ID_List[index]},  읽어온데이터: {String.Join(", ", spcData.d)}" });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("\nLOG FILE저장 시 에러 발생. \n원래에러: " + ex.Message + ex.StackTrace + " ");
                                }
                                spcData.Clear();

                                //다음 측정시간까지 기다림.
                                while (true)
                                {
                                    System.Threading.Thread.Sleep(500);
                                    if (DateTime.Now.Second == spcData.cycleResetSecond)
                                        break;
                                }

                                //나머지 코드를 skip함.
                                continue;
                            }

                            readRetryCounter = 0;
                            connRetryCounter = 0;

                            spcData.data_OK = true;


                            if ((spcData.cycleResetSecond - 3) != spcData.d[17])
                            {      // 다음 수집 시간을 센서의 원래 측정시간 +1 초로 세팅함.
                                spcData.cycleResetSecond = (spcData.d[17] + 3) >= 60 ? (spcData.d[17] + 3) - 60 : (spcData.d[17] + 3);
                                oldData.cycleResetSecond = (spcData.d[17] + 3) >= 60 ? (spcData.d[17] + 3) - 60 : (spcData.d[17] + 3);
                            }
                            oldDataExists = true;

                            preprocessData(spcData);
                            oldData.d = spcData.d;
                            preprocessData(oldData);

                            spcData.consolePrintText += "시간: " + spcData.DateAndTime + $"({oldData.DateAndTime}) , ID:" + spcData.dID + ", " + spcData.ToString();
                            oldData.consolePrintText = "시간: " + oldData.DateAndTime + ", ID:" + oldData.dID + ", " + oldData.ToString();
                            oldData.DateAndTime = spcData.DateAndTime;
                        }

                        if (!spcData.data_OK && (DateTime.Now - spcData.timeNow).TotalSeconds >= DELAYTIME)
                        {
                            oldData.DateAndTime = Convert.ToDateTime(oldData.DateAndTime).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            //Console.WriteLine($"\n{spcData.data_OK}, (cycleResetSecond:{spcData.cycleResetSecond}, Now: {DateTime.Now.Second}\n");
                            spcData.insertDB_OK = oldData.StoreDataToDB(gloVar);

                            oldData.consolePrintText = "시간: " + oldData.DateAndTime + ", ID:" + oldData.dID + ", " + oldData.ToString();
                            spcData.consolePrintText = (oldData.consolePrintText + "(기존데이터)");


                            // LOG FILE에 저장함.
                            try
                            {
                                File.AppendAllLines(folderPath, new string[] { spcData.consolePrintText });
                                spcData.consolePrintText += "(LOG+)";
                            }
                            catch (Exception ex)
                            {
                                spcData.consolePrintText += "(LOG-)"; // + ex.Message;
                                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                                try
                                {
                                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex, "LOG", "LogFileWrite", "LOG File 수집 시 에러 발생. ", oldData.dID, gloVar.modbusClient_List[index].IPAddress, gloVar.Port_List[index].ToString(), "");
                                }
                                catch (Exception ex2)
                                {
                                    Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                                }
                            }

                        }
                        else
                        {
                            spcData.insertDB_OK = spcData.StoreDataToDB(gloVar);
                            spcData.consolePrintText += " 정상 ";
                        }
                        spcData.data_OK = true;
                        oldData.data_OK = true;

                        if (spcData.insertDB_OK)
                            spcData.consolePrintText += " dbInsert_OK. ";
                        else
                            spcData.consolePrintText += " dbInsert_NG. ";
                        Console.WriteLine(" " + spcData.consolePrintText + " ");
                        spcData.Clear();        // object 재사용을 위해서 기존값을 초기화 시킵니다.

                    }
                    else
                    {
                        connRetryCounter += 1;
                        spcData.consolePrintText += "Client Connection Failed. ";
                        if (connRetryCounter > 5)
                            gloVar.bad_clientFlag_List[index] = true;           // modbusClient 연결이 안되는 횟수가 5개 이상이 되는 조건이
                                                                                // modbusClient Object가 쓸수없는 형태임을 나타냄 (true 값).
                    }

                    gloVar.modbusClient_List[index].Disconnect();
                    spcData.d = null;
                    System.Threading.Thread.Sleep(500);

                    // spcData.data_OK == false, 또한 정상적인 데이터가 안들어왔었다라는 의미임로
                    // 다음 수집을 1분후에 실행하지 말고 'spcData.data_OK = true' 줄을 만날때까지 수집을 실행합니다.

                    while (spcData.data_OK)
                    {
                        System.Threading.Thread.Sleep(500);
                        if (DateTime.Now.Second == spcData.cycleResetSecond)
                            break;
                    }


                }
            }
            catch (Exception ex)
            {
                gloVar.bad_clientFlag_List[index] = true;

                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.

                try
                {
                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex, "Modbus", "데이터 수집", "데이터 수집 시 에러 발생. ", oldData.dID, gloVar.modbusClient_List[index].IPAddress, gloVar.Port_List[index].ToString(), "");
                }
                catch (Exception ex2)
                {
                    File.AppendAllLines(folderPath, new string[] { "ErrorLog저장 시 에러 발생. 에러 매시지 원본: " + ex.Message + ex.StackTrace });
                    Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                }

            }
        }

        /// <summary>
        /// spcData.d[] 배열에 있는 데이터를 전처리하고 spcData에 저장해주는 함수
        /// </summary>
        /// <param name="spcData">데이터저장용 class 인스턴스</param>
        private static void preprocessData(SpcData spcData)
        {
            spcData.sTime = $"20{spcData.d[12].ToString("D2")}-{spcData.d[13].ToString("D2")}-{spcData.d[14].ToString("D2")} {spcData.d[15].ToString("D2")}:{spcData.d[16].ToString("D2")}:{spcData.d[17].ToString("D2")}.000";
            spcData.dID_long = $"{(char)spcData.d[0]}{(char)spcData.d[1]}-{(char)spcData.d[2]}{(char)spcData.d[3]}-{(char)spcData.d[4]}{(char)spcData.d[5]}-{(char)spcData.d[6]}{(char)spcData.d[7]}-{(char)spcData.d[8]}{(char)spcData.d[9]}-{(char)spcData.d[10]}{(char)spcData.d[11]}";
            spcData.sTemperature = $"{spcData.d[18].ToString("D2")}.{spcData.d[19].ToString("D2")}";
            spcData.sHumidity = $"{spcData.d[20].ToString("D2")}.{spcData.d[21].ToString("D2")}";


            spcData.highValue = spcData.d[24] >= 0 ? spcData.d[24] : 65536 + spcData.d[24];
            spcData.lowValue = spcData.d[25] >= 0 ? spcData.d[25] : 65536 + spcData.d[25];
            spcData.sParticle03 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();

            spcData.highValue = spcData.d[26] >= 0 ? spcData.d[26] : 65536 + spcData.d[26];
            spcData.lowValue = spcData.d[27] >= 0 ? spcData.d[27] : 65536 + spcData.d[27];
            spcData.sParticle05 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();

            spcData.highValue = spcData.d[28] >= 0 ? spcData.d[28] : 65536 + spcData.d[28];
            spcData.lowValue = spcData.d[29] >= 0 ? spcData.d[29] : 65536 + spcData.d[29];
            spcData.sParticle10 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();

            spcData.highValue = spcData.d[32] >= 0 ? spcData.d[32] : 65536 + spcData.d[32];
            spcData.lowValue = spcData.d[33] >= 0 ? spcData.d[33] : 65536 + spcData.d[33];
            spcData.sParticle50 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();

            spcData.highValue = spcData.d[34] >= 0 ? spcData.d[34] : 65536 + spcData.d[34];
            spcData.lowValue = spcData.d[35] >= 0 ? spcData.d[35] : 65536 + spcData.d[35];
            spcData.sParticle100 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();

            spcData.highValue = spcData.d[36] >= 0 ? spcData.d[36] : 65536 + spcData.d[36];
            spcData.lowValue = spcData.d[37] >= 0 ? spcData.d[37] : 65536 + spcData.d[37];
            spcData.sParticle250 = (spcData.highValue == 65535 && spcData.lowValue == 65535) ? "-1" : (spcData.highValue * 65536 + spcData.lowValue).ToString();
        }

        /// <summary>
        /// ModbusClient연결 함수
        /// </summary>
        /// <param name="gloVar">전역 변수</param>
        /// <param name="index">ModbusClient 또는 IP 또는 Port List 'index' 번호</param>
        /// <returns>정상 연결 시 true, 아닐땐 false</returns>
        private static bool connect(GlobalVariables gloVar, int index)
        {
            try
            {
                if (gloVar.modbusClient_List[index] == null || gloVar.bad_clientFlag_List[index] == true)
                {
                    gloVar.modbusClient_List[index] = new ModbusClient(gloVar.IPAddress_List[index], gloVar.Port_List[index]);
                    gloVar.modbusClient_List[index].UnitIdentifier = (byte)gloVar.ID_List[index];
                    gloVar.bad_clientFlag_List[index] = false;
                }

                if (!gloVar.modbusClient_List[index].Connected)
                    gloVar.modbusClient_List[index].Connect();
                //Console.WriteLine($"ModbusClient (ID: {gloVar.ID_List[index]}) Connection status: " + gloVar.modbusClient_List[index].Connected);
            }
            catch (Exception e)
            {
                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                try
                {
                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e, "Modbus", "통신", "Modbus 통신연결 시 에러 발생. ", gloVar.ID_List[index], gloVar.modbusClient_List[index].IPAddress, gloVar.Port_List[index].ToString(), "");
                }
                catch (Exception ex2)
                {
                    File.AppendAllLines(folderPath, new string[] { "ErrorLog저장 시 에러 발생. 에러매시지 원본:" + e.Message + e.StackTrace });
                    Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                }
            }
            return gloVar.modbusClient_List[index].Connected;
        }
    }
}
