using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ERPS_API.App_Start;
using BLL;
using DAL_MySQL;
using System.Web.Security;
using ERPS_API.Utils;
using System.Web.Script.Serialization;
using System.Net;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Model;
using Newtonsoft.Json;

namespace ERPS_API.Controller
{
    public class LoginController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        [HttpPost]
        [Route("api/Login")]
        public IHttpActionResult Login()
        {
            try
            {
                var result = new UserDataPackage<User>();
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                HttpRequestBase request = context.Request;

                string str = new System.IO.StreamReader(request.InputStream).ReadToEnd();

                User userInfo = JsonConvert.DeserializeObject<User>(str);

                userInfo.password = ValidCodeUtils.EncryptPassword(userInfo.password);  

                if (!ValidateUser(userInfo.uid, userInfo.password))
                {
                    result = new UserDataPackage<User> { code = 20001 };  
                    return new PageResult(result, Request);
                }

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, userInfo.uid, DateTime.Now,
                DateTime.Now.AddHours(1), true, string.Format("{0}&{1}", userInfo.uid, userInfo.password),
                FormsAuthentication.FormsCookiePath);
                string authTicket = FormsAuthentication.Encrypt(ticket);

                result = new UserDataPackage<User>{ data = new User { token = authTicket }, code = 20000 };

                return new PageResult(result, Request);
            }
            catch (Exception ex)
            {
                return new PageResult(ex.ToString(), Request);
            }
        }


        [HttpGet]
        [Route("api/ValidatePass")]
        public IHttpActionResult ValidatePass(int uid,string password)
        {
            try
            {
                var result = new UserDataPackage<User>();
                password = ValidCodeUtils.EncryptPassword(password);
                if (!ValidateUser(uid, password))
                {
                    return new PageResult("error", Request);
                }
                result = new UserDataPackage<User> { code = 20000 };
                return new PageResult(result, Request);
            }
            catch (Exception)
            {
                throw;
            }
        }



        //校验用户ID和密码
        private bool ValidateUser(string strUser, string strPwd)
        {
            return db.suser.Count(u => u.UID == strUser && u.Pass == strPwd) > 0;
        }

        //校验UID和密码
        private bool ValidateUser(int uid, string strPwd)
        {
            return db.suser.Count(u => u.UserId == uid && u.Pass == strPwd) > 0;
        }

    }
}