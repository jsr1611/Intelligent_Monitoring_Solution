using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary
{
    public class AppInfo
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        private static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
        private static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);
        public static string StartupPath
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder(260);
                GetModuleFileName(NullHandleRef, stringBuilder, stringBuilder.Capacity);
                return Path.GetDirectoryName(stringBuilder.ToString());
            }
        }
    }
}