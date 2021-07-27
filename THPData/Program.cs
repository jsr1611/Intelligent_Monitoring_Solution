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
            gloVar.dbUID = "dlitdb";
            gloVar.dbPWD = "dlitdb";
            gloVar.dbServerName = "localhost";
            gloVar.sqlConn = new SqlConnection();
            gloVar.sqlConStr = $@"Data Source={gloVar.dbServerName};Initial Catalog={gloVar.dbName};User id={gloVar.dbUID};Password={gloVar.dbPWD};Integrated Security=True";


            gloVar.ID_List = new int[] { 1, 2, 3, 4};
            gloVar.modbusClient_List = new ModbusClient[2];

        }
    }
}
