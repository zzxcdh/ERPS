using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DAL_MySQL;
using Z.EntityFramework.Plus;
using Model;

namespace BLL
{
    public class WhInventory
    {
        private erpsEntities erpsEntities;
        public WhInventory()
        {
            erpsEntities = new erpsEntities();
        }

        /// <summary>
        /// 库存-查询
        /// </summary>
        /// <param name="pdtID"></param>      
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<v_whinventoryquery> GetWhInventoryQuery(int pagesize, int currentPage,string pdtID, out int total)
        {
            try
            {
                List<v_whinventoryquery> list = new List<v_whinventoryquery>();
                var tem = from f in erpsEntities.v_whinventoryquery.AsNoTracking()                          
                          select f;                
                if (!string.IsNullOrEmpty(pdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(pdtID));
                }
                tem = tem.OrderBy(o => o.PdtID);

                total = tem.Count();

                tem = tem
                    .Skip(pagesize * (currentPage - 1))
                    .Take(pagesize);
                list = tem.ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 库存预警-查询
        /// </summary>
        /// <param name="purOrderNo"></param>
        /// <param name="supID"></param>
        /// <param name="updUser"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<vwhinventorywarning> GetWhInventoryWarning(int pagesize, int currentPage, string pdtID,string orderby,string order,out int total)
        {
            try
            {
                List<vwhinventorywarning> list = new List<vwhinventorywarning>();
                var tem = from f in erpsEntities.vwhinventorywarning.AsNoTracking()                          
                          select f;                
                if (!string.IsNullOrEmpty(pdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(pdtID));
                }

                if (string.IsNullOrEmpty(orderby) || string.IsNullOrEmpty(order)) // 没有排序信息，直接按预警排序
                {
                    tem = tem.OrderBy(o => o.WN1).ThenBy(o => o.WN2).ThenBy(o => o.WN3);
                }
                else
                {
                    string orderPhase = "ASC";
                    if (order.ToLower() == "descending") orderPhase = "DESC";
                    tem = Tool.SetQueryableOrder(tem, orderby, orderPhase);                    
                }

                total = tem.Count();

                tem = tem
                    .Skip(pagesize * (currentPage - 1))
                    .Take(pagesize);
                list = tem.ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 库存产品-查询
        /// </summary>
        /// <param name="purOrderNo"></param>
        /// <param name="supID"></param>
        /// <param name="updUser"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<mproduct> GetWhProduct(int pagesize, int currentPage,string whId, string pdtID, string pdtName, string spec, string unit, string pdtType, string startDate, string endDate, out int total)
        {
            try
            {
                List<mproduct> list = new List<mproduct>();
                var tem = from p in erpsEntities.mproduct
                          join t in erpsEntities.mproducttype on p.PdtType equals t.PdtTypeID
                          join w in erpsEntities.twhinventory on p.PdtID equals w.PdtID
                          where w.WHID.Equals(whId)
                          select p;
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
                tem = tem.OrderBy(p => p.PdtID);

                total = tem.Count();

                tem = tem
                    .Skip(pagesize * (currentPage - 1))
                    .Take(pagesize);
                list = tem.ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
