using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionTool
{
    public class EncriptionFunction
    {
        //加密
        public static string Encryption(string express)
        {
            //CspParameters param = new CspParameters();
            //param.KeyContainerName = "create2017";//密匙容器的名称，保持加密解密一致才能解密成功
            //using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            //{
            //    byte[] plaindata = Encoding.UTF8.GetBytes(express);//将要加密的字符串转换为字节数组
            //    byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
            //    return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            //}
            // 字符串加密
            string strKey = "12345678";
            string strKiv = "abcdefgd";
            byte[] buffer = Encoding.UTF8.GetBytes(express);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strKiv)), CryptoStreamMode.Write);
           encStream.Write(buffer, 0, buffer.Length);
           encStream.FlushFinalBlock();
           return Convert.ToBase64String(ms.ToArray()).Replace("+", "%");
        }

        //解密
        public static string Decrypt(string ciphertext)
        {
            //CspParameters param = new CspParameters();
            //param.KeyContainerName = "create2017";
            //using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            //{
            //    byte[] encryptdata = Convert.FromBase64String(ciphertext);
            //    byte[] decryptdata = rsa.Decrypt(encryptdata, false);
            //    return Encoding.UTF8.GetString(decryptdata);
            //}
            string strKey = "12345678";
            string strKiv = "abcdefgd";
           ciphertext = ciphertext.Replace("%", "+");
           byte[] buffer = Convert.FromBase64String(ciphertext);
           MemoryStream ms = new MemoryStream();
           DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
           CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strKiv)), CryptoStreamMode.Write);
           encStream.Write(buffer, 0, buffer.Length);
           encStream.FlushFinalBlock();
           return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
