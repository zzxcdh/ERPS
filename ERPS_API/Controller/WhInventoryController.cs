using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERPS_API.App_Start;
using BLL;
using DAL_MySQL;
using Model;
using System.Web.Security;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using ERPS_API.Utils;

namespace ERPS_API.Controller
{
    public class WhInventoryController : ApiController
    {
        private erpsEntities db = new erpsEntities();
        private WhInventory whInv = new WhInventory();

        //[Authorize]
        [HttpGet]
        [Route("api/GetWhInventoryQuery")]
        public IHttpActionResult GetWhInventoryQuery()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象
            string pagesize = request.Params["pagesize"];
            string currentPage = request.Params["currentPage"];
            string pdtID = request.Params["pdtID"];
            int pageSize = int.Parse(pagesize);
            int CurrentPage = int.Parse(currentPage);

            var list = db.v_whinventoryquery.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(pdtID)) {
                list = list.Where(w => w.PdtID == pdtID);
            }
            var result = (from w in list
                          group w by new { w.PdtID,w.PdtName,w.PdtType,w.PdtTypeName,w.Spec,w.Unit } into grp
                          select new
                          {
                              //PdtID = grp.PdtID,
                              //PdtName = grp.PdtName,
                              //PdtType = grp.PdtType,
                              //PdtTypeName = grp.PdtTypeName,
                              //Spec = grp.Spec,
                              Pdt = grp.Key,
                              WhList = (from n in grp
                                        select new
                                        {
                                            n.WHID,
                                            n.WHName,
                                            n.InvNum
                                        }
                                        ).ToList()
                          }
                          );
            result = result.OrderBy(o => o.Pdt);
            var oData = new { total = result.Count(), rows = result.Skip(pageSize * (CurrentPage - 1)).Take(pageSize).ToList() };
            return Ok(oData);

            //var result = new BaseDataPackage<v_whinventoryquery>();
            //try
            //{
            //    HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            //    HttpRequestBase request = context.Request;//定义传统request对象
            //    string pagesize = request.Params["pagesize"];
            //    string currentPage = request.Params["currentPage"];
            //    string pdtID = request.Params["pdtID"];

            //    List<v_whinventoryquery> list;
            //    int pageSize = int.Parse(pagesize);
            //    int CurrentPage = int.Parse(currentPage);
            //    Dictionary<string, string> ob = new Dictionary<string, string>();
            //    list = whInv.GetWhInventoryQuery(pageSize, CurrentPage, pdtID, out int total);
            //    if (list.Count > 0)
            //    {
            //        ob.Add("total", total.ToString());
            //        result.DataList = list;
            //        result.Keys = ob;
            //        result.Status = ApiStatusCode.OK;
            //        result.Message = "查询成功";
            //        return result;
            //    }
            //    else
            //    {
            //        result.DataList = null;
            //        result.Status = ApiStatusCode.FAIL;
            //        result.Message = "暂无数据";
            //        return result;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result.Status = ApiStatusCode.EXCEPTION;
            //    result.Message = "发生异常=>" + ex.Message;
            //    return result;
            //}
        }        

        //[Authorize]
        [HttpGet]
        [Route("api/GetWhInventoryWarning")]
        public BaseDataPackage<vwhinventorywarning> GetWhInventoryWarning()
        {
            var result = new BaseDataPackage<vwhinventorywarning>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string pdtID = request.Params["pdtID"];
                string orderby = request.Params["orderby"];
                string order = request.Params["order"];

                List<vwhinventorywarning> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = whInv.GetWhInventoryWarning(pageSize, CurrentPage, pdtID,orderby,order, out int total);
                if (list.Count > 0)
                {
                    ob.Add("total", total.ToString());
                    result.DataList = list;
                    result.Keys = ob;
                    result.Status = ApiStatusCode.OK;
                    result.Message = "查询成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "暂无数据";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Status = ApiStatusCode.EXCEPTION;
                result.Message = "发生异常=>" + ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 获取产品指定月份的计算明细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWhInventoryWarningDetail")]
        public IHttpActionResult GetWhInventoryWarningDetail()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象            
            string month = request.Params["month"];
            string pdtID = request.Params["pdtID"];            
            int mon = int.Parse(month);
            DateTime dtNow = DateTime.Now;
            DateTime dtFsDay = new DateTime(dtNow.Year, dtNow.Month, 1);

            DateTime dtPur = dtFsDay.AddDays(20); // 当月第20号，采购计算用
            DateTime dtSale = dtFsDay.AddMonths(1); //下个月第一天，销售及预测计算用
            if(mon>=0 && mon <= 3)
            {
                dtPur = dtPur.AddMonths(mon);
                dtSale = dtSale.AddMonths(mon);
            }
            else
            {
                return new PageResult("参数不合法", Request);
            }

            var list1 = db.v_purorderdtl.Where(v => v.PdtID == pdtID && v.State != "F" && v.DelDate < dtPur);
            var list2 = db.v_saleorderdtl.Where(v => v.PdtID == pdtID && v.State != "F" && v.DelDate < dtSale);
            var list3 = db.v_saleforcastdtl.AsEnumerable().Where(v => v.PdtID == pdtID && v.State != "F" && DateTime.Parse(v.FocaMonth) <dtSale);
            //var list3 = db.tsaleforcastdtl.Where(v => v.PdtID == pdtID && v.State != "F");
            list1 = list1.OrderBy(v => v.UpdateDate);
            list2 = list2.OrderBy(v => v.DtlUpdateDate);
            list3 = list3.OrderBy(v => v.DtlUpdateDate);
            var oData = new { data1= list1.ToList(), data2= list2.ToList(), data3= list3.ToList() };
            //oData = new { total = list1.Count(), rows = list1, data2 = list2, data3 = list3 };
            return Ok(oData);            
        }

        /// <summary>
        /// 获取产品指定月份的计算明细        
        /// 包含初始库存，出入库历史记录及预警计算明细信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWhInventoryDetail2")]
        public IHttpActionResult GetWhInventoryWarningDetail2()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象            
            string month = request.Params["month"];
            string pdtID = request.Params["pdtID"];
            string mode = "0"; // 获取类型: "0"，只获取历史记录，"1",获取所有记录
            int mon = int.Parse(month);
            DateTime dtNow = DateTime.Now;
            DateTime dtFsDay = new DateTime(dtNow.Year, dtNow.Month, 1);
            DateTime dtHis = new DateTime(2020, 11, 1);

            DateTime dtPur = dtFsDay.AddDays(20); // 当月第20号，采购计算用
            DateTime dtSale = dtFsDay.AddMonths(1); //下个月第一天，销售及预测计算用
            if (mon >= 0 && mon <= 3)
            {
                mode = "1";            
                dtPur = dtPur.AddMonths(mon);
                dtSale = dtSale.AddMonths(mon);
            }
            else if (mon == -1)
            {
                mode = "0";
            }
            else
            {
                return new PageResult("参数不合法", Request);
            }

            // var lstWhHis = db.twhinventory_his.Where(v => v.PdtID == pdtID && v.DATE < dtHis); //最初导入的库存记录，2020-10-1
            var lstWhHis = (from h in db.twhinventory_his
                            where h.PdtID == pdtID && h.DATE < dtHis
                            select new
                            {
                                mkDate = h.DATE,
                                cmpName = "初始库存",
                                NO = "",
                                inNum = h.InvNum,
                                outNum = 0,
                                opeType = ""
                            }
                            ); //最初导入的库存记录，2020-10-1
            // db.twhstockrecords.Where(v => v.PdtID == pdtID && v.State != "C");
            //var lstStockRcds = (from o in db.twhstockrecords
            //                    join p in db.v_purorderdtl on new { a = o.PurOrderNO, b = o.RefSeqNo.Value } equals new { a = p.PurOrderNO, b = p.SeqNo } into tmp1
            //                    from t1 in tmp1.DefaultIfEmpty()
            //                    join s in db.v_saleorderdtl on new { a = o.SaleOrderNO, b = o.RefSeqNo.Value } equals new { a = s.SaleOrderNO, b = s.SeqNo } into tmp2                                
            //                    from r in tmp2.DefaultIfEmpty()
            //                    where r.PdtID == pdtID && r.CreateDate > dtHis
            //                    // orderby r.CreateDate 
            //                    select new
            //                    {
            //                        mkDate = o.PurOrderNO != ""?  t1.DelDate :(o.SaleOrderNO!=""? r.DelDate : o.CreateDate),
            //                        cmpName = o.PurOrderNO != "" ? t1.SupName : (o.SaleOrderNO != "" ? r.CusName : ""),
            //                        NO = o.PurOrderNO != "" ? t1.PurOrderNO : (o.SaleOrderNO != "" ? r.SaleOrderNO : ""),
            //                        IN_NUM = o.PurOrderNO != "" ? o.Num : 0,
            //                        OUT_NUM = o.SaleOrderNO != "" ? o.Num : 0,
            //                        o.OpeType
            //                    }
            //                    );
            string[] lstOpeType = new string[] { "I", "O", "Y", "K", "TI", "TO" }; //所含的出入库作业，20201230

            var lstStockRcds = (from o in db.twhstockrecords
                                 from p in db.v_purorderdtl.Where(c=>c.PurOrderNO == o.PurOrderNO && c.SeqNo == o.RefSeqNo.Value).DefaultIfEmpty()
                                 from s in db.v_saleorderdtl.Where(c => c.SaleOrderNO == o.SaleOrderNO && c.SeqNo == o.RefSeqNo.Value).DefaultIfEmpty()                                 
                                 where o.PdtID == pdtID && o.CreateDate > dtHis && o.State != "C" && lstOpeType.Contains(o.OpeType)
                                // orderby r.CreateDate 
                                select new
                                {
                                    mkDate = Nullable.Equals(o.PurOrderNO,null) ? (Nullable.Equals(o.SaleOrderNO, null) ? o.CreateDate : (Nullable.Equals(s.DelDate, null) ? o.CreateDate : s.DelDate)) : (Nullable.Equals(p.DelDate,null)? o.CreateDate : p.DelDate),
                                    cmpName = Nullable.Equals(o.PurOrderNO, null) ? (Nullable.Equals(o.SaleOrderNO, null) ? "" : s.CusName) : p.SupName,                                     
                                    NO = Nullable.Equals(o.PurOrderNO, null) ? (Nullable.Equals(o.SaleOrderNO, null) ? "" : s.SaleOrderNO) : p.PurOrderNO,
                                    inNum = Nullable.Equals(o.PurOrderNO, null) ? 0 : o.Num,
                                    outNum = Nullable.Equals(o.SaleOrderNO, null) ? 0 : o.Num,
                                    opeType = o.OpeType
                                }
                                );
     
            if (mode == "1")
            {
                // var list1 = db.v_purorderdtl.Where(v => v.PdtID == pdtID && v.State != "F" && v.DelDate < dtPur);
                var lstPurOrderDtl = (
                        from p in db.v_purorderdtl
                        where p.PdtID == pdtID && p.State != "F" && p.DelDate < dtPur
                        orderby p.UpdateDate
                        select new
                        {
                            mkDate = p.DelDate,
                            cmpName = p.SupName,
                            NO = p.PurOrderNO,
                            inNum = p.LftNum,
                            outNum = 0,
                            opeType = "P"
                        }
                    );
                // var list2 = db.v_saleorderdtl.Where(v => v.PdtID == pdtID && v.State != "F" && v.DelDate < dtSale);
                var lstSaleOrderDtl = (
                        from p in db.v_saleorderdtl
                        where p.PdtID == pdtID && p.State != "F" && p.DelDate < dtSale
                        orderby p.DtlUpdateDate
                        select new
                        {
                            mkDate = p.DelDate,
                            cmpName = p.CusName,
                            NO = p.SaleOrderNO,
                            inNum = p.LftNum,
                            outNum = 0,
                            opeType = "S"
                        }
                    );

                //var list3 = db.v_saleforcastdtl.AsEnumerable().Where(v => v.PdtID == pdtID && v.State != "F" && DateTime.Parse(v.FocaMonth) < dtSale);
                var lstSaleForcastDtl = (
                        from p in db.v_saleorderdtl
                        where p.PdtID == pdtID && p.State != "F" && p.DelDate < dtSale
                        orderby p.DtlUpdateDate
                        select new
                        {
                            mkDate = p.DelDate,
                            cmpName = p.CusName,
                            NO = p.SaleFocaNO,
                            inNum = p.LftNum,
                            outNum = 0,
                            opeType = "F"
                        }
                    );

                //var list3 = db.tsaleforcastdtl.Where(v => v.PdtID == pdtID && v.State != "F");
                //list1 = list1.OrderBy(v => v.UpdateDate);
                //list2 = list2.OrderBy(v => v.DtlUpdateDate);
                //list3 = list3.OrderBy(v => v.DtlUpdateDate);
                var oData = new { data1 = lstWhHis.ToList(), data2 = lstStockRcds.ToList(), data3 = lstPurOrderDtl.ToList(), data4 = lstSaleOrderDtl.ToList(), data5 = lstSaleForcastDtl.ToList() };
                //oData = new { total = list1.Count(), rows = list1, data2 = list2, data3 = list3 };
                return Ok(oData);
            }
            else
            {
                var oData = new { data1 = lstWhHis.ToList(), data2 = lstStockRcds.ToList()};
                //oData = new { total = list1.Count(), rows = list1, data2 = list2, data3 = list3 };
                return Ok(oData);
            }
            
        }

        //[Authorize]
        /// <summary>
        /// 查询指定仓库的产品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetWhProduct")]
        public IHttpActionResult GetWhProduct()
        {
            var result = new BaseDataPackage<mproduct>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentpage = request.Params["currentPage"];
                string whID = request.Params["whID"];
                string pdtID = request.Params["pdtID"];
                string pdtName = request.Params["pdtName"];
                string spec = request.Params["spec"];
                string unit = request.Params["unit"];
                string pdtType = request.Params["pdtType"];
                string startDate = request.Params["startDate"];
                string endDate = request.Params["endDate"];

                int pageSize = int.Parse(pagesize);
                int currentPage = int.Parse(currentpage);

                List<mproduct> list = new List<mproduct>();
                var tem = from p in db.mproduct
                          join t in db.mproducttype on p.PdtType equals t.PdtTypeID
                          join w in db.twhinventory on p.PdtID equals w.PdtID
                          where w.WHID.Equals(whID) && w.InvNum > 0
                          select new
                          {
                              p.CreateDate,
                              p.CreateID,
                              p.MakeIn,
                              p.MgrInfo,
                              p.PdtID,
                              p.PdtName,
                              p.PdtType,
                              p.PurPrice,
                              p.Remark,
                              p.SalPrice,
                              p.Spec,
                              p.Unit,
                              p.UpdateDate,
                              p.UpdateID,
                              w.WHID,
                              w.InvNum
                          };
                if (!string.IsNullOrEmpty(pdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(pdtID));
                }

                if (!string.IsNullOrEmpty(pdtName))
                {
                    tem = tem.Where(p => p.PdtName.Contains(pdtName));
                }
                if (!string.IsNullOrEmpty(spec))
                {
                    tem = tem.Where(p => p.Spec.Contains(spec));
                }
                if (!string.IsNullOrEmpty(unit))
                {
                    tem = tem.Where(p => p.Unit.Contains(unit));
                }
                if (!string.IsNullOrEmpty(pdtType))
                {
                    tem = tem.Where(p => p.PdtType == pdtType);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    DateTime date = Convert.ToDateTime(startDate);
                    tem = tem.Where(p => p.CreateDate >= date);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    DateTime date = Convert.ToDateTime(endDate);
                    date = date.AddDays(1);
                    tem = tem.Where(p => p.CreateDate < date);
                }

                if (tem == null)
                {
                    return NotFound();
                }

                tem = tem.OrderBy(p => p.PdtID);

                var oData = new { total = tem.Count(), rows = tem.Skip(pageSize * (currentPage - 1)).Take(pageSize).ToList() };
                return Ok(oData);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
