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
using OfficeOpenXml;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    public class ProductsController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 获取所有产品信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Getmproduct")]
        public IHttpActionResult Getmproduct(int pagesize, int currentPage)
        {
            var listPro = from p in db.mproduct
                          join t in db.mproducttype
                          on p.PdtType equals t.PdtTypeID into tt
                          from t in tt.DefaultIfEmpty()
                          let p1 = new
                          {
                              p.PdtID,
                              p.PdtName,
                              p.PdtType,
                              p.Spec,
                              p.Unit,
                              p.PurPrice,
                              p.SalPrice,
                              p.MakeIn,
                              p.MgrInfo,
                              p.Remark,
                              p.CreateID,
                              p.CreateDate,
                              p.UpdateID,
                              p.UpdateDate,
                              t.PdtTypeName
                          }
                          select p1;
            listPro = listPro.OrderByDescending(P => P.CreateDate);
            var oData = new { total = listPro.Count(), rows = listPro.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件获取产品信息
        /// </summary>
        /// <param name="pdtID"></param>
        /// <param name="pdtName"></param>
        /// <param name="spec"></param>
        /// <param name="unit"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetmproductByCondition")]
        public IHttpActionResult GetmproductByCondition(string pdtID, string pdtName, string spec, string unit, string pdtType, string startDate, string endDate, int pagesize, int currentPage)
        {
            var listPro = from p in db.mproduct
                          join t in db.mproducttype
                          on p.PdtType equals t.PdtTypeID into tt
                          from t in tt.DefaultIfEmpty()
                          let p1 = new
                          {
                              p.PdtID,
                              p.PdtName,
                              p.PdtType,
                              p.Spec,
                              p.Unit,
                              p.PurPrice,
                              p.SalPrice,
                              p.MakeIn,
                              p.MgrInfo,
                              p.Remark,
                              p.CreateID,
                              p.CreateDate,
                              p.UpdateID,
                              p.UpdateDate,
                              t.PdtTypeName
                          }
                          select p1;
            if (!string.IsNullOrEmpty(pdtID))
            {
                listPro = listPro.Where(p => p.PdtID.Contains(pdtID));
            }
            if (!string.IsNullOrEmpty(pdtName))
            {
                listPro = listPro.Where(p => p.PdtName.Contains(pdtName));
            }
            if (!string.IsNullOrEmpty(spec))
            {
                listPro = listPro.Where(p => p.Spec.Contains(spec));
            }
            if (!string.IsNullOrEmpty(unit))
            {
                listPro = listPro.Where(p => p.Unit.Contains(unit));
            }
            if (!string.IsNullOrEmpty(pdtType))
            {
                listPro = listPro.Where(p => p.PdtType == pdtType);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listPro = listPro.Where(p => p.CreateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listPro = listPro.Where(p => p.CreateDate < date);
            }
            if (listPro == null)
            {
                return NotFound();
            }

            listPro = listPro.OrderByDescending(p => p.CreateDate);
            var oData = new { total = listPro.Count(), rows = listPro.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据id修改产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mproduct"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/Putmproduct")]
        public IHttpActionResult Putmproduct(string id, mproduct mproduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mproduct.PdtID)
            {
                return BadRequest();
            }

            db.Entry(mproduct).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (!mproductExists(id))
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
        /// 新增产品信息
        /// </summary>
        /// <param name="mproduct"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Postmproduct")]
        public IHttpActionResult Postmproduct(mproduct mproduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            mproduct.CreateDate = DateTime.Now;
            db.mproduct.Add(mproduct);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (mproductExists(mproduct.PdtID))
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
        /// 上传文件
        /// </summary>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/mproduct/import")]
        public BaseDataPackage<mproduct> MproductImport()
        {
            var result = new BaseDataPackage<mproduct>();

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

                List<mproduct> list = null;
                //Dictionary<string, string> ob = new Dictionary<string, string>();
                try
                {
                    string tmpFileName = FilePath + FileName + type;
                    file.SaveAs(tmpFileName);

                    MProduct sche = new MProduct();
                    list = this.ExcelToMproductList(tmpFileName, UserID);
                    if (list.Count > 0)
                    {
                        int ret = sche.ImportMProductList(list, out int addNum, out int updNum, out int cfNum);

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
        /// 删除产品信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/Deletemproduct")]
        public IHttpActionResult Deletemproduct(string delUid)
        {
            String[] strArray = delUid.Split(',');
            foreach (string id in strArray)
            {
                var list = db.mproduct.Where(p => p.PdtID == id).FirstOrDefault();
                db.mproduct.Remove(list);
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

        private bool mproductExists(string id)
        {
            return db.mproduct.Count(e => e.PdtID == id) > 0;
        }

        /// <summary>
        /// 将Excel导入DataTable
        /// </summary>
        /// <param name="filepath">导入的文件路径（包括文件名）</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>DataTable</returns>
        private List<mproduct> ExcelToMproductList(string filepath, string userID)
        {
            List<mproduct> lstPdt = new List<mproduct>();

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
                if (!sheet.Cells[1, 1].Value.Equals("产品编码") ||
                     !sheet.Cells[1, 2].Value.Equals("产品名称") ||
                     !sheet.Cells[1, 3].Value.Equals("产品规格") ||
                     !sheet.Cells[1, 4].Value.Equals("产地") ||
                     !sheet.Cells[1, 5].Value.Equals("单位") ||
                     !sheet.Cells[1, 6].Value.Equals("进货单价") ||
                     !sheet.Cells[1, 7].Value.Equals("产品类型编码")
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
                        lstPdt.Add(new mproduct
                        {
                            PdtID = sheet.Cells[i, 1].Value.ToString(),
                            PdtName = sheet.Cells[i, 2].Value?.ToString(),
                            Spec = sheet.Cells[i, 3].Value?.ToString(),
                            MakeIn = sheet.Cells[i, 4].Value?.ToString(),
                            Unit = sheet.Cells[i, 5].Value?.ToString(),
                            PurPrice = float.Parse(sheet.Cells[i, 6].Value?.ToString()),
                            PdtType = sheet.Cells[i, 7].Value?.ToString(),
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