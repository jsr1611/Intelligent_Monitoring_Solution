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
            Console.Write("Program started.\n");
            GlobalVariables gloVar = new GlobalVariables();


            gloVar.dbName = "SENSORDATA";
            gloVar.dataTable = gloVar.dbName[0] + "_DATATABLE";
            gloVar.deviceTable = gloVar.dbName[0] + "_DEVICES";
            gloVar.dbUID = "dlitdb";
            gloVar.dbPWD = "dlitdb";
            gloVar.dbServerName = "localhost";
            gloVar.sqlConn = new SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=False; MultipleActiveResultSets=True";

            gloVar.sanghanHahanTable = gloVar.dbName[0] + "_SanghanHahan";

            SpcData spcData = new SpcData();

            //gloVar.devTbColumns = spcData.GetTableColumnNames(gloVar, gloVar.deviceTable).ToArray();
            gloVar.sqlCmdText = $"SELECT sID FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] ORDER BY sID ASC";
            gloVar.ID_List = Array.ConvertAll(spcData.GetColumnDataAsList(gloVar, "int", "sID", gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
            gloVar.sqlCmdText = $"SELECT sIPAddress FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] ORDER BY sID ASC";
            gloVar.IPAddress_List = spcData.GetColumnDataAsList(gloVar, "string", "sIPAddress", gloVar.sqlCmdText).ToArray();
            gloVar.sqlCmdText = $"SELECT sPortNumber FROM [{gloVar.dbName}].[dbo].[{gloVar.deviceTable}] ORDER BY sID ASC";
            gloVar.Port_List = Array.ConvertAll(spcData.GetColumnDataAsList(gloVar, "string", "sPortNumber", gloVar.sqlCmdText).ToArray(), s => int.Parse(s));
            gloVar.modbusClient_List = new ModbusClient[gloVar.ID_List.Length];
            System.Threading.Thread[] thread_List = new System.Threading.Thread[gloVar.ID_List.Length];
            string THREADINFO = string.Empty;


            while (true)
            {
                for (int i = 0; i < gloVar.ID_List.Length; i++)
                {
                    try
                    {


                        if (thread_List[i] == null || !thread_List[i].IsAlive || thread_List[i].ThreadState == System.Threading.ThreadState.Aborted) // thread_List[i].ThreadState != System.Threading.ThreadState.Running || 
                        {

                            if (thread_List[i] == null)
                                THREADINFO += "|\t" + (i + 1) + "." + "THREAD No. " + i + " IS NULL! \t|";
                            else if (!thread_List[i].IsAlive)
                                THREADINFO += "|\t" + (i + 1) + "." + thread_List[i].Name + " IS NOT ALIVE! \t|";
                            //else if (thread_List[i].ThreadState != System.Threading.ThreadState.Running)
                            //    Console.WriteLine($"thread_List[i].ThreadState != System.Threading.ThreadState.Running ? { thread_List[i].ThreadState != System.Threading.ThreadState.Running};");
                            else if (thread_List[i].ThreadState == System.Threading.ThreadState.Aborted)
                                THREADINFO += "|\t" + (i + 1) + "." + thread_List[i].Name + " IS ABORTED! \t|";
                            else
                            {
                                THREADINFO += "|\t" + (i + 1) + "." + thread_List[i].Name + "'s STATE IS SOMETHING ELSE, BUT NOT ALIVE! \t|";
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
                            THREADINFO += "|\t" + (i + 1) + "." + thread_List[i].Name + " IS ALIVE! \t|";
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                    }
                }
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
                        spcData.consolePrintText += "Connected. Reading... ";
                        while (spcData.d == null || spcData.d[22] != -1)
                            spcData.d = gloVar.modbusClient_List[index].ReadInputRegisters(0, 38);
                    }
                    else
                    {

                        retryCounter += 1;
                        spcData.consolePrintText += "Couldn't connect. ";
                        if (retryCounter > 5)
                            break;
                    }


                    if (spcData.d != null && spcData.d.Length >= 38)
                    {
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
                        Console.WriteLine(" " + spcData.consolePrintText + " No data?");
                    }

                    gloVar.modbusClient_List[index].Disconnect();
                    spcData.d = null;
                    System.Threading.Thread.Sleep(1000);

                    while (DateTime.Now.Second != 0 && counter == 1)
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
