using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary
{
    //Dp = Differential pressure (DP100) / 차압 데이터
    public class DpData
    {
        private int sensorId;
        private string timestamp;
        private string mmAqua;
        private string pascal;
        private string mbar;
        private string kpascal;
        private string hpascal;
        private string inchH2O;
        private string mmHg;
        private string inchHg;


        private bool hpa_on;
        private bool inchh2o_on;
        private bool inchhg_on;
        private bool kpascal_on;
        private bool mbar_on;
        private bool mmAqua_on;
        private bool mmhg_on;
        private bool pascal_on;
        
        public DpData()
        {

        }

        public bool check_hpaOn
        {
            get { return hpa_on; }
            set { hpa_on = value; }
        }

        public bool check_inchh2oOn
        {
            get { return inchh2o_on; }
            set { inchh2o_on = value; }
        }
        public bool check_inchhgOn
        {
            get { return inchhg_on; }
            set { inchhg_on = value; }
        }
        public bool check_kpaOn
        {
            get { return kpascal_on; }
            set { kpascal_on = value; }
        }
        public bool check_mbarOn
        {
            get { return mbar_on; }
            set { mbar_on = value; }
        }
        public bool check_mmh2oOn
        {
            get { return mmAqua_on; }
            set { mmAqua_on = value; }
        }
        public bool check_mmhgOn
        {
            get { return mmhg_on; }
            set { mmhg_on = value; }
        }
        public bool check_paOn
        {
            get { return pascal_on; }
            set { pascal_on = value; }
        }





        /// <summary>
        /// 서버 시간 기준으로 데이터 수집(측정) 시간
        /// </summary>
        public string pcTimestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public int sID
        {
            get { return sensorId; }
            set { sensorId = value; }
        }

        public string s_mmH2o
        {
            get { return mmAqua; }
            set { mmAqua = value; }
        }


        public string s_pa
        {
            get { return pascal; }
            set { pascal = value; }
        }

        public string s_mbar
        {
            get { return mbar; }
            set { mbar = value; }
        }

        public string s_kpa
        {
            get { return kpascal; }
            set { kpascal = value; }
        }


        public string s_hpa
        {
            get { return hpascal; }
            set { hpascal = value; }
        }


        public string s_inchH2O
        {
            get { return inchH2O; }
            set { inchH2O = value; }
        }

        public string s_mmHg
        {
            get { return mmHg; }
            set { mmHg = value; }
        }

        public string s_inchHg
        {
            get { return inchHg; }
            set { inchHg = value; }
        }




        /*private string mmAqua;
        private string pascal;
        private string mbar;
        private string kpascal;
        private string hpascal;
        private string inchH2O;
        private string mmHg;
        private string inchHg;*/

        public override string ToString()
        {
            return sID.ToString() + "번 센서, PC 시간: " + timestamp + ", mmH2O: " + mmAqua + ", Pa: " + pascal + "" +
                ", mbar: " + mbar + ", kPa: " + kpascal + ", hPa:" + hpascal + ", inchH2O: " + inchH2O + ", mmHg: " + mmHg + ", inchHg: " + inchHg + " ";
        }
    }
}

