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
using DAL_MySQL;
using ERPS_API.Utils;
using Z.EntityFramework.Plus;

namespace ERPS_API.Controller
{
    //盘点控制器
    public class twhcheckrecordsController : ApiController
    {
        private erpsEntities db = new erpsEntities();



        [HttpGet]
        [Route("api/GettwhcheckrecordsAll")]
        public IHttpActionResult GettwhcheckrecordsAll(int pagesize, int currentPage)
        {
            var listCus = from c in db.twhcheckrecords
                          join w in db.mwarehouse
                          on c.WHID equals w.WHID
                          let p1 = new
                          {
                              chkNO = c.ChkNO,
                              updateID = c.UpdateID,
                              chkStatus = c.ChkStatus,
                              whid = c.WHID,
                              whName = w.WHName,
                              remark = c.Remark,
                              chkDate = c.ChkDate,
                              updateDate = c.UpdateDate
                          }
                          select p1;
            listCus = listCus.OrderByDescending(u => u.updateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件查询盘点信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GettwhcheckrecordsByCondition")]
        public IHttpActionResult GettwhcheckrecordsByCondition(string warId, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listCus = from c in db.twhcheckrecords
                          join w in db.mwarehouse
                          on c.WHID equals w.WHID
                          let p1 = new
                          {
                              chkNO = c.ChkNO,
                              updateID = c.UpdateID,
                              chkStatus = c.ChkStatus,
                              whid = c.WHID,
                              whName = w.WHName,
                              remark = c.Remark,
                              chkDate = c.ChkDate,
                              updateDate = c.UpdateDate
                          }
                          select p1;
            if (!string.IsNullOrEmpty(warId))
            {
                listCus = listCus.Where(w => w.whid == warId);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listCus = listCus.Where(w => w.updateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listCus = listCus.Where(w => w.updateDate < date);
            }
            listCus = listCus.OrderByDescending(u => u.updateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 查询所有盘点所需的产品信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GettwhcheckrecordsDtl")]
        public IHttpActionResult GettwhcheckrecordsDtl()
        {
            var listCus = from p in db.mproduct
                          let p1 = new
                          {
                              pdtID = p.PdtID,
                              pdtName = p.PdtName,
                              spec = p.Spec,
                              unit = p.Unit,
                              invNum = 0,
                              realNum = 0,
                              difNum = 0,
                              updateDate = p.UpdateDate
                          }
                          select p1;
            listCus = listCus.OrderByDescending(c => c.updateDate);
            return Ok(listCus);
        }

        /// <summary>
        /// 根据仓库ID查询盘点详细信息
        /// </summary>
        /// <param name="chkNO"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GettwhcheckrecordsDtl")]
        public IHttpActionResult GettwhcheckrecordsDtl(string warId)
        {
            var listCus = from w in db.mwarehouse
                          join i in db.twhinventory
                          on w.WHID equals i.WHID
                          join p in db.mproduct
                          on i.PdtID equals p.PdtID
                          let p1 = new
                          {
                              pdtID = i.PdtID,
                              pdtName = p.PdtName,
                              spec = p.Spec,
                              unit = p.Unit,
                              invNum = i.InvNum,
                              realNum = i.InvNum,
                              difNum = 0,
                              updateDate = w.UpdateDate
                          }
                          where w.WHID == warId
                          select p1;
            listCus = listCus.OrderByDescending(c => c.updateDate);
            return Ok(listCus);
        }

        [HttpGet]
        [Route("api/GettwhcheckrecorddtlByChkNO")]
        public IHttpActionResult GettwhcheckrecorddtlByChkNO(string chkNO)
        {
            var listCus = from w in db.twhcheckrecorddtl.AsNoFilter()
                          join i in db.mproduct
                          on w.PdtID equals i.PdtID
                          let p1 = new
                          {
                              pdtID = i.PdtID,
                              pdtName = i.PdtName,
                              spec = i.Spec,
                              unit = i.Unit,
                              invNum = w.InvNum,
                              realNum = w.RealNum,
                              difNum = w.DifNum,
                              updateDate = w.UpdateDate
                          }
                          where w.ChkNO == chkNO
                          select p1;
            return Ok(listCus);
        }

        /// <summary>
        /// 查询当天最新的盘点号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetChkNO")]
        public IHttpActionResult GetChkNO()
        {
            try
            {
                int seq = 0;
                twhcheckrecords t = new twhcheckrecords();
                DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")); //今天
                DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")); //明天
                var listCus = from c in db.twhcheckrecords
                              where c.ChkDate >= dtToday && c.ChkDate < dtNexDay
                              orderby c.ChkDate descending
                              select c;

                if (listCus.ToList<twhcheckrecords>().Count > 0)
                {
                    t = listCus.ToList<twhcheckrecords>()[0];
                    seq = int.Parse(t.ChkNO.Substring(t.ChkNO.Length - 3, 3));
                }
                return Ok(seq);
            }
            catch (Exception ex)
            {
                return new PageResult("error", Request);
            }
        }

        /// <summary>
        /// 库存调整
        /// </summary>
        /// <param name="twhcheckrecorddtl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Posttwhcheckrecords")]
        public IHttpActionResult Posttwhcheckrecords()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();

            string chkNO = request.Params["chkNO"];
            string warId = request.Params["warId"];
            string chkDate = request.Params["chkDate"];
            string remark = request.Params["remark"];
            string updateId = request.Params["updateId"];
            List<twhcheckrecorddtl> listTwhDtl = js.Deserialize<List<twhcheckrecorddtl>>(request.Params["listTwhDtl"]);
            List<twhstockrecords> listStock = new List<twhstockrecords>();
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    if (twhcheckrecordsExists(chkNO))
                    {
                        db.twhcheckrecords.Where(c => c.ChkNO == chkNO)
                            .Update(c => new twhcheckrecords
                            {
                                ChkStatus = "F",
                                UpdateID = updateId,
                                Remark = remark,
                                UpdateDate = DateTime.Now
                            });
                    }
                    else
                    {
                        twhcheckrecords t = new twhcheckrecords();
                        t.ChkNO = chkNO;
                        t.WHID = warId;
                        t.ChkDate = DateTime.Parse(chkDate);
                        t.Remark = remark;
                        t.ChkStatus = "F";
                        t.AreaID = "1";
                        t.PosiID = "1";
                        t.UpdateID = updateId;
                        t.UpdateDate = DateTime.Parse(chkDate);
                        db.twhcheckrecords.Add(t);
                    }

                    for (int i = 0; i < listTwhDtl.Count; i++)
                    {
                        string ChkNO = listTwhDtl[i].ChkNO = chkNO;
                        string PdtID = listTwhDtl[i].PdtID;
                        float InvNum = listTwhDtl[i].InvNum;
                        float RealNum = listTwhDtl[i].RealNum;
                        float DifNum = listTwhDtl[i].DifNum;

                        twhstockrecords stockrecords = new twhstockrecords();
                        stockrecords.WHID = warId;
                        stockrecords.AreaID = "1";
                        stockrecords.PosiID = "1";
                        stockrecords.PdtID = listTwhDtl[i].PdtID;
                        stockrecords.RefWHID = warId;
                        stockrecords.RefAreaID = "1";
                        stockrecords.RefPosiID = "1";
                        stockrecords.UpdateID = updateId;
                        var date = DateTime.Now;
                        stockrecords.UpdateDate = date;
                        stockrecords.CreateDate = date;

                        //调整库存前先锁住行
                        //string sqlQuery = "select * from twhinventory where whid ='" + warId + "' and pdtid='" + PdtID + "' for update";                        
                        //var inventory = db.twhinventory.SqlQuery(sqlQuery).FirstOrDefault();
                        var inventory = db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == PdtID).FirstOrDefault();
                        float invNum = (float)inventory.InvNum;

                        if (listTwhDtl[i].DifNum == 0) break;
                        if (listTwhDtl[i].DifNum > 0)
                        {
                            stockrecords.Num = listTwhDtl[i].DifNum;
                            stockrecords.OpeType = "Y";
                            db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == PdtID)
                                .Update(inv => new twhinventory { InvNum = RealNum });
                        }
                        else
                        {
                            stockrecords.Num = Math.Abs(listTwhDtl[i].DifNum);
                            stockrecords.OpeType = "K";
                            db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == PdtID)
                                .Update(inv => new twhinventory { InvNum = RealNum });
                        }

                        if (twhcheckrecorddtlExists(ChkNO, PdtID))
                        {
                            db.twhcheckrecorddtl.Where(c => c.ChkNO == ChkNO && c.PdtID == PdtID)
                                .Update(c => new twhcheckrecorddtl
                                {
                                    InvNum = InvNum,
                                    RealNum = RealNum,
                                    DifNum = DifNum,
                                    UpdateID = updateId,
                                    UpdateDate = DateTime.Now,
                                    AdjStatus = "F"
                                });
                        }
                        else
                        {
                            twhcheckrecorddtl tw = new twhcheckrecorddtl();
                            listTwhDtl[i].ChkNO = chkNO;
                            listTwhDtl[i].AdjStatus = "F";
                            listTwhDtl[i].UpdateID = updateId;
                            tw = listTwhDtl[i];
                            db.twhcheckrecorddtl.Add(tw);
                        }
                        listStock.Add(stockrecords);
                    }
                    db.twhstockrecords.AddRange(listStock);

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult("error", Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 新增盘点
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PosttwhcheckrecordsDtl")]
        public IHttpActionResult PosttwhcheckrecordsDtl()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();

            string chkNO = request.Params["chkNO"];
            string warId = request.Params["warId"];
            string chkDate = request.Params["chkDate"];
            string remark = request.Params["remark"];
            string updateId = request.Params["updateId"];
            List<twhcheckrecorddtl> listTwhDtl = js.Deserialize<List<twhcheckrecorddtl>>(request.Params["listTwhDtl"]);
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    twhcheckrecords t = new twhcheckrecords();
                    t.ChkNO = chkNO;
                    t.WHID = warId;
                    t.ChkDate = DateTime.Parse(chkDate);
                    t.Remark = remark;
                    t.ChkStatus = "N";
                    t.AreaID = "1";
                    t.PosiID = "1";
                    t.UpdateID = updateId;
                    t.UpdateDate = DateTime.Parse(chkDate);

                    for (int i = 0; i < listTwhDtl.Count; i++)
                    {
                        string ChkNO = listTwhDtl[i].ChkNO;
                        string PdtID = listTwhDtl[i].PdtID;
                        float InvNum = listTwhDtl[i].InvNum;
                        float RealNum = listTwhDtl[i].RealNum;
                        float DifNum = listTwhDtl[i].DifNum;

                        twhcheckrecorddtl tw = new twhcheckrecorddtl();
                        listTwhDtl[i].ChkNO = chkNO;
                        listTwhDtl[i].AdjStatus = "N";
                        listTwhDtl[i].UpdateID = updateId;
                        listTwhDtl[i].UpdateDate = DateTime.Parse(chkDate);
                        tw = listTwhDtl[i];
                        db.twhcheckrecorddtl.Add(tw);
                    }
                    db.twhcheckrecords.Add(t);

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult("error", Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 修改盘点
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PosttwhcheckrecordsDtlEdit")]
        public IHttpActionResult PosttwhcheckrecordsDtlEdit()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();

            string chkNO = request.Params["chkNO"];
            string warId = request.Params["warId"];
            string chkDate = request.Params["chkDate"];
            string remark = request.Params["remark"];
            string updateId = request.Params["updateId"];
            List<twhcheckrecorddtl> listTwhDtl = js.Deserialize<List<twhcheckrecorddtl>>(request.Params["listTwhDtl"]);
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var twhcheckrecords = db.twhcheckrecords.AsNoFilter().Where(w => w.ChkNO == chkNO).FirstOrDefault();
                    twhcheckrecords.UpdateID = updateId;
                    twhcheckrecords.Remark = remark;
                    twhcheckrecords.UpdateDate = DateTime.Now;
                    db.Entry(twhcheckrecords).State = EntityState.Unchanged;
                    db.Entry(twhcheckrecords).Property(p => p.UpdateID).IsModified = true;
                    db.Entry(twhcheckrecords).Property(p => p.Remark).IsModified = true;
                    db.Entry(twhcheckrecords).Property(p => p.UpdateDate).IsModified = true;

                    db.twhcheckrecorddtl.Where(w => w.ChkNO == chkNO).Delete();
                    db.SaveChanges();
                    for (int i = 0; i < listTwhDtl.Count; i++)
                    {
                        string ChkNO = listTwhDtl[i].ChkNO;
                        string PdtID = listTwhDtl[i].PdtID;
                        float InvNum = listTwhDtl[i].InvNum;
                        float RealNum = listTwhDtl[i].RealNum;
                        float DifNum = listTwhDtl[i].DifNum;

                        twhcheckrecorddtl tw = new twhcheckrecorddtl();
                        listTwhDtl[i].ChkNO = chkNO;
                        listTwhDtl[i].AdjStatus = "N";
                        listTwhDtl[i].UpdateID = updateId;
                        listTwhDtl[i].UpdateDate = DateTime.Now;
                        tw = listTwhDtl[i];
                        db.twhcheckrecorddtl.Add(tw);
                    }


                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult("error", Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 删除盘点
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletetwhcheckrecord")]
        public IHttpActionResult Deletetwhcheckrecord()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象

            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> delLists = js.Deserialize<List<string>>(request.Params["delUid"]);

            int ret = 0;
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i];

                        var vm = db.twhcheckrecorddtl.Where(m => m.ChkNO == id);
                        vm.ToList().ForEach(t => db.Entry(t).State = EntityState.Deleted);
                        var vm2 = db.twhcheckrecords.Where(m => m.ChkNO == id).FirstOrDefault();
                        db.twhcheckrecords.Remove(vm2);
                        db.twhcheckrecorddtl.RemoveRange(vm);
                    }

                    ret = db.SaveChanges();
                    tran.Commit();
                    if (ret > 0)
                    {
                        return Content<string>(HttpStatusCode.OK, "OK");
                    }
                    else {
                        return Content<string>(HttpStatusCode.OK, "NG");
                    }                    
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult("error: " + ex.Message, Request);
                }
            }

            
        }

        /// <summary>
        /// 库存变更锁定测试
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [Route("api/testWhIn")]
        public IHttpActionResult testWhIn()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();

            string whID = request.Params["whID"];
            string pdtID = request.Params["pdtID"];
            string strInvNum = request.Params["invNum"];
            float invNum = float.Parse(strInvNum);

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    //调整库存前先锁住行
                    string sqlQuery = "select * from twhinventory where whid ='" + whID + "' and pdtid='" + pdtID + "' for update";
                    //string sqlQuery = "select * from twhinventory where whid ='" + whID + "' and pdtid='" + pdtID + "'";
                    var inventory = db.twhinventory.SqlQuery(sqlQuery).FirstOrDefault();
                    inventory.InvNum = inventory.InvNum + invNum;

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult("error", Request);
                }
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

        private bool twhcheckrecordsExists(string chkNO)
        {
            return db.twhcheckrecords.Count(e => e.ChkNO == chkNO) > 0;
        }

        private bool twhcheckrecorddtlExists(string chkNO, string pdtID)
        {
            return db.twhcheckrecorddtl.Count(e => e.ChkNO == chkNO && e.PdtID == pdtID) > 0;
        }
    }
}