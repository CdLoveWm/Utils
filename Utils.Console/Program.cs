using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Utils.Encrypt;
using Utils.QrCode;
using sconsole = System.Console;

namespace Utils.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            sconsole.WriteLine("Hello World!");

            #region 加密解密
            //string key = "12345678900000001234567890000000";
            //string encStr = AesUtil.EncryptByAES("RedChen", key);
            //sconsole.WriteLine(encStr);
            //string decStr = AesUtil.DecryptByAES(encStr, key);
            //sconsole.WriteLine(decStr);
            #endregion

            #region 二维码生成
            string content = "二维码内容";
            string src = "/QrCode.png";
            QrCodeUtil.GenerateQrCode(content, src);
            #endregion

            sconsole.ReadLine();
        }
    }
}
