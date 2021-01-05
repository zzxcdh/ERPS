using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DAL_MySQL;
using Z.EntityFramework.Plus;
using Model;
using System.Data.Entity.Infrastructure;

namespace BLL
{
    public class SaleForcast
    {
        private erpsEntities erpsEntities;
        public SaleForcast()
        {
            erpsEntities = new erpsEntities();
        }

        public List<v_saleforcast> GetSaleForcast(string CusID, string CusName, string SaleFocaNO, string State, string SDate, string EDate, int pagesize, int currentPage, string orderby, string order, out int total)
        {
            try
            {
                List<v_saleforcast> list = new List<v_saleforcast>();
                var tem = from f in erpsEntities.v_saleforcast.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(SaleFocaNO))
                {
                    tem = tem.Where(w => w.SaleFocaNO.Contains(SaleFocaNO));
                }
                if (!string.IsNullOrEmpty(CusID))
                {
                    tem = tem.Where(w => w.CusID.Contains(CusID));
                }
                if (!string.IsNullOrEmpty(CusName))
                {
                    tem = tem.Where(w => w.CusName.Contains(CusName));
                }
                if (!string.IsNullOrEmpty(State))
                {
                    tem = tem.Where(w => w.State.Contains(State));
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
        /// 销售预测-查询
        /// </summary>
        /// <param name="CusID"></param>
        /// <param name="CusName"></param>
        /// <param name="PdtID"></param>
        /// <param name="SaleFocaNO"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <param name="pagesize"></param>
        /// <param name="currentPage"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<v_saleforcastdtl> GetSaleForcastDtl(string CusID, string CusName, string PdtID, string PdtName, string SaleFocaNO, string SDate, string EDate, int pagesize, int currentPage, out int total)
        {
            try
            {
                List<v_saleforcastdtl> list = new List<v_saleforcastdtl>();
                var tem = from f in erpsEntities.v_saleforcastdtl.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(SaleFocaNO))
                {
                    tem = tem.Where(w => w.SaleFocaNO.Contains(SaleFocaNO));
                }
                if (!string.IsNullOrEmpty(CusID))
                {
                    tem = tem.Where(w => w.CusID.Contains(CusID));
                }
                if (!string.IsNullOrEmpty(CusName))
                {
                    tem = tem.Where(w => w.CusName.Contains(CusName));
                }
                if (!string.IsNullOrEmpty(PdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(PdtID));
                }
                if (!string.IsNullOrEmpty(PdtName))
                {
                    tem = tem.Where(w => w.PdtName.Contains(PdtName));
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
                tem = tem.OrderByDescending(o => o.CreateDate).ThenBy(o => o.SeqNo);

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

        public List<v_saleorder> GetSaleOrder(string CusID, string CusName, string SaleOrderNO, string State, string SDate, string EDate, string CreateID, int pagesize, int currentPage, string orderby, string order, out int total)
        {
            try
            {
                List<v_saleorder> list = new List<v_saleorder>();
                var tem = from f in erpsEntities.v_saleorder.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(SaleOrderNO))
                {
                    tem = tem.Where(w => w.SaleOrderNO.Contains(SaleOrderNO));
                }
                if (!string.IsNullOrEmpty(CusID))
                {
                    tem = tem.Where(w => w.CusID.Contains(CusID));
                }
                if (!string.IsNullOrEmpty(CusName))
                {
                    tem = tem.Where(w => w.CusName.Contains(CusName));
                }
                if (!string.IsNullOrEmpty(State))
                {
                    tem = tem.Where(w => w.State.Contains(State));
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
                if (!string.IsNullOrEmpty(CreateID))
                {
                    tem = tem.Where(w => w.CreateID.Contains(CreateID));
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

        public List<v_saleorderdtl> GetSaleOrderDtl(string CusID, string CusName, string PdtID, string PdtName, string SaleOrderNO, string SDate, string EDate, string CreateID, int pagesize, int currentPage, out int total)
        {
            try
            {
                List<v_saleorderdtl> list = new List<v_saleorderdtl>();
                var tem = from f in erpsEntities.v_saleorderdtl.AsNoTracking()
                          select f;
                if (!string.IsNullOrEmpty(SaleOrderNO))
                {
                    tem = tem.Where(w => w.SaleOrderNO.Contains(SaleOrderNO));
                }
                if (!string.IsNullOrEmpty(CusID))
                {
                    tem = tem.Where(w => w.CusID.Contains(CusID));
                }
                if (!string.IsNullOrEmpty(CusName))
                {
                    tem = tem.Where(w => w.CusName.Contains(CusName));
                }
                if (!string.IsNullOrEmpty(PdtID))
                {
                    tem = tem.Where(w => w.PdtID.Contains(PdtID));
                }
                if (!string.IsNullOrEmpty(PdtName))
                {
                    tem = tem.Where(w => w.PdtName.Contains(PdtName));
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
                if (!string.IsNullOrEmpty(CreateID))
                {
                    tem = tem.Where(w => w.DtlCreateID.Contains(CreateID));
                }
                tem = tem.OrderByDescending(o => o.CreateDate).ThenBy(o => o.SeqNo);

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
        /// 根据预测编号获取预测明细
        /// </summary>
        /// <param name="SaleFocaNO"></param>
        /// <returns></returns>
        public List<v_saleforcastdtl> GetSaleForcastDtl(string SaleFocaNO)
        {
            try
            {
                List<v_saleforcastdtl> list = new List<v_saleforcastdtl>();
                var tem = from f in erpsEntities.v_saleforcastdtl.AsNoTracking()
                          where f.SaleFocaNO == SaleFocaNO
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
        /// 根据销售预测号获取预测明细及附件
        /// </summary>
        /// <param name="SaleFocaNO"></param>
        /// <param name="tsaleforcastatts"></param>
        /// <returns></returns>
        public List<v_saleforcastdtl> GetSaleForcastDtlAndAtt(string SaleFocaNO, out List<tsaleforcastatt> tsaleforcastatts)
        {
            try
            {
                List<v_saleforcastdtl> list = new List<v_saleforcastdtl>();
                list = erpsEntities.v_saleforcastdtl.AsNoTracking().Where(w => w.SaleFocaNO == SaleFocaNO).ToList();

                tsaleforcastatts = erpsEntities.tsaleforcastatt.Where(w => w.SaleFocaNO == SaleFocaNO).ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 根据订单号获取订单明细及附件
        /// </summary>
        /// <param name="SaleOrderNO"></param>
        /// <param name="tsaleorderatts"></param>
        /// <returns></returns>
        public List<v_saleorderdtl> GetSaleOrderDtlAndAtt(string SaleOrderNO, out List<tsaleorderatt> tsaleorderatts)
        {
            try
            {
                List<v_saleorderdtl> list = new List<v_saleorderdtl>();
                list = erpsEntities.v_saleorderdtl.AsNoTracking().Where(w => w.SaleOrderNO == SaleOrderNO).ToList();

                tsaleorderatts = erpsEntities.tsaleorderatt.Where(w => w.SaleOrderNO == SaleOrderNO).ToList();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 生成预测代码
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string SetSaleForcastNo(string date)
        {
            var SaleForcastNo = "";
            DateTime dateTime = Convert.ToDateTime(date);
            var isExists = erpsEntities.tsaleforcast.Any(a => a.CreDate == dateTime);
            if (isExists)
            {
                string maxid = erpsEntities.tsaleforcast.Where(w => w.CreDate == dateTime).OrderByDescending(o => o.CreateDate).FirstOrDefault().SaleFocaNO;
                var arr = maxid.Split('-');
                int seq = int.Parse(arr[2]);
                //int seq = erpsEntities.tsaleforcast.Where(w => w.CreDate == dateTime).Count();
                seq++;
                SaleForcastNo = "YC-" + date.Replace("-", "") + "-" + seq.ToString("000");
            }
            else
            {
                SaleForcastNo = "YC-" + date.Replace("-", "") + "-001";
            }
            return SaleForcastNo;
        }

        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string SetSaleOrderNo(string date)
        {
            var SaleOrderNo = "";
            DateTime dateTime = Convert.ToDateTime(date);
            var isExists = erpsEntities.tsaleorder.Any(a => a.CreDate == dateTime);
            if (isExists)
            {
                string maxid = erpsEntities.tsaleorder.Where(w => w.CreDate == dateTime).OrderByDescending(o => o.CreateDate).FirstOrDefault().SaleOrderNO;
                var arr = maxid.Split('-');
                int seq = int.Parse(arr[2]);
                //int seq = erpsEntities.tsaleorder.Where(w => w.CreDate == dateTime).Count();
                seq++;
                SaleOrderNo = "XS-" + date.Replace("-", "") + "-" + seq.ToString("000");
            }
            else
            {
                SaleOrderNo = "XS-" + date.Replace("-", "") + "-001";
            }
            return SaleOrderNo;
        }

        /// <summary>
        /// 销售预测-新增
        /// </summary>
        /// <param name="tsaleforcast"></param>
        /// <param name="tsaleforcastdtls"></param>
        /// <param name="tsaleforcastatts"></param>
        /// <returns></returns>
        public int AddSaleForcastDtl(tsaleforcast tsaleforcast, List<tsaleforcastdtl> tsaleforcastdtls, List<tsaleforcastatt> tsaleforcastatts)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    erpsEntities.tsaleforcast.Add(tsaleforcast);
                    erpsEntities.tsaleforcastdtl.AddRange(tsaleforcastdtls);
                    erpsEntities.tsaleforcastatt.AddRange(tsaleforcastatts);

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

        public int AddSaleOrderDtl(tsaleorder tsaleorder, List<tsaleorderdtl> tsaleorderdtls, List<tsaleorderatt> tsaleorderatts)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    erpsEntities.tsaleorder.Add(tsaleorder);
                    erpsEntities.tsaleorderdtl.AddRange(tsaleorderdtls);
                    erpsEntities.tsaleorderatt.AddRange(tsaleorderatts);
                    if (!string.IsNullOrEmpty(tsaleorderdtls[0].SaleFocaNO))
                    {
                        //冲销对应销售预测信息
                        var newOrderDtl = (tsaleorderdtls.GroupBy(g => new { g.SeqNo, g.PdtID })
                                          .Select(s => new
                                          {
                                              SaleFocaNO = s.Max(m => m.SaleFocaNO),
                                              SeqNo = s.Max(m => m.SeqNo),
                                              CusID = s.Max(m => m.CusID),
                                              PdtID = s.Max(m => m.PdtID),
                                              OrderNum = s.Sum(m => m.OrderNum)
                                          })).ToList();
                        bool isF = true;
                        for (int i = 0; i < newOrderDtl.Count; i++)
                        {
                            var SaleFocaNO = newOrderDtl[i].SaleFocaNO;
                            var SeqNo = newOrderDtl[i].SeqNo;
                            var obj = erpsEntities.tsaleforcastdtl
                                .Where(w => w.SaleFocaNO == SaleFocaNO && w.SeqNo == SeqNo).FirstOrDefault();

                            erpsEntities.tsaleforcastdtl.Attach(obj);

                            var num = (obj.LftNum - newOrderDtl[i].OrderNum) < 0 ? 0 : obj.LftNum - newOrderDtl[i].OrderNum;
                            obj.LftNum = num;
                            if (num == 0)
                            {
                                obj.State = "F";
                                erpsEntities.Entry(obj).Property(xx => xx.State).IsModified = true;
                            }
                            else
                            {
                                isF = false;
                            }

                            erpsEntities.Entry(obj).Property(xx => xx.LftNum).IsModified = true;
                        }
                        if (isF)
                        {
                            var SaleFocaNO = newOrderDtl[0].SaleFocaNO;
                            var saleforcast = erpsEntities.tsaleforcast
                                .Where(w => w.SaleFocaNO == SaleFocaNO).FirstOrDefault();

                            erpsEntities.tsaleforcast.Attach(saleforcast);

                            saleforcast.State = "F";
                            erpsEntities.Entry(saleforcast).Property(xx => xx.State).IsModified = true;
                        }
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

        public int EditSaleForcastDtl(string SaleForcastNo, List<tsaleforcastdtl> tsaleforcastdtls, List<tsaleforcastatt> tsaleforcastatts)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {

                    var vm = erpsEntities.tsaleforcastdtl.Where(m => m.SaleFocaNO == SaleForcastNo);
                    vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                    erpsEntities.tsaleforcastdtl.RemoveRange(vm);

                    erpsEntities.tsaleforcastdtl.AddRange(tsaleforcastdtls);
                    erpsEntities.tsaleforcastatt.AddRange(tsaleforcastatts);

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

        public int EditSaleOrderDtl(string SaleOrderNO, tsaleorder tsaleorder, List<tsaleorderdtl> tsaleorderdtls, List<tsaleorderatt> tsaleorderatts)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    var ms = erpsEntities.tsaleorder.Where(w => w.SaleOrderNO == SaleOrderNO).FirstOrDefault();
                    erpsEntities.Entry(ms).State = EntityState.Deleted;
                    erpsEntities.tsaleorder.Remove(ms);

                    var vm = erpsEntities.tsaleorderdtl.Where(m => m.SaleOrderNO == SaleOrderNO).ToList();
                    vm.ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                    erpsEntities.tsaleorderdtl.RemoveRange(vm);

                    ret += erpsEntities.SaveChanges();

                    erpsEntities.tsaleorder.Add(tsaleorder);
                    erpsEntities.tsaleorderdtl.AddRange(tsaleorderdtls);
                    erpsEntities.tsaleorderatt.AddRange(tsaleorderatts);
                    ret += erpsEntities.SaveChanges();

                    if (!string.IsNullOrEmpty(tsaleorderdtls[0].SaleFocaNO))
                    {
                        //冲销对应销售预测信息
                        var newOrderDtl = (tsaleorderdtls.GroupBy(g => new { g.SeqNo, g.PdtID })
                                          .Select(s => new
                                          {
                                              SaleFocaNO = s.Max(m => m.SaleFocaNO),
                                              SeqNo = s.Max(m => m.SeqNo),
                                              CusID = s.Max(m => m.CusID),
                                              PdtID = s.Max(m => m.PdtID),
                                              OrderNum = s.Sum(m => m.OrderNum)
                                          })).ToList();
                        bool isF = true;
                        for (int i = 0; i < newOrderDtl.Count; i++)
                        {
                            var SaleFocaNO = newOrderDtl[i].SaleFocaNO;
                            var SeqNo = newOrderDtl[i].SeqNo;
                            var obj = erpsEntities.tsaleforcastdtl
                                .Where(w => w.SaleFocaNO == SaleFocaNO && w.SeqNo == SeqNo).FirstOrDefault();

                            erpsEntities.tsaleforcastdtl.Attach(obj);

                            var num = (obj.FocaNum - newOrderDtl[i].OrderNum) < 0 ? 0 : obj.FocaNum - newOrderDtl[i].OrderNum;
                            obj.LftNum = num;
                            if (num == 0)
                            {
                                obj.State = "F";
                                erpsEntities.Entry(obj).Property(xx => xx.State).IsModified = true;
                            }
                            else
                            {
                                obj.State = "N";
                                erpsEntities.Entry(obj).Property(xx => xx.State).IsModified = true;
                                isF = false;
                            }

                            erpsEntities.Entry(obj).Property(xx => xx.LftNum).IsModified = true;
                        }
                        if (isF)
                        {
                            var SaleFocaNO = newOrderDtl[0].SaleFocaNO;
                            var saleforcast = erpsEntities.tsaleforcast
                                .Where(w => w.SaleFocaNO == SaleFocaNO).FirstOrDefault();

                            erpsEntities.tsaleforcast.Attach(saleforcast);

                            saleforcast.State = "F";
                            erpsEntities.Entry(saleforcast).Property(xx => xx.State).IsModified = true;
                        }
                        else {
                            var SaleFocaNO = newOrderDtl[0].SaleFocaNO;
                            var saleforcast = erpsEntities.tsaleforcast
                                .Where(w => w.SaleFocaNO == SaleFocaNO).FirstOrDefault();

                            erpsEntities.tsaleforcast.Attach(saleforcast);

                            saleforcast.State = "N";
                            erpsEntities.Entry(saleforcast).Property(xx => xx.State).IsModified = true;
                        }
                    }

                    ret += erpsEntities.SaveChanges();
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

        public int DelSaleForcast(List<DelList> delLists)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i].id;
                        int seqNo = delLists[i].seqNo;

                        var vm = erpsEntities.tsaleforcastdtl.Where(m => m.SaleFocaNO == id && m.SeqNo == seqNo);
                        vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                        erpsEntities.tsaleforcastdtl.RemoveRange(vm);
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

        public int DelSaleForcastData(List<DelList> delLists)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i].id;

                        var vm = erpsEntities.tsaleforcastdtl.Where(m => m.SaleFocaNO == id);
                        vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                        var vm2 = erpsEntities.tsaleforcast.Where(m => m.SaleFocaNO == id).FirstOrDefault();
                        erpsEntities.tsaleforcast.Remove(vm2);
                        erpsEntities.tsaleforcastdtl.RemoveRange(vm);
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

        public int DelSaleOrder(List<DelListDelDate> delLists)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i].id;
                        int seqNo = delLists[i].seqNo;
                        DateTime delDate = delLists[i].delDate;

                        var vm = erpsEntities.tsaleorderdtl.Where(m => m.SaleOrderNO == id && m.SeqNo == seqNo && m.DelDate == delDate);
                        vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                        erpsEntities.tsaleorderdtl.RemoveRange(vm);
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

        public int DelSaleOrderData(List<DelList> delLists)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < delLists.Count; i++)
                    {
                        string id = delLists[i].id;

                        var vm = erpsEntities.tsaleorderdtl.Where(m => m.SaleOrderNO == id);
                        vm.ToList().ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                        var vm2 = erpsEntities.tsaleorder.Where(m => m.SaleOrderNO == id).FirstOrDefault();
                        erpsEntities.tsaleorder.Remove(vm2);
                        erpsEntities.tsaleorderdtl.RemoveRange(vm);
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

        public int DelSaleForcastAtt(string SaleFocaNO, string AttFileName)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    var list = erpsEntities.tsaleforcastatt.Where(w => w.SaleFocaNO == SaleFocaNO && w.AttFileName == AttFileName).FirstOrDefault();
                    erpsEntities.tsaleforcastatt.Remove(list);

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

        /// <summary>
        /// 销售订单- 内示取消: 销售订单删除, 相关内示数据加回
        /// </summary>
        /// <param name="saleOrderNo"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int CancelSaleForcast(string saleOrderNo, string userID)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    // 修改销售预测表
                    string SaleFocaNO = "";
                    int SeqNo = 0;
                    float OrderNum = 0;
                    var saleodrdtls = erpsEntities.tsaleorderdtl.AsNoTracking().Where(x => x.SaleOrderNO.Equals(saleOrderNo)).ToList();
                    for (int i = 0; i < saleodrdtls.Count; i++)
                    {
                        SaleFocaNO = saleodrdtls[i].SaleFocaNO;
                        SeqNo = saleodrdtls[i].SeqNo;
                        OrderNum = saleodrdtls[i].OrderNum.Value;
                        var saleforcdt = erpsEntities.tsaleforcastdtl.AsNoTracking().Where(w => w.SaleFocaNO == SaleFocaNO && w.SeqNo == SeqNo).FirstOrDefault();
                        saleforcdt.LftNum += OrderNum;
                        saleforcdt.UpdateID = userID;
                        saleforcdt.UpdateDate = DateTime.Now;
                        erpsEntities.Entry(saleforcdt).State = EntityState.Unchanged;
                        erpsEntities.Entry(saleforcdt).Property(x => x.LftNum).IsModified = true;
                        erpsEntities.Entry(saleforcdt).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(saleforcdt).Property(x => x.UpdateDate).IsModified = true;
                        if (saleforcdt.LftNum == saleforcdt.FocaNum)
                        {
                            saleforcdt.State = "N";
                            erpsEntities.Entry(saleforcdt).Property(x => x.State).IsModified = true;
                        }

                    }
                    // 修改预测单头表
                    var saleforcast = erpsEntities.tsaleforcast.AsNoTracking().Where(w => w.SaleFocaNO == SaleFocaNO).FirstOrDefault();
                    saleforcast.State = "N";
                    saleforcast.UpdateID = userID;
                    saleforcast.UpdateDate = DateTime.Now;
                    erpsEntities.Entry(saleforcast).State = EntityState.Unchanged;
                    erpsEntities.Entry(saleforcast).Property(x => x.State).IsModified = true;
                    erpsEntities.Entry(saleforcast).Property(x => x.UpdateID).IsModified = true;
                    erpsEntities.Entry(saleforcast).Property(x => x.UpdateDate).IsModified = true;

                    // 删除销售订单
                    saleodrdtls.ForEach(t => erpsEntities.Entry(t).State = EntityState.Deleted);
                    erpsEntities.tsaleorderdtl.RemoveRange(saleodrdtls);
                    
                    var saleord = erpsEntities.tsaleorder.AsNoTracking().Where(w => w.SaleOrderNO == saleOrderNo).FirstOrDefault();
                    erpsEntities.Entry(saleord).State = EntityState.Deleted;

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
        /// 取消销售订单出库
        /// 销售订单数量状态恢复，出入库记录标记为”C,取消“，没有的话加个列， 库存数量恢复。
        /// </summary>
        /// <param name="saleOrderNo"></param>
        /// <returns></returns>
        public int CancelPurOrderStockIn(string saleOrderNo, string userID)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    // 1 修改订单明细表内容
                    var saleOrder = erpsEntities.tsaleorder.AsNoTracking().Where(x => x.SaleOrderNO.Equals(saleOrderNo)).ToList();
                    if (saleOrder.Count < 1) return ret;
                    var salodr = saleOrder.FirstOrDefault<tsaleorder>();
                    salodr.State = "N";
                    salodr.UpdateID = userID;
                    salodr.UpdateDate = DateTime.Now;
                    erpsEntities.Entry(salodr).State = EntityState.Unchanged;
                    erpsEntities.Entry(salodr).Property(x => x.State).IsModified = true;
                    erpsEntities.Entry(salodr).Property(x => x.UpdateID).IsModified = true;
                    erpsEntities.Entry(salodr).Property(x => x.UpdateDate).IsModified = true;

                    // 修改订单明细内容
                    var saleodrdtls = erpsEntities.tsaleorderdtl.AsNoTracking().Where(x => x.SaleOrderNO.Equals(saleOrderNo)).ToList();
                    for (int i = 0; i < saleodrdtls.Count; i++)
                    {
                        saleodrdtls[i].DelNum = 0;
                        saleodrdtls[i].LftNum = saleodrdtls[i].OrderNum;
                        saleodrdtls[i].State = "N";
                        saleodrdtls[i].UpdateID = userID;
                        saleodrdtls[i].UpdateDate = DateTime.Now;
                        erpsEntities.Entry(saleodrdtls[i]).State = EntityState.Unchanged;
                        erpsEntities.Entry(saleodrdtls[i]).Property(x => x.DelNum).IsModified = true;
                        erpsEntities.Entry(saleodrdtls[i]).Property(x => x.LftNum).IsModified = true;
                        erpsEntities.Entry(saleodrdtls[i]).Property(x => x.State).IsModified = true;
                        erpsEntities.Entry(saleodrdtls[i]).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(saleodrdtls[i]).Property(x => x.UpdateDate).IsModified = true;
                    }

                    string whid = "";
                    string pdtid = "";
                    float invNum = 0;
                    var date = DateTime.Now;
                    var stockrcds = erpsEntities.twhstockrecords.AsNoTracking().Where(w => w.SaleOrderNO.Equals(saleOrderNo) && w.State != "C").ToList();
                    for (int i = 0; i < stockrcds.Count; i++)
                    {
                        // 1 修改出入库记录表
                        stockrcds[i].State = "C"; //状态                        
                        //stockrcds[i].UpdateID = userID;
                        stockrcds[i].UpdateDate = date;
                        erpsEntities.Entry(stockrcds[i]).State = EntityState.Unchanged;
                        erpsEntities.Entry(stockrcds[i]).Property(x => x.State).IsModified = true;
                        //erpsEntities.Entry(stockrcds[i]).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(stockrcds[i]).Property(x => x.UpdateDate).IsModified = true;

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
                        tmpstockrcds.OpeType = "IO";
                        tmpstockrcds.CreateDate = date;
                        tmpstockrcds.UpdateID = userID;
                        tmpstockrcds.UpdateDate = date;
                        erpsEntities.twhstockrecords.Add(tmpstockrcds);
                    }
                    var invrcdsGroups = stockrcds.GroupBy(g => new { PdtID = g.PdtID, WHID = g.WHID })
                        .Select(s => new
                        {
                            PdtID = s.Key.PdtID,
                            WHID = s.Key.WHID,
                            InvNum = s.Sum(m => m.Num)
                        }).ToList();
                    for (int i = 0; i < invrcdsGroups.Count; i++)
                    {
                        whid = invrcdsGroups[i].WHID;
                        pdtid = invrcdsGroups[i].PdtID;
                        invNum = invrcdsGroups[i].InvNum.Value;
                        // 2, 修改库存表
                        var invrcds = erpsEntities.twhinventory.AsNoTracking().Where(w => w.WHID.Equals(whid) && w.PdtID.Equals(pdtid)).ToList();
                        if (invrcds.Count > 0)
                        {
                            var invrcd = invrcds.FirstOrDefault<twhinventory>();
                            invrcd.InvNum = invrcd.InvNum + invNum;
                            erpsEntities.Entry(invrcd).State = EntityState.Unchanged;
                            erpsEntities.Entry(invrcd).Property(x => x.InvNum).IsModified = true;
                        }
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

        public int ImportList(List<mcustomerprodprice> lstpdt, out int addNum, out int updNum, out int cfNum)
        {
            int ret = 0;
            using (var tran = erpsEntities.Database.BeginTransaction())
            {
                try
                {
                    addNum = 0;
                    updNum = 0;
                    cfNum = 0;
                    cfNum = lstpdt.GroupBy(x => new { x.CusID, x.PdtID, x.Model }).Where(x => x.Count() > 1).Count();

                    //去除重复的数据
                    lstpdt = lstpdt.Where((x, i) => lstpdt.FindIndex(n => n.CusID == x.CusID && n.PdtID == x.PdtID && n.Model == x.Model) == i).ToList();

                    var all = erpsEntities.mcustomerprodprice.AsNoTracking().ToList();

                    //新增                    
                    var addlist = new List<mcustomerprodprice>();
                    var updlist = new List<mcustomerprodprice>();

                    for (int a = 0; a < lstpdt.Count; a++)
                    {
                        bool tmb = true;
                        for (int i = 0; i < all.Count; i++)
                        {
                            if (lstpdt[a].CusID == all[i].CusID && lstpdt[a].PdtID == all[i].PdtID && lstpdt[a].Model == all[i].Model)
                            {
                                tmb = false;
                            }
                        }
                        if (tmb)
                        {
                            addlist.Add(lstpdt[a]);
                        }
                    }

                    for (int i = 0; i < all.Count; i++)
                    {
                        for (int a = 0; a < lstpdt.Count; a++)
                        {
                            if (lstpdt[a].CusID == all[i].CusID && lstpdt[a].PdtID == all[i].PdtID && lstpdt[a].Model == all[i].Model)
                            {
                                updlist.Add(lstpdt[a]);
                            }
                        }
                    }

                    erpsEntities.mcustomerprodprice.AddRange(addlist);

                    //修改
                    // var updlist = lstpdt.Where(w => allID.Contains(w.SupID)).ToList();
                    for (int i = 0; i < updlist.Count; i++)
                    {
                        mcustomerprodprice mcustomerprodprice = new mcustomerprodprice();
                        mcustomerprodprice = updlist[i];
                        erpsEntities.mcustomerprodprice.Attach(mcustomerprodprice);

                        erpsEntities.Entry(mcustomerprodprice).State = EntityState.Unchanged;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.CusID).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.PdtID).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.Model).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.MakeIn).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.SalePrice).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.UpdateID).IsModified = true;
                        erpsEntities.Entry(mcustomerprodprice).Property(x => x.UpdateDate).IsModified = true;
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
