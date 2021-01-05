using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DAL_MySQL;

namespace BLL
{
    public class MSupplier
    {
        private erpsEntities erpsEntities;

        public MSupplier()
        {
            erpsEntities = new erpsEntities();
        }

        public int ImportList(List<msupplier> lstpdt, out int addNum, out int updNum, out int cfNum)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    addNum = 0;
                    updNum = 0;
                    cfNum = 0;
                    cfNum = lstpdt.GroupBy(x => x.SupID).Where(x => x.Count() > 1).Count();

                    //去除重复的ID数据
                    lstpdt = lstpdt.Where((x, i) => lstpdt.FindIndex(n => n.SupID == x.SupID) == i).ToList();

                    var all = erpsEntities.msupplier.AsNoTracking().ToList();
                    var allID = all.Select(s => s.SupID).ToList();
                    //新增
                    var addlist = lstpdt.Where(w => !allID.Contains(w.SupID)).ToList();
                    erpsEntities.msupplier.AddRange(addlist);

                    //修改
                    var updlist = lstpdt.Where(w => allID.Contains(w.SupID)).ToList();
                    for (int i = 0; i < updlist.Count; i++)
                    {
                        msupplier msupplier = new msupplier();
                        msupplier = updlist[i];
                        erpsEntities.msupplier.Attach(msupplier);

                        erpsEntities.Entry(msupplier).State = EntityState.Unchanged;
                        erpsEntities.Entry(msupplier).Property(x => x.SupID).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.SupName).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.Contact).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.Phone).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.Address).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(msupplier).Property(x => x.UpdateDate).IsModified = true;
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
