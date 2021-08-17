using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary
{
    public class GlobalMethods
    {



        /// <summary>
        /// 주어진 테이블의 모든 Column명들을 List형태로 반환함.
        /// </summary>
        /// <param name="tableName">테이블명</param>
        /// <returns></returns>
        public List<string> GetTableColumnNames(GlobalVariables g, string tableName)
        {
            List<string> tbColNames = new List<string>();
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
                    Console.WriteLine(ex.Message);
                }
            }

            return tbColNames;
        }




        /// <summary>
        /// 사용중인 센서 장비 ID를 읽어오기
        /// </summary>
        /// <returns></returns>
        public List<int> GetSensorIDs(GlobalVariables g)
        {
            List<int> S_IDs = new List<int>();
            string cmdStr = $@"SELECT {g.DevTbColumns[0]} FROM {g.dbName}.dbo.{g.deviceTable}";
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
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return S_IDs;
        }




        /// <summary>
        /// DB에 테이블이 존재하는지 확인해주는 함수.
        /// </summary>
        /// <param name="tableName">테이블명</param>
        /// <returns>존재하면 true, 아니면 false</returns>
        public bool IfTableExists(string sqlConStr, string tableName)
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
                    Console.WriteLine("DB에 테이블이 존재하는지 확인하는중 에러 발생: " + ex.Message +". "+ ex.StackTrace);
                }
            }

            return res;
        }



        /// <summary>
        /// Sensor 데이터를 저장하기 위해 테이블을 생성해 주는 함수.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool CreateDataTable(GlobalVariables g, string dbName)
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
                        Console.WriteLine("DB에 테이블을 생성하는중 에러 발생: " + ex.Message + ". "+ex.StackTrace);
                        Console.ReadKey();
                    }
                }
            }
            return res;
        }

    }
}
