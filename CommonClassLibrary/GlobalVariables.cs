using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using EasyModbus;

namespace CommonClassLibrary 
{
    /// <summary>
    /// 여러 프로젝트에 필요한 변수들을 선언함. 
    /// Object instance 생성후 사용가능.
    /// </summary>
    public class GlobalVariables
    {
        // Client 관련 변수들의 선언
        public string[] COMPort_List { get; set; }
        public ModbusClient[] modbusClient_List { get; set; }
        public bool[] bad_clientFlag_List { get; set; }
        public int[] ID_List { get; set; } 
        public int[] Port_List { get; set; }
        public string[] IPAddress_List { get; set; } 
        
        
        // 데이터베이스 및 SQL Connection 관련 변수들의 선언
        public string dbServerName { get; set; }
        public string dbName { get; set; }
        public string dbUID { get; set; }
        public string dbPWD { get; set; }

        public string dataTable { get; set; }
        public string dCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[] S_DTColumns { get; set; } 
        public string deviceTable { get; set; }
        public string[] DevTbColumns { get; set; }
        public string sanghanHahanTable { get; set; }
        public List<string> sanghanHahanColumns { get; set; }



        public SqlConnection sqlConn { get; set; }
        public SqlCommand sqlCmd { get; set; }
        public SqlDataReader sqlRdr { get; set; }
        public SqlDataAdapter sqlDAptr { get; set; }
        public SqlTransaction transaction_w { get; set; }
        public SqlTransaction transaction_e { get; set; }
        public string sqlConStr { get; set; }
        public string sqlCmdText { get; set; }
        public string sqlDefltCmdText { get; set; }

        // Common data variables
        
        public DateTime timeNow { get; set; }
        public int[] d { get; set; }
        public string DateAndTime { get; set; }
        public int dID { get; set; }
        public bool insertDB_OK { get; set; }
        public Mutex mutex_lock = new Mutex();
        public Mutex mutex_lock_e = new Mutex(); 

        /// <summary>
        /// Item1: table명, Item2: table존재여부, Item3: DB TableColumn명록
        /// <list type="table">
        /// DB TableColumn명록: 
        /// <list type="table">[0] errDateTime NOT NULL, [1] errCategory NOT NULL, [2] errType NOT NULL, [3] errContent NOT NULL, [4] errDescription, [5] sID, [6] sIP, [7] sPORT, [8] Remarks.
        /// </list>
        /// </list>
        /// </summary>
        public ValueTuple<string, bool, List<string>> ErrorMsgTable { get; set; }
    }
}
