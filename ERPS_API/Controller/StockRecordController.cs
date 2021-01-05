using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using DAL_MySQL;
using ERPS_API.Utils;
using Model;
using Z.EntityFramework.Plus;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 出入库记录控制器
    /// </summary>
    public class StockRecordController : ApiController
    {
        private erpsEntities db = new erpsEntities();
        ExcelHelper excelHelper = new ExcelHelper();
        /// <summary>
        /// 入库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PostInWar")]
        public IHttpActionResult PostInWar()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string purOrderNO = request.Params["purOrderNO"];
            string warId = request.Params["warId"];
            string purchaseDate = request.Params["purchaseDate"];
            List<Stockrecords> listStoc = js.Deserialize<List<Stockrecords>>(request.Params["purorderdtl"]);
            List<WarehouseReceipt> listWr = js.Deserialize<List<WarehouseReceipt>>(request.Params["purorderdtl"]);
            List<twhstockrecords> listTwh = new List<twhstockrecords>();
            string no = "";
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    int fNum = 0;
                    no = purOrderNO + DateTime.Now.ToString("HHmmss");
                    for (int i = 0; i < listStoc.Count; i++)
                    {
                        int seqNo = listStoc[i].seqNo;
                        string pdtId = listStoc[i].pdtID;
                        float lftNum = listStoc[i].lftNum;
                        string remark = listStoc[i].remark;

                        twhstockrecords t = new twhstockrecords();
                        t.WHID = warId;
                        t.AreaID = i.ToString();
                        t.PosiID = i.ToString();
                        t.PdtID = pdtId;
                        t.Num = lftNum;
                        t.RefWHID = i.ToString();
                        t.RefAreaID = i.ToString();
                        t.RefPosiID = i.ToString();
                        t.OpeType = "I";
                        t.PurOrderNO = purOrderNO;
                        t.RefSeqNo = seqNo;
                        t.No = no;                        
                        var dateNow = DateTime.Now;
                        t.CreateDate = dateNow;
                        t.UpdateDate = dateNow;
                        if (!string.IsNullOrEmpty(purchaseDate))
                        {
                            DateTime date = Convert.ToDateTime(purchaseDate);
                            var tmp0 = date.ToString("yyyy-MM-dd");
                            var tmp1 = dateNow.ToLongTimeString().ToString();
                            purchaseDate = tmp0 + " " + tmp1;
                            date = Convert.ToDateTime(purchaseDate);
                            t.UpdateDate = date;
                        }
                        t.Remark = remark;
                        listTwh.Add(t);

                        var purorderdtl = db.tpurorderdtl
                            .Where(p => p.PurOrderNO == purOrderNO && p.PdtID == pdtId && p.SeqNo == seqNo).FirstOrDefault();

                        if (purorderdtl.LftNum > 0)
                        {
                            float num = (float)purorderdtl.LftNum - lftNum;
                            float delNum = (float)purorderdtl.OrderNum - num;
                            db.tpurorderdtl.Where(p => p.PurOrderNO == purOrderNO && p.PdtID == pdtId && p.SeqNo == seqNo)
                                .Update(p => new tpurorderdtl { LftNum = num, DelNum = delNum, UpdateDate = DateTime.Now });

                            if (num == 0)
                            {
                                purorderdtl.State = "F";

                            }
                            else
                            {
                                fNum++;
                            }
                        }
                    }

                    var repeat = listStoc.GroupBy(s => s.pdtID).Select(s => new
                    {
                        pdtID = s.Key,
                        lftNum = s.Sum(a => a.lftNum)
                    });

                    foreach (var r in repeat)
                    {
                        if (twhinventoryExists(warId, r.pdtID))
                        {
                            //var repeatUpdate = listStoc.GroupBy(s => s.pdtID).Select(s => new
                            //{
                            //    pdtID = s.Key,
                            //    lftNum = s.Sum(a => a.lftNum)
                            //}).Where(s => s.pdtID == r.pdtID).FirstOrDefault();

                            //float lftNum = repeatUpdate.lftNum;

                            var inventory = db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == r.pdtID).FirstOrDefault();
                            float total = (float)inventory.InvNum + r.lftNum;
                            db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == r.pdtID)
                                .Update(inv => new twhinventory() { InvNum = total, UpdateDate = DateTime.Now });
                        }
                        else
                        {
                            //var query = listStoc.GroupBy(x => x.pdtID).Where(g => g.Count() > 1 && g.Key == pdtId).Select(y => y.Key).ToList();

                            //if (query.Count > 0) lftNum = 0;

                            //var repeatAdd = listStoc.GroupBy(s => s.pdtID).Select(s => new
                            //{
                            //    pdtID = s.Key,
                            //    lftNum = s.Sum(a => a.lftNum)
                            //}).Where(s => s.pdtID == r.pdtID).FirstOrDefault();

                            //float lftNum = repeatAdd.lftNum;

                            twhinventory inv = new twhinventory();
                            inv.WHID = warId;
                            inv.AreaID = "0";
                            inv.PosiID = "0";
                            inv.PdtID = r.pdtID;
                            inv.InvNum = r.lftNum;
                            inv.UpdateDate = DateTime.Now;
                            db.twhinventory.Add(inv);
                        }
                    }

                    //部分入库时fNum也会是0，这种情况下用fNum判断会有问题
                    //if (fNum == 0)
                    //{
                    //    db.tpurorder.Where(p => p.PurOrderNO == purOrderNO)
                    //        .Update(p => new tpurorder() { State = "F", UpdateDate = DateTime.Now });
                    //}

                    //全部入库完成后才修改单头状态为F
                    db.SaveChanges();
                    var isNotFinish = db.tpurorderdtl.AsNoTracking().Any(w => w.PurOrderNO == purOrderNO && (w.State == "N" || w.State == null));
                    if (!isNotFinish)
                    {
                        db.tpurorder.Where(s => s.PurOrderNO == purOrderNO)
                            .Update(p => new tpurorder() { State = "F", UpdateDate = DateTime.Now });
                    }

                    db.twhstockrecords.AddRange(listTwh);


                    db.SaveChanges();
                    tran.Commit();

                    excelHelper.SaveInWarInfo(purOrderNO, warId, purchaseDate, listWr, no);

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult(ex.ToString(), Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PostOutWar")]
        public IHttpActionResult PostOutWar()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string saleOrderNo = request.Params["saleOrderNo"];
            string warId = request.Params["warId"];
            string deliveryDate = request.Params["deliveryDate"];
            List<Stockrecords> listStoc = js.Deserialize<List<Stockrecords>>(request.Params["salorderdtl"]);
            List<OutboundOrder> listOdo = js.Deserialize<List<OutboundOrder>>(request.Params["salorderdtl"]);
            List<twhstockrecords> listTwh = new List<twhstockrecords>();
            string no = "";
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var repeat = listStoc.GroupBy(s => s.pdtID).Select(s => new {
                        pdtID = s.Key,
                        lftNum = s.Sum(a => a.lftNum),
                        invNum = s.Where(a => a.pdtID == s.Key).Select(a => a.invNum).FirstOrDefault()
                    });


                    foreach (var r in repeat)
                    {
                        if (r.lftNum > r.invNum)
                        {
                            return new PageResult("库存不足", Request);
                        }
                    }

                    no = saleOrderNo + DateTime.Now.ToString("HHmmss");
                    for (int i = 0; i < listStoc.Count; i++)
                    {
                        int seqNo = listStoc[i].seqNo;
                        string pdtId = listStoc[i].pdtID;
                        float lftNum = listStoc[i].lftNum;
                        string remark = listStoc[i].remark;

                        twhstockrecords t = new twhstockrecords();
                        t.WHID = warId;
                        t.AreaID = i.ToString();
                        t.PosiID = i.ToString();
                        t.PdtID = pdtId;
                        t.Num = lftNum;
                        t.RefWHID = i.ToString();
                        t.RefAreaID = i.ToString();
                        t.RefPosiID = i.ToString();
                        t.OpeType = "O";
                        t.SaleOrderNO = saleOrderNo;
                        t.RefSeqNo = seqNo;
                        t.No = no;
                        var dateNow = DateTime.Now;
                        t.CreateDate = dateNow;
                        t.UpdateDate = dateNow;
                        if (!string.IsNullOrEmpty(deliveryDate))
                        {
                            DateTime date = Convert.ToDateTime(deliveryDate);
                            var tmp0 = date.ToString("yyyy-MM-dd");
                            var tmp1 = dateNow.ToLongTimeString().ToString();
                            deliveryDate = tmp0 + " " + tmp1;
                            date = Convert.ToDateTime(deliveryDate);
                            t.UpdateDate = date;
                        }
                        t.Remark = remark;
                        listTwh.Add(t);

                        var tsaleorderdtl = db.tsaleorderdtl
                            .Where(s => s.SaleOrderNO == saleOrderNo && s.PdtID == pdtId && s.SeqNo == seqNo).FirstOrDefault();

                        if (tsaleorderdtl.LftNum > 0)
                        {
                            float num = (float)tsaleorderdtl.LftNum - lftNum;
                            float delNum = (float)tsaleorderdtl.OrderNum - num;
                            db.tsaleorderdtl.Where(s => s.SaleOrderNO == saleOrderNo && s.PdtID == pdtId && s.SeqNo == seqNo)
                                .Update(p => new tsaleorderdtl { LftNum = num, DelNum = delNum, UpdateDate = DateTime.Now });

                            if (num <= 0)
                            {
                                tsaleorderdtl.State = "F";
                            }
                        }

                        if (twhinventoryExists(warId, pdtId))
                        {
                            var r = listStoc.GroupBy(s => s.pdtID).Select(s => new
                            {
                                pdtID = s.Key,
                                lftNum = s.Sum(a => a.lftNum)
                            }).Where(s => s.pdtID == pdtId).FirstOrDefault();

                            lftNum = r.lftNum;

                            var inventory = db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == pdtId).FirstOrDefault();
                            float total = (float)inventory.InvNum - lftNum;
                            db.twhinventory.Where(inv => inv.WHID == warId && inv.PdtID == pdtId)
                                .Update(inv => new twhinventory() { InvNum = total, UpdateDate = DateTime.Now });
                        }
                        //else
                        //{
                        //    twhinventory inv = new twhinventory();
                        //    inv.WHID = warId;
                        //    inv.AreaID = i.ToString();
                        //    inv.PosiID = i.ToString();
                        //    inv.PdtID = pdtId;
                        //    inv.InvNum = lftNum;
                        //    inv.UpdateDate = DateTime.Now;
                        //    db.twhinventory.Add(inv);
                        //}

                    }

                    db.SaveChanges();
                    var isNotFinish = db.tsaleorderdtl.AsNoTracking().Any(w => w.SaleOrderNO == saleOrderNo && (w.State == "N" || w.State == null));
                    if (!isNotFinish)
                    {
                        db.tsaleorder.Where(s => s.SaleOrderNO == saleOrderNo)
                            .Update(p => new tsaleorder() { State = "F", UpdateDate = DateTime.Now });
                    }

                    db.twhstockrecords.AddRange(listTwh);

                    db.SaveChanges();
                    tran.Commit();

                    excelHelper.SaveOutWarInfo(saleOrderNo, warId, deliveryDate, listOdo, no);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult(ex.ToString(), Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 移库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/StockTransfer")]
        public IHttpActionResult StockTransfer()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string frmWarId = request.Params["frmWarId"];
            string toWarId = request.Params["toWarId"];
            string purchaseDate = request.Params["purchaseDate"];
            List<Stockrecords> listStoc = js.Deserialize<List<Stockrecords>>(request.Params["tsfDtl"]);            
            List<twhstockrecords> listTwh = new List<twhstockrecords>();
            
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {                    
                    //调整库存前先锁住两个库的对应所有产品的行，如果产品在目标库不存在，先新建记录再锁住
                    string sqlQuery = "select * from twhinventory where whid ='{0}' and areaid ='0' and posiid='0' and pdtid='{1}' for update";
                    for(int i=0;i<listStoc.Count;i++)
                    {
                        string sql1 = string.Format(sqlQuery, frmWarId, listStoc[i].pdtID);
                        string sql2 = string.Format(sqlQuery, toWarId, listStoc[i].pdtID);

                        var inv1 = db.twhinventory.SqlQuery(sql1).FirstOrDefault();
                        if(inv1==null || inv1.PdtID==null || inv1.PdtID=="")
                        {
                            twhinventory newInv1 = new twhinventory();
                            newInv1.WHID = frmWarId;
                            newInv1.PdtID = listStoc[i].pdtID;
                            newInv1.AreaID = "0";
                            newInv1.PosiID = "0";
                            newInv1.InvNum = 0;
                            db.twhinventory.Add(newInv1);
                            db.SaveChanges();
                            inv1 = db.twhinventory.SqlQuery(sql1).FirstOrDefault();
                        }
                        //实时库存不够，回滚，返回
                        if(inv1.InvNum < listStoc[i].lftNum)
                        {
                            tran.Rollback(); 
                            return new PageResult("库存不足", Request);
                        }

                        //来源库减库存
                        inv1.InvNum = inv1.InvNum - listStoc[i].lftNum;

                        var inv2 = db.twhinventory.SqlQuery(sql2).FirstOrDefault();
                        if (inv2 == null || inv2.PdtID == null || inv2.PdtID == "")
                        {
                            twhinventory newInv2 = new twhinventory();
                            newInv2.WHID = toWarId;
                            newInv2.PdtID = listStoc[i].pdtID;
                            newInv2.AreaID = "0";
                            newInv2.PosiID = "0";
                            newInv2.InvNum = 0;
                            db.twhinventory.Add(newInv2);
                            db.SaveChanges();

                            inv2 = db.twhinventory.SqlQuery(sql2).FirstOrDefault();
                        }

                        //目标库加库存
                        inv2.InvNum = inv2.InvNum + listStoc[i].lftNum;

                        //新增出入库记录，移库新增两条           
                        //新增出库记录
                        float lftNum = listStoc[i].lftNum;
                        twhstockrecords t = new twhstockrecords();
                        t.WHID = frmWarId;
                        t.AreaID = "0";
                        t.PosiID = "0";
                        t.PdtID = listStoc[i].pdtID;
                        t.Num = lftNum;
                        t.RefWHID = toWarId;
                        t.RefAreaID = "0";
                        t.RefPosiID = "0";
                        t.OpeType = "TO";
                        t.SaleOrderNO = "";
                        t.No = "";
                        var dateNow = DateTime.Now;
                        t.CreateDate = dateNow;
                        t.UpdateDate = dateNow;
                        if (!string.IsNullOrEmpty(purchaseDate))
                        {
                            DateTime date = Convert.ToDateTime(purchaseDate);
                            var tmp0 = date.ToString("yyyy-MM-dd");
                            var tmp1 = dateNow.ToLongTimeString().ToString();
                            purchaseDate = tmp0 + " " + tmp1;
                            date = Convert.ToDateTime(purchaseDate);
                            t.UpdateDate = date;
                        }                                              
                        listTwh.Add(t);

                        //新增入库记录                                                                    
                        twhstockrecords t2 = new twhstockrecords();
                        t2.WHID = toWarId;
                        t2.AreaID = "0";
                        t2.PosiID = "0";
                        t2.PdtID = listStoc[i].pdtID;
                        t2.Num = lftNum;
                        t2.RefWHID = frmWarId;
                        t2.RefAreaID = "0";
                        t2.RefPosiID = "0";
                        t2.OpeType = "TI";
                        t2.SaleOrderNO = "";
                        t2.No = "";
                        t2.CreateDate = dateNow;
                        t2.UpdateDate = dateNow;
                        if (!string.IsNullOrEmpty(purchaseDate))
                        {
                            DateTime date = Convert.ToDateTime(purchaseDate);
                            var tmp0 = date.ToString("yyyy-MM-dd");
                            var tmp1 = dateNow.ToLongTimeString().ToString();
                            purchaseDate = tmp0 + " " + tmp1;
                            date = Convert.ToDateTime(purchaseDate);
                            t2.UpdateDate = date;
                        }
                        listTwh.Add(t2);
                    }                    

                    db.twhstockrecords.AddRange(listTwh);

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return new PageResult(ex.ToString(), Request);
                }
            }
            return Content<string>(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// 根据条件获取出入库记录
        /// </summary>
        /// <param name="pdtID"></param>
        /// <param name="whID"></param>
        /// <param name="updUser"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="opeType"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<twhstockrecords>))]
        [HttpGet]
        [Route("api/InOutRecordQuery")]
        public IHttpActionResult InOutRecordQuery(string pdtID, string whID, string updUser, string startDate, string endDate, string opeType, int pagesize, int currentPage)
        {
            var listStk = from c in db.twhstockrecords
                          where c.State != "C"
                          select c;
            if (!string.IsNullOrEmpty(pdtID))
            {
                listStk = listStk.Where(c => c.PdtID.Contains(pdtID));
            }
            if (!string.IsNullOrEmpty(whID))
            {
                listStk = listStk.Where(c => c.WHID.Contains(whID));
            }
            if (!string.IsNullOrEmpty(updUser))
            {
                listStk = listStk.Where(c => c.UpdateID.Contains(updUser));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listStk = listStk.Where(c => c.UpdateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listStk = listStk.Where(c => c.UpdateDate < date);
            }
            if (!string.IsNullOrEmpty(opeType))
            {
                listStk = listStk.Where(c => c.OpeType.Contains(opeType));
            }
            if (listStk == null)
            {
                return NotFound();
            }

            listStk = listStk.OrderByDescending(c => c.UpdateDate);
            var oData = new { total = listStk.Count(), rows = listStk.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 获取操作类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetopeType")]
        public IHttpActionResult GetopeType()
        {
            var listOrder = (from o in db.twhstockrecords
                             select o.OpeType).Distinct();
            return Ok(listOrder);
        }


        /// <summary>
        /// 获取所有获取出入库记录
        /// </summary>
        [HttpGet]
        [Route("api/InOutRecordQuery2")]
        public IHttpActionResult InOutRecordQuery2(int pagesize, int currentPage)
        {
            var listCus = from c in db.twhstockrecords
                          select c;
            listCus = listCus.OrderByDescending(u => u.UpdateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 查询入库详细信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InRecordQuery")]
        public IHttpActionResult InRecordQuery(int pagesize, int currentPage)
        {
            var listCus = from c in db.twhstockrecords
                          join p in db.mproduct on c.PdtID equals p.PdtID
                          join o in db.tpurorderdtl on new { a = c.PurOrderNO, b = c.PdtID, c = c.RefSeqNo.Value } 
                                                equals new { a = o.PurOrderNO, b = o.PdtID, c = o.SeqNo }
                          join s in db.msupplier on o.SupID equals s.SupID
                          join w in db.mwarehouse on c.WHID equals w.WHID
                          let p1 = new
                          {
                              opeType = c.OpeType,
                              updateDate = c.UpdateDate,
                              purOrderNO = o.PurOrderNO,
                              supID = s.SupID,
                              supName = s.SupName,
                              address = s.Address,
                              phone = s.Phone,
                              contact = s.Contact,
                              whid = c.WHID,
                              whName = w.WHName,
                              pdtID = c.PdtID,
                              pdtName = p.PdtName,
                              delDate = o.DelDate,
                              spec = p.Spec,
                              unit = p.Unit,
                              num = c.Num,
                              purPrice = o.UnitPrice,
                              total = c.Num * o.UnitPrice,
                              remark = c.Remark
                          }
                          where c.OpeType == "I" && c.State != "C"
                          select p1;
            string str = listCus.ToString();
            listCus = listCus.OrderByDescending(u => u.updateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件查询入库详细信息
        /// </summary>
        /// <param name="purOrderNO"></param>
        /// <param name="whName"></param>
        /// <param name="supID"></param>
        /// <param name="pdtID"></param>
        /// <param name="pdtName"></param>
        /// <param name="spec"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/InRecordQueryByCondition")]
        public IHttpActionResult InRecordQueryByCondition(string purOrderNO, string whName, string supID, string pdtID,
           string pdtName, string spec, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listCus = from s in db.twhstockrecords
                          join p in db.mproduct on s.PdtID equals p.PdtID
                          join o in db.tpurorderdtl on new { a = s.PurOrderNO, b = s.PdtID, c = s.RefSeqNo.Value }
                                                equals new { a = o.PurOrderNO, b = o.PdtID, c = o.SeqNo }
                          join sup in db.msupplier on o.SupID equals sup.SupID
                          join w in db.mwarehouse on s.WHID equals w.WHID
                          let p1 = new
                          {
                              opeType = s.OpeType,
                              updateDate = s.UpdateDate,
                              purOrderNO = o.PurOrderNO,
                              supID = sup.SupID,
                              supName = sup.SupName,
                              address = sup.Address,
                              phone = sup.Phone,
                              contact = sup.Contact,
                              whid = s.WHID,
                              whName = w.WHName,
                              pdtID = s.PdtID,
                              pdtName = p.PdtName,
                              delDate = o.DelDate,
                              spec = p.Spec,
                              unit = p.Unit,
                              num = s.Num,
                              purPrice = o.UnitPrice,
                              total = s.Num * o.UnitPrice,
                              remark = s.Remark
                          }
                          where s.OpeType == "I" && s.State != "C"
                          select p1;

            if (!string.IsNullOrEmpty(purOrderNO))
            {
                listCus = listCus.Where(s => s.purOrderNO.Contains(purOrderNO));
            }
            if (!string.IsNullOrEmpty(whName))
            {
                listCus = listCus.Where(s => s.whName.Contains(whName));
            }
            if (!string.IsNullOrEmpty(supID))
            {
                listCus = listCus.Where(s => s.supID.Contains(supID));
            }
            if (!string.IsNullOrEmpty(pdtID))
            {
                listCus = listCus.Where(s => s.pdtID.Contains(pdtID));
            }
            if (!string.IsNullOrEmpty(pdtName))
            {
                listCus = listCus.Where(s => s.pdtName.Contains(pdtName));
            }
            if (!string.IsNullOrEmpty(spec))
            {
                listCus = listCus.Where(s => s.spec.Contains(spec));
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
        /// 查询出库详细信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OutRecordQuery")]
        public IHttpActionResult OutRecordQuery(int pagesize, int currentPage)
        {


            var listCus = from c in db.twhstockrecords
                          join p in db.mproduct on c.PdtID equals p.PdtID
                          join o in db.tsaleorderdtl on new { a = c.SaleOrderNO, b = c.PdtID, c = c.RefSeqNo.Value }
                                                 equals new { a = o.SaleOrderNO, b = o.PdtID, c = o.SeqNo }
                          join s in db.mcustomer on o.CusID equals s.CusID
                          join w in db.mwarehouse on c.WHID equals w.WHID
                          join mcp in db.mcustomerprodprice on new { c.PdtID, o.CusID, o.Model } equals new { mcp.PdtID, mcp.CusID, mcp.Model }
                          let p1 = new
                          {
                              opeType = c.OpeType,
                              updateDate = c.UpdateDate,
                              saleOrderNO = o.SaleOrderNO,
                              cusID = s.CusID,
                              cusName = s.CusName,
                              address = s.Address,
                              phone = s.Phone,
                              contact = s.Contact,
                              whid = c.WHID,
                              whName = w.WHName,
                              pdtID = c.PdtID,
                              pdtName = p.PdtName,
                              model = o.Model,
                              delDate = o.DelDate,
                              spec = p.Spec,
                              unit = p.Unit,
                              num = c.Num,
                              salPrice = mcp.SalePrice,
                              total = c.Num * mcp.SalePrice,
                              remark = c.Remark,
                              remark1 = o.Remark1,
                              remark2 = o.Remark2
                          }
                          where c.OpeType == "O" && c.State != "C"
                          select p1;
            listCus = listCus.OrderByDescending(u => u.updateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件查询出库详细信息
        /// </summary>
        /// <param name="saleOrderNO"></param>
        /// <param name="whName"></param>
        /// <param name="cusID"></param>
        /// <param name="pdtID"></param>
        /// <param name="pdtName"></param>
        /// <param name="spec"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/OutRecordQueryByCondition")]
        public IHttpActionResult OutRecordQueryByCondition(string saleOrderNO, string whName, string cusID, string pdtID, string pdtName, string model, string spec,
                                                           string startDate, string endDate, int pagesize, int currentPage)
        {
            var listCus = from c in db.twhstockrecords
                          join p in db.mproduct on c.PdtID equals p.PdtID
                          join o in db.tsaleorderdtl on new { a = c.SaleOrderNO, b = c.PdtID, c = c.RefSeqNo.Value }
                                                 equals new { a = o.SaleOrderNO, b = o.PdtID, c = o.SeqNo }
                          join s in db.mcustomer on o.CusID equals s.CusID
                          join w in db.mwarehouse on c.WHID equals w.WHID
                          join mcp in db.mcustomerprodprice on new { c.PdtID, o.CusID, o.Model } equals new { mcp.PdtID, mcp.CusID, mcp.Model }
                          let p1 = new
                          {
                              opeType = c.OpeType,
                              updateDate = c.UpdateDate,
                              saleOrderNO = o.SaleOrderNO,
                              cusID = s.CusID,
                              cusName = s.CusName,
                              address = s.Address,
                              phone = s.Phone,
                              contact = s.Contact,
                              whid = c.WHID,
                              whName = w.WHName,
                              pdtID = c.PdtID,
                              pdtName = p.PdtName,
                              model = o.Model,
                              delDate = o.DelDate,
                              spec = p.Spec,
                              unit = p.Unit,
                              num = c.Num,
                              salPrice = mcp.SalePrice,
                              total = c.Num * mcp.SalePrice,
                              remark = c.Remark,
                              remark1 = o.Remark1,
                              remark2 = o.Remark2
                          }
                          where c.OpeType == "O" && c.State != "C"
                          select p1;

            if (!string.IsNullOrEmpty(saleOrderNO))
            {
                listCus = listCus.Where(s => s.saleOrderNO.Contains(saleOrderNO));
            }
            if (!string.IsNullOrEmpty(whName))
            {
                listCus = listCus.Where(s => s.whName.Contains(whName));
            }
            if (!string.IsNullOrEmpty(cusID))
            {
                listCus = listCus.Where(s => s.cusID.Contains(cusID));
            }
            if (!string.IsNullOrEmpty(pdtID))
            {
                listCus = listCus.Where(s => s.pdtID.Contains(pdtID));
            }
            if (!string.IsNullOrEmpty(pdtName))
            {
                listCus = listCus.Where(s => s.pdtName.Contains(pdtName));
            }
            if (!string.IsNullOrEmpty(model))
            {
                listCus = listCus.Where(s => s.model.Contains(model));
            }
            if (!string.IsNullOrEmpty(spec))
            {
                listCus = listCus.Where(s => s.spec.Contains(spec));
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
        /// 查询库存调拨详细信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TransferRecordQuery")]
        public IHttpActionResult TransferRecordQuery(string frmWhID, string toWhID, string pdtID,
           string pdtName, string spec, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listCus = from s in db.twhstockrecords
                          join p in db.mproduct on s.PdtID equals p.PdtID
                          join w in db.mwarehouse on s.WHID equals w.WHID
                          join w2 in db.mwarehouse on s.RefWHID equals w2.WHID
                          let p1 = new
                          {
                              opeType = s.OpeType,
                              updateDate = s.UpdateDate,
                              frmWhID = s.WHID,
                              frmWhName = w.WHName,
                              toWhID = s.RefWHID,
                              toWhName = w2.WHName,
                              pdtID = s.PdtID,
                              pdtName = p.PdtName,
                              spec = p.Spec,
                              unit = p.Unit,
                              num = s.Num,
                              remark = s.Remark
                          }
                          where (s.OpeType == "TI" || s.OpeType == "TO") && s.State != "C"
                          select p1;

            if (!string.IsNullOrEmpty(frmWhID))
            {
                listCus = listCus.Where(s => s.frmWhID.Contains(frmWhID));
            }
            if (!string.IsNullOrEmpty(toWhID))
            {
                listCus = listCus.Where(s => s.toWhID.Contains(toWhID));
            }
            if (!string.IsNullOrEmpty(pdtID))
            {
                listCus = listCus.Where(s => s.pdtID.Contains(pdtID));
            }
            if (!string.IsNullOrEmpty(pdtName))
            {
                listCus = listCus.Where(s => s.pdtName.Contains(pdtName));
            }
            if (!string.IsNullOrEmpty(spec))
            {
                listCus = listCus.Where(s => s.spec.Contains(spec));
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
        /// 修改库存记录表备注
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ModifyRemark")]
        public IHttpActionResult Putsuser(twhstockrecords t)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (t.OpeType == "I")
                {
                    db.twhstockrecords.Where(s => s.WHID == t.WHID && s.PdtID == t.PdtID && s.UpdateDate == t.UpdateDate && s.PurOrderNO == t.PurOrderNO)
                        .Update(p => new twhstockrecords { Remark = t.Remark });
                }
                else
                {
                    db.twhstockrecords.Where(s => s.WHID == t.WHID && s.PdtID == t.PdtID && s.UpdateDate == t.UpdateDate && s.SaleOrderNO == t.SaleOrderNO)
                        .Update(p => new twhstockrecords { Remark = t.Remark });
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return new PageResult(ex.ToString(), Request);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool twhstockrecordsExists(string id)
        {
            return db.twhstockrecords.Count(e => e.WHID == id) > 0;
        }

        private bool twhinventoryExists(string warId, string pdtId)
        {
            return db.twhinventory.Count(i => i.WHID == warId && i.PdtID == pdtId) > 0;
        }
    }
}