using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BLL;
using DAL_MySQL;
using ERPS_API.App_Start;
using ERPS_API.Utils;
using Model;
using OfficeOpenXml;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 供应商控制器
    /// </summary>
    public class SuppliersController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 获取所有供应商信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Getmsupplier")]
        public IHttpActionResult Getmsupplier(int pagesize, int currentPage)
        {
            var listSup = from p in db.msupplier
                          select p;
            listSup = listSup.OrderByDescending(s => s.CreateDate);
            var oData = new { total = listSup.Count(), rows = listSup.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        [HttpGet]
        [Route("api/Getsupplier")]
        public BaseDataPackage<msupplier> Getsupplier()
        {
            var result = new BaseDataPackage<msupplier>();
            try
            {
                erpsEntities db = new erpsEntities();
                var list = db.msupplier.ToList();
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
                    result.Message = "暂无供应商数据";
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
        /// 根据条件获取供应商信息
        /// </summary>
        /// <param name="supID"></param>
        /// <param name="supName"></param>
        /// <param name="phone"></param>
        /// <param name="contact"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="address"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmsupplierByCondition")]
        public IHttpActionResult GetmsupplierByCondition(string supID, string supName, string phone, string contact, string startDate, string endDate, string address, int pagesize, int currentPage)
        {
            var listSup = from s in db.msupplier
                          select s;
            if (!string.IsNullOrEmpty(supID))
            {
                listSup = listSup.Where(s => s.SupID.Contains(supID));
            }
            if (!string.IsNullOrEmpty(supName))
            {
                listSup = listSup.Where(s => s.SupName.Contains(supName));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                listSup = listSup.Where(s => s.Phone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(contact))
            {
                listSup = listSup.Where(s => s.Contact.Contains(contact));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listSup = listSup.Where(s => s.CreateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listSup = listSup.Where(s => s.CreateDate < date);
            }
            if (!string.IsNullOrEmpty(address))
            {
                listSup = listSup.Where(s => s.Address.Contains(address));
            }
            if (listSup == null)
            {
                return NotFound();
            }

            listSup = listSup.OrderByDescending(s => s.CreateDate);
            var oData = new { total = listSup.Count(), rows = listSup.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据id修改供应商信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msupplier"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/Putmsupplier")]
        public IHttpActionResult Putmsupplier(string id, msupplier msupplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != msupplier.SupID)
            {
                return BadRequest();
            }

            db.Entry(msupplier).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (!msupplierExists(id))
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
        /// 新增供应商信息
        /// </summary>
        /// <param name="msupplier"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Postmsupplier")]
        public IHttpActionResult Postmsupplier(msupplier msupplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.msupplier.Add(msupplier);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (msupplierExists(msupplier.SupID))
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
        /// 删除供应商信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletemsupplier")]
        public IHttpActionResult Deletemsupplier(string delUid)
        {
            String[] strArray = delUid.Split(',');
            foreach (string id in strArray)
            {
                var list = db.msupplier.Where(s => s.SupID == id).FirstOrDefault();
                db.msupplier.Remove(list);
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

        /// <summary>
        /// 获取供应商单价管理数据-分页
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="SupID"></param>
        /// <param name="SupName"></param>
        /// <param name="PdtID"></param>
        /// <param name="PdtName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetSupplierProdPrice")]
        public BaseDataPackage<v_supplierprodprice> GetSupplierProdPrice(string pagesize, string currentPage, string orderby, string order, string SupID, string SupName, string PdtID, string PdtName)
        {
            var result = new BaseDataPackage<v_supplierprodprice>();
            erpsEntities db = new erpsEntities();
            try
            {
                List<v_supplierprodprice> list = new List<v_supplierprodprice>();
                int pageSize = int.Parse(pagesize);
                int CurrentPage = int.Parse(currentPage);
                Dictionary<string, string> ob = new Dictionary<string, string>();

                var tem = from f in db.v_supplierprodprice.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(SupID))
                {
                    tem = tem.Where(w => w.SupID.Contains(SupID));
                }
                if (!string.IsNullOrEmpty(SupName))
                {
                    tem = tem.Where(w => w.SupName.Contains(SupName));
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
        /// 新增供应商单价数据
        /// </summary>
        /// <param name="msupplier"></param>
        /// <returns></returns>
        [ResponseType(typeof(msupplierprodprice))]
        [HttpPost]
        [Route("api/AddSupplierProdPrice")]
        public BaseDataPackage<string> AddSupplierProdPrice(msupplierprodprice msupplier)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();
            if (!ModelState.IsValid)
            {
                result.Status = ApiStatusCode.FAIL;
                result.Message = "无效数据";
                return result;
            }

            msupplier.CreateDate = DateTime.Now;
            db.msupplierprodprice.Add(msupplier);

            try
            {
                db.SaveChanges();
                result.Status = ApiStatusCode.OK;
                result.Message = "提交成功";
                return result;
            }
            catch (Exception ex)
            {
                if (db.msupplierprodprice.Count(e => e.SupID == msupplier.SupID && e.PdtID == msupplier.PdtID && e.Model == msupplier.Model) > 0)
                {
                    result.Message = "发生异常=>" + "已存在该供应商编号+产品编号+车型组合";
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
        /// 修改供应商别单价数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msupplier"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [HttpPatch]
        [Route("api/EditSupplierProdPrice")]
        public BaseDataPackage<string> EditSupplierProdPrice(string id, msupplierprodprice msupplier)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();

            msupplier.UpdateDate = DateTime.Now;
            db.Entry(msupplier).State = EntityState.Modified;

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
        [Route("api/DelSupplierProdPrice")]
        public BaseDataPackage<string> DelSupplierProdPrice(List<DelThreePrimarykeys> data)
        {
            var result = new BaseDataPackage<string>();
            erpsEntities db = new erpsEntities();

            for (int i = 0; i < data.Count; i++)
            {
                var PdtID = data[i].Primary1.ToString();
                var SupID = data[i].Primary2.ToString();
                var Model = data[i].Primary3.ToString();
                var list = db.msupplierprodprice.Where(c => c.PdtID == PdtID && c.SupID == SupID && c.Model == Model).FirstOrDefault();
                db.msupplierprodprice.Remove(list);
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool msupplierExists(string id)
        {
            return db.msupplier.Count(e => e.SupID == id) > 0;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/msupplier/import")]
        public BaseDataPackage<msupplier> MsupplierImport()
        {
            var result = new BaseDataPackage<msupplier>();

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

                List<msupplier> list = null;
                //Dictionary<string, string> ob = new Dictionary<string, string>();
                try
                {
                    string tmpFileName = FilePath + FileName + type;
                    file.SaveAs(tmpFileName);

                    MSupplier sche = new MSupplier();
                    list = this.ExcelToMSupplierList(tmpFileName, UserID);
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
        private List<msupplier> ExcelToMSupplierList(string filepath, string userID)
        {
            List<msupplier> lstPdt = new List<msupplier>();

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
                if (!sheet.Cells[1, 1].Value.Equals("供应商编号") ||
                     !sheet.Cells[1, 2].Value.Equals("单位名称") ||
                     !sheet.Cells[1, 3].Value.Equals("联系人") ||
                     !sheet.Cells[1, 4].Value.Equals("电话") ||
                     !sheet.Cells[1, 5].Value.Equals("地址") ||
                     !sheet.Cells[1, 6].Value.Equals("备注")
                     )
                {
                    return lstPdt;
                }
                #endregion

                #region get last row index
                int lastRow = sheet.Dimension.End.Row;
                //while (sheet.Cells[lastRow, 1].Value == null || sheet.Cells[lastRow, 1].Value == "")
                //{
                //    lastRow--;
                //}
                #endregion

                #region read datas
                for (int i = 2; i <= lastRow; i++)
                {
                    if (sheet.Cells[i, 1].Value != null)
                    {
                        lstPdt.Add(new msupplier
                        {
                            SupID = sheet.Cells[i, 1].Value.ToString(),
                            SupName = sheet.Cells[i, 2].Value?.ToString(),
                            Contact = sheet.Cells[i, 3].Value?.ToString(),
                            Phone = sheet.Cells[i, 4].Value?.ToString(),
                            Address = sheet.Cells[i, 5].Value?.ToString(),
                            Remark = sheet.Cells[i, 6].Value?.ToString(),
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