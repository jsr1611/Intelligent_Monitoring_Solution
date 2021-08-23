using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CommonClassLibrary
{
    /// <summary>
    /// 여러 프로젝트에 필요한 메서드들을 선언함. 
    /// Static 타입이기 때문에 instance 생성필요없시 그대로 사용가능. 
    /// </summary>
    public static class GlobalMethods
    {


        /// <summary>
        /// 테이블 특정 Column의 데이터를 반환함.
        /// </summary>
        /// <param name="gloVar">GlobalVariables: 전역변수 클래스</param>
        /// <param name="dataType">데이터 타입: string, int, double</param>
        /// <param name="ColumnName">테이블명</param>
        /// <param name="sqlCmdText">SQL쿼리문</param>
        /// <returns></returns>
        public static List<string> GetColumnDataAsList(GlobalVariables gloVar, string dataType, string ColumnName, string sqlCmdText)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            List<string> res = new List<string>();
            using (SqlConnection con = new SqlConnection(gloVar.sqlConStr))
            {
                try
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCmdText, con))
                    {
                        using (SqlDataAdapter sqlDAptr = new System.Data.SqlClient.SqlDataAdapter(sqlCmd))
                        {
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
                    }

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error getting data as list for table column: {ColumnName}. Error msg: {ex.Message}. {ex.StackTrace}");
                }

            }
            return res;
        }




        /// <summary>
        /// 주어진 테이블의 모든 Column명들을 List형태로 반환함.
        /// </summary>
        /// <param name="g">전역변수 클래스</param>
        /// <param name="tableName">테이블명</param>
        /// <returns></returns>
        public static List<string> GetTableColumnNames(GlobalVariables g, string tableName)
        {
            List<string> tbColNames = new List<string>();
            bool someTableExists = IfTableExists(g.sqlConStr, tableName);
            if (someTableExists)
            {
                string[] restrictions = new string[4] { null, null, $"{tableName}", null };
                using (SqlConnection con = new SqlConnection(g.sqlConStr))
                {
                    try
                    {
                        if (con.State != ConnectionState.Open)
                        {
                            con.Open();
                        }
                        DataTable dt = con.GetSchema("Columns", restrictions);
                        var dv = dt.DefaultView;
                        dv.Sort = "ORDINAL_POSITION ASC";
                        dt = dv.ToTable();
                        tbColNames = dt.AsEnumerable().Select(x => x.Field<string>("COLUMN_NAME")).ToList();
                    }

                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"테이블 컬럼명들을 불러오는중 에러 발생: Table명:{tableName}, db명:{g.dbName}. " + ex.Message + ". " + ex.StackTrace);
                    }

                }
            }
            else
            {
                Console.WriteLine($"테이블 컬럼명들을 불러오는중 에러 발생. 사유: DB테이블을 찾을수 없음. Table명:{tableName}, DB명:{g.dbName}.");
            }
            return tbColNames;
        }




        /// <summary>
        /// 사용중인 센서 장비 ID를 읽어오기
        /// </summary>
        /// <param name="g">전역변수 클래스</param>
        /// <returns>List형태의 센서ID들의  </returns>
        public static List<int> GetSensorIDs(GlobalVariables g)
        {
            List<int> S_IDs = new List<int>();
            if (g.DevTbColumns != null && g.DevTbColumns.Length > 0)
            {
                string cmdStr = $@"SELECT {g.DevTbColumns[0]} FROM {g.dbName}.dbo.{g.deviceTable} WHERE {g.DevTbColumns[g.DevTbColumns.Length -1]} = 'YES' ORDER BY {g.DevTbColumns[0]}";
                // 사용여부 확인없이 모든 ID정보를 불러오는 쿼리
                string cmdStrWithoutUsage = $@"SELECT {g.DevTbColumns[0]} FROM {g.dbName}.dbo.{g.deviceTable} ORDER BY {g.DevTbColumns[0]}";
                using (SqlConnection conn = new SqlConnection(g.sqlConStr))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(cmdStr, conn))
                    {
                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                            {
                                while (sqlDataReader.Read())
                                {
                                    S_IDs.Add(Convert.ToInt32(sqlDataReader[g.DevTbColumns[0]]));
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            //Console.WriteLine("센서 ID 정보를 불러오는중 에러 발생: " + ex.Message + ". " + ex.StackTrace);
                            try
                            {
                                S_IDs.Clear();
                                sqlCommand.CommandText = cmdStrWithoutUsage;
                                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                                {
                                    while (sqlDataReader.Read())
                                    {
                                        S_IDs.Add(Convert.ToInt32(sqlDataReader[g.DevTbColumns[0]]));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("센서 ID 정보를 불러오는중 에러 발생: " + ex.Message + ". " + ex.StackTrace);
                            }
                        }
                    }
                }
            }

            return S_IDs;
        }



        /// <summary>
        /// Collect error messages and save into database.
        /// </summary>
        /// <param name="errTimestamp"></param>
        /// <param name="ex"></param>
        /// <param name="errCategory"></param>
        /// <param name="errType"></param>
        /// <param name="errDescription"></param>
        /// <param name="id"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="remarks"></param>
        public static void ErrorMsgCollect(GlobalVariables gloVar, string errTimestamp, Exception ex, string errCategory, string errType, string errDescription, int id, string ip, string port, string remarks)
        {
            //string ErrorMsgTable = "D_ERROR_MESSAGES";


            string errorContent = string.Empty;
            if (ex.Message.Length != 0)
            {
                errorContent = $"ErrMsg: {ex.Message}. \nStackTrace: {ex.StackTrace}. \nSource: {ex.Source}. TargetSite: {ex.TargetSite}";
            }
            else
            {
                errorContent = "Non-Exception Error";
            }
            try
            {
                using (SqlConnection con = new SqlConnection(gloVar.sqlConStr))
                {
                    gloVar.mutex_lock_e.WaitOne();
                    con.Open();
                    //Console.Write("\n Starting mutex_lock for " + dID + " THD " + System.Threading.Thread.CurrentThread.Name);
                    gloVar.transaction_e = con.BeginTransaction();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = $"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{gloVar.ErrorMsgTable.Item1}') INSERT INTO {gloVar.ErrorMsgTable.Item1} VALUES(" +
                                $"'{errTimestamp}'" +
                                $", '{errCategory}'" +
                                $", '{errType}'" +
                                $", '{errorContent}'" +
                                $", '{errDescription}'" +
                                $", {id}" +
                                $", '{ip}'" +
                                $", '{port}'" +
                                $", '{remarks}'" +
                                $");";
                            cmd.Transaction = gloVar.transaction_e;
                            cmd.Connection = con;
                            cmd.ExecuteNonQuery();
                            gloVar.transaction_e.Commit();
                            //Console.WriteLine($"에러 메시지가 DB에 저장되었다. DB명: {gloVar.dbName}, 에러메시지 저장 테이블명: {gloVar.ErrorMsgTable.Item1}");
                        }
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            gloVar.transaction_e.Rollback();
                        }
                        catch (Exception e2)
                        {

                        }

                    }
                    finally
                    {
                        gloVar.transaction_e.Dispose();
                        gloVar.mutex_lock_e.ReleaseMutex();
                    }
                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine("\n에러 메시지가 성공적으로 저장되지 않았습니다.\n" + ex2.Message + ex2.StackTrace);
            }

        }



        /// <summary>
        /// ErrorMessage 수집을 위해 Table생성
        /// </summary>
        /// <param name="errorMsgTableName">Table명</param>
        public static void ErrorMsgCollect(GlobalVariables gloVar, string errorMsgTableName)
        {
            List<string> TableColumns = new List<string>() {
                        "errDateTime"
                        , "errCategory"
                        , "errType"
                        , "errContent"
                        , "errDescription"
                        , "sID"
                        , "sIP"
                        , "sPORT"
                        , "Remarks"
                    };
            bool tbCreated = gloVar.ErrorMsgTable.Item2;
            if (tbCreated == false)
            {
                string errorMsgCreateTable = $"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{errorMsgTableName}') CREATE TABLE {errorMsgTableName} (" +
                $" {TableColumns[0]} NVARCHAR(50) NOT NULL" +
                $",{TableColumns[1]} NVARCHAR(255) NOT NULL" +
                $",{TableColumns[2]} NVARCHAR(255) NOT NULL" +
                $",{TableColumns[3]} NVARCHAR(MAX) NOT NULL" +
                $",{TableColumns[4]} NVARCHAR(MAX) NULL" +
                $",{TableColumns[5]} int NULL" +
                $",{TableColumns[6]} NVARCHAR(255) NULL" +
                $",{TableColumns[7]} NVARCHAR(50) NULL" +
                $",{TableColumns[8]} NVARCHAR (255) NULL" +
                $", INDEX IX_{TableColumns[0]} NONCLUSTERED({TableColumns[0]})" +
                $", INDEX IX_{TableColumns[1]} NONCLUSTERED({TableColumns[1]})" +
                $", INDEX IX_{TableColumns[2]} NONCLUSTERED({TableColumns[2]})" +
                $")";

                using (SqlConnection con = new SqlConnection(gloVar.sqlConStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(errorMsgCreateTable, con))
                    {
                        cmd.ExecuteNonQuery();
                        tbCreated = true;
                    }
                }
            }

            gloVar.ErrorMsgTable = (errorMsgTableName, tbCreated, TableColumns);
        }



        /// <summary>
        /// DB에 테이블이 존재하는지 확인해주는 함수.
        /// </summary>
        /// <param name="tableName">테이블명</param>
        /// <returns>존재하면 true, 아니면 false</returns>
        public static bool IfTableExists(string sqlConStr, string tableName)
        {
            bool res = false;

            string sqlCheckTbCreated = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{tableName}';";

            using (SqlConnection myConn = new SqlConnection(sqlConStr))
            {
                try
                {
                    if (myConn.State != ConnectionState.Open)
                    {
                        myConn.Open();
                    }
                    using (SqlCommand checkTbCreatedCmd = new SqlCommand(sqlCheckTbCreated, myConn))
                    {
                        using (SqlDataReader r = checkTbCreatedCmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                if (Convert.ToInt32(r.GetValue(0)) == 1)
                                {
                                    res = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("DB에 테이블이 존재하는지 확인하는중 에러 발생: " + ex.Message + ". " + ex.StackTrace);
                }
            }

            return res;
        }



        /// <summary>
        /// Sensor 데이터를 저장하기 위해 테이블을 생성해 주는 함수.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool CreateDataTable(GlobalVariables g, string dbName)
        {
            string timeStampName = "DateAndTime";

            string dataTableName = $"{dbName}[0]" + "_DATATABLE";
            bool res = IfTableExists(g.sqlConStr, dataTableName);

            if (!res)
            {
                //Remove the comments below and change the sql script to create a data table
                //string sqlCreateDataTable = $"CREATE TABLE {dataTableName} (" +
                //    $" {g.DevTbColumns[0]} INT NOT NULL " +
                //    $", INDEX IX_{g.DevTbColumns[0]} NONCLUSTERED({g.DevTbColumns[0]}) " +
                //    $", {g.DevTbColumns[1]} NVARCHAR(25) NOT NULL " +
                //    $", {timeStampName} NVARCHAR(25) NOT NULL" +
                //    $", INDEX IX_{timeStampName} NONCLUSTERED({timeStampName}) );";

                /*
                --This creates a primary key
                , CONSTRAINT PK_MyTable PRIMARY KEY CLUSTERED(a)

                --This creates a unique nonclustered index on columns b and c
                , CONSTRAINT IX_MyTable1 UNIQUE(b, c)

                --This creates a standard non-clustered index on(d, e)
                ,INDEX IX_MyTable4 NONCLUSTERED(d, e)

                    */


                using (SqlConnection myConn = new SqlConnection(g.sqlConStr))
                {
                    try
                    {
                        if (myConn.State != ConnectionState.Open)
                        {
                            myConn.Open();
                        }
                        //Remove comments below to run the command
                        //using (SqlCommand createTableCmd = new SqlCommand(sqlCreateDataTable, myConn))
                        //{
                        //    createTableCmd.ExecuteNonQuery();
                        //    res = true;
                        //}
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("DB에 테이블을 생성하는중 에러 발생: " + ex.Message + ". " + ex.StackTrace);
                        Console.ReadKey();
                    }
                }
            }
            return res;
        }



        /// <summary>
        /// DB정보(서버명 및 DB명)를 출력해주는 함수.
        /// </summary>
        /// <param name="g">전역 변수 클래스</param>
        public static void printDataBaseDetails(GlobalVariables g)
        {
            Console.WriteLine($"----------------DB 정보----------------\nServer Name:\t{g.dbServerName}\nDB Name:\t{g.dbName}\n---------------------------------------\n");
        }
    }
}
