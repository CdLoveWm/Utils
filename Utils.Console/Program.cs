using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Utils.Encrypt;
using sconsole = System.Console;

namespace Utils.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            sconsole.WriteLine("Hello World!");

            string key = "12345678900000001234567890000000";
            string encStr = AesUtil.EncryptByAES("RedChen", key);
            sconsole.WriteLine(encStr);
            string decStr = AesUtil.DecryptByAES(encStr, key);
            sconsole.WriteLine(decStr);

            sconsole.ReadLine();
        }
    }
}
