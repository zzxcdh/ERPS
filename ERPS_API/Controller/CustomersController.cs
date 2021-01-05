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
using BLL;
using DAL_MySQL;
using ERPS_API.Utils;
using ERPS_API.App_Start;
using OfficeOpenXml;
using System.Web;
using System.IO;

namespace ERPS_API.Controller
{
    /// <summary>
    /// 客户控制器
    /// </summary>
    public class CustomersController : ApiController
    {
        private erpsEntities db = new erpsEntities();

        /// <summary>
        /// 获取所有客户信息
        /// </summary>
        [HttpGet]
        [Route("api/Getmcustomer")]
        public IHttpActionResult Getmcustomer(int pagesize, int currentPage)
        {
            var listCus = from c in db.mcustomer
                          select c;
            listCus = listCus.OrderByDescending(u => u.CreateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }

        /// <summary>
        /// 根据条件获取客户信息
        /// </summary>
        /// <param name="cusID"></param>
        /// <param name="cusName"></param>
        /// <param name="phone"></param>
        /// <param name="contact"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="address"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<mcustomer>))]
        [HttpGet]
        [Route("api/GetmcustomerByCondition")]
        public IHttpActionResult GetmcustomerByCondition(string cusID, string cusName, string phone, string contact, string startDate, string endDate, string address, int pagesize, int currentPage)
        {
            var listCus = from c in db.mcustomer
                          select c;
            if (!string.IsNullOrEmpty(cusID))
            {
                listCus = listCus.Where(c => c.CusID.Contains(cusID));
            }
            if (!string.IsNullOrEmpty(cusName))
            {
                listCus = listCus.Where(c => c.CusName.Contains(cusName));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                listCus = listCus.Where(c => c.Phone.Contains(phone));
            }
            if (!string.IsNullOrEmpty(contact))
            {
                listCus = listCus.Where(c => c.Contact.Contains(contact));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime date = Convert.ToDateTime(startDate);
                listCus = listCus.Where(c => c.CreateDate >= date);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime date = Convert.ToDateTime(endDate);
                date = date.AddDays(1);
                listCus = listCus.Where(c => c.CreateDate < date);
            }
            if (!string.IsNullOrEmpty(address))
            {
                listCus = listCus.Where(c => c.Address.Contains(address));
            }
            if (listCus == null)
            {
                return NotFound();
            }
             
            listCus = listCus.OrderByDescending(c => c.CreateDate);
            var oData = new { total = listCus.Count(), rows = listCus.Skip(pagesize * (currentPage - 1)).Take(pagesize).ToList() };
            return Ok(oData);
        }
        
        /// <summary>
        /// 根据id修改客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mcustomer"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [HttpPatch]
        [Route("api/Putmcustomer")]
        public IHttpActionResult Putmcustomer(string id, mcustomer mcustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mcustomer.CusID)
            {
                return BadRequest();
            }

            db.Entry(mcustomer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (mcustomerExists(mcustomer.CusID))
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
        /// 新增客户信息
        /// </summary>
        /// <param name="mcustomer"></param>
        /// <returns></returns>
        [ResponseType(typeof(mcustomer))]
        [HttpPost]
        [Route("api/Postmcustomer")]
        public IHttpActionResult Postmcustomer(mcustomer mcustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            mcustomer.CreateDate = DateTime.Now;
            db.mcustomer.Add(mcustomer);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                if (mcustomerExists(mcustomer.CusID))
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
        /// 删除客户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [HttpDelete]
        [Route("api/Deletemcustomer")]
        public IHttpActionResult Deletemcustomer(string delUid)
        {
            String[] strArray = delUid.Split(',');
            foreach (string id in strArray)
            {
                var list = db.mcustomer.Where(c => c.CusID == id).FirstOrDefault();
                db.mcustomer.Remove(list);
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

        private bool mcustomerExists(string id)
        {
            return db.mcustomer.Count(e => e.CusID == id) > 0;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/mcustomer/import")]
        public BaseDataPackage<mcustomer> McustomerImport()
        {
            var result = new BaseDataPackage<mcustomer>();

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

                List<mcustomer> list = null;
                //Dictionary<string, string> ob = new Dictionary<string, string>();
                try
                {
                    string tmpFileName = FilePath + FileName + type;
                    file.SaveAs(tmpFileName);

                    MCustomer sche = new MCustomer();
                    list = this.ExcelToMCustomerList(tmpFileName, UserID);
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
        private List<mcustomer> ExcelToMCustomerList(string filepath, string userID)
        {
            List<mcustomer> lstPdt = new List<mcustomer>();

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
                        lstPdt.Add(new mcustomer
                        {
                            CusID = sheet.Cells[i, 1].Value.ToString(),
                            CusName = sheet.Cells[i, 2].Value?.ToString(),
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