using DAL_MySQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL
{
    public class MProduct
    {
        private erpsEntities erpsEntities;

        public MProduct()
        {
            erpsEntities = new erpsEntities();
        }

        public int ImportMProductList(List<mproduct> lstpdt, out int addNum, out int updNum, out int cfNum)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    addNum = 0;
                    updNum = 0;
                    cfNum = 0;
                    cfNum = lstpdt.GroupBy(x => x.PdtID).Where(x => x.Count() > 1).Count();

                    //去除重复的ID数据
                    lstpdt = lstpdt.Where((x, i) => lstpdt.FindIndex(n => n.PdtID == x.PdtID) == i).ToList();

                    var all = erpsEntities.mproduct.AsNoTracking().ToList();
                    var allID = all.Select(s => s.PdtID).ToList();
                    //新增
                    var addlist = lstpdt.Where(w => !allID.Contains(w.PdtID)).ToList();
                    erpsEntities.mproduct.AddRange(addlist);

                    //修改
                    var updlist = lstpdt.Where(w => allID.Contains(w.PdtID)).ToList();
                    for (int i = 0; i < updlist.Count; i++)
                    {
                        mproduct mproduct = new mproduct();
                        mproduct = updlist[i];
                        //var curState = erpsEntities.Entry<mproduct>(mproduct).State;                      
                        erpsEntities.mproduct.Attach(mproduct);

                        erpsEntities.Entry(mproduct).State = EntityState.Modified;
                        erpsEntities.Entry(mproduct).Property(x => x.SalPrice).IsModified = false;                        
                        erpsEntities.Entry(mproduct).Property(x => x.MgrInfo).IsModified = false;
                        erpsEntities.Entry(mproduct).Property(x => x.Remark).IsModified = false;
                        erpsEntities.Entry(mproduct).Property(x => x.CreateID).IsModified = false;
                        erpsEntities.Entry(mproduct).Property(x => x.CreateDate).IsModified = false;
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