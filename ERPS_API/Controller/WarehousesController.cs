using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DAL_MySQL;
using ERPS_API.Utils;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 仓库控制器
    /// </summary>
    public class WarehousesController : ApiController
    {
        private erpsEntities db = new erpsEntities();


        /// <summary>
        /// 获取所有仓库ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmwarehouseId")]
        public IHttpActionResult GetmwarehouseId()
        {
            var listWar = from w in db.mwarehouse
                          select w.WHID;
            return Ok(listWar);
        }

        /// <summary>
        /// 获取所有仓库ID和仓库名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmwarehouseIdAndName")]
        public IHttpActionResult GetmwarehouseIdAndName()
        {
            var listWar = from w in db.mwarehouse
                          let p1 = new
                          {
                              value = w.WHID,
                              label = w.WHName
                          }
                          select p1;
            return Ok(listWar);
        }

        /// <summary>
        /// 获取所有仓库信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Getmwarehouse")]
        public IHttpActionResult Getmwarehouse(int pagesize, int currentPage)
        {
            var listWar = from w in db.mwarehouse
                          select w;
            listWar = listWar.OrderByDescending(w => w.CreateDate);
            var oData = new { total = listWar.Count(), rows = listWar.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件获取仓库信息
        /// </summary>
        /// <param name="wHID"></param>
        /// <param name="wHName"></param>
        /// <param name="phone"></param>
        /// <param name="contacts"></param>
        /// <param name="address"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmwarehouseByCondition")]
        public IHttpActionResult GetmwarehouseByCondition(string whid, string whName, string phone, string contacts, string address, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listWar = from w in db.mwarehouse
                          select w;
            if (!string.IsNullOrEmpty(whid))
            {
                listWar = listWar.Where(w => w.WHID.Contains(whid));
            }
            if (!string.IsNullOrEmpty(whName))
            {
                listWar = listWar.Where(w => w.WHName.Contains(whName));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                listWar = listWar.Where(w => w.Phone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(contacts))
            {
                listWar = listWar.Where(w => w.Contacts.Contains(contacts));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listWar = listWar.Where(w => w.CreateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listWar = listWar.Where(w => w.CreateDate < date);
            }
            if (!string.IsNullOrEmpty(address))
            {
                listWar = listWar.Where(w => w.Address.Contains(address));
            }
            if (listWar == null)
            {
                return NotFound();
            }

            listWar = listWar.OrderByDescending(w => w.CreateDate);
            var oData = new { total = listWar.Count(), rows = listWar.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据id修改仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mwarehouse"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/Putmwarehouse")]
        public IHttpActionResult Putmwarehouse(string id, mwarehouse mwarehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mwarehouse.WHID)
            {
                return BadRequest();
            }

            db.Entry(mwarehouse).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (!mwarehouseExists(id))
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
        /// 新增仓库信息
        /// </summary>
        /// <param name="mwarehouse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Postmwarehouse")]
        public IHttpActionResult Postmwarehouse(mwarehouse mwarehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            mwarehouse.CreateDate = DateTime.Now;
            db.mwarehouse.Add(mwarehouse);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (mwarehouseExists(mwarehouse.WHID))
                {
                    return new PageResult("Conflict", Request);
                }
                else
                {
                    throw;
                }
            }

            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 删除仓库信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletemwarehouse")]
        public IHttpActionResult Deletemwarehouse(string delUid)
        {
            String[] strArray = delUid.Split(',');
            foreach (string id in strArray)
            {
                var list = db.mwarehouse.Where(w => w.WHID == id).FirstOrDefault();
                db.mwarehouse.Remove(list);
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

        private bool mwarehouseExists(string id)
        {
            return db.mwarehouse.Count(e => e.WHID == id) > 0;
        }
    }
}