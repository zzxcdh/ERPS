using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DAL_MySQL;

namespace BLL
{
    public class MCustomer
    {
        private erpsEntities erpsEntities;

        public MCustomer()
        {
            erpsEntities = new erpsEntities();
        }

        public int ImportList(List<mcustomer> lstpdt, out int addNum, out int updNum, out int cfNum)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    addNum = 0;
                    updNum = 0;
                    cfNum = 0;
                    cfNum = lstpdt.GroupBy(x => x.CusID).Where(x => x.Count() > 1).Count();

                    //去除重复的ID数据
                    lstpdt = lstpdt.Where((x, i) => lstpdt.FindIndex(n => n.CusID == x.CusID) == i).ToList();

                    var all = erpsEntities.mcustomer.AsNoTracking().ToList();
                    var allID = all.Select(s => s.CusID).ToList();
                    //新增
                    var addlist = lstpdt.Where(w => !allID.Contains(w.CusID)).ToList();
                    erpsEntities.mcustomer.AddRange(addlist);

                    //修改
                    var updlist = lstpdt.Where(w => allID.Contains(w.CusID)).ToList();
                    for (int i = 0; i < updlist.Count; i++)
                    {
                        mcustomer mcustomer = new mcustomer();
                        mcustomer = updlist[i];
                        erpsEntities.mcustomer.Attach(mcustomer);

                        erpsEntities.Entry(mcustomer).State = EntityState.Unchanged;
                        erpsEntities.Entry(mcustomer).Property(x => x.CusID).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.CusName).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.Contact).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.Phone).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.Address).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(mcustomer).Property(x => x.UpdateDate).IsModified = true;
                    }
                    ret = erpsEntities.SaveChanges();
                    tran.Commit();
                    addNum = addlist.Count;
                    updNum = updlist.Count;
                    if (ret > 0) { ret = 1; }
                    return ret;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ret = -1;
                    throw ex;
                }
            }
        }
    }
}
