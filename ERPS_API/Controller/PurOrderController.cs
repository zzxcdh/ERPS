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
    public class PurOrderController : ApiController
    {
        private erpsEntities db = new erpsEntities();        
        private PurOrder purOrder = new PurOrder();

        //[Authorize]
        [HttpGet]
        [Route("api/GetPurOrder")]
        public BaseDataPackage<v_purorder> GetPurOrder()
        {
            var result = new BaseDataPackage<v_purorder>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string pagesize = request.Params["pagesize"];
                string currentPage = request.Params["currentPage"];
                string SupID = request.Params["SupID"];
                string UpdateID = request.Params["UpdateID"];
                string PurOrderNo = request.Params["PurOrderNo"];
                string SDate = request.Params["SDate"];
                string EDate = request.Params["EDate"];
                string status = request.Params["Status"];

                //SaleForcast sale = new SaleForcast();
                List<v_purorder> list;
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();
                list = purOrder.GetPurOrder(PurOrderNo, SupID, UpdateID, SDate, EDate,status, pageSize, CurrentPage, out int total);
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
        [Route("api/GetPurOrderDtl_PurOrderNO")]
        public BaseDataPackage<v_purorderdtl> GetPurOrderDtl_PurOrderNO(string purOrderNO)
        {
            var result = new BaseDataPackage<v_purorderdtl>();
            try
            {
                List<v_purorderdtl> list;

                list = purOrder.GetPurOrderDtlAndAtt(purOrderNO, out List<tpurorderatt> tpurorderatts);
                if (list.Count > 0)
                {
                    result.DataList = list;
                    result.ObjList = tpurorderatts.Cast<object>().ToList();
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
        /// 导出采购订单详细信息所调用的接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/GetPurOrderDtl_PurOrderNOList")]
        public IHttpActionResult GetPurOrderDtl_PurOrderNOList()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string purOrder = request.Params["purOrderNOList"];

            String[] purOrderArray = purOrder.Split(',');

            List<Object> saleorderdtls = new List<object>();
            try
            {
                erpsEntities db = new erpsEntities();
                foreach (string id in purOrderArray)
                {
                    var list = (from dtl in db.tpurorderdtl
                                join pur in db.tpurorder on dtl.PurOrderNO equals pur.PurOrderNO
                                join s in db.msupplier on dtl.SupID equals s.SupID
                                join p in db.mproduct on dtl.PdtID equals p.PdtID
                                where dtl.PurOrderNO == id
                                let p1 = new
                                {
                                    dtl.PurOrderNO,     //订单号
                                    dtl.SupID,          //供应商编号
                                    s.SupName,          //供应商名称
                                    pur.AppointDate,    //约定日期
                                    dtl.PdtID,          //产品编号
                                    p.PdtName,          //产品名称
                                    p.Spec,             //产品规格
                                    p.Unit,             //单位
                                    p.MakeIn,           //产地
                                    dtl.UnitPrice,      //价格
                                    dtl.OrderNum,       //合同数量
                                    dtl.LftNum,         //剩余数量
                                    dtl.DelDate,        //纳期
                                    dtl.Remark          //备注
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


        [HttpPost]
        [Route("api/AddPurOrderDtl")]
        public BaseDataPackage<string> AddPurOrderDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string CreDate = request.Form["CreDate"];
                //SaleForcast sale = new SaleForcast();
                string PurOrderNo = purOrder.SetPurOrderNo(CreDate);

                tpurorder tpurorder = new tpurorder();
                tpurorder.PurOrderNO = PurOrderNo;
                tpurorder.SupID = request.Form["SupID"];
                tpurorder.CreDate = Convert.ToDateTime(CreDate);
                tpurorder.TotalNum = float.Parse(request.Form["TotalNum"]);
                tpurorder.TotalAmount = float.Parse(request.Form["TotalAmount"]);
                tpurorder.AppointDate = Convert.ToDateTime(request.Form["AppointDate"]);
                tpurorder.UpdateID = request.Form["UserID"];
                tpurorder.State = "N";
                tpurorder.UpdateDate = DateTime.Now;

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<tpurorderdtl> list = js.Deserialize<List<tpurorderdtl>>(request.Params["list"]);

                List<tpurorderdtl> tpurodrdtls = new List<tpurorderdtl>();
                
                for (int i = 0; i < list.Count; i++)
                {
                    tpurorderdtl tpurodrdtl = new tpurorderdtl();
                    tpurodrdtl.PurOrderNO = PurOrderNo;
                    tpurodrdtl.SeqNo = i + 1;
                    tpurodrdtl.SupID = tpurorder.SupID;
                    tpurodrdtl.PdtID = list[i].PdtID;
                    tpurodrdtl.OrderNum = Convert.ToSingle(list[i].OrderNum);
                    tpurodrdtl.LftNum = Convert.ToSingle(list[i].LftNum);
                    tpurodrdtl.UnitPrice = list[i].UnitPrice;
                    tpurodrdtl.TotalAmount = list[i].TotalAmount;
                    tpurodrdtl.DelDate = list[i].DelDate;
                    tpurodrdtl.State = "N";
                    tpurodrdtl.Remark = list[i].Remark;
                    tpurodrdtl.UpdateID = tpurorder.UpdateID;
                    tpurodrdtl.UpdateDate = DateTime.Now;
                    tpurodrdtls.Add(tpurodrdtl);
                }

                List<tpurorderatt> tpurorderatts = new List<tpurorderatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + PurOrderNo + "/";
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
                        tpurorderatt tpurorderatt = new tpurorderatt();
                        string path = basePath + files[i].FileName;

                        tpurorderatt.PurOrderNO = PurOrderNo;
                        tpurorderatt.AttFileName = url + files[i].FileName;
                        tpurorderatt.UpdateID = tpurorderatt.UpdateID;
                        tpurorderatt.UpdateDate = DateTime.Now;

                        tpurorderatts.Add(tpurorderatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = purOrder.AddPurOrderDtl(tpurorder, tpurodrdtls, tpurorderatts);
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
        [Route("api/EditPurOrderDtl")]
        public BaseDataPackage<string> EditPurOrderDtl()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                string PurOrderNO = request.Form["PurOrderNO"];
                string SupID = request.Form["SupID"];
                string UserID = request.Form["UserID"];
                string CreDate = request.Form["CreDate"];                                

                tpurorder tpurorder = new tpurorder();
                tpurorder.PurOrderNO = PurOrderNO;
                tpurorder.SupID = request.Form["SupID"];
                tpurorder.CreDate = Convert.ToDateTime(CreDate);
                tpurorder.TotalNum = float.Parse(request.Form["TotalNum"]);
                tpurorder.TotalAmount = float.Parse(request.Form["TotalAmount"]);
                tpurorder.AppointDate = Convert.ToDateTime(request.Form["AppointDate"]);
                tpurorder.UpdateID = UserID;
                tpurorder.UpdateDate = DateTime.Now;

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<v_purorderdtl> list = js.Deserialize<List<v_purorderdtl>>(request.Params["list"]);
                list.RemoveAll(r => string.IsNullOrEmpty(r.PdtID));
                List<tpurorderdtl> tpurorderdtls = new List<tpurorderdtl>();
                
                for (int i = 0; i < list.Count; i++)
                {
                    tpurorderdtl tpurorderdtl = new tpurorderdtl();
                    tpurorderdtl.PurOrderNO = PurOrderNO;
                    tpurorderdtl.SeqNo = i + 1;
                    tpurorderdtl.SupID = SupID;
                    tpurorderdtl.PdtID = list[i].PdtID;
                    tpurorderdtl.DelDate = list[i].DelDate;
                    tpurorderdtl.OrderNum = Convert.ToSingle(list[i].OrderNum);
                    tpurorderdtl.LftNum = Convert.ToSingle(list[i].LftNum);
                    tpurorderdtl.UnitPrice = list[i].UnitPrice;
                    tpurorderdtl.TotalAmount = list[i].TotalAmount;
                    tpurorderdtl.State = list[i].State == null ? "N" : list[i].State;
                    tpurorderdtl.Remark = list[i].Remark;                                        
                    tpurorderdtl.UpdateID = UserID;
                    tpurorderdtl.UpdateDate = DateTime.Now;

                    tpurorderdtls.Add(tpurorderdtl);
                }

                List<tpurorderatt> tpurorderatts = new List<tpurorderatt>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                string url = "/upload/" + PurOrderNO + "/";
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
                        tpurorderatt tpurorderatt = new tpurorderatt();
                        string path = basePath + files[i].FileName;

                        tpurorderatt.PurOrderNO = PurOrderNO;
                        tpurorderatt.AttFileName = url + files[i].FileName;
                        tpurorderatt.UpdateID = UserID;
                        tpurorderatt.UpdateDate = DateTime.Now;

                        tpurorderatts.Add(tpurorderatt);
                        tmpPath.Add(path);
                    }
                }

                int ret = purOrder.EditPurOrderDtl(PurOrderNO, tpurorder,tpurorderdtls, tpurorderatts);
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

        [HttpGet]
        [Route("api/GetCustomer")]
        public BaseDataPackage<mcustomer> GetCustomer()
        {
            var result = new BaseDataPackage<mcustomer>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.mcustomer.ToList();
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
                    result.Message = "暂无客户数据";
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
        [Route("api/DelPurOrder")]
        public BaseDataPackage<string> DelPurOrder()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DelList> list = js.Deserialize<List<DelList>>(request.Params["delList"]);
                int ret = purOrder.DelPurOrder(list);
                if (ret == 1)
                {
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
        [Route("api/CancelPurOrder")]
        public BaseDataPackage<string> CancelPurOrder(string PurOrderNO, string UserID,string NegativeInventory)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                int ret = purOrder.CancelPurOrderStockIn(PurOrderNO, UserID, NegativeInventory, out string errPdtID, out string errWHID);
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
                    if (ret == -9)
                    {
                        Dictionary<string, string> ob = new Dictionary<string, string>();
                        ob.Add("ret", "-9");
                        result.Keys = ob;
                        result.Message = string.Format("{0} 仓库下的 {1} 产品库存不足", errWHID, errPdtID);
                    }
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
        /// 删除附件
        /// </summary>
        /// <param name="PurOrderNO"></param>
        /// <param name="AttFileName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DelPurOrderAtt")]
        public BaseDataPackage<string> DelPurOrderAtt(string PurOrderNO, string AttFileName)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                int ret = purOrder.DelPurOrderAtt(PurOrderNO, AttFileName);
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
        /// 选择采购订单冲销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SelPurOrderDtlWriteOff")]
        public BaseDataPackage<string> SelPurOrderDtlWriteOff()
        {
            var result = new BaseDataPackage<string>();
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<SterilizationSel> list = js.Deserialize<List<SterilizationSel>>(request.Params["List"]);

                erpsEntities db = new erpsEntities();
                db.Database.Log = (log) => { System.Diagnostics.Debug.WriteLine(log); };
                int ret = 0;

                //for (int i = 0; i < list.Count; i++)
                //{
                //    string PurOrderNO = list[i].saleFocaNO;
                //    int SeqNo = list[i].seqNo;
                //    float LftNum = list[i].lftNum;
                //    var entitys = db.tpurorderdtl.Where(w => w.PurOrderNO == PurOrderNO && w.SeqNo == SeqNo);
                //    entitys.ToList().ForEach(item =>
                //    {
                //        item.LftNum = LftNum;
                //        item.State = "F";
                //        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                //    });
                //}

                string PurOrderNO2 = list.Count>0? list[0].saleFocaNO:"";                
                //var unFunishOrderDtls = db.tpurorderdtl.Where(w => w.PurOrderNO == PurOrderNO2 && (!w.State.Equals("F") || w.State == null)).ToList<tpurorderdtl>();
                int fNum = 0; //所有已经冲销的明细数量
                var unFunishOrderDtls = db.tpurorderdtl.Where(w => w.PurOrderNO == PurOrderNO2).ToList<tpurorderdtl>();
                for(int i=0;i< unFunishOrderDtls.Count; i++)
                {
                    if (unFunishOrderDtls[i].State == "F")
                    {
                        fNum++;
                    }
                    else
                    {
                        for(int j=0;j< list.Count;j++)
                        {
                            if(unFunishOrderDtls[i].PurOrderNO == list[j].saleFocaNO && unFunishOrderDtls[i].SeqNo==list[j].seqNo)
                            {
                                unFunishOrderDtls[i].LftNum = list[j].lftNum;
                                unFunishOrderDtls[i].State = "F";
                                fNum++;
                            }
                        }
                    }
                }
                //如果所有明细的状态都是"F"完成，把整体订单的状态都改成完成
                if (fNum == unFunishOrderDtls.Count)
                {
                    var purOrder = db.tpurorder.Where(w => w.PurOrderNO == PurOrderNO2).FirstOrDefault<tpurorder>();
                    purOrder.State = "F";
                }

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
        /// 全部采购订单冲销
        /// </summary>
        /// <param name="PurOrderNO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WholePurOrderWriteOff")]
        public BaseDataPackage<string> WholePurOrderWriteOff(string PurOrderNO)
        {
            var result = new BaseDataPackage<string>();
            try
            {
                erpsEntities db = new erpsEntities();
                int ret = 0;

                var purorder = db.tpurorder.Where(w => w.PurOrderNO == PurOrderNO).FirstOrDefault<tpurorder>();
                purorder.State = "F";
                
                var entitys = db.tpurorderdtl.Where(w => w.PurOrderNO == PurOrderNO);
                entitys.ToList().ForEach(item =>
                {
                    item.LftNum = 0;
                    item.State = "F";
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
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
        /// 根据采购订单号获取产品详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetProductByOrderNO")]
        public IHttpActionResult GetProductByOrderNO(string id, int pagesize, int currentPage)
        {
            var listOrderdtl = (from o in db.tpurorderdtl
                                join p in db.mproduct
                                on o.PdtID equals p.PdtID
                                where o.PurOrderNO == id && o.LftNum > 0
                                let p1 = new
                                {
                                    o.SeqNo,
                                    p.PdtID,
                                    p.PdtName,
                                    p.Spec,
                                    p.Unit,
                                    o.LftNum,
                                    o.UnitPrice,
                                    TotalAmount = o.LftNum * o.UnitPrice,
                                    o.Remark
                                }
                                select p1);
            listOrderdtl = listOrderdtl.OrderBy(o => o.PdtID).ThenBy(o => o.PdtID);
            var oData = new { total = listOrderdtl.Count(), rows = listOrderdtl.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        [HttpGet]
        [Route("api/GettpurorderId")]
        public IHttpActionResult GettpurorderId()
        {
            var listOrder = from o in db.tpurorder
                            select o.PurOrderNO;
            return Ok(listOrder);
        }

        [HttpGet]
        [Route("api/GettpurorderIdAndName")]
        public IHttpActionResult GettpurorderIdAndName()
        {
            var listWar = from t in db.tpurorder
                          join s in db.msupplier
                          on t.SupID equals s.SupID
                          let p1 = new
                          {
                              value = t.PurOrderNO,
                              label = s.SupName
                          }
                          where t.State != "F" 
                          select p1;
            return Ok(listWar);
        }

        private bool tpurorderExists(string id)
        {
            return db.tpurorder.Count(e => e.PurOrderNO == id) > 0;
        }
    }
}
