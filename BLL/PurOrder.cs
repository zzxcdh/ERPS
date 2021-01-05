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
    public class PurOrder
    {
        private erpsEntities erpsEntities;
        public PurOrder()
        {
            erpsEntities = new erpsEntities();
        }

        /// <summary>
        /// 采购订单-查询
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
        public List<v_purorder> GetPurOrder(string purOrderNo, string supID, string updUser, string SDate, string EDate,string status, int pagesize, int currentPage, out int total)
        {
            try
            {
                List<v_purorder> list = new List<v_purorder>();
                var tem = from f in erpsEntities.v_purorder.AsNoTracking()                          
                          select f;
                if (!string.IsNullOrEmpty(purOrderNo))
                {
                    tem = tem.Where(w => w.PurOrderNO.Contains(purOrderNo));
                }
                if (!string.IsNullOrEmpty(supID))
                {
                    tem = tem.Where(w => w.SupID.Contains(supID));
                }
                if (!string.IsNullOrEmpty(updUser))
                {
                    tem = tem.Where(w => w.UpdateID.Contains(updUser));
                }
                if (!string.IsNullOrEmpty(SDate))
                {
                    DateTime date = Convert.ToDateTime(SDate);
                    tem = tem.Where(w => w.CreDate >= date);
                }
                if (!string.IsNullOrEmpty(EDate))
                {
                    DateTime date = Convert.ToDateTime(EDate);
                    date = date.AddDays(1);
                    tem = tem.Where(w => w.CreDate < date);
                }
                if (!string.IsNullOrEmpty(status) && status != "A")
                {
                    tem = tem.Where(w => w.State.Equals(status));
                }
                tem = tem.OrderBy(o => o.CreDate).ThenBy (o=>o.PurOrderNO);

                total = tem.Count();

                tem = tem
                    .Skip(pagesize * (currentPage - 1))
                    .Take(pagesize);
                list = tem.ToList();
                return list;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// 根据采购编号获取采购明细
        /// </summary>
        /// <param name="PurOrderNo"></param>
        /// <returns></returns>
        public List<v_purorderdtl> GetSaleForcastDtl(string purOrderNo)
        {
            try
            {
                List<v_purorderdtl> list = new List<v_purorderdtl>();
                var tem = from f in erpsEntities.v_purorderdtl.AsNoTracking()
                          where f.PurOrderNO == purOrderNo
                          select f;
                list = tem.ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据采购订单号获取采购明细及附件
        /// </summary>
        /// <param name="PurOrderNo"></param>
        /// <param name="tpurorderatt"></param>
        /// <returns></returns>
        public List<v_purorderdtl> GetPurOrderDtlAndAtt(string purOrderNo, out List<tpurorderatt> tpurorderatts)
        {
            try
            {
                List<v_purorderdtl> list = new List<v_purorderdtl>();
                list = erpsEntities.v_purorderdtl.AsNoTracking().Where(w => w.PurOrderNO == purOrderNo).ToList();

                tpurorderatts = erpsEntities.tpurorderatt.Where(w => w.PurOrderNO == purOrderNo).ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 生成采购订单号        
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string SetPurOrderNo(string date)
        {                        
            var PurOrderNo = "";
            DateTime dateTime = Convert.ToDateTime(date);
            var isExists = erpsEntities.tpurorder.Any(a => a.CreDate == dateTime);
            if (isExists)
            {
                //int seq = erpsEntities.tpurorder.Where(w => w.CreDate == dateTime).Count();
                string maxid = erpsEntities.tpurorder.Where(w => w.CreDate == dateTime).OrderByDescending(o => o.PurOrderNO).FirstOrDefault().PurOrderNO;
                var arr = maxid.Split('-');
                int seq = int.Parse(arr[2]);
                seq++;
                PurOrderNo = "CG-" + date.Replace("-", "") + "-" + seq.ToString("000");
            }
            else
            {
                PurOrderNo = "CG-" + date.Replace("-", "") + "-001";
            }
            return PurOrderNo;
        }

        /// <summary>
        /// 采购订单-新增
        /// </summary>
        /// <param name="tpurorder"></param>
        /// <param name="tpurorderdtl"></param>
        /// <param name="tpurorderatt"></param>
        /// <returns></returns>
        public int AddPurOrderDtl(tpurorder tpurodr, List<tpurorderdtl> tpurodrdtl, List<tpurorderatt> tpurodratt)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    erpsEntities.tpurorder.Add(tpurodr);
                    erpsEntities.tpurorderdtl.AddRange(tpurodrdtl);
                    erpsEntities.tpurorderatt.AddRange(tpurodratt);

                    ret = erpsEntities.SaveChanges();
                    tran.Commit();
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

        public int EditPurOrderDtl(string purOrderNo, List<tpurorderdtl> tpurodrdtl, List<tpurorderatt> tpurodratt)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    var vm = erpsEntities.tpurorderdtl.Where(m => m.PurOrderNO == purOrderNo);
                    vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                    erpsEntities.tpurorderdtl.RemoveRange(vm);

                    erpsEntities.tpurorderdtl.AddRange(tpurodrdtl);
                    erpsEntities.tpurorderatt.AddRange(tpurodratt);

                    ret = erpsEntities.SaveChanges();
                    tran.Commit();
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

        public int EditPurOrderDtl(string purOrderNo,tpurorder tpurorder, List<tpurorderdtl> tpurodrdtl, List<tpurorderatt> tpurodratt)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    var tpodr = erpsEntities.tpurorder.Attach(tpurorder);
                    erpsEntities.Entry(tpurorder).Property("TotalNum").IsModified = true;
                    erpsEntities.Entry(tpurorder).Property("TotalAmount").IsModified = true;
                    erpsEntities.Entry(tpurorder).Property("Remark").IsModified = true;
                    erpsEntities.Entry(tpurorder).Property("AppointDate").IsModified = true;
                    erpsEntities.Entry(tpurorder).Property("UpdateID").IsModified = true;
                    erpsEntities.Entry(tpurorder).Property("UpdateDate").IsModified = true;                    
                    var vm = erpsEntities.tpurorderdtl.Where(m => m.PurOrderNO == purOrderNo);
                    vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                    erpsEntities.tpurorderdtl.RemoveRange(vm);

                    erpsEntities.tpurorderdtl.AddRange(tpurodrdtl);
                    erpsEntities.tpurorderatt.AddRange(tpurodratt);

                    ret = erpsEntities.SaveChanges();
                    tran.Commit();
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


        public int DelPurOrder(List<DelList> delLists) {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i].id;                        

                        var vm = erpsEntities.tpurorderdtl.Where(m => m.PurOrderNO == id);
                        vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                        var vm2 = erpsEntities.tpurorder.Where(m => m.PurOrderNO == id).FirstOrDefault();
                        erpsEntities.tpurorder.Remove(vm2);
                        erpsEntities.tpurorderdtl.RemoveRange(vm);
                    }

                    ret = erpsEntities.SaveChanges();
                    tran.Commit();
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

        /// <summary>
        /// 取消采购清单入库
        /// 采购订单数量状态恢复，出入库记录标记为”C,取消“，没有的话加个列， 库存数量恢复。
        /// </summary>
        /// <param name="purOrderNo"></param>
        /// <returns></returns>
        public int CancelPurOrderStockIn(string purOrderNo, string userID,string NegativeInventory, out string errPdtID,out string errWHID)
        {
            int ret = 0;
            errPdtID = "";
            errWHID = "";
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    // 1 修改订单明表内容
                    var purOrder = erpsEntities.tpurorder.AsNoTracking().Where(x => x.PurOrderNO.Equals(purOrderNo)).ToList();
                    if (purOrder.Count < 1) return ret;
                    var purodr = purOrder.FirstOrDefault<tpurorder>();
                    purodr.State = "N";
                    //purodr.UpdateID = userID;
                    //purodr.UpdateDate = DateTime.Now;
                    //erpsEntities.tpurorder.Attach(purodr);
                    erpsEntities.Entry(purodr).State = EntityState.Unchanged;
                    erpsEntities.Entry(purodr).Property(x => x.State).IsModified = true;

                    // 修改订单明细内容
                    var purodrdtls = erpsEntities.tpurorderdtl.AsNoTracking().Where(x => x.PurOrderNO.Equals(purOrderNo)).ToList();
                    for(int i=0;i<purodrdtls.Count;i++)
                    {
                        purodrdtls[i].DelNum = 0;
                        purodrdtls[i].LftNum = purodrdtls[i].OrderNum;
                        purodrdtls[i].State = "N";
                        //erpsEntities.tpurorderdtl.Attach(purodrdtls[i]);
                        //purodrdtls[i].UpdateDate = DateTime.Now;
                        erpsEntities.Entry(purodrdtls[i]).State = EntityState.Unchanged;
                        erpsEntities.Entry(purodrdtls[i]).Property(x => x.DelNum).IsModified = true;
                        erpsEntities.Entry(purodrdtls[i]).Property(x => x.LftNum).IsModified = true;
                        erpsEntities.Entry(purodrdtls[i]).Property(x => x.State).IsModified = true;                        
                    }

                    string whid = "";
                    string pdtid = "";
                    float invNum = 0;
                    var date = DateTime.Now;
                    var stockrcds = erpsEntities.twhstockrecords.AsNoTracking().Where(w => w.PurOrderNO.Equals(purOrderNo) && w.State != "C" ).ToList();
                    for (int i = 0; i < stockrcds.Count; i++)
                    {
                        whid = stockrcds[i].WHID;
                        pdtid = stockrcds[i].PdtID;
                        // 1 修改出入库记录表
                        stockrcds[i].State = "C"; //状态
                        stockrcds[i].UpdateDate = date;
                        //stockrcds[i].UpdateID = userID;
                        erpsEntities.Entry(stockrcds[i]).State = EntityState.Unchanged;
                        erpsEntities.Entry(stockrcds[i]).Property(x => x.State).IsModified = true;
                        erpsEntities.Entry(stockrcds[i]).Property(x => x.UpdateDate).IsModified = true;
                        //erpsEntities.Entry(stockrcds[i]).Property(x => x.UpdateID).IsModified = true;                        

                        // 2, 修改库存表                        
                        //var invrcds = erpsEntities.twhinventory.Where(w => w.WHID.Equals(whid) && w.PdtID.Equals(pdtid)).AsNoTracking().ToList();
                        //if (invrcds.Count > 0)
                        //{
                        //    var invrcd = invrcds.FirstOrDefault<twhinventory>();
                        //    invrcd.InvNum = invrcd.InvNum - stockrcds[i].Num;
                        //    erpsEntities.Entry(invrcd).State = EntityState.Unchanged;
                        //    erpsEntities.Entry(invrcd).Property(x => x.InvNum).IsModified = true;
                        //}
                        twhstockrecords tmpstockrcds = new twhstockrecords();
                        tmpstockrcds.AreaID = stockrcds[i].AreaID;
                        tmpstockrcds.No = stockrcds[i].No;
                        tmpstockrcds.Num = stockrcds[i].Num;                        
                        tmpstockrcds.PdtID = stockrcds[i].PdtID;
                        tmpstockrcds.PosiID = stockrcds[i].PosiID;
                        tmpstockrcds.PurOrderNO = stockrcds[i].PurOrderNO;
                        tmpstockrcds.RefAreaID = stockrcds[i].RefAreaID;
                        tmpstockrcds.RefPosiID = stockrcds[i].RefPosiID;
                        tmpstockrcds.RefSeqNo = stockrcds[i].RefSeqNo;
                        tmpstockrcds.RefWHID = stockrcds[i].RefWHID;
                        tmpstockrcds.Remark = stockrcds[i].Remark;
                        tmpstockrcds.SaleOrderNO = stockrcds[i].SaleOrderNO;                        
                        tmpstockrcds.WHID = stockrcds[i].WHID;
                        tmpstockrcds.State = "C";
                        tmpstockrcds.OpeType = "IC";                        
                        tmpstockrcds.CreateDate = date;
                        tmpstockrcds.UpdateID = userID;                        
                        tmpstockrcds.UpdateDate = date;
                        erpsEntities.twhstockrecords.Add(tmpstockrcds);
                    }

                    //var stockrcdsGroupByWhIDPdtID = erpsEntities.twhstockrecords.Where(w => w.PurOrderNO.Equals(purOrderNo) && w.State != "C")
                    //    .GroupBy(a => new { a.WHID, a.PdtID }).Select(g => new { InvKey = g.Key, InvNum = g.Sum(itm => itm.Num) }).ToList();
                    var stockrcdsGroupByWhIDPdtID = (from a in erpsEntities.twhstockrecords
                                                    where a.PurOrderNO == purOrderNo && a.State != "C"
                                                    group a by new { a.WHID, a.PdtID } into gg
                                                    select new
                                                    {
                                                        WHID = gg.Key.WHID,
                                                        PdtID = gg.Key.PdtID,
                                                        InvNum = gg.Sum(c=>c.Num)
                                                    }).AsNoTracking().ToList();

                    for (int i = 0; i < stockrcdsGroupByWhIDPdtID.Count; i++)
                    {
                        whid = stockrcdsGroupByWhIDPdtID[i].WHID;
                        pdtid = stockrcdsGroupByWhIDPdtID[i].PdtID;
                        invNum = stockrcdsGroupByWhIDPdtID[i].InvNum.Value;

                        // 2, 修改库存表                        
                        var invrcds = erpsEntities.twhinventory.Where(w => w.WHID.Equals(whid) && w.PdtID.Equals(pdtid)).AsNoTracking().ToList();
                        if (invrcds.Count > 0)
                        {
                            var invrcd = invrcds.FirstOrDefault<twhinventory>();
                            invrcd.InvNum = invrcd.InvNum - invNum;
                            if (invrcd.InvNum < 0 && NegativeInventory != "Y")
                            {
                                ret = -9;
                                errPdtID = pdtid;
                                errWHID = whid;
                                break;
                            }
                            erpsEntities.Entry(invrcd).State = EntityState.Unchanged;
                            erpsEntities.Entry(invrcd).Property(x => x.InvNum).IsModified = true;
                        }
                    }
                    
                    if (ret == -9)
                    {
                        tran.Rollback();
                    }
                    else {      
                        ret = erpsEntities.SaveChanges();
                        tran.Commit();
                        if (ret > 0) {
                            ret = 1;
                        }
                    }
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

        public int DelPurOrderAtt(string purOrderNo, string AttFileName)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    var list = erpsEntities.tpurorderatt.Where(w=>w.PurOrderNO== purOrderNo && w.AttFileName==AttFileName).FirstOrDefault();
                    erpsEntities.tpurorderatt.Remove(list);

                    ret = erpsEntities.SaveChanges();
                    tran.Commit();                  
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

