using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace CommonClassLibrary
{
    //Spc = Smart particle counter (SPC3000) || 온습도 및 파티클 데이터
    public class SpcData
    {
        public string objName { get; set; }
        public string DateAndTime { get; set; }
        public int dID { get; set; }
        public bool data_OK { get; set; }
        public bool insertDB_OK { get; set; }
        public int[] d { get; set; }
        public DateTime timeNow { get; set; }
        public int cycleResetSecond { get; set; }


        public string dID_long { get; set; }
        public string sTime { get; set; }
        public string sTemperature { get; set; }
        public string sHumidity { get; set; }
        public string sParticle03 { get; set; }
        public string sParticle05 { get; set; }
        public string sParticle10 { get; set; }
        public string sParticle50 { get; set; }
        public string sParticle100 { get; set; }
        public string sParticle250 { get; set; }

        public bool temperature_on { get; set; }
        public bool humidity_on { get; set; }
        public bool sParticle03_on { get; set; }
        public bool sParticle05_on { get; set; }
        public bool sParticle10_on { get; set; }
        public bool sParticle50_on { get; set; }
        public bool sParticle100_on { get; set; }
        public bool sParticle250_on { get; set; }




        public SqlConnection sqlConn { get; set; }
        public SqlCommand sqlCmd { get; set; }
        public SqlDataReader sqlRdr { get; set; }
        public SqlDataAdapter sqlDAptr { get; set; }
        public string sqlConStr { get; set; }
        public string sqlCmdText { get; set; }
        public string sqlDefltCmdText { get; set; }


        public Int64 highValue { get; set; }
        public Int64 lowValue { get; set; }
        public List<string> tbColumns { get; set; }
        public List<string> sensorTypes { get; set; }
        public DataTable dtbl { get; set; }
        public DataView dview { get; set; }
        public string[] resultingArray { get; set; }
        public bool duplicate { get; set; }
        public string consolePrintText { get; set; }

        public void setSqlFields(SqlConnection sqlConn)
        {
            this.sqlConn = sqlConn;
        }




        /// <summary>
        /// 수집여부 확인 및 Data클레스에 적용 후 데이터 반환
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorCategory"></param>
        /// <returns></returns>
        public void SetData(GlobalVariables gloVar, int sensorId, string sensorCategory)
        {
            
            if (gloVar.sanghanHahanColumns == null || gloVar.sanghanHahanColumns.Count == 0)
                gloVar.sanghanHahanColumns = GlobalMethods.GetTableColumnNames(gloVar, gloVar.sanghanHahanTable).ToList();

            if (gloVar.sanghanHahanColumns.Count == 0)
            {
                Console.WriteLine($"{gloVar.sanghanHahanTable} table 존재하지 않거나 다른 에러가 발생했습니다.");
            }
            else
            {
                //sqlCmdText = $"SELECT DISTINCT {gloVar.sanghanHahanColumns[2]} FROM [{gloVar.dbName}].[dbo].[{gloVar.sanghanHahanTable}] WHERE {gloVar.sanghanHahanColumns[0]} = '{sensorCategory}'";
                //sensorTypes = GetColumnDataAsList(gloVar, "string", gloVar.sanghanHahanColumns[2], sqlCmdText); // sqlCmdText is used for getting sensorTypes

                sqlCmdText = $"SELECT {gloVar.sanghanHahanColumns[2]}, {gloVar.sanghanHahanColumns[7]} FROM [{gloVar.dbName}].[dbo].[{gloVar.sanghanHahanTable}] WHERE {gloVar.sanghanHahanColumns[0]} = '{sensorCategory}' AND {gloVar.sanghanHahanColumns[1]} = {sensorId}";

                CheckSqlConnAndCmd(gloVar);
                
                sqlCmd.CommandText = sqlCmdText;
                sqlRdr = sqlCmd.ExecuteReader();
                while (sqlRdr.Read())
                {
                    if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("temperature"))
                        temperature_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("humidity"))
                        humidity_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle03"))
                        sParticle03_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle05"))
                        sParticle05_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle10"))
                        sParticle10_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle50"))
                        sParticle50_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle100"))
                        sParticle100_on = sqlRdr.GetString(1).Equals("Yes");
                    else if (sqlRdr[gloVar.sanghanHahanColumns[2]].Equals("particle250"))
                        sParticle250_on = sqlRdr.GetString(1).Equals("Yes");
                }
                sqlRdr.Close();
                sqlCmd.Dispose();

            }

        }

        public string printUsage()
        {
            return " 수집여부: " + $"온도({temperature_on}), 습도({humidity_on}), 파티클: 0.3({sParticle03_on}), 0.5({sParticle05_on}), 1.0({sParticle10_on}), 5.0({sParticle50_on}), 10.0({sParticle100_on}), 25.0({sParticle250_on}) ";
        }



        /// <summary>
        /// Clears the field values to default as follows:
        /// string => string.Empty;
        /// int => 0;
        /// bool => false;
        /// </summary>

        public void Clear()
        {
            //DateAndTime = string.Empty;
            insertDB_OK = false;
            dID_long = string.Empty;
            sTime = string.Empty;
            sTemperature = string.Empty;
            sHumidity = string.Empty;
            sParticle03 = string.Empty;
            sParticle05 = string.Empty;
            sParticle10 = string.Empty;
            sParticle50 = string.Empty;
            sParticle100 = string.Empty;
            sParticle250 = string.Empty;

            //temperature_on = false;
            //humidity_on = false;
            //sParticle03_on = false;
            //sParticle05_on = false;
            //sParticle10_on = false;
            //sParticle50_on = false;
            //sParticle100_on = false;
            //sParticle250_on = false;


            highValue = 0;
            lowValue = 0;
            //gloVar.sanghanHahanColumns = null;
            //sensorTypes = null;
            dtbl = null;
            dview = null;
            resultingArray = null;
            duplicate = false;
            consolePrintText = string.Empty;
            d = null;

        }


        /// <summary>
        /// Check if SQLConnection is open and SQLCommand is ready to take commandText
        /// </summary>
        /// <param name="gloVar"></param>
        public void CheckSqlConnAndCmd(GlobalVariables gloVar)
        {
            try
            {
            if (sqlConn == null)
            {
                sqlConn = new System.Data.SqlClient.SqlConnection(gloVar.sqlConStr);
                sqlConn.Open();
            }
            else if (sqlConn.ConnectionString.Length == 0)
            {
                sqlConn.ConnectionString = gloVar.sqlConStr;
                sqlConn.Open();
            }
            else
            {
                if (sqlConn.State != System.Data.ConnectionState.Open)
                    sqlConn.Open();
            }

            if (sqlCmd == null)
                sqlCmd = new System.Data.SqlClient.SqlCommand();

            //sqlCmd.CommandText = sqlCmdText;
            sqlCmd.Connection = sqlConn;
            }
            catch (Exception e)
            {
                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                try
                {
                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), e, "DB", "SQL Connection Init", "SQL Conneciton 변수 초기화 시 에러 발생.", dID, gloVar.IPAddress_List[gloVar.ID_List.ToList().IndexOf(dID)], gloVar.Port_List[gloVar.ID_List.ToList().IndexOf(dID)].ToString(), "CheckSqlConnAndCmd(GlobalVariables gloVar)");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                }
            }
        }





        /// <summary>
        /// 수집된 데이터를 DB에 저장해주는 함수
        /// </summary>
        /// <param name="data">수집 데이터</param>
        /// <returns>수집이 잘 되면 true반환함</returns>
        public bool StoreDataToDB(GlobalVariables gloVar)
        {
            bool res = false;

            


            if (dID != 0 && DateAndTime.Length != 0 && sTime.Length != 0)
            {
                //Console.Write("\n Waiting at mutex_lock for " + dID + " THD " + System.Threading.Thread.CurrentThread.Name);
                gloVar.mutex_lock.WaitOne();
                //Console.Write("\n Starting mutex_lock for " + dID + " THD " + System.Threading.Thread.CurrentThread.Name);
                
                //SQL Connection, SQLCommand, SQl
                CheckSqlConnAndCmd(gloVar);
                gloVar.transaction_w = sqlConn.BeginTransaction();
                try
                {

                    sqlCmdText = string.Empty;

                    //---------온도 저장-------------------->
                    if (temperature_on)
                        sqlCmdText = $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'temperature'" + // sensorCode
                                    $", '{sTemperature}'" +
                                    $", '');";

                    //---------습도 저장-------------------->
                    if (humidity_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                     $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'humidity'" + // sensorCode
                                    $", '{sHumidity}'" +
                                    $", '');";

                    //---------파티클(0.3um)  저장-------------------->
                    if (sParticle03_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle03'" + // sensorCode
                                    $", '{sParticle03}'" +
                                    $", '');";

                    //---------파티클(0.5um)  저장-------------------->
                    if (sParticle05_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle05'" + // sensorCode
                                    $", '{sParticle05}'" +
                                    $", '');";


                    //---------파티클(1.0um)  저장-------------------->
                    if (sParticle10_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle10'" + // sensorCode
                                    $", '{sParticle10}'" +
                                    $", '');";

                    //---------파티클(5.0um)  저장-------------------->
                    if (sParticle50_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle50'" + // sensorCode
                                    $", '{sParticle50}'" +
                                    $", '');";

                    //---------파티클(10.0um)  저장-------------------->
                    if (sParticle100_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle100'" + // sensorCode
                                    $", '{sParticle100}'" +
                                    $", '');";


                    //---------파티클(25.0um) 저장-------------------->
                    if (sParticle250_on)
                        sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {dID}" +
                                    $", 'particle250'" + // sensorCode
                                    $", '{sParticle250}'" +
                                    $", '');";


                    // -----------------DB Insert실행 부분------------->
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = sqlCmdText;


                    sqlCmd.Transaction = gloVar.transaction_w;
                    if (sqlCmd.CommandText.Length > 0)
                    {
                        sqlCmd.ExecuteNonQuery();
                        gloVar.transaction_w.Commit();
                        res = true;
                    }

                }
                catch (Exception ex)
                {
                    //Console.WriteLine("\n DB Insert Exception. ID: " + dID + ". Now Rolling Back." + ex.Message + ex.StackTrace);
                    // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                    try
                    {
                        GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex, "DB", "SQL Insert", "데이터 저장 시 에러 발생. ", dID, $"{gloVar.IPAddress_List[gloVar.ID_List.ToList().IndexOf(dID)]}", $"{gloVar.Port_List[gloVar.ID_List.ToList().IndexOf(dID)]}", "");
                    }
                    catch (Exception ex22)
                    {
                        Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex22.Message + ex22.StackTrace + " ");
                    }

                    try
                    {
                        gloVar.transaction_w.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        //Console.WriteLine("\n DB Transaction RollBack Exception. ID: " + dID + ". " + ex2.Message + ex2.StackTrace);
                        // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                        try
                        {
                            GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex2, "DB", "SQL Transaction", "SQL Transaction Rollback 시 에러 발생. ", dID, $"{gloVar.IPAddress_List[gloVar.ID_List.ToList().IndexOf(dID)]}", $"{gloVar.Port_List[gloVar.ID_List.ToList().IndexOf(dID)]}", "");
                        }
                        catch (Exception ex22)
                        {
                            Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex22.Message + ex22.StackTrace + " ");
                        }
                    }

                }
                finally
                {
                    gloVar.transaction_w.Dispose();
                    //Console.Write(" Releasing mutex_lock for " + dID + " THD " + System.Threading.Thread.CurrentThread.Name + " .\n");
                    gloVar.mutex_lock.ReleaseMutex();
                }
            }

            return res;
        }






        /// <summary>
        /// 중복데이터가 이미 저장이 되어있는지 확인
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="sensorTimestamp"></param>
        /// <returns></returns>
        public bool DataAlreadyExists(GlobalVariables gloVar, int sensorId, string sensorTimestamp, string pcTimestamp)
        {
            //mutex_lock2.WaitOne();
            //semaphore.WaitOne();
            duplicate = false;
            try
            {
                sqlCmdText = $"SELECT TOP 1 1 FROM {gloVar.dataTable} WHERE {gloVar.S_DTColumns[2]} = {sensorId} AND {gloVar.S_DTColumns[1]} LIKE '{sensorTimestamp.Substring(0, sensorTimestamp.Length - 7)}%' AND {gloVar.S_DTColumns[0]} LIKE '{pcTimestamp.Substring(0, pcTimestamp.Length - 7)}%';";

                CheckSqlConnAndCmd(gloVar);

                sqlRdr = sqlCmd.ExecuteReader();
                if (sqlRdr.HasRows)
                {
                    duplicate = true;
                }
                else
                {
                    //Console.WriteLine("\nSQL New Data vs Old:" + timestamp + " ");
                    //Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                // ErrorMessage || Exception 정보를 ErrorMsgs테이블에 저장함.
                try
                {
                    GlobalMethods.ErrorMsgCollect(gloVar, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex, "DB", "SQL General", "중복 데이터 체크 시 에러 발생. ", sensorId, $"{gloVar.IPAddress_List[gloVar.ID_List.ToList().IndexOf(dID)]}", $"{gloVar.Port_List[gloVar.ID_List.ToList().IndexOf(dID)]}", "");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("\nErrorLog저장 시 에러 발생. \n원래에러: " + ex2.Message + ex2.StackTrace + " ");
                }
                //Console.WriteLine("중복데이터 확인 시 에러 발생. " + ex.Message + ex.StackTrace);
            }
            //mutex_lock2.ReleaseMutex();
            //semaphore.Release();
            return duplicate;

        }




        /// <summary>
        /// Return all data as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "sTime: " + sTime + ", 온도: " + sTemperature + ", 습도: " + sHumidity + ", p0.3: " + sParticle03 + ", p0.5: " + sParticle05 + ", p1.0: " + sParticle10 + ", p5.0: " + sParticle50 + ", p10.0: " + sParticle100 + ", p25.0: " + sParticle250 + " ";
        }



    }
}
