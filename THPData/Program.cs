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
                                THREADINFO += "|\t" + i + "." + "THREAD No. " + i + " IS NULL! \t|";
                            else if (!thread_List[i].IsAlive)
                                THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS NOT ALIVE! \t|";
                            //else if (thread_List[i].ThreadState != System.Threading.ThreadState.Running)
                            //    Console.WriteLine($"thread_List[i].ThreadState != System.Threading.ThreadState.Running ? { thread_List[i].ThreadState != System.Threading.ThreadState.Running};");
                            else if (thread_List[i].ThreadState == System.Threading.ThreadState.Aborted)
                                THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS ABORTED! \t|";
                            else
                            {
                                THREADINFO += "|\t" + i + "." + thread_List[i].Name + "'s STATE IS SOMETHING ELSE, BUT NOT ALIVE! \t|";
                            }
                            SpcData spcData1 = new SpcData();
                            spcData1.setSqlFields(gloVar.sqlConn);
                            //Console.WriteLine($" \nStartProgram(spcData1, gloVar, i) index = {i} ");
                            thread_List[i] = new System.Threading.Thread(() => StartProgram(spcData1, gloVar, i));

                            if (thread_List[i].Name == null || thread_List[i].Name.Length == 0)
                                thread_List[i].Name = "THREAD No. " + i;
                            thread_List[i].IsBackground = true;
                            thread_List[i].Start();
                        }
                        else
                        {
                            THREADINFO += "|\t" + i + "." + thread_List[i].Name + " IS ALIVE! \t|";
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
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
            spcData.consolePrintText += $"THD {index} started. ";
            bool connected; // = false;
            int counter = 0;
            int retryCounter = 0;
            try
            {
                spcData.dID = gloVar.ID_List[index];
                spcData.SetData(gloVar, spcData.dID, "particle");
                spcData.consolePrintText += spcData.printUsage() + ".\n";
                while (true)
                {
                    //spcData.timeNow = DateTime.Now;
                    counter += 1;
                    connected = false;
                    connected = connect(gloVar, index);

                    if (connected)
                    {
                        //spcData.consolePrintText += "Connected. Reading... ";
                        while (spcData.d == null || spcData.d.Length < 23 || spcData.d[22] != -1)
                            spcData.d = gloVar.modbusClient_List[index].ReadInputRegisters(0, 38);
                    }
                    else
                    {

                        retryCounter += 1;
                        spcData.consolePrintText += "Client Connection Failed. ";
                        if (retryCounter > 5)
                            break;
                    }


                    if (spcData.d != null && spcData.d.Length >= 38)
                    {
                        if (spcData.cycleResetSecond != spcData.d[17])
                            spcData.cycleResetSecond = spcData.d[17];
                        spcData.DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

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



                        spcData.insertDB_OK = spcData.StoreDataToDB(gloVar);

                        spcData.consolePrintText += "시간: " + spcData.DateAndTime + ", ID:" + spcData.dID + ", ";

                        spcData.consolePrintText += spcData.ToString();
                        if (spcData.insertDB_OK)
                            spcData.consolePrintText += " dbInsert_OK. ";
                        else
                            spcData.consolePrintText += " dbInsert_NG. ";
                        Console.WriteLine(" " + spcData.consolePrintText + " ");
                        spcData.Clear();
                        spcData.DateAndTime = string.Empty;
                        spcData.insertDB_OK = false;

                    }
                    else
                    {
                        Console.WriteLine(" " + spcData.consolePrintText + " Couldn't Read Any Data.");
                    }

                    gloVar.modbusClient_List[index].Disconnect();
                    spcData.d = null;
                    System.Threading.Thread.Sleep(1000);

                    while (DateTime.Now.Second != (spcData.cycleResetSecond + 1) && counter == 1)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    counter = 0;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in reading data. index: {index}" + ex.Message + ex.StackTrace);
            }
        }

        private static bool connect(GlobalVariables gloVar, int index)
        {
            try
            {
                if (gloVar.modbusClient_List[index] == null)
                {
                    gloVar.modbusClient_List[index] = new ModbusClient(gloVar.IPAddress_List[index], gloVar.Port_List[index]);
                    gloVar.modbusClient_List[index].UnitIdentifier = (byte)gloVar.ID_List[index];
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
