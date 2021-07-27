using System;
using System.Collections.Generic;
using EasyModbus;
using CommonClassLibrary;


namespace AirConData
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Program started.\n");
            GlobalVariables gloVar = new GlobalVariables();

            gloVar.dbName = "AIRCONDATA";
            gloVar.dataTable = gloVar.dbName[0] + "_DATATABLE";
            gloVar.dbUID = "dlitdb";
            gloVar.dbPWD = "dlitdb";
            gloVar.dbServerName = "localhost";
            gloVar.sqlConn = new System.Data.SqlClient.SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=True";




            gloVar.ID_List = new int[] { 1, 2, 3, 4, 5, 6 };
            gloVar.modbusClient_List = new ModbusClient[2];
            gloVar.COMPort_List = new string[] { "COM4", "COM5" };


            // Connect to all modbus clients
            connect(gloVar);

            int clientNum = 0;
            bool[] d0 = null;
            AirData airData = new AirData();


            while (true)
            {
                gloVar.timeNow = DateTime.Now;
                for (int i = 0; i < gloVar.ID_List.Length; i++)
                {
                    try
                    {
                        clientNum = i <= 3 ? 0 : 1;

                        gloVar.modbusClient_List[clientNum].UnitIdentifier = (byte)gloVar.ID_List[i];
                        //Console.Write("\nTrying to read from client no: " + g.clients[clientNum].UnitIdentifier + " on port " + g.clients[clientNum].SerialPort + " ");
                        if (!gloVar.modbusClient_List[clientNum].Connected)
                            connect(gloVar, clientNum);
                        d0 = gloVar.modbusClient_List[clientNum].ReadCoils(0, 32);
                        gloVar.d = gloVar.modbusClient_List[clientNum].ReadHoldingRegisters(0, 37);

                        try
                        {
                            gloVar.dID = gloVar.ID_List[i];
                            gloVar.DateAndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            airData.setData(d0, gloVar.d);
                            gloVar.insertDB_OK = airData.insertIntoDB(gloVar);

                            Console.Write($"시간: {gloVar.DateAndTime} " + airData.ToString());

                            if (gloVar.insertDB_OK)
                                Console.WriteLine(", DB_Insert_OK ");
                            else
                            {
                                Console.WriteLine(" DB_Insert_NG ");
                            }
                        }
                        catch (Exception ex1)
                        {
                            Console.Write($"SQL Insertion Error with client: {gloVar.modbusClient_List[clientNum].UnitIdentifier} at port: {gloVar.COMPort_List[clientNum]}. \n{ex1.Message}. {ex1.StackTrace}\n");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.Write($"Data Collection Error with client: {gloVar.modbusClient_List[clientNum].UnitIdentifier} at port: {gloVar.COMPort_List[clientNum]}. \n{ex.Message}. {ex.StackTrace}\n");
                    }

                }

                Console.Write("\n ------------ DATA COLLECTION SUCCESSFUL ------------ \n");
                while (DateTime.Now.Second != 0 && gloVar.timeNow.Second < 59)
                {
                    System.Threading.Thread.Sleep(500);
                    //Console.WriteLine(" I am waiting for 00 s clock:  " + DateTime.Now.ToString("HH:mm:ss.fff"));
                }
                //Console.WriteLine(" I am ready for next cycle.. ");

            }
        }






        /// <summary>
        /// Connects to a Modbus client
        /// </summary>
        /// <param name="gloVar">GlobalVaribales</param>
        /// <param name="clientNum">modbus client index</param>
        private static void connect(GlobalVariables gloVar, int clientNum)
        {
            try
            {
                if (gloVar.modbusClient_List[clientNum].Connected == false)
                {
                    gloVar.modbusClient_List[clientNum].Connect();
                    Console.Write(" Connected to client on port " + gloVar.modbusClient_List[clientNum].SerialPort + "\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nModbusClient Connection Error with client: {gloVar.modbusClient_List[clientNum].UnitIdentifier} at port: {gloVar.COMPort_List[clientNum]}. \n{ex.Message}. {ex.StackTrace}");
            }
        }


        /// <summary>
        /// Connects to all ModbusClients.
        /// </summary>
        /// <param name="gloVar">GlobalVariables</param>
        private static void connect(GlobalVariables gloVar)
        {
            for (int clientNum = 0; clientNum < gloVar.modbusClient_List.Length; clientNum++)
            {
                try
                {
                    if (gloVar.modbusClient_List[clientNum] == null || gloVar.modbusClient_List[clientNum].Connected == false)
                    {
                        gloVar.modbusClient_List[clientNum] = new ModbusClient(gloVar.COMPort_List[clientNum]);
                        gloVar.modbusClient_List[clientNum].Parity = System.IO.Ports.Parity.None;
                        gloVar.modbusClient_List[clientNum].StopBits = System.IO.Ports.StopBits.One;
                        gloVar.modbusClient_List[clientNum].Connect();
                        Console.Write(" Connected to client on port " + gloVar.modbusClient_List[clientNum].SerialPort + "\n");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nModbusClient Connection Error with client: {gloVar.modbusClient_List[clientNum].UnitIdentifier} at port: {gloVar.COMPort_List[clientNum]}. \n{ex.Message}. {ex.StackTrace}");
                }

            }
        }

    }
}
