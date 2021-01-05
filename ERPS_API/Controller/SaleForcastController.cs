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
using System.Web.Http.Description;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using OfficeOpenXml;
using ERPS_API.Utils;
using Newtonsoft.Json.Linq;

namespace ERPS_API.Controller
{
    public class SaleForcastController : ApiController
    {
        private SaleForcast sale = new SaleForcast();

        //[Authorize]

        [HttpGet]
        [Route("api/GetSaleForcast")]
        public BaseDataPackage<v_saleforcast> GetSaleForcast()
        {
            var result = new BaseDataPackage<v_saleforcast>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string orderby = request.Params["orderby"];
                string order = request.Params["order"];
                string SaleFocaNO = request.Params["SaleFocaNO"];
                string CusID = request.Params["CusID"];
                string CusName = request.Params["CusName"];
                string State = request.Params["State"];
                string SDate = request.Params["SDate"];
                string EDate = request.Params["EDate"];

                //SaleForcast sale = new SaleForcast();
                List<v_saleforcast> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = sale.GetSaleForcast(CusID, CusName, SaleFocaNO, State, SDate, EDate, pageSize, CurrentPage, orderby, order, out int total);
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

        [HttpGet]
        [Route("api/GetSaleForcastDtl")]
        public BaseDataPackage<v_saleforcastdtl> GetSaleForcastDtl()
        {
            var result = new BaseDataPackage<v_saleforcastdtl>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string SaleFocaNO = request.Params["SaleFocaNO"];
                string CusID = request.Params["CusID"];
                string CusName = request.Params["CusName"];
                string PdtID = request.Params["PdtID"];
                string PdtName = request.Params["PdtName"];
                string SDate = request.Params["SDate"];
                string EDate = request.Params["EDate"];

                //SaleForcast sale = new SaleForcast();
                List<v_saleforcastdtl> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = sale.GetSaleForcastDtl(CusID, CusName, PdtID, PdtName, SaleFocaNO, SDate, EDate, pageSize, CurrentPage, out int total);
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

        [HttpGet]
        [Route("api/GetSaleOrder")]
        public BaseDataPackage<v_saleorder> GetSaleOrder()
        {
            var result = new BaseDataPackage<v_saleorder>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string orderby = request.Params["orderby"];
                string order = request.Params["order"];
                string SaleOrderNO = request.Params["SaleOrderNO"];
                string CusID = request.Params["CusID"];
                string CusName = request.Params["CusName"];
                string State = request.Params["State"];
                string SDate = request.Params["SDate"];
                string EDate = request.Params["EDate"];
                string CreateID = request.Params["CreateID"];

                //SaleForcast sale = new SaleForcast();
                List<v_saleorder> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = sale.GetSaleOrder(CusID, CusName, SaleOrderNO, State, SDate, EDate, CreateID, pageSize, CurrentPage, orderby, order, out int total);
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

        [HttpGet]
        [Route("api/GetSaleOrderDtl")]
        public BaseDataPackage<v_saleorderdtl> GetSaleOrderDtl()
        {
            var result = new BaseDataPackage<v_saleorderdtl>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string SaleOrderNO = request.Params["SaleOrderNO"];
                string CusID = request.Params["CusID"];
                string CusName = request.Params["CusName"];
                string PdtID = request.Params["PdtID"];
                string PdtName = request.Params["PdtName"];
                string SDate = request.Params["SDate"];
                string EDate = request.Params["EDate"];
                string CreateID = request.Params["CreateID"];

                //SaleForcast sale = new SaleForcast();
                List<v_saleorderdtl> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = sale.GetSaleOrderDtl(CusID, CusName, PdtID, PdtName, SaleOrderNO, SDate, EDate, CreateID, pageSize, CurrentPage, out int total);
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

        [HttpGet]
        [Route("api/GetSaleForcastDtlList_SaleFocaNO")]
        public BaseDataPackage<v_saleforcastdtl> GetSaleForcastDtlList_SaleFocaNO(string SaleFocaNO)
        {
            var result = new BaseDataPackage<v_saleforcastdtl>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.v_saleforcastdtl.Where(w => w.SaleFocaNO == SaleFocaNO).ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
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
        /// 导出销售预测详细信息所调用的接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetSaleForcastDtlList_SaleFocaNOList")]
        public IHttpActionResult GetSaleForcastDtlList_SaleFocaNOList()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string saleFoca = request.Params["SaleFocaNOList"];

            String[] saleFocaArray = saleFoca.Split(',');

            List<Object> saleFocadtls = new List<object>();
            try
            {
                erpsEntities db = new erpsEntities();
                foreach (string id in saleFocaArray)
                {
                    var list = (from dtl in db.tsaleforcastdtl
                                join c in db.mcustomer on dtl.CusID equals c.CusID into cc
                                from c in cc.DefaultIfEmpty()
                                join p in db.mproduct on dtl.PdtID equals p.PdtID into pp
                                from p in pp.DefaultIfEmpty()
                                join ctp in db.mcustomerprodprice on new { a = dtl.PdtID, b = dtl.CusID, c = dtl.Model } equals
                                                                     new { a = ctp.PdtID, b = ctp.CusID, c = ctp.Model } into ctpp
                                from ctp in ctpp.DefaultIfEmpty()
                                join s in db.tsaleforcast on dtl.SaleFocaNO equals s.SaleFocaNO into ss
                                from s in ss.DefaultIfEmpty()
                                where dtl.SaleFocaNO == id
                                let p1 = new
                                {
                                    dtl.SaleFocaNO,     //预测号
                                    dtl.CusID,          //客户编号
                                    c.CusName,          //客户名称
                                    CreDate = s.CreDate.ToString(),          //预测日期
                                    dtl.PdtID,          //产品编号
                                    p.PdtName,          //产品名称
                                    p.Spec,             //产品规格
                                    dtl.Model,          //车型
                                    p.Unit,             //单位
                                    p.MakeIn,           //产地
                                    ctp.CusItmName,     //客户产品名称
                                    ctp.SalePrice,      //价格
                                    dtl.FocaMonth,      //内示月份
                                    dtl.FocaNum,        //内示数量
                                    dtl.LftNum,         //剩余数量
                                    dtl.Remark1,        //备注1
                                    dtl.Remark2         //备注2
                                }
                                select p1);

                    saleFocadtls.AddRange(list);
                }
            }
            catch (Exception ex)
            {
                return new PageResult(ex.ToString(), Request);
            }

            return new PageResult(saleFocadtls, Request);
        }

        [HttpGet]
        [Route("api/GetSaleForcastDtl_SaleFocaNO")]
        public BaseDataPackage<v_saleforcastdtl> GetSaleForcastDtl_SaleFocaNO(string SaleFocaNO)
        {
            var result = new BaseDataPackage<v_saleforcastdtl>();
            try
            {
                List<v_saleforcastdtl> list;

                list = sale.GetSaleForcastDtlAndAtt(SaleFocaNO, out List<tsaleforcastatt> tsaleforcastatts);
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.ObjList = tsaleforcastatts.Cast<object>().ToList();
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

        [HttpGet]
        [Route("api/GetSaleOrderDtlList_SaleOrderNO")]
        public BaseDataPackage<v_saleorderdtl> GetSaleOrderDtlList_SaleOrderNO(string SaleOrderNO)
        {
            var result = new BaseDataPackage<v_saleorderdtl>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.v_saleorderdtl.Where(w => w.SaleOrderNO == SaleOrderNO).ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
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
        /// 导出销售订单详细信息所调用的接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetSaleOrderDtlList_SaleOrderNOList")]
        public IHttpActionResult GetSaleOrderDtlList_SaleOrderNOList()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string saleOrder = request.Params["saleOrderList"];

            String[] saleOrderArray = saleOrder.Split(',');

            List<Object> saleorderdtls = new List<Object>();
            try
            {
                erpsEntities db = new erpsEntities();
                foreach (string id in saleOrderArray)
                {
                    var list = (from dtl in db.tsaleorderdtl
                                join c in db.mcustomer on dtl.CusID equals c.CusID into cc
                                from c in cc.DefaultIfEmpty()
                                join p in db.mproduct on dtl.PdtID equals p.PdtID into pp
                                from p in pp.DefaultIfEmpty()
                                join ctp in db.mcustomerprodprice on new { a = dtl.PdtID, b = dtl.CusID, c = dtl.Model } equals
                                                                     new { a = ctp.PdtID, b = ctp.CusID, c = ctp.Model } into ctpp
                                from ctp in ctpp.DefaultIfEmpty()
                                join s in db.tsaleorder on dtl.SaleOrderNO equals s.SaleOrderNO into ss
                                from s in ss.DefaultIfEmpty()
                                join sf in db.tsaleforcastdtl on new { a = dtl.SaleFocaNO, b = dtl.SeqNo } equals
                                                                 new { a = sf.SaleFocaNO, b = sf.SeqNo } into sff
                                from cf in sff.DefaultIfEmpty()
                                where dtl.SaleOrderNO == id
                                let p1 = new
                                {
                                    dtl.SaleOrderNO,                            //销售单号
                                    dtl.CusID,                                  //客户编号
                                    c.CusName,                                  //客户名称
                                    AppointDate = s.AppointDate.ToString(),     //交货日期
                                    dtl.PdtID,                                  //产品编码
                                    p.PdtName,                                  //产品名称
                                    p.Spec,                                     //产品规格
                                    p.Unit,                                     //单位
                                    p.MakeIn,                                   //产地
                                    ctp.CusItmName,                             //客户产品名称
                                    ctp.SalePrice,                              //价格
                                    dtl.FocaMonth,                              //内示月份
                                    dtl.OrderNum,                               //订单数量
                                    dtl.LftNum,                                 //剩余数量
                                    DelDate = dtl.DelDate.ToString(),           //纳期
                                    dtl.Remark1,                                //备注1
                                    dtl.Remark2                                 //备注2
                                }
                                select p1);


                    saleorderdtls.AddRange(list);
                }
            }
            catch (Exception ex)
            {
                return new PageResult(ex.ToString(), Request);
            }

            return new PageResult(saleorderdtls, Request);
        }



        [HttpGet]
        [Route("api/GetSaleOrderDtl_SaleOrderNO")]
        public BaseDataPackage<v_saleorderdtl> GetSaleOrderDtl_SaleOrderNO(string SaleOrderNO)
        {
            var result = new BaseDataPackage<v_saleorderdtl>();
            try
            {
                List<v_saleorderdtl> list;

                list = sale.GetSaleOrderDtlAndAtt(SaleOrderNO, out List<tsaleorderatt> tsaleorderatts);
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.ObjList = tsaleorderatts.Cast<object>().ToList();
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

        [HttpGet]
        [Route("api/GetSaleForcastDtl_SaleFocaNO_LftNum")]
        public BaseDataPackage<v_saleforcastdtl> GetSaleForcastDtl_SaleFocaNO_LftNum(string SaleFocaNO)
        {
            var result = new BaseDataPackage<v_saleforcastdtl>();
            erpsEntities db = new erpsEntities();
            try
            {
                List<v_saleforcastdtl> list;

                list = db.v_saleforcastdtl.Where(w => w.SaleFocaNO == SaleFocaNO && w.State != "F" && w.LftNum.Value > 0).ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
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

        [HttpPost]
        [Route("api/AddSaleForcastDtl")]
        public BaseDataPackage<string> AddSaleForcastDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string CreDate = request.Form["CreDate"];
                //SaleForcast sale = new SaleForcast();
                string SaleForcastNo = sale.SetSaleForcastNo(CreDate);

                tsaleforcast tsaleforcast = new tsaleforcast();
                tsaleforcast.SaleFocaNO = SaleForcastNo;
                tsaleforcast.CusID = request.Form["CusID"];
                tsaleforcast.State = "N";
                tsaleforcast.CreDate = Convert.ToDateTime(CreDate);
                tsaleforcast.CreateID = request.Form["UserID"];
                tsaleforcast.CreateDate = DateTime.Now;

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<SaleForacstDtlSimple> list = js.Deserialize<List<SaleForacstDtlSimple>>(request.Params["list"]);

                List<tsaleforcastdtl> tsaleforcastdtls = new List<tsaleforcastdtl>();
                for (int i = 0; i < list.Count; i++)
                {
                    tsaleforcastdtl tsaleforcastdtl = new tsaleforcastdtl();
                    tsaleforcastdtl.SaleFocaNO = SaleForcastNo;
                    tsaleforcastdtl.SeqNo = i + 1;
                    tsaleforcastdtl.CusID = tsaleforcast.CusID;
                    tsaleforcastdtl.PdtID = list[i].PdtID;
                    tsaleforcastdtl.Model = list[i].Model;
                    tsaleforcastdtl.FocaMonth = list[i].FocaMonth;
                    tsaleforcastdtl.FocaNum = Convert.ToSingle(list[i].FocaNum);
                    tsaleforcastdtl.LftNum = tsaleforcastdtl.FocaNum;
                    tsaleforcastdtl.State = "N";
                    tsaleforcastdtl.Remark1 = list[i].Remark1;
                    tsaleforcastdtl.Remark2 = list[i].Remark2;
                    tsaleforcastdtl.CreateID = tsaleforcast.CreateID;
                    tsaleforcastdtl.CreateDate = DateTime.Now;
                    tsaleforcastdtls.Add(tsaleforcastdtl);
                }

                List<tsaleforcastatt> tsaleforcastatts = new List<tsaleforcastatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + SaleForcastNo + "/";
                string basePath = HttpContext.Current.Server.MapPath(url);

                List<string> tmpPath = new List<string>();
                if (files.Count > 0)
                {
                    //如果目录不存在，则创建目录
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        tsaleforcastatt tsaleforcastatt = new tsaleforcastatt();
                        string path = basePath + files[i].FileName;

                        tsaleforcastatt.SaleFocaNO = SaleForcastNo;
                        tsaleforcastatt.AttFileName = url + files[i].FileName;
                        tsaleforcastatt.CreateID = tsaleforcast.CreateID;
                        tsaleforcastatt.CreateDate = DateTime.Now;

                        tsaleforcastatts.Add(tsaleforcastatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = sale.AddSaleForcastDtl(tsaleforcast, tsaleforcastdtls, tsaleforcastatts);
                if (ret == 1)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].SaveAs(tmpPath[i]);
                    }
                    result.Status = ApiStatusCode.OK;
                    result.Message = "提交成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "提交失败";
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

        [HttpPost]
        [Route("api/AddSaleOrderDtl")]
        public BaseDataPackage<string> AddSaleOrderDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string SaleFocaNO = request.Form["SaleFocaNO"];
                string CreDate = request.Form["CreDate"];
                string SaleOrderNO = sale.SetSaleOrderNo(CreDate);

                tsaleorder tsaleorder = new tsaleorder();
                tsaleorder.SaleOrderNO = SaleOrderNO;
                tsaleorder.CusID = request.Form["CusID"];
                tsaleorder.CreDate = Convert.ToDateTime(CreDate);
                tsaleorder.TotalAmount = Convert.ToSingle(request.Form["TotalAmount"]);
                tsaleorder.TotalNumber = Convert.ToSingle(request.Form["TotalNumber"]);
                tsaleorder.AppointDate = Convert.ToDateTime(request.Form["AppointDate"]);
                tsaleorder.State = "N";
                tsaleorder.Remark = request.Form["Remark"];
                tsaleorder.CreateID = request.Form["UserID"];
                tsaleorder.CreateDate = DateTime.Now;

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<SaleOrderDtlSimple> list = js.Deserialize<List<SaleOrderDtlSimple>>(request.Params["list"]);

                List<tsaleorderdtl> tsaleorderdtls = new List<tsaleorderdtl>();
                for (int i = 0; i < list.Count; i++)
                {
                    tsaleorderdtl tsaleorderdtl = new tsaleorderdtl();
                    tsaleorderdtl.SaleOrderNO = SaleOrderNO;
                    if (!string.IsNullOrEmpty(SaleFocaNO))
                    {
                        tsaleorderdtl.SeqNo = string.IsNullOrEmpty(list[i].SeqNo) ? i + 1 : int.Parse(list[i].SeqNo);
                    }
                    else
                    {
                        tsaleorderdtl.SeqNo = i + 1;
                    }

                    tsaleorderdtl.SaleFocaNO = SaleFocaNO;
                    tsaleorderdtl.CusID = tsaleorder.CusID;
                    tsaleorderdtl.PdtID = list[i].PdtID;
                    tsaleorderdtl.Model = list[i].Model;
                    tsaleorderdtl.FocaMonth = list[i].FocaMonth;
                    tsaleorderdtl.OrderNum = Convert.ToSingle(list[i].OrderNum);
                    tsaleorderdtl.LftNum = tsaleorderdtl.OrderNum;
                    tsaleorderdtl.DelNum = Convert.ToSingle(list[i].DelNum);
                    tsaleorderdtl.DelDate = Convert.ToDateTime(list[i].DelDate);
                    tsaleorderdtl.OrderPrice = Convert.ToSingle(list[i].OrderPrice);
                    tsaleorderdtl.State = "N";
                    tsaleorderdtl.Remark1 = list[i].Remark1;
                    tsaleorderdtl.Remark2 = list[i].Remark2;
                    tsaleorderdtl.CreateID = tsaleorder.CreateID;
                    tsaleorderdtl.CreateDate = DateTime.Now;
                    tsaleorderdtls.Add(tsaleorderdtl);
                }

                List<tsaleorderatt> tsaleordertts = new List<tsaleorderatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + SaleOrderNO + "/";
                string basePath = HttpContext.Current.Server.MapPath(url);

                List<string> tmpPath = new List<string>();
                if (files.Count > 0)
                {
                    //如果目录不存在，则创建目录
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        tsaleorderatt tsaleforcastatt = new tsaleorderatt();
                        string path = basePath + files[i].FileName;

                        tsaleforcastatt.SaleOrderNO = SaleOrderNO;
                        tsaleforcastatt.AttFileName = url + files[i].FileName;
                        tsaleforcastatt.CreateID = tsaleorder.CreateID;
                        tsaleforcastatt.CreateDate = DateTime.Now;

                        tsaleordertts.Add(tsaleforcastatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = sale.AddSaleOrderDtl(tsaleorder, tsaleorderdtls, tsaleordertts);
                if (ret == 1)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].SaveAs(tmpPath[i]);
                    }
                    result.Status = ApiStatusCode.OK;
                    result.Message = "提交成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "提交失败";
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

        [HttpPost]
        [Route("api/EditSaleForcastDtl")]
        public BaseDataPackage<string> EditSaleForcastDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string SaleForcastNo = request.Form["SaleFocaNO"];
                string CusID = request.Form["CusID"];
                string UserID = request.Form["UserID"];

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<v_saleforcastdtl> list = js.Deserialize<List<v_saleforcastdtl>>(request.Params["list"]);
                list.RemoveAll(r => string.IsNullOrEmpty(r.PdtID));
                List<tsaleforcastdtl> tsaleforcastdtls = new List<tsaleforcastdtl>();

                int newSeqNo = list.Max(M => M.SeqNo) + 1;
                for (int i = 0; i < list.Count; i++)
                {
                    tsaleforcastdtl tsaleforcastdtl = new tsaleforcastdtl();
                    tsaleforcastdtl.SaleFocaNO = SaleForcastNo;
                    if (string.IsNullOrEmpty(list[i].SeqNo.ToString()) || list[i].SeqNo == 0)
                    {
                        tsaleforcastdtl.SeqNo = newSeqNo;
                        newSeqNo++;
                        tsaleforcastdtl.State = "N";
                        tsaleforcastdtl.LftNum = Convert.ToSingle(list[i].FocaNum);
                    }
                    else
                    {
                        tsaleforcastdtl.SeqNo = list[i].SeqNo;
                        tsaleforcastdtl.State = list[i].DtlState;
                        tsaleforcastdtl.LftNum = list[i].LftNum;
                    }

                    tsaleforcastdtl.CusID = CusID;
                    tsaleforcastdtl.PdtID = list[i].PdtID;
                    tsaleforcastdtl.Model = list[i].Model;
                    tsaleforcastdtl.FocaMonth = list[i].FocaMonth;
                    tsaleforcastdtl.FocaNum = Convert.ToSingle(list[i].FocaNum);
                    tsaleforcastdtl.Remark1 = list[i].Remark1;
                    tsaleforcastdtl.Remark2 = list[i].Remark2;
                    if (list[i].DtlCreateID != null)
                    {
                        tsaleforcastdtl.CreateID = list[i].DtlCreateID;
                        tsaleforcastdtl.CreateDate = list[i].DtlCreateDate;
                    }
                    else
                    {
                        tsaleforcastdtl.CreateID = UserID;
                        tsaleforcastdtl.CreateDate = DateTime.Now;
                    }
                    tsaleforcastdtl.UpdateID = UserID;
                    tsaleforcastdtl.UpdateDate = DateTime.Now;

                    tsaleforcastdtls.Add(tsaleforcastdtl);
                }

                List<tsaleforcastatt> tsaleforcastatts = new List<tsaleforcastatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + SaleForcastNo + "/";
                string basePath = HttpContext.Current.Server.MapPath(url);

                List<string> tmpPath = new List<string>();
                if (files.Count > 0)
                {
                    //如果目录不存在，则创建目录
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        tsaleforcastatt tsaleforcastatt = new tsaleforcastatt();
                        string path = basePath + files[i].FileName;

                        tsaleforcastatt.SaleFocaNO = SaleForcastNo;
                        tsaleforcastatt.AttFileName = url + files[i].FileName;
                        tsaleforcastatt.CreateID = UserID;
                        tsaleforcastatt.CreateDate = DateTime.Now;

                        tsaleforcastatts.Add(tsaleforcastatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = sale.EditSaleForcastDtl(SaleForcastNo, tsaleforcastdtls, tsaleforcastatts);
                if (ret == 1)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].SaveAs(tmpPath[i]);
                    }
                    result.Status = ApiStatusCode.OK;
                    result.Message = "修改成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "修改失败";
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

        [HttpPost]
        [Route("api/EditSaleOrderDtl")]
        public BaseDataPackage<string> EditSaleOrderDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string SaleOrderNO = request.Form["SaleOrderNO"];
                string CusID = request.Form["CusID"];
                string UserID = request.Form["UserID"];

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<v_saleorderdtl> list = js.Deserialize<List<v_saleorderdtl>>(request.Params["list"]);
                list.RemoveAll(r => string.IsNullOrEmpty(r.PdtID));
                tsaleorder tsaleorder = new tsaleorder();
                tsaleorder.SaleOrderNO = SaleOrderNO;
                tsaleorder.CusID = CusID;
                tsaleorder.CreDate = Convert.ToDateTime(list[0].CreDate);
                tsaleorder.TotalAmount = Convert.ToSingle(request.Form["TotalAmount"]);
                tsaleorder.TotalNumber = Convert.ToSingle(request.Form["TotalNumber"]);
                tsaleorder.AppointDate = Convert.ToDateTime(request.Form["AppointDate"]);
                tsaleorder.Remark = request.Form["Remark"];
                tsaleorder.State = list[0].State;
                tsaleorder.CreateID = list[0].DtlCreateID;
                tsaleorder.CreateDate = list[0].DtlCreateDate;
                tsaleorder.UpdateID = UserID;
                tsaleorder.UpdateDate = DateTime.Now;

                List<tsaleorderdtl> tsaleorderdtls = new List<tsaleorderdtl>();

                int newSeqNo = list.Max(M => M.SeqNo) + 1;
                for (int i = 0; i < list.Count; i++)
                {
                    tsaleorderdtl tsaleorderdtl = new tsaleorderdtl();
                    tsaleorderdtl.SaleOrderNO = SaleOrderNO;
                    if (string.IsNullOrEmpty(list[i].SeqNo.ToString()) || list[i].SeqNo == 0)
                    {//修改时新增的数据
                        tsaleorderdtl.SeqNo = newSeqNo;
                        newSeqNo++;
                        tsaleorderdtl.State = "N";
                        tsaleorderdtl.LftNum = Convert.ToSingle(list[i].OrderNum);
                        tsaleorderdtl.SaleFocaNO = "";
                    }
                    else
                    {
                        tsaleorderdtl.SeqNo = list[i].SeqNo;
                        tsaleorderdtl.State = list[i].DtlState;
                        tsaleorderdtl.LftNum = Convert.ToSingle(list[i].LftNum);
                        tsaleorderdtl.SaleFocaNO = list[i].SaleFocaNO;
                    }                
                    
                    tsaleorderdtl.CusID = CusID;
                    tsaleorderdtl.PdtID = list[i].PdtID;
                    tsaleorderdtl.Model = list[i].Model;
                    tsaleorderdtl.FocaMonth = list[i].FocaMonth;
                    tsaleorderdtl.DelNum = Convert.ToSingle(list[i].DelNum);
                    tsaleorderdtl.OrderNum = Convert.ToSingle(list[i].OrderNum);                    
                    tsaleorderdtl.DelDate = Convert.ToDateTime(list[i].DelDate);
                    tsaleorderdtl.OrderPrice = Convert.ToSingle(list[i].OrderPrice);
                    tsaleorderdtl.Remark1 = list[i].Remark1;
                    tsaleorderdtl.Remark2 = list[i].Remark2;
                    if (list[i].DtlCreateID != null)
                    {
                        tsaleorderdtl.CreateID = list[i].DtlCreateID;
                        tsaleorderdtl.CreateDate = list[i].DtlCreateDate;
                    }
                    else
                    {
                        tsaleorderdtl.CreateID = UserID;
                        tsaleorderdtl.CreateDate = DateTime.Now;
                    }
                    tsaleorderdtl.UpdateID = UserID;
                    tsaleorderdtl.UpdateDate = DateTime.Now;

                    tsaleorderdtls.Add(tsaleorderdtl);
                }

                List<tsaleorderatt> tsaleorderatts = new List<tsaleorderatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + SaleOrderNO + "/";
                string basePath = HttpContext.Current.Server.MapPath(url);

                List<string> tmpPath = new List<string>();
                if (files.Count > 0)
                {
                    //如果目录不存在，则创建目录
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        tsaleorderatt tsaleorderatt = new tsaleorderatt();
                        string path = basePath + files[i].FileName;

                        tsaleorderatt.SaleOrderNO = SaleOrderNO;
                        tsaleorderatt.AttFileName = url + files[i].FileName;
                        tsaleorderatt.CreateID = UserID;
                        tsaleorderatt.CreateDate = DateTime.Now;

                        tsaleorderatts.Add(tsaleorderatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = sale.EditSaleOrderDtl(SaleOrderNO, tsaleorder, tsaleorderdtls, tsaleorderatts);
                if (ret == 1)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].SaveAs(tmpPath[i]);
                    }
                    result.Status = ApiStatusCode.OK;
                    result.Message = "修改成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "修改失败";
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

        //[HttpGet]
        //[Route("api/GetCustomer")]
        //public BaseDataPackage<mcustomer> GetCustomer()
        //{
        //    var result = new BaseDataPackage<mcustomer>();
        //    try
        //    {
        //        erpsEntities db = new erpsEntities();
        //        var list = db.mcustomer.ToList();
        //        if (list.Count > 0)
        //        {
        //            result.DataList = list;
        //            result.Status = ApiStatusCode.OK;
        //            result.Message = "查询成功";
        //            return result;
        //        }
        //        else
        //        {
        //            result.DataList = null;
        //            result.Status = ApiStatusCode.FAIL;
        //            result.Message = "暂无客户数据";
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = ApiStatusCode.EXCEPTION;
        //        result.Message = "发生异常=>" + ex.Message;
        //        return result;
        //    }
        //}

        [HttpGet]
        [Route("api/GetCustomerSaleFoca")]
        public BaseDataPackage<object> GetCustomerSaleFoca(string CusID)
        {
            var result = new BaseDataPackage<object>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = (from a in db.tsaleforcast
                            join b in db.mcustomer
                            on a.CusID equals b.CusID
                            where a.CusID == CusID && a.State != "F"
                            let a1 = new
                            {
                                a.SaleFocaNO,
                                a.CusID,
                                b.CusName,
                                a.CreDate,
                                a.CreateDate
                            }
                            select a1).OrderByDescending(o => o.CreateDate).ToList();

                if (list.Count > 0)
                {
                    result.DataList = list.Cast<object>().ToList();
                    result.Status = ApiStatusCode.OK;
                    result.Message = "查询成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "暂无产品数据";
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

        [HttpGet]
        [Route("api/GetCustomerprodprice")]
        public BaseDataPackage<v_customerprodprice> GetCustomerprodprice(string CusID)
        {
            var result = new BaseDataPackage<v_customerprodprice>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.v_customerprodprice.Where(w => w.CusID == CusID).ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.Status = ApiStatusCode.OK;
                    result.Message = "查询成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "暂无产品数据";
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


        [HttpGet]
        [Route("api/GetSupProdprice")]
        public BaseDataPackage<v_supplierprodprice> GetSupProdprice(string SupID)
        {
            var result = new BaseDataPackage<v_supplierprodprice>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.v_supplierprodprice.Where(w => w.SupID == SupID).ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.Status = ApiStatusCode.OK;
                    result.Message = "查询成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "暂无产品数据";
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

        [HttpGet]
        [Route("api/GetProduct")]
        public BaseDataPackage<mproduct> GetProduct()
        {
            var result = new BaseDataPackage<mproduct>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.mproduct.ToList();
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.Status = ApiStatusCode.OK;
                    result.Message = "查询成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "暂无产品数据";
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

        [HttpPost]
        [Route("api/DelSaleForcast")]
        public BaseDataPackage<string> DelSaleForcast()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DelList> list = js.Deserialize<List<DelList>>(request.Params["delList"]);
                int ret = sale.DelSaleForcast(list);
                if (ret == 1)
                {
                    //删除最后条数时删除单头和附件
                    erpsEntities db = new erpsEntities();
                    var tmplist = list.Select(s => s.id).Distinct().ToList();
                    for (int i = 0; i < tmplist.Count; i++)
                    {
                        string SaleFocaNO = tmplist[i];
                        var mx = db.tsaleforcastdtl.Any(w => w.SaleFocaNO == SaleFocaNO);
                        if (!mx)
                        {
                            Clear_Directors(HttpContext.Current.Server.MapPath("/upload/" + SaleFocaNO));
                            db.tsaleforcastatt.Where(w => w.SaleFocaNO == SaleFocaNO).Delete();
                            db.tsaleforcast.Where(w => w.SaleFocaNO == SaleFocaNO).Delete();
                        }
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "删除成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "删除失败";
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

        [HttpPost]
        [Route("api/DelSaleForcastData")]
        public BaseDataPackage<string> DelSaleForcastData()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DelList> list = js.Deserialize<List<DelList>>(request.Params["delList"]);
                int ret = sale.DelSaleForcastData(list);
                if (ret == 1)
                {
                    //删除附件
                    erpsEntities db = new erpsEntities();
                    for (int i = 0; i < list.Count; i++)
                    {
                        string SaleFocaNO = list[i].id;
                        Clear_Directors(HttpContext.Current.Server.MapPath("/upload/" + SaleFocaNO));
                        db.tsaleforcastatt.Where(w => w.SaleFocaNO == SaleFocaNO).Delete();
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "删除成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "删除失败";
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

        [HttpPost]
        [Route("api/DelSaleOrder")]
        public BaseDataPackage<string> DelSaleOrder()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DelListDelDate> list = js.Deserialize<List<DelListDelDate>>(request.Params["delList"]);
                int ret = sale.DelSaleOrder(list);
                if (ret == 1)
                {
                    //删除最后条数时删除单头和附件
                    erpsEntities db = new erpsEntities();
                    var tmplist = list.Select(s => s.id).Distinct().ToList();
                    for (int i = 0; i < tmplist.Count; i++)
                    {
                        string SaleOrderNO = tmplist[i];
                        var mx = db.tsaleorderdtl.Any(w => w.SaleOrderNO == SaleOrderNO);
                        if (!mx)
                        {
                            Clear_Directors(HttpContext.Current.Server.MapPath("/upload/" + SaleOrderNO));
                            db.tsaleorderatt.Where(w => w.SaleOrderNO == SaleOrderNO).Delete();
                            db.tsaleorder.Where(w => w.SaleOrderNO == SaleOrderNO).Delete();
                        }
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "删除成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "删除失败";
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

        [HttpPost]
        [Route("api/DelSaleOrderData")]
        public BaseDataPackage<string> DelSaleOrderData()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DelList> list = js.Deserialize<List<DelList>>(request.Params["delList"]);
                int ret = sale.DelSaleOrderData(list);
                if (ret == 1)
                {
                    //删除附件
                    erpsEntities db = new erpsEntities();
                    for (int i = 0; i < list.Count; i++)
                    {
                        string SaleOrderNO = list[i].id;
                        Clear_Directors(HttpContext.Current.Server.MapPath("/upload/" + SaleOrderNO));
                        db.tsaleorderatt.Where(w => w.SaleOrderNO == SaleOrderNO).Delete();
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "删除成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "删除失败";
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

        public void Clear_Directors(string path)
        {
            if (Directory.Exists(path))
            {
                //获取该路径下的文件夹路径
                string[] directorsList = Directory.GetDirectories(path);
                foreach (string directory in directorsList)
                {
                    Directory.Delete(directory, true);//删除该文件夹及该文件夹下包含的文件
                }
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="SaleFocaNO"></param>
        /// <param name="AttFileName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DelSaleForcastAtt")]
        public BaseDataPackage<string> DelSaleForcastAtt(string SaleFocaNO, string AttFileName)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                int ret = sale.DelSaleForcastAtt(SaleFocaNO, AttFileName);
                if (ret == 1)
                {
                    //删除文件
                    File.Delete(HttpContext.Current.Server.MapPath(AttFileName));
                    result.Status = ApiStatusCode.OK;
                    result.Message = "删除成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "删除失败";
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
        /// 预测-选择冲销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SelSetUpWriteOff")]
        public BaseDataPackage<string> SelSetUpWriteOff()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<SterilizationSel> list = js.Deserialize<List<SterilizationSel>>(request.Params["List"]);

                erpsEntities db = new erpsEntities();
                int ret = 0;
                string SaleFocaNO = list[0].saleFocaNO;
                for (int i = 0; i < list.Count; i++)
                {

                    int SeqNo = list[i].seqNo;
                    float LftNum = list[i].lftNum;
                    var entitys = db.tsaleforcastdtl.Where(w => w.SaleFocaNO == SaleFocaNO && w.SeqNo == SeqNo).FirstOrDefault();

                    entitys.LftNum = LftNum;
                    if (LftNum == 0)
                    {
                        entitys.State = "F";
                    }
                    db.Entry(entitys).State = EntityState.Modified;
                }

                ret = db.SaveChanges();

                if (ret > 0)
                {
                    bool fang = true;
                    var s = db.tsaleforcastdtl.Where(w => w.SaleFocaNO == SaleFocaNO).ToList();
                    for (int i = 0; i < s.Count; i++)
                    {
                        if (string.IsNullOrEmpty(s[i].LftNum.ToString()) || s[i].LftNum > 0)
                        {
                            fang = false;
                        }
                    }
                    //已全部冲销为0，单头状态改为F
                    if (fang)
                    {
                        db.tsaleforcast.Where(w => w.SaleFocaNO == SaleFocaNO)
                            .Update(u => new tsaleforcast() { State = "F" });
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "冲销成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "冲销失败";
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
        /// 销售-选择冲销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SelSetUpWriteOff_Order")]
        public BaseDataPackage<string> SelSetUpWriteOff_Order()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<SterilizationSel> list = js.Deserialize<List<SterilizationSel>>(request.Params["List"]);

                erpsEntities db = new erpsEntities();
                int ret = 0;
                string SaleOrderNO = list[0].saleFocaNO;
                for (int i = 0; i < list.Count; i++)
                {
                    int SeqNo = list[i].seqNo;
                    float LftNum = list[i].lftNum;
                    var entitys = db.tsaleorderdtl.Where(w => w.SaleOrderNO == SaleOrderNO && w.SeqNo == SeqNo).FirstOrDefault();

                    entitys.LftNum = LftNum;
                    if (LftNum == 0)
                    {
                        entitys.State = "F";
                    }
                    db.Entry(entitys).State = EntityState.Modified;
                }

                ret = db.SaveChanges();

                if (ret > 0)
                {
                    bool fang = true;
                    var s = db.tsaleorderdtl.Where(w => w.SaleOrderNO == SaleOrderNO).ToList();
                    for (int i = 0; i < s.Count; i++)
                    {
                        if (string.IsNullOrEmpty(s[i].LftNum.ToString()) || s[i].LftNum > 0)
                        {
                            fang = false;
                        }
                    }
                    //已全部冲销为0，单头状态改为F
                    if (fang)
                    {
                        db.tsaleorder.Where(w => w.SaleOrderNO == SaleOrderNO)
                            .Update(u => new tsaleorder() { State = "F" });
                    }

                    result.Status = ApiStatusCode.OK;
                    result.Message = "冲销成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "冲销失败";
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
        /// 预测-全部冲销
        /// </summary>
        /// <param name="SaleFocaNO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SetUpWriteOffAll")]
        public BaseDataPackage<string> SetUpWriteOffAll(string SaleFocaNO)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                erpsEntities db = new erpsEntities();
                int ret = 0;

                var f1 = db.tsaleforcast.Where(w => w.SaleFocaNO == SaleFocaNO);
                f1.ToList().ForEach(item =>
                {
                    item.State = "F";
                    db.Entry(item).State = EntityState.Modified;
                });
                var entitys = db.tsaleforcastdtl.Where(w => w.SaleFocaNO == SaleFocaNO);
                entitys.ToList().ForEach(item =>
                {
                    item.LftNum = 0;
                    item.State = "F";
                    db.Entry(item).State = EntityState.Modified;
                });
                ret = db.SaveChanges();

                if (ret > 0)
                {
                    result.Status = ApiStatusCode.OK;
                    result.Message = "冲销成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "冲销失败";
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
        /// 销售-全部冲销
        /// </summary>
        /// <param name="SaleOrderNO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SetUpWriteOffAll_Order")]
        public BaseDataPackage<string> SetUpWriteOffAll_Order(string SaleOrderNO)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                erpsEntities db = new erpsEntities();
                int ret = 0;

                var f1 = db.tsaleorder.Where(w => w.SaleOrderNO == SaleOrderNO);
                f1.ToList().ForEach(item =>
                {
                    item.State = "F";
                    db.Entry(item).State = EntityState.Modified;
                });
                var entitys = db.tsaleorderdtl.Where(w => w.SaleOrderNO == SaleOrderNO);
                entitys.ToList().ForEach(item =>
                {
                    item.LftNum = 0;
                    item.State = "F";
                    db.Entry(item).State = EntityState.Modified;
                });
                ret = db.SaveChanges();

                if (ret > 0)
                {
                    result.Status = ApiStatusCode.OK;
                    result.Message = "冲销成功";
                    return result;
                }
                else
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "冲销失败";
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
        /// 获取客户别单价管理数据-分页
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="CusID"></param>
        /// <param name="CusName"></param>
        /// <param name="PdtID"></param>
        /// <param name="PdtName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetCustomerProdPrice")]
        public BaseDataPackage<v_customerprodprice> GetCustomerProdPrice(string pagesize, string currentPage, string orderby, string order, string CusID, string CusName, string PdtID, string PdtName)
        {
            var result = new BaseDataPackage<v_customerprodprice>();
            erpsEntities db = new erpsEntities();
            try
            {
                List<v_customerprodprice> list = new List<v_customerprodprice>();
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();

                var tem = from f in db.v_customerprodprice.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(CusID))
                {
                    tem = tem.Where(w => w.CusID.Contains(CusID));
                }
                if (!string.IsNullOrEmpty(CusName))
                {
                    tem = tem.Where(w => w.CusName.Contains(CusName));
                }
                if (!string.IsNullOrEmpty(PdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(PdtID));
                }
                if (!string.IsNullOrEmpty(PdtName))
                {
                    tem = tem.Where(w => w.PdtName.Contains(PdtName));
                }

                if (string.IsNullOrEmpty(orderby) || string.IsNullOrEmpty(order)) // 没有排序信息，直接按预警排序
                {
                    tem = tem.OrderByDescending(o => o.CreateDate);
                }
                else
                {
                    string orderPhase = "ASC";
                    if (order.ToLower() == "descending") orderPhase = "DESC";
                    tem = Tool.SetQueryableOrder(tem, orderby, orderPhase);
                }

                int total = tem.Count();

                tem = tem
                    .Skip(pageSize * (CurrentPage - 1))
                    .Take(pageSize);
                list = tem.ToList();

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
        /// 新增客户别单价数据
        /// </summary>
        /// <param name="mcustomer"></param>
        /// <returns></returns>
        [ResponseType(typeof(mcustomerprodprice))]
        [HttpPost]
        [Route("api/AddCustomerProdPrice")]
        public BaseDataPackage<string> AddCustomerProdPrice(mcustomerprodprice mcustomer)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();
            if (!ModelState.IsValid)
            {
                result.Status = ApiStatusCode.FAIL;
                result.Message = "无效数据";
                return result;
            }

            mcustomer.CreateDate = DateTime.Now;
            db.mcustomerprodprice.Add(mcustomer);

            try
            {
                db.SaveChanges();
                result.Status = ApiStatusCode.OK;
                result.Message = "提交成功";
                return result;
            }
            catch (Exception ex)
            {
                if (db.mcustomerprodprice.Count(e => e.CusID == mcustomer.CusID && e.PdtID == mcustomer.PdtID) > 0)
                {
                    result.Message = "发生异常=>" + "已存在该客户编号和产品编号组合";
                }
                else
                {
                    result.Message = "发生异常=>" + ex.Message;
                }
                result.Status = ApiStatusCode.EXCEPTION;
                return result;
            }
        }

        /// <summary>
        /// 修改客户别单价数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mcustomer"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [HttpPatch]
        [Route("api/EditCustomerProdPrice")]
        public BaseDataPackage<string> EditCustomerProdPrice(string id, mcustomerprodprice mcustomer)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();
            //if (!ModelState.IsValid)
            //{
            //    result.Status = ApiStatusCode.FAIL;
            //    result.Message = "无效数据";
            //    return result;
            //}

            mcustomer.UpdateDate = DateTime.Now;
            db.Entry(mcustomer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                result.Status = ApiStatusCode.OK;
                result.Message = "修改成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "发生异常=>" + ex.Message;
                result.Status = ApiStatusCode.EXCEPTION;
                return result;
            }
        }

        [HttpPost]
        [Route("api/DelCustomerProdPrice")]
        public BaseDataPackage<string> DelCustomerProdPrice(List<DelThreePrimarykeys> data)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();

            for (int i = 0; i < data.Count; i++)
            {
                var PdtID = data[i].Primary1.ToString();
                var CusID = data[i].Primary2.ToString();
                var Model = data[i].Primary3.ToString();
                var list = db.mcustomerprodprice.Where(c => c.PdtID == PdtID && c.CusID == CusID && c.Model == Model).FirstOrDefault();
                db.mcustomerprodprice.Remove(list);
            }

            try
            {
                db.SaveChanges();
                result.Status = ApiStatusCode.OK;
                result.Message = "删除成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "发生异常=>" + ex.Message;
                result.Status = ApiStatusCode.EXCEPTION;
                return result;
            }
        }  

        protected override void Dispose(bool disposing)
        {
            erpsEntities db = new erpsEntities();
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/cancelSaleForcast")]
        public BaseDataPackage<string> cancelSaleForcast(string SaleOrderNO, string UserID)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                int ret = sale.CancelSaleForcast(SaleOrderNO, UserID);
                if (ret == 1)
                {
                    result.Status = ApiStatusCode.OK;
                    result.Message = "取消成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "取消失败";
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

        [HttpPost]
        [Route("api/CancelSaleOrder")]
        public BaseDataPackage<string> CancelSaleOrder(string SaleOrderNO, string UserID)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                int ret = sale.CancelPurOrderStockIn(SaleOrderNO, UserID);
                if (ret == 1)
                {
                    result.Status = ApiStatusCode.OK;
                    result.Message = "取消成功";
                    return result;
                }
                else
                {
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "取消失败";
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
        /// 上传文件
        /// </summary>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/mcustomerprodprice/import")]
        public BaseDataPackage<mcustomerprodprice> McustomerprodpriceImport()
        {
            var result = new BaseDataPackage<mcustomerprodprice>();

            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象
            string UserID = request.Form["UserID"];
            HttpFileCollection filelist = HttpContext.Current.Request.Files;

            if (filelist != null && filelist.Count > 0)
            {
                HttpPostedFile file = filelist[0];
                string Tpath = "/Import/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                string filename = file.FileName;
                string FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string FilePath = HttpContext.Current.Server.MapPath("~/" + Tpath);
                string type = System.IO.Path.GetExtension(filename);
                DirectoryInfo di = new DirectoryInfo(FilePath);
                if (!di.Exists) { di.Create(); }

                List<mcustomerprodprice> list = null;
                //Dictionary<string, string> ob = new Dictionary<string, string>();
                try
                {
                    string tmpFileName = FilePath + FileName + type;
                    file.SaveAs(tmpFileName);

                    SaleForcast sche = new SaleForcast();
                    list = this.ExcelToMCustomerprodpriceList(tmpFileName, UserID);
                    if (list.Count > 0)
                    {
                        int ret = sche.ImportList(list, out int addNum, out int updNum, out int cfNum);

                        //result.Keys = ob;
                        result.Status = ApiStatusCode.OK;
                        if (cfNum > 0)
                        {
                            result.Message = string.Format("数据导入成功,新增数据： {0}条, 修改数据： {1}条, Excel中有 {2} 条重复数据未导入", addNum, updNum, cfNum);
                        }
                        else
                        {
                            result.Message = string.Format("数据导入成功,新增数据： {0}条, 修改数据： {1}条", addNum, updNum);
                        }
                    }
                    else
                    {
                        result.Status = ApiStatusCode.FAIL;
                        result.Message = "导入数据失败,请确认所选文件是否正确";
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    result.DataList = null;
                    result.Status = ApiStatusCode.FAIL;
                    result.Message = "导入数据失败：" + ex.Message;
                    return result;
                }
            }
            else
            {
                result.Status = ApiStatusCode.EXCEPTION;
                result.Message = "上传导入文件出现异常!";
                return result;
            }
        }

        /// <summary>
        /// 将Excel导入DataTable
        /// </summary>
        /// <param name="filepath">导入的文件路径（包括文件名）</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>DataTable</returns>
        private List<mcustomerprodprice> ExcelToMCustomerprodpriceList(string filepath, string userID)
        {
            List<mcustomerprodprice> lstPdt = new List<mcustomerprodprice>();

            FileInfo newFile = new FileInfo(filepath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(newFile))
            {
                ExcelWorksheet sheet = p.Workbook.Worksheets[0];

                #region check excel format
                if (sheet == null)
                {
                    return lstPdt;
                }
                if (!sheet.Cells[1, 1].Value.Equals("客户编号") ||
                     !sheet.Cells[1, 2].Value.Equals("客户名称") ||
                     !sheet.Cells[1, 3].Value.Equals("车型") ||
                     !sheet.Cells[1, 4].Value.Equals("产品编码") ||
                     !sheet.Cells[1, 5].Value.Equals("产品名称") ||
                     !sheet.Cells[1, 6].Value.Equals("产品规格") ||
                     !sheet.Cells[1, 7].Value.Equals("产地") ||
                     !sheet.Cells[1, 8].Value.Equals("单位") ||
                     !sheet.Cells[1, 9].Value.Equals("销售单价")
                     )
                {
                    return lstPdt;
                }
                #endregion

                #region get last row index
                int lastRow = sheet.Dimension.End.Row;
                //while (sheet.Cells[lastRow, 1].Value == null || sheet.Cells[lastRow, 3].Value == null)
                //{
                //    lastRow--;
                //}
                #endregion

                #region read datas
                for (int i = 2; i <= lastRow; i++)
                {
                    if (sheet.Cells[i, 1].Value != null &&
                        sheet.Cells[i, 3].Value != null &&
                        sheet.Cells[i, 4].Value != null)
                    {
                        lstPdt.Add(new mcustomerprodprice
                        {
                            CusID = sheet.Cells[i, 1].Value.ToString(),
                            Model = sheet.Cells[i, 3].Value.ToString(),
                            PdtID = sheet.Cells[i, 4].Value.ToString(),
                            MakeIn = sheet.Cells[i, 7].Value?.ToString(),
                            SalePrice = float.Parse(sheet.Cells[i, 9].Value?.ToString()),
                            UpdateDate = DateTime.Now,
                            UpdateID = userID
                        });
                    }
                }
                #endregion
            }

            return lstPdt;
        }
    }
}