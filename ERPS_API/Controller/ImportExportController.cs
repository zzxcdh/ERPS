using DAL_MySQL;
using ERPS_API.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ERPS_API.Controller
{
    public class ImportExportController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 导出入库单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InWarExport")]
        public IHttpActionResult InWarExport()
        {
            string name = "";
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                HttpRequestBase request = context.Request;

                JavaScriptSerializer js = new JavaScriptSerializer();
                string purOrderNO = request.Params["purOrderNO"];
                string warId = request.Params["warId"];
                string purchaseDate = request.Params["purchaseDate"];
                name = request.Params["no"];
                List<WarehouseReceipt> listWr = js.Deserialize<List<WarehouseReceipt>>(request.Params["warehouseReceipt"]);


                #region 获取详细信息
                var list = from c in db.twhstockrecords
                           join p in db.mproduct on c.PdtID equals p.PdtID into pp
                           from p in pp.DefaultIfEmpty()
                           join o in db.tpurorderdtl on new { c.PurOrderNO, c.PdtID } equals new { o.PurOrderNO, o.PdtID } into oo
                           from o in oo.DefaultIfEmpty()
                           join s in db.msupplier on o.SupID equals s.SupID into ss
                           from s in ss.DefaultIfEmpty()
                           join w in db.mwarehouse on c.WHID equals w.WHID into ww
                           from w in ww.DefaultIfEmpty()
                           let p1 = new
                           {
                               supID = s.SupID,
                               supName = s.SupName,
                               phone = s.Phone,
                               contact = s.Contact,
                               whName = w.WHName,
                               purPrice = p.PurPrice,
                               total = c.Num * p.PurPrice,
                               updateID = c.UpdateID,
                               updateDate = c.UpdateDate
                           }
                           where c.OpeType == "I" && c.PurOrderNO == purOrderNO && c.WHID == warId
                           select p1;
                var stockrecord = list.OrderByDescending(u => u.updateDate).FirstOrDefault();
                #endregion

                int i = 0;
                FileInfo newFile = new FileInfo(@"d:\ERPS表单\模板\入库单模板.xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                using (var p = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    int pages = (listWr.Count / 17) + 1;

                    //页数
                    worksheet.SetValue(5, 41, 1);
                    //采购订单号
                    worksheet.Cells["F7"].Value = purOrderNO;

                    if (stockrecord != null)
                    {
                        //供应商代码
                        worksheet.Cells["N7"].Value = stockrecord.supID == null ? "" : stockrecord.supID;
                        //供应商名称
                        worksheet.Cells["W7"].Value = stockrecord.supName == null ? "" : stockrecord.supName;
                        //日期
                        worksheet.Cells["AH7"].Value = stockrecord.updateDate;
                        worksheet.Cells["AH7"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //联系人
                        worksheet.Cells["F9"].Value = stockrecord.contact == null ? "" : stockrecord.contact;
                        //联系方式
                        worksheet.Cells["N9"].Value = stockrecord.phone == null ? "" : stockrecord.phone;
                        //入库作业者
                        worksheet.Cells["AJ31"].Value = stockrecord.updateID == null ? "" : stockrecord.updateID;
                        //入库日期
                        worksheet.Cells["AJ32"].Value = DateTime.Now;
                        if (!string.IsNullOrEmpty(purchaseDate))
                        {
                            DateTime date = Convert.ToDateTime(purchaseDate);
                            worksheet.Cells["AJ32"].Value = purchaseDate;
                        }
                        worksheet.Cells["AJ32"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //入库仓库
                        worksheet.Cells["AJ33"].Value = stockrecord.whName == null ? "" : stockrecord.whName;
                    }

                    //页数位置
                    int pageIndex = 49;
                    //模板内容的总行数（包括空行，用于控制两个表格之间的间距）
                    int totalRows = 44;
                    for (int a = 1; a < pages; a++)
                    {
                        //复制
                        worksheet.Cells[1, 1, 34, 42].Copy(worksheet.Cells[a * totalRows + 1, 1]);
                        //页数
                        worksheet.SetValue(pageIndex, 41, a + 1);
                        pageIndex = pageIndex + 44;
                    }

                    //每页数据填充位置
                    int row = 0;
                    //每页数据填充起始位置
                    int startRow = 0;
                    //当前数据所在的页数
                    int currentPage = 0;
                    //当前页合计
                    float currTotal = 0;
                    //第一页合计起始位置
                    int totalIndex = 29;
                    //记录总合计
                    //float total = 0;
                    
                    foreach (var entity in listWr)
                    {

                        //记录单条数据总价
                        float sum = 0;

                        if (i % 17 == 0)
                        {
                            //初始化金额总计，用于记录每一页的总计
                            currTotal = 0;
                            //初始化每页第一行表格数据的插入位置
                            row = 0;
                        }

                        sum = entity.purPrice * entity.num;

                        currTotal = currTotal + sum;

                        //total = total + sum;

                        currentPage = (i / 17) + 1;
                        startRow = 12 + 44 * (currentPage - 1);

                        if ((i + 1) % 17 == 0 || i == listWr.Count - 1)
                        {
                            if (i < 17)
                            {
                                worksheet.SetValue(totalIndex, 35, currTotal);
                            }
                            else
                            {
                                totalIndex = totalIndex + 44;
                                //合计
                                worksheet.SetValue(totalIndex, 35, currTotal);
                            }
                        }

                        //产品类别
                        worksheet.SetValue(startRow + row, 3, entity.pdtType);
                        //产品代码
                        worksheet.SetValue(startRow + row, 7, entity.pdtID);
                        //规格
                        worksheet.SetValue(startRow + row, 11, entity.spec);
                        //产地
                        worksheet.SetValue(startRow + row, 26, entity.makeIn);
                        //单位
                        worksheet.SetValue(startRow + row, 32, entity.unit);
                        //单价
                        worksheet.SetValue(startRow + row, 35, entity.purPrice);
                        //数量
                        worksheet.SetValue(startRow + row, 38, entity.num);

                        i++;
                        row++;
                    }

                    // name = "入库单-" + purOrderNO + DateTime.Now.ToString("HHmmss");
                    p.SaveAs(new FileInfo(@"d:\ERPS表单\" + name + ".xlsx"));
                }
            }
            catch (Exception)
            {
                return new PageResult("error", Request);
            }
            return Content<string>(HttpStatusCode.OK, name);
        }


        /// <summary>
        /// 导出入库历史记录单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InWarHistoricalExport")]
        public IHttpActionResult InWarHistoricalExport()
        {
            string name = "";
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                HttpRequestBase request = context.Request;

                JavaScriptSerializer js = new JavaScriptSerializer();
                string inWarNo = request.Params["No"];


                #region 获取信息
                var warrecord = from inwar in db.inwarrecord
                           where inwar.InWarNo == inWarNo
                           select inwar;
                var record = warrecord.FirstOrDefault();

                var warrecorddtl = from inwardtl in db.inwarrecorddtl
                                   where inwardtl.InWarNo == inWarNo
                                   select inwardtl;
                var listWarrecorddtl = warrecorddtl.ToList<inwarrecorddtl>();
               #endregion

                int i = 0;
                FileInfo newFile = new FileInfo(@"d:\ERPS表单\模板\入库单模板.xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var p = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    int pages = (listWarrecorddtl.Count / 17) + 1;

                    //页数
                    worksheet.SetValue(5, 41, 1);

                    if (record != null)
                    {
                        //采购订单号
                        worksheet.Cells["F7"].Value = record.PurOrderNO;
                        //供应商代码
                        worksheet.Cells["N7"].Value = record.SupID == null ? "" : record.SupID;
                        //供应商名称
                        worksheet.Cells["W7"].Value = record.SupName == null ? "" : record.SupName;
                        //日期
                        worksheet.Cells["AH7"].Value = record.InWarDate;
                        worksheet.Cells["AH7"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //联系人
                        worksheet.Cells["F9"].Value = record.Contact == null ? "" : record.Contact;
                        //联系方式
                        worksheet.Cells["N9"].Value = record.Phone == null ? "" : record.Phone;
                        //入库作业者
                        worksheet.Cells["AJ31"].Value = record.UpdateName == null ? "" : record.UpdateName;
                        //入库日期
                        worksheet.Cells["AJ32"].Value = record.InWarDate;
                        worksheet.Cells["AJ32"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //入库仓库
                        worksheet.Cells["AJ33"].Value = record.WHName == null ? "" : record.WHName;
                    }

                    //页数位置
                    int pageIndex = 49;
                    //模板内容的总行数（包括空行，用于控制两个表格之间的间距）
                    int totalRows = 44;
                    for (int a = 1; a < pages; a++)
                    {
                        //复制
                        worksheet.Cells[1, 1, 34, 42].Copy(worksheet.Cells[a * totalRows + 1, 1]);
                        //页数
                        worksheet.SetValue(pageIndex, 41, a + 1);
                        pageIndex = pageIndex + 44;
                    }

                    //每页数据填充位置
                    int row = 0;
                    //每页数据填充起始位置
                    int startRow = 0;
                    //当前数据所在的页数
                    int currentPage = 0;
                    //当前页合计
                    float currTotal = 0;
                    //第一页合计起始位置
                    int totalIndex = 29;
                    //记录总合计
                    //float total = 0;

                    foreach (var entity in listWarrecorddtl)
                    {

                        //记录单条数据总价
                        float sum = 0;

                        if (i % 17 == 0)
                        {
                            //初始化金额总计，用于记录每一页的总计
                            currTotal = 0;
                            //初始化每页第一行表格数据的插入位置
                            row = 0;
                        }

                        sum = entity.PurPrice.Value * entity.Num.Value;

                        currTotal = currTotal + sum;

                        //total = total + sum;

                        currentPage = (i / 17) + 1;
                        startRow = 12 + 44 * (currentPage - 1);

                        if ((i + 1) % 17 == 0 || i == listWarrecorddtl.Count - 1)
                        {
                            if (i < 17)
                            {
                                worksheet.SetValue(totalIndex, 35, currTotal);
                            }
                            else
                            {
                                totalIndex = totalIndex + 44;
                                //合计
                                worksheet.SetValue(totalIndex, 35, currTotal);
                            }
                        }

                        //产品类别
                        worksheet.SetValue(startRow + row, 3, entity.PdtType);
                        //产品代码
                        worksheet.SetValue(startRow + row, 7, entity.PdtID);
                        //规格
                        worksheet.SetValue(startRow + row, 11, entity.Spec);
                        //产地
                        worksheet.SetValue(startRow + row, 26, entity.MakeIn);
                        //单位
                        worksheet.SetValue(startRow + row, 32, entity.Unit);
                        //单价
                        worksheet.SetValue(startRow + row, 35, entity.PurPrice);
                        //数量
                        worksheet.SetValue(startRow + row, 38, entity.Num);

                        i++;
                        row++;
                    }

                    name = record.InWarNo;
                    p.SaveAs(new FileInfo(@"d:\ERPS表单\" + name + ".xlsx"));
                }
            }
            catch (Exception ex)
            {
                return new PageResult("error", Request);
            }
            return Content<string>(HttpStatusCode.OK, name);
        }




        /// <summary>
        /// 导出出库单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OutWarExport")]
        public IHttpActionResult OutWarExport()
        {
            string name = "";
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                HttpRequestBase request = context.Request;

                JavaScriptSerializer js = new JavaScriptSerializer();
                string saleOrderNO = request.Params["saleOrderNO"];
                string warId = request.Params["warId"];
                string deliveryDate = request.Params["deliveryDate"];
                List<OutboundOrder> listOdo = js.Deserialize<List<OutboundOrder>>(request.Params["outboundOrder"]);


                #region 获取详细信息
                var list = from c in db.twhstockrecords
                           join p in db.mproduct on c.PdtID equals p.PdtID into pp
                           from p in pp.DefaultIfEmpty()
                           join o in db.tsaleorderdtl on new { c.SaleOrderNO, c.PdtID } equals new { o.SaleOrderNO, o.PdtID } into oo
                           from o in oo.DefaultIfEmpty()
                           join m in db.mcustomerprodprice on new { o.CusID, o.PdtID, o.Model } equals new { m.CusID, m.PdtID, m.Model } into mm
                           from m in mm.DefaultIfEmpty()
                           join s in db.mcustomer on o.CusID equals s.CusID into ss
                           from s in ss.DefaultIfEmpty()
                           join w in db.mwarehouse on c.WHID equals w.WHID into ww
                           from w in ww.DefaultIfEmpty()
                           let p1 = new
                           {
                               cusName = s.CusName,
                               saleOrderNO = o.SaleOrderNO,
                               whName = w.WHName,
                               updateID = c.UpdateID,
                               updateDate = c.UpdateDate,
                               contact = s.Contact,
                               phone = s.Phone,
                               address = s.Address
                           }
                           where c.OpeType == "O" && c.SaleOrderNO == saleOrderNO && c.WHID == warId
                           select p1;
                var stockrecord = list.OrderByDescending(u => u.updateDate).FirstOrDefault();
                #endregion

                int i = 0;
                FileInfo newFile = new FileInfo(@"d:\ERPS表单\模板\出库单模板.xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var p = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    int pages = (listOdo.Count / 18) + 1;

                    //页数
                    worksheet.SetValue(7, 42, 1);
                    //订单号
                    worksheet.Cells["AF9"].Value = saleOrderNO;
                    //发货日
                    worksheet.Cells["W11"].Value = deliveryDate;
                    worksheet.Cells["W11"].Style.Numberformat.Format = "YYYY/MM/DD";

                    if (stockrecord != null)
                    {
                        //客户名
                        worksheet.Cells["O7"].Value = stockrecord.cusName == null ? "" : stockrecord.cusName;
                        //收货人
                        worksheet.Cells["F12"].Value = stockrecord.contact == null ? "" : stockrecord.contact;
                        //联系方式
                        worksheet.Cells["N12"].Value = stockrecord.phone == null ? "" : stockrecord.phone;
                        //地址
                        worksheet.Cells["W12"].Value = stockrecord.address == null ? "" : stockrecord.address;
                        //出库作业者
                        worksheet.Cells["AK34"].Value = stockrecord.updateID == null ? "" : stockrecord.updateID;
                        //出库日期
                        worksheet.Cells["AK35"].Value = DateTime.Now;
                        if (!string.IsNullOrEmpty(deliveryDate))
                        {
                            DateTime date = Convert.ToDateTime(deliveryDate);
                            worksheet.Cells["AK35"].Value = deliveryDate;
                        }
                        worksheet.Cells["AK35"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //出库仓库
                        worksheet.Cells["AK36"].Value = stockrecord.whName == null ? "" : stockrecord.whName;
                    }

                    //页数位置
                    int pageIndex = 54;
                    //模板内容的总行数（包括空行，用于控制两个表格之间的间距）
                    int totalRows = 47;
                    for (int a = 1; a < pages; a++)
                    {
                        //复制
                        worksheet.Cells[1, 1, 37, 43].Copy(worksheet.Cells[a * totalRows + 1, 1]);
                        //页数
                        worksheet.SetValue(pageIndex, 42, a + 1);
                        pageIndex = pageIndex + 47;
                    }

                    //每页数据填充位置
                    int row = 0;
                    //每页数据填充起始位置
                    int startRow = 0;
                    //当前数据所在的页数
                    int currentPage = 0;

                    foreach (var entity in listOdo)
                    {
                        currentPage = (i / 18) + 1;
                        startRow = 15 + 46 * (currentPage - 1);
                        if (i % 18 == 0)
                        {
                            //每页第一行数据时清零
                            row = 0;
                        }
                        //序号
                        worksheet.SetValue(startRow + row, 4, i + 1);
                        //产品编号
                        worksheet.SetValue(startRow + row, 6, entity.pdtID);
                        //产品名称
                        worksheet.SetValue(startRow + row, 13, entity.pdtName);
                        //车型
                        worksheet.SetValue(startRow + row, 18, entity.model);
                        //产地
                        worksheet.SetValue(startRow + row, 20, entity.makeIn);
                        //仓库管理信息
                        worksheet.SetValue(startRow + row, 22, entity.mgrInfo);
                        //规格
                        worksheet.SetValue(startRow + row, 26, entity.spec);
                        //单位
                        worksheet.SetValue(startRow + row, 29, entity.unit);
                        //应发数量
                        worksheet.SetValue(startRow + row, 31, entity.orderNum);
                        //实发数量
                        worksheet.SetValue(startRow + row, 33, entity.num);
                        //备注
                        worksheet.SetValue(startRow + row, 35, entity.remark);

                        i++;
                        row++;
                    }

                    name = "出库单-" + saleOrderNO + DateTime.Now.ToString("HHmmss");
                    p.SaveAs(new FileInfo(@"d:\ERPS表单\" + name + ".xlsx"));
                }
            }
            catch (Exception)
            {
                return new PageResult("error", Request);
            }
            return Content<string>(HttpStatusCode.OK, name);
        }


        /// <summary>
        /// 导出出库历史记录单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/OutWarHistoricalExport")]
        public IHttpActionResult OutWarHistoricalExport()
        {
            string name = "";
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                HttpRequestBase request = context.Request;

                JavaScriptSerializer js = new JavaScriptSerializer();
                string outWarNo = request.Params["No"];


                #region 获取信息
                var warrecord = from outwar in db.outwarrecord
                                where outwar.OutWarNo == outWarNo
                                select outwar;
                var record = warrecord.FirstOrDefault();

                var warrecorddtl = from outwar in db.outwarrecorddtl
                                   where outwar.OutWarNo == outWarNo
                                   select outwar;
                var listWarrecorddtl = warrecorddtl.ToList<outwarrecorddtl>();
                #endregion


                int i = 0;
                FileInfo newFile = new FileInfo(@"d:\ERPS表单\模板\出库单模板.xlsx");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var p = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = p.Workbook.Worksheets[0];

                    int pages = (listWarrecorddtl.Count / 18) + 1;

                    //页数
                    worksheet.SetValue(7, 42, 1);

                    if (record != null)
                    {
                        //订单号
                        worksheet.Cells["AF9"].Value = record.SaleOrderNO;
                        //发货日
                        worksheet.Cells["W11"].Value = record.DeliveryDate;
                        worksheet.Cells["W11"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //客户名
                        worksheet.Cells["O7"].Value = record.CusName == null ? "" : record.CusName;
                        //收货人
                        worksheet.Cells["F12"].Value = record.Contact == null ? "" : record.Contact;
                        //联系方式
                        worksheet.Cells["N12"].Value = record.Phone == null ? "" : record.Phone;
                        //地址
                        worksheet.Cells["W12"].Value = record.Address == null ? "" : record.Address;
                        //出库作业者
                        worksheet.Cells["AK34"].Value = record.UpdateName == null ? "" : record.UpdateName;
                        //出库日期
                        worksheet.Cells["AK35"].Value = record.DeliveryDate;
                        worksheet.Cells["AK35"].Style.Numberformat.Format = "YYYY/MM/DD";
                        //出库仓库
                        worksheet.Cells["AK36"].Value = record.WHName == null ? "" : record.WHName;
                    }

                    //页数位置
                    int pageIndex = 54;
                    //模板内容的总行数（包括空行，用于控制两个表格之间的间距）
                    int totalRows = 47;
                    for (int a = 1; a < pages; a++)
                    {
                        //复制
                        worksheet.Cells[1, 1, 37, 43].Copy(worksheet.Cells[a * totalRows + 1, 1]);
                        //页数
                        worksheet.SetValue(pageIndex, 42, a + 1);
                        pageIndex = pageIndex + 47;
                    }

                    //每页数据填充位置
                    int row = 0;
                    //每页数据填充起始位置
                    int startRow = 0;
                    //当前数据所在的页数
                    int currentPage = 0;

                    foreach (var entity in listWarrecorddtl)
                    {
                        currentPage = (i / 18) + 1;
                        startRow = 15 + 46 * (currentPage - 1);
                        if (i % 18 == 0)
                        {
                            //每页第一行数据时清零
                            row = 0;
                        }
                        //序号
                        worksheet.SetValue(startRow + row, 4, i + 1);
                        //产品编号
                        worksheet.SetValue(startRow + row, 6, entity.PdtId);
                        //产品名称
                        worksheet.SetValue(startRow + row, 13, entity.PdtName);
                        //车型
                        worksheet.SetValue(startRow + row, 18, entity.Model);
                        //产地
                        worksheet.SetValue(startRow + row, 20, entity.MakeIn);
                        //仓库管理信息
                        worksheet.SetValue(startRow + row, 22, entity.MgrInfo);
                        //规格
                        worksheet.SetValue(startRow + row, 26, entity.Spec);
                        //单位
                        worksheet.SetValue(startRow + row, 29, entity.Unit);
                        //应发数量
                        worksheet.SetValue(startRow + row, 31, entity.OrderNum);
                        //实发数量
                        worksheet.SetValue(startRow + row, 33, entity.Num);
                        //备注
                        worksheet.SetValue(startRow + row, 35, entity.Remark);

                        i++;
                        row++;
                    }

                    name = record.OutWarNo;
                    p.SaveAs(new FileInfo(@"d:\ERPS表单\" + name + ".xlsx"));
                }
            }
            catch (Exception)
            {
                return new PageResult("error", Request);
            }
            return Content<string>(HttpStatusCode.OK, name);
        }

        //表单下载
        [HttpGet]
        [Route("api/GetExcel")]
        public HttpResponseMessage GetExcelFile(string name)
        {
            try
            {
                var FilePath = @"d:\ERPS表单\" + name + ".xlsx";
                var stream = new FileStream(FilePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name + ".xlsx"
                };
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}
