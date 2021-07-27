using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CommonClassLibrary;

namespace THPData
{
    public class SpcData
    {


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



        internal List<string> tbColumns { get; set; }
        internal List<string> sensorTypes { get; set; }
        internal DataTable dtbl { get; set; }
        internal DataView dview { get; set; }
        internal string[] resultingArray { get; set; }
        internal bool duplicate { get; set; }


        /// <summary>
        /// 수집여부 확인 및 Data클레스에 적용 후 데이터 반환
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sensorId"></param>
        /// <param name="sensorCategory"></param>
        /// <returns></returns>
        public void SetData(GlobalVariables gloVar, int sensorId, string sensorCategory)
        {
            if (tbColumns == null || tbColumns.Count == 0)
                tbColumns = GetTableColumnNames(gloVar, gloVar.sanghanHahanTable);

            if (tbColumns.Count == 0)
            {
                Console.WriteLine($"{gloVar.sanghanHahanTable} table 존재하지 않거나 다른 에러가 발생했습니다.");
            }

            gloVar.sqlCmdText = $"SELECT DISTINCT {tbColumns[2]} FROM [{gloVar.dbName}].[dbo].[{gloVar.sanghanHahanTable}] WHERE {tbColumns[0]} = '{sensorCategory}'";
            sensorTypes = GetColumnDataAsList(gloVar, "string", tbColumns[2]); // sqlCmdText is used for getting sensorTypes

            gloVar.sqlCmdText = $"SELECT {tbColumns[2]}, {tbColumns[7]} FROM [{gloVar.dbName}].[dbo].[{gloVar.sanghanHahanTable}] WHERE {tbColumns[0]} = '{sensorCategory}' AND {tbColumns[1]} = {sensorId}";

            CheckSqlConnAndCmd(gloVar);


            gloVar.sqlRdr = gloVar.sqlCmd.ExecuteReader();
            while (gloVar.sqlRdr.Read())
            {
                if (gloVar.sqlRdr[tbColumns[2]].Equals("temperature"))
                    temperature_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("humidity"))
                    humidity_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle03"))
                    sParticle03_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle05"))
                    sParticle05_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle10"))
                    sParticle10_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle50"))
                    sParticle50_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle100"))
                    sParticle100_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
                else if (gloVar.sqlRdr[tbColumns[2]].Equals("particle250"))
                    sParticle250_on = gloVar.sqlRdr.GetString(1).Equals("Yes");
            }
        }


        /// <summary>
        /// 주어진 테이블의 모든 Column명들을 List형태로 반환함.
        /// </summary>
        /// <param name="gloVar">GlobalVariables Object instance </param>
        /// <param name="tableName">table명</param>
        /// <returns>tbColumns List of string values.</returns>
        public List<string> GetTableColumnNames(GlobalVariables gloVar, string tableName)
        {
            tbColumns = new List<string>();
            try
            {
                CheckSqlConnAndCmd(gloVar);
                resultingArray = new string[4] { null, null, $"{tableName}", null }; // string[] restrictions = resultingArray
                dtbl = gloVar.sqlConn.GetSchema("Columns", resultingArray);
                dview = dtbl.DefaultView;
                dview.Sort = "ORDINAL_POSITION ASC";
                dtbl = dview.ToTable();
                tbColumns = dtbl.AsEnumerable().Select(x => x.Field<string>("COLUMN_NAME")).ToList();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error in getting column names for table: {tableName}. \n" + ex.Message + ex.StackTrace);
            }

            return tbColumns;
        }



        /// <summary>
        /// 테이블 특정 Column의 데이터를 반환함.
        /// </summary>
        /// <param name="dataType">데이터 타입: string, int, double</param>
        /// <param name="sqlStr">SQL쿼리문</param>
        /// <param name="ColumnName">테이블명</param>
        /// <returns></returns>
        public List<string> GetColumnDataAsList(GlobalVariables gloVar, string dataType, string ColumnName)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            List<string> res = new List<string>();

            CheckSqlConnAndCmd(gloVar);



            if (gloVar.sqlDAptr == null)
                gloVar.sqlDAptr = new System.Data.SqlClient.SqlDataAdapter();

            try
            {
                gloVar.sqlDAptr.SelectCommand = gloVar.sqlCmd;
                gloVar.sqlDAptr.Fill(ds);
                if (dataType.Equals("string"))
                {
                    res = ds.Tables[0].AsEnumerable().Select(x => x.Field<string>(ColumnName)).ToList();
                }
                else if (dataType.Equals("int"))
                {
                    res = ds.Tables[0].AsEnumerable().Select(x => x.Field<int>(ColumnName).ToString()).ToList();
                }
                else if (dataType.Equals("double"))
                {
                    res = ds.Tables[0].AsEnumerable().Select(x => x.Field<double>(ColumnName).ToString()).ToList();
                }
                else
                {
                    return res;
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error getting data as list for table column: {ColumnName}. Error msg: {ex.Message}. {ex.StackTrace}");
            }

            return res;
        }


        /// <summary>
        /// Check if SQLConnection is open and SQLCommand is ready to take commandText
        /// </summary>
        /// <param name="gloVar"></param>
        private void CheckSqlConnAndCmd(GlobalVariables gloVar)
        {
            if (gloVar.sqlConn == null)
            {
                gloVar.sqlConn = new System.Data.SqlClient.SqlConnection(gloVar.sqlConStr);
                gloVar.sqlConn.Open();
            }
            else if (gloVar.sqlConn.ConnectionString.Length == 0)
            {
                gloVar.sqlConn.ConnectionString = gloVar.sqlConStr;
                gloVar.sqlConn.Open();
            }
            else
            {
                if (gloVar.sqlConn.State != System.Data.ConnectionState.Open)
                    gloVar.sqlConn.Open();
            }

            if (gloVar.sqlCmd == null)
                gloVar.sqlCmd = new System.Data.SqlClient.SqlCommand();

            gloVar.sqlCmd.CommandText = gloVar.sqlCmdText;
            gloVar.sqlCmd.Connection = gloVar.sqlConn;
        }





        /// <summary>
        /// 수집된 데이터를 DB에 저장해주는 함수
        /// </summary>
        /// <param name="data">수집 데이터</param>
        /// <returns>수집이 잘 되면 true반환함</returns>
        private Tuple<bool, string> StoreDataToDB(GlobalVariables gloVar)
        {
            bool res = false;
            string txt = string.Empty;

            CheckSqlConnAndCmd(gloVar);


            if (gloVar.dID != 0)
            {
                gloVar.transaction = gloVar.sqlConn.BeginTransaction();
                try
                {

                    gloVar.sqlCmdText = string.Empty;

                    //---------온도 저장-------------------->
                    if (temperature_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'temperature'" + // sensorCode
                                    $", '{sTemperature}'" +
                                    $", '');";

                    //---------습도 저장-------------------->
                    if (humidity_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                     $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'humidity'" + // sensorCode
                                    $", '{sHumidity}'" +
                                    $", '');";

                    //---------파티클(0.3um)  저장-------------------->
                    if (sParticle03_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle03'" + // sensorCode
                                    $", '{sParticle03}'" +
                                    $", '');";

                    //---------파티클(0.5um)  저장-------------------->
                    if (sParticle05_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle05'" + // sensorCode
                                    $", '{sParticle05}'" +
                                    $", '');";


                    //---------파티클(1.0um)  저장-------------------->
                    if (sParticle10_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle10'" + // sensorCode
                                    $", '{sParticle10}'" +
                                    $", '');";

                    //---------파티클(5.0um)  저장-------------------->
                    if (sParticle50_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle50'" + // sensorCode
                                    $", '{sParticle50}'" +
                                    $", '');";

                    //---------파티클(10.0um)  저장-------------------->
                    if (sParticle100_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle100'" + // sensorCode
                                    $", '{sParticle100}'" +
                                    $", '');";


                    //---------파티클(25.0um) 저장-------------------->
                    if (sParticle250_on)
                        gloVar.sqlCmdText += $"INSERT INTO {gloVar.dataTable} VALUES(" +
                                    $"'{gloVar.DateAndTime}'" +
                                    $", '{sTime}'" +
                                    $", {gloVar.dID}" +
                                    $", 'particle250'" + // sensorCode
                                    $", '{sParticle250}'" +
                                    $", '');";


                    // -----------------DB Insert실행 부분------------->

                    gloVar.sqlCmd.CommandType = CommandType.Text;
                    gloVar.sqlCmd.CommandText = gloVar.sqlCmdText;
                    //mutex_lock.WaitOne();
                    if (gloVar.sqlCmd.CommandText.Length > 0)
                    {
                        gloVar.sqlCmd.Transaction = gloVar.transaction;
                        if (!DataAlreadyExists(gloVar, gloVar.dID, gloVar.DateAndTime))
                        {
                            gloVar.sqlCmd.ExecuteNonQuery();
                            gloVar.transaction.Commit();
                            res = true;
                        }
                        else
                        {
                            txt = " 중복 ";
                        }
                    }
                    //mutex_lock.ReleaseMutex();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n" + ex.Message + ex.StackTrace);
                    try
                    {
                        gloVar.transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("\n" + ex2.Message + ex2.StackTrace);
                    }

                }
                finally
                {
                    gloVar.transaction.Dispose();
                }
            }

            return new Tuple<bool, string>(res, txt);
        }






        /// <summary>
        /// 중복데이터가 이미 저장이 되어있는지 확인
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public bool DataAlreadyExists(GlobalVariables gloVar, int sensorId, string timestamp)
        {
            //mutex_lock2.WaitOne();
            //semaphore.WaitOne();
            duplicate = false;
            try
            {
                gloVar.sqlCmdText = $"SELECT TOP 1 1 FROM {gloVar.dataTable} WHERE {gloVar.S_DTColumns[2]} = {sensorId} AND {gloVar.S_DTColumns[1]} LIKE '{timestamp.Substring(0, timestamp.Length - 7)}%';";

                CheckSqlConnAndCmd(gloVar);

                gloVar.sqlRdr = gloVar.sqlCmd.ExecuteReader();
                if (gloVar.sqlRdr.HasRows)
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
                Console.WriteLine("중복데이터 확인 시 에러 발생. " + ex.Message + ex.StackTrace);
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
            return "sTime: " + sTime + ", s_id_hex: " + dID_long + ", 온도: " + sTemperature + ", 습도: " + sHumidity + ", p0.3: " + sParticle03 + ", p0.5: " + sParticle05 + ", p1.0: " + sParticle10 + ", p5.0: " + sParticle50 + ", p10.0: " + sParticle100 + ", p25.0: " + sParticle250 + " ";
        }



    }
}
