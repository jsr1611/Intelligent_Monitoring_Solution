using CommonClassLibrary;
using EasyModbus;
using System;
using System.Data.SqlClient;

namespace THPData
{
    class Program
    {
        static void Main(string[] args)
        {

            GlobalVariables gloVar = new GlobalVariables();

            // ini 읽기 //////
            IniFile ini = new IniFile();
            ini.Load(AppInfo.StartupPath + "\\" + "Setting.ini");
            string program_title = ini["PROGRAM"]["TITLE"].ToString();
            Console.Title = program_title; //"IMS Particle Data Collector App";     // Program title on top left side of the window
            string D_SERVERNAME = ini["DBSetting"]["SERVERNAME"].ToString();
            string D_NAME = ini["DBSetting"]["DBNAME"].ToString();
            string D_ID = ini["DBSetting"]["ID"].ToString();
            string D_PW = ini["DBSetting"]["PW"].ToString();


            Console.WriteLine($"############ {program_title} has started. ############");



            gloVar.dbName = D_NAME;
            gloVar.dataTable = gloVar.dbName[0] + "_DATATABLE";
            gloVar.deviceTable = gloVar.dbName[0] + "_DEVICES";
            gloVar.dbUID = D_ID;
            gloVar.dbPWD = D_PW;
            gloVar.dbServerName = D_SERVERNAME;
            gloVar.sanghanHahanTable = gloVar.dbName[0] + "_SanghanHahan";

            gloVar.sqlConn = new SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=False; MultipleActiveResultSets=True";


            SpcData spcData = new SpcData();

            gloVar.DevTbColumns = spcData.GetTableColumnNames(gloVar, gloVar.deviceTable);
            gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[0]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
            gloVar.ID_List = Array.ConvertAll(spcData.GetColumnDataAsList(gloVar, "int", "sID", gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
            gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[5]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
            gloVar.IPAddress_List = spcData.GetColumnDataAsList(gloVar, "string", "sIPAddress", gloVar.sqlCmdText).ToArray();
            gloVar.sqlCmdText = $"SELECT {gloVar.DevTbColumns[6]} FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] WHERE {gloVar.DevTbColumns[gloVar.DevTbColumns.Length - 1]} = 'YES' ORDER BY {gloVar.DevTbColumns[0]} ASC";
            gloVar.Port_List = Array.ConvertAll(spcData.GetColumnDataAsList(gloVar, "string", "sPortNumber", gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
            gloVar.modbusClient_List = new ModbusClient[gloVar.ID_List.Length];
            gloVar.bad_clientFlag_List = new bool[gloVar.ID_List.Length];
            System.Threading.Thread[] thread_List = new System.Threading.Thread[gloVar.ID_List.Length];
            string THREADINFO = string.Empty;

            int i = 0;
            while (true)
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
                        Console.WriteLine(e.ToString());
                    }
                    i += 1;
                }
                i = 0;
                Console.WriteLine(" " + THREADINFO + " ");
                System.Threading.Thread.Sleep(60000);
                Console.WriteLine("---------------------- DATA COLLECTION SUCCESSFUL ----------------------");
                THREADINFO = string.Empty;

            }

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

                                Console.Write($"\n\n 시간: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}(old:{spcData.DateAndTime}). ({(DateTime.Now - spcData.timeNow).TotalSeconds.ToString("F")}초), sensorID: {gloVar.ID_List[index]}, NEW DATA : ");
                                //위 while문을 나가는 조건이 spcDaa.data_OK 속성을 true로 만듭니다.
                                for (int k = 0; k < spcData.d.Length - 20; k++)
                                {
                                    Console.Write(" " + spcData.d[k] + " ");
                                }
                                Console.WriteLine();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                gloVar.bad_clientFlag_List[index] = true;
                                readRetryCounter = 0;
                            }

                            if ((DateTime.Now - spcData.timeNow).TotalSeconds >= 50 && oldDataExists)
                            {
                                Console.WriteLine($"\n + BREAK LOOP for sID: {spcData.dID} + \n");
                                break;
                            }

                        }

                        if (spcData.d != null && spcData.d.Length >= 38 && spcData.d[22] == -1)
                        {
                            spcData.DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                            readRetryCounter = 0;
                            connRetryCounter = 0;

                            spcData.data_OK = true;


                            if ((spcData.cycleResetSecond - 3) != spcData.d[17])
                            {      // 다음 수집 시간을 센서의 원래 측정시간 +1 초로 세팅함.
                                spcData.cycleResetSecond = (spcData.d[17] + 3)  >= 60 ? (spcData.d[17] + 3) - 60 : (spcData.d[17] + 3);
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

                        if (!spcData.data_OK && (DateTime.Now - spcData.timeNow).TotalSeconds >= 50)
                        {
                            oldData.DateAndTime = Convert.ToDateTime(oldData.DateAndTime).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            Console.WriteLine($"\n{spcData.data_OK}, (cycleResetSecond:{spcData.cycleResetSecond}, Now: {DateTime.Now.Second}\n");
                            spcData.insertDB_OK = oldData.StoreDataToDB(gloVar);

                            oldData.consolePrintText = "시간: " + oldData.DateAndTime + ", ID:" + oldData.dID + ", " + oldData.ToString();
                            spcData.consolePrintText = (oldData.consolePrintText + "(기존데이터)");
                        }
                        else
                        {
                            spcData.insertDB_OK = spcData.StoreDataToDB(gloVar);
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
                                                                                // modbusClient Object가 쓸수없는 형태임을 나타냄.
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
                Console.WriteLine($"Error in reading data. index: {index}" + ex.Message + ex.StackTrace);
            }
        }

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
            catch (Exception)
            {
                try
                {
                    Console.WriteLine($"Modbus Connection Error with client : {gloVar.ID_List[index]}, ip address: {gloVar.IPAddress_List[index]}, port: {gloVar.Port_List[index]}, index: {index}");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error while reporting other error: " + ex.Message + ex.StackTrace);
                }

            }
            return gloVar.modbusClient_List[index].Connected;
        }
    }
}
