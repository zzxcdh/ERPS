using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using System.Web.Security;
using DAL_MySQL;
using ERPS_API.App_Start;
using ERPS_API.Utils;
using Model;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    // [Authorize]
    public class UsersController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 用户登录成功后获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("api/UserInfo")]
        public IHttpActionResult Getsuser(String token)
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            //解密ticket
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(token);

            string userName = ticket.Name;

            var user = from u in db.suser
                       join r in db.srole on u.RoleID equals r.RoleID
                       where u.UID == userName
                       let p1 = new
                       {
                           u.UID,
                           u.UserName,
                           u.StaffId,
                           u.Position,
                           u.Department,
                           r.RoleName
                       }
                       select p1;
            var oData = user.FirstOrDefault();
            var result = new UserDataPackage<Object>{ data = oData, code = 20000 };
        
            return new PageResult(result, Request);
        }


        /// <summary>
        /// 用户退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Logout")]
        public IHttpActionResult Logout()
        {
            FormsAuthentication.SignOut();
            var result = new UserDataPackage<User>{ code = 20000 };
            return new PageResult(result, Request);
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Getsuser")]
        public IHttpActionResult Getsuser(int pagesize, int currentPage)
        {
            var listUser = from u in db.suser
                           select u;
            listUser = listUser.OrderByDescending(u => u.CreateDate);
            var oData = new { total = listUser.Count(), rows = listUser.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }


        /// <summary>
        /// 根据条件获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="department"></param>
        /// <param name="position"></param>
        /// <param name="role"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetsuserByCondition")]
        public IHttpActionResult GetsuserByCondition(string id,string name,string phone,string department,string position,string role,int pagesize, int currentPage)
        {
            var listUser = from u in db.suser
                           select u;
            if (!string.IsNullOrEmpty(id))
            {
                listUser = listUser.Where(u => u.UID.Contains(id));
            }
            if (!string.IsNullOrEmpty(name))
            {
                listUser = listUser.Where(u => u.UserName.Contains(name));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                listUser = listUser.Where(u => u.Phone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(department))
            {
                listUser = listUser.Where(u => u.Department == department);
            }
            if (!string.IsNullOrEmpty(position))
            {
                listUser = listUser.Where(u => u.Position == position);
            }
            if (!string.IsNullOrEmpty(role))
            {
                listUser = listUser.Where(u => u.RoleID == role);
            }
            if (listUser == null)
            {
                return NotFound();
            }

            listUser = listUser.OrderByDescending(u => u.CreateDate);
            var oData = new { total = listUser.Count(), rows = listUser.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据id修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="suser"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/Putsuser")]
        public IHttpActionResult Putsuser(int id,suser suser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != suser.UserId)
            {
                return BadRequest();
            }
            suser.UpdateDate = DateTime.Now;

            db.Entry(suser).State = EntityState.Modified;

            if (suser.Pass == null || suser.Pass.Trim() == "")
            {
                db.Entry(suser).Property("Pass").IsModified = false;
            }
            else
            {
                suser.Pass = ValidCodeUtils.EncryptPassword(suser.Pass);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (suserExists(suser.UID))
                {
                    return new PageResult("Conflict", Request);
                }
                else
                {
                    throw;
                }
            }
        
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 新增用户信息
        /// </summary>
        /// <param name="suser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Postsuser")]
        public IHttpActionResult Postsuser(suser suser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (suserExists(suser.UID))
            {
                return new PageResult("Conflict", Request);
            }
            suser.Pass = ValidCodeUtils.EncryptPassword(suser.Pass);
            suser.CreateDate = DateTime.Now;
            db.suser.Add(suser);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletesuser")]
        public IHttpActionResult Deletesuser(String delUid)
        {
            String[] strArray = delUid.Split(',');
            int[] intArray = Array.ConvertAll(strArray, int.Parse);
            foreach (int id in intArray)
            {
                var list = db.suser.Where(su => su.UserId == id).FirstOrDefault();
                db.suser.Remove(list);
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return Content<string>(HttpStatusCode.OK, "OK");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool suserExists(string id)
        {
            return db.suser.Count(e => e.UID == id) > 0;
        }
    }
}