using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TtsUwpTest
{
    class LogTest
    {
        public static void LogWrite(string str)
        {
            FileStream fs = new FileStream(ApplicationData.Current.LocalFolder.Path + "//log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(str);
            }

        }
    }
}
