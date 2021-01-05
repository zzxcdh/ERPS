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
    /// 产品分类控制器
    /// </summary>
    public class ProductTypesController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 获取全部产品分类名称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmproducttypeName")]
        public IHttpActionResult GetmproducttypeName()
        {
            var listProdType = from p in db.mproducttype
                               select p.PdtTypeName;
            return Ok(listProdType);
        }

        /// <summary>
        /// 获取全部产品分类信息 --- 不分页
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmproducttypeAll")]
        public IHttpActionResult Getmproducttype()
        {
            var listProdType = from p in db.mproducttype
                               select p;
            listProdType = listProdType.OrderByDescending(p => p.CreateDate);            
            return Ok(listProdType);
        }

        /// <summary>
        /// 获取全部产品分类信息 --- 分页
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Getmproducttype")]
        public IHttpActionResult Getmproducttype(int pagesize, int currentPage)
        {
            var listProdType = from p in db.mproducttype
                          select p;
            listProdType = listProdType.OrderByDescending(p => p.CreateDate);
            var oData = new { total = listProdType.Count(), rows = listProdType.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件获取产品分类信息
        /// </summary>
        /// <param name="pdtTypeID"></param>
        /// <param name="pdtTypeName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmproducttypeByCondition")]
        public IHttpActionResult GetmproducttypeByCondition(string pdtTypeID, string pdtTypeName, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listProdType = from p in db.mproducttype
                          select p;
            if (!string.IsNullOrEmpty(pdtTypeID))
            {
                listProdType = listProdType.Where(p => p.PdtTypeID.Contains(pdtTypeID));
            }
            if (!string.IsNullOrEmpty(pdtTypeName))
            {
                listProdType = listProdType.Where(p => p.PdtTypeName.Contains(pdtTypeName));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listProdType = listProdType.Where(p => p.CreateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listProdType = listProdType.Where(p => p.CreateDate < date);
            }
            if (listProdType == null)
            {
                return NotFound();
            }

            listProdType = listProdType.OrderByDescending(p => p.CreateDate);
            var oData = new { total = listProdType.Count(), rows = listProdType.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据id修改产品分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mproducttype"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/Putmproducttype")]
        public IHttpActionResult Putmproducttype(string id, mproducttype mproducttype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mproducttype.PdtTypeID)
            {
                return BadRequest();
            }

            db.Entry(mproducttype).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (!mproducttypeExists(id))
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
        /// 新增产品分类信息
        /// </summary>
        /// <param name="mproducttype"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Postmproducttype")]
        public IHttpActionResult Postmproducttype(mproducttype mproducttype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.mproducttype.Add(mproducttype);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (mproducttypeExists(mproducttype.PdtTypeID))
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
        /// 删除产品分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletemproducttype")]
        public IHttpActionResult Deletemproducttype(string delUid)
        {
            String[] strArray = delUid.Split(',');
            foreach (string id in strArray)
            {
                var list = db.mproducttype.Where(p => p.PdtTypeID == id).FirstOrDefault();
                db.mproducttype.Remove(list);
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

        private bool mproducttypeExists(string id)
        {
            return db.mproducttype.Count(e => e.PdtTypeID == id) > 0;
        }
    }
}