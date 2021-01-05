using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ERPS_API.App_Start
{
    public class ValidCodeUtils
    {
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="prePassword"></param>
        /// <returns></returns>
        public static string EncryptPassword(string prePassword)
        {
            string returnPassword = "";
            byte[] HashData;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] buffer = System.Text.Encoding.Default.GetBytes(prePassword);
            HashData = md5.ComputeHash(buffer);
            returnPassword = BitConverter.ToString(HashData);
            returnPassword = returnPassword.Replace("-", "");
            return returnPassword;
        }
    }
}