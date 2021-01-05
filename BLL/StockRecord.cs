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
    class StockRecord
    {
        private erpsEntities erpsEntities;
        public StockRecord()
        {
            erpsEntities = new erpsEntities();
        }

        /// <summary>
        /// 出入库记录查询
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
        public List<twhstockrecords> GetStockRecords(string pdtID, string whID, string updUser, string SDate, string EDate, string opeType, int pagesize, int currentPage, out int total)
        {
            try
            {
                List<twhstockrecords> list = new List<twhstockrecords>();
                var tem = from f in erpsEntities.twhstockrecords.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(pdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(pdtID));
                }
                if (!string.IsNullOrEmpty(whID))
                {
                    tem = tem.Where(w => w.WHID.Contains(whID));
                }
                if (!string.IsNullOrEmpty(updUser))
                {
                    tem = tem.Where(w => w.UpdateID.Contains(updUser));
                }
                if (!string.IsNullOrEmpty(SDate))
                {
                    DateTime date = Convert.ToDateTime(SDate);
                    tem = tem.Where(w => w.UpdateDate >= date);
                }
                if (!string.IsNullOrEmpty(EDate))
                {
                    DateTime date = Convert.ToDateTime(EDate);
                    date = date.AddDays(1);
                    tem = tem.Where(w => w.UpdateDate < date);
                }
                if (!string.IsNullOrEmpty(opeType))
                {
                    tem = tem.Where(w => w.OpeType.Contains(opeType));
                }
                tem = tem.OrderBy(o => o.UpdateDate).ThenBy(o => o.PdtID);

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
