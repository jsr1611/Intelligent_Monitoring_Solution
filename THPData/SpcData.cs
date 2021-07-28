using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CommonClassLibrary;

namespace THPData
{
    public class SpcData
    {

        public string DateAndTime { get; set; }
        public int dID { get; set; }
        public bool insertDB_OK { get; set; }
        public int[] d { get; set; }
        public DateTime timeNow { get; set; }

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
        public SqlTransaction transaction { get; set; }
        public string sqlConStr { get; set; }
        public string sqlCmdText { get; set; }
        public string sqlDefltCmdText { get; set; }


        internal Int64 highValue { get; set; }
        internal Int64 lowValue { get; set; }
        private List<string> tbColumns { get; set; }
        private List<string> sensorTypes { get; set; }
        private DataTable dtbl { get; set; }
        private DataView dview { get; set; }
        private string[] resultingArray { get; set; }
        private bool duplicate { get; set; }
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
                gloVar.sanghanHahanColumns = GetTableColumnNames(gloVar, gloVar.sanghanHahanTable);

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
        /// 주어진 테이블의 모든 Column명들을 List형태로 반환함.
        /// </summary>
        /// <param name="gloVar">GlobalVariables Object instance </param>
        /// <param name="tableName">table명</param>
        /// <returns>gloVar.sanghanHahanColumns List of string values.</returns>
        public List<string> GetTableColumnNames(GlobalVariables gloVar, string tableName)
        {
            if (gloVar.sanghanHahanColumns == null)
                gloVar.sanghanHahanColumns = new List<string>();
            try
            {
                CheckSqlConnAndCmd(gloVar);
                resultingArray = new string[4] { null, null, $"{tableName}", null }; // string[] restrictions = resultingArray
                dtbl = sqlConn.GetSchema("Columns", resultingArray);
                dview = dtbl.DefaultView;
                dview.Sort = "ORDINAL_POSITION ASC";
                dtbl = dview.ToTable();
                gloVar.sanghanHahanColumns = dtbl.AsEnumerable().Select(x => x.Field<string>("COLUMN_NAME")).ToList();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error in getting column names for table: {tableName}. \n" + ex.Message + ex.StackTrace);
            }

            sqlCmd.Dispose();

            return gloVar.sanghanHahanColumns;
        }



        /// <summary>
        /// 테이블 특정 Column의 데이터를 반환함.
        /// </summary>
        /// <param name="dataType">데이터 타입: string, int, double</param>
        /// <param name="sqlStr">SQL쿼리문</param>
        /// <param name="ColumnName">테이블명</param>
        /// <returns></returns>
        public List<string> GetColumnDataAsList(GlobalVariables gloVar, string dataType, string ColumnName, string sqlCmdText)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            List<string> res = new List<string>();


            CheckSqlConnAndCmd(gloVar);



            if (sqlDAptr == null)
                sqlDAptr = new System.Data.SqlClient.SqlDataAdapter();

            try
            {
                sqlCmd.CommandText = sqlCmdText;
                sqlDAptr.SelectCommand = sqlCmd;
                sqlDAptr.Fill(ds);
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

                    //return res;
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error getting data as list for table column: {ColumnName}. Error msg: {ex.Message}. {ex.StackTrace}");
            }
            sqlDAptr.Dispose();
            sqlCmd.Dispose();
            return res;
        }

        /// <summary>
        /// Clears the field values to default as follows:
        /// string => string.Empty;
        /// int => 0;
        /// bool => false;
        /// </summary>

        public void Clear()
        {
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

        }


        /// <summary>
        /// Check if SQLConnection is open and SQLCommand is ready to take commandText
        /// </summary>
        /// <param name="gloVar"></param>
        private void CheckSqlConnAndCmd(GlobalVariables gloVar)
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

            sqlCmd.CommandText = sqlCmdText;
            sqlCmd.Connection = sqlConn;
        }





        /// <summary>
        /// 수집된 데이터를 DB에 저장해주는 함수
        /// </summary>
        /// <param name="data">수집 데이터</param>
        /// <returns>수집이 잘 되면 true반환함</returns>
        public bool StoreDataToDB(GlobalVariables gloVar)
        {
            bool res = false;

            CheckSqlConnAndCmd(gloVar);


            if (dID != 0 && DateAndTime.Length != 0 && sTime.Length != 0)
            {
                transaction = sqlConn.BeginTransaction();
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
                    //mutex_lock.WaitOne();
                    if (sqlCmd.CommandText.Length > 0)
                    {
                        sqlCmd.Transaction = transaction;
                        //if (!DataAlreadyExists(gloVar, dID, DateAndTime))
                        //{
                        sqlCmd.ExecuteNonQuery();
                        transaction.Commit();
                        res = true;
                        //}
                        //else
                        //{
                        //    txt = " 중복 ";
                        //}
                    }
                    //mutex_lock.ReleaseMutex();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n" + ex.Message + ex.StackTrace);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("\n" + ex2.Message + ex2.StackTrace);
                    }

                }
                finally
                {
                    sqlConn.Close();
                    //sqlConn.Dispose();
                    transaction.Dispose();
                }
            }

            return res;
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
                sqlCmdText = $"SELECT TOP 1 1 FROM {gloVar.dataTable} WHERE {gloVar.S_DTColumns[2]} = {sensorId} AND {gloVar.S_DTColumns[1]} LIKE '{timestamp.Substring(0, timestamp.Length - 7)}%';";

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
