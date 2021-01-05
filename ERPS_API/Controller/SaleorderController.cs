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

namespace ERPS_API.Controller
{
    /// <summary>
    /// 销售订单控制器
    /// </summary>
    public class SaleorderController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        [HttpGet]
        [Route("api/GetProductBySaleOrderNO")]
        public IHttpActionResult GetProductBySaleOrderNO(string id, string warId, int pagesize, int currentPage)
        {
            var listOrderdtl = (from o in db.tsaleorderdtl
                                join p in db.mproduct
                                on o.PdtID equals p.PdtID
                                join m in db.mcustomerprodprice on new { o.PdtID, o.CusID, o.Model } equals new { m.PdtID, m.CusID, m.Model }
                                join i in db.twhinventory on new {a = o.PdtID, b = warId } equals new { a = i.PdtID , b = i.WHID} into ii
                                from i in ii.DefaultIfEmpty()
                                where o.SaleOrderNO == id && o.LftNum > 0
                                let p1 = new
                                {
                                    o.SeqNo,
                                    o.PdtID,
                                    p.PdtName,
                                    p.Spec,
                                    m.Model,
                                    p.MgrInfo,
                                    p.MakeIn,
                                    m.CusItmCD,
                                    m.CusItmName,
                                    p.Unit,
                                    o.OrderNum,
                                    o.LftNum,
                                    i.InvNum,
                                    m.SalePrice,
                                    TotalAmount = o.OrderNum * m.SalePrice,
                                    Remark = o.Remark1
                                }
                                select p1);
            listOrderdtl = listOrderdtl.OrderBy(o => o.PdtID).ThenBy(o => o.PdtID);
            var oData = new { total = listOrderdtl.Count(), rows = listOrderdtl.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        [HttpGet]
        [Route("api/GetSaleorderNo")]
        public IHttpActionResult GetSaleorderNo()
        {
            var listSaleOrder = from s in db.tsaleorder
                                select s.SaleOrderNO;
            return Ok(listSaleOrder);
        }

        [HttpGet]
        [Route("api/GetSaleorderNoAndName")]
        public IHttpActionResult GetSaleorderNoAndName()
        {
            var listSaleOrder = from s in db.tsaleorder
                                join c in db.mcustomer
                                on s.CusID equals c.CusID
                                let p1 = new
                                {
                                    value = s.SaleOrderNO,
                                    label = c.CusName
                                }
                                where s.State != "F"
                                select p1;
            return Ok(listSaleOrder);
        }

        // GET: api/Saleorder
        public IQueryable<tsaleorderdtl> Gettsaleorderdtl()
        {
            return db.tsaleorderdtl;
        }

        // GET: api/Saleorder/5
        [ResponseType(typeof(tsaleorderdtl))]
        public IHttpActionResult Gettsaleorderdtl(string id)
        {
            tsaleorderdtl tsaleorderdtl = db.tsaleorderdtl.Find(id);
            if (tsaleorderdtl == null)
            {
                return NotFound();
            }

            return Ok(tsaleorderdtl);
        }

        // PUT: api/Saleorder/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttsaleorderdtl(string id, tsaleorderdtl tsaleorderdtl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tsaleorderdtl.SaleOrderNO)
            {
                return BadRequest();
            }

            db.Entry(tsaleorderdtl).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tsaleorderdtlExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Saleorder
        [ResponseType(typeof(tsaleorderdtl))]
        public IHttpActionResult Posttsaleorderdtl(tsaleorderdtl tsaleorderdtl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tsaleorderdtl.Add(tsaleorderdtl);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (tsaleorderdtlExists(tsaleorderdtl.SaleOrderNO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tsaleorderdtl.SaleOrderNO }, tsaleorderdtl);
        }

        // DELETE: api/Saleorder/5
        [ResponseType(typeof(tsaleorderdtl))]
        public IHttpActionResult Deletetsaleorderdtl(string id)
        {
            tsaleorderdtl tsaleorderdtl = db.tsaleorderdtl.Find(id);
            if (tsaleorderdtl == null)
            {
                return NotFound();
            }

            db.tsaleorderdtl.Remove(tsaleorderdtl);
            db.SaveChanges();

            return Ok(tsaleorderdtl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tsaleorderdtlExists(string id)
        {
            return db.tsaleorderdtl.Count(e => e.SaleOrderNO == id) > 0;
        }
    }
}