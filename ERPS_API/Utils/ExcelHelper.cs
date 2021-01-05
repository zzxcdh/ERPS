using DAL_MySQL;
using ERPS_API.Controller;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ERPS_API.Utils
{
    public class ExcelHelper
    {
        private erpsEntities db = new erpsEntities();
        public void SaveInWarInfo(string purOrderNO, string warId, string purchaseDate, List<WarehouseReceipt> listWr,string no)
        {
            List<inwarrecord> inwarrecords = new List<inwarrecord>();
            List<inwarrecorddtl> inwarrecorddtls = new List<inwarrecorddtl>();
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    #region 获取详细信息
                    var list = from c in db.twhstockrecords
                               join p in db.mproduct on c.PdtID equals p.PdtID into pp
                               from p in pp.DefaultIfEmpty()
                               join o in db.tpurorderdtl on new { c.PurOrderNO, c.PdtID } equals new { o.PurOrderNO, o.PdtID } into oo
                               from o in oo.DefaultIfEmpty()
                               join s in db.msupplier on o.SupID equals s.SupID into ss
                               from s in ss.DefaultIfEmpty()
                               join w in db.mwarehouse on c.WHID equals w.WHID into ww
                               from w in ww.DefaultIfEmpty()
                               let p1 = new
                               {
                                   supID = s.SupID,
                                   supName = s.SupName,
                                   phone = s.Phone,
                                   contact = s.Contact,
                                   whName = w.WHName,
                                   purPrice = p.PurPrice,
                                   total = c.Num * p.PurPrice,
                                   updateID = c.UpdateID,
                                   updateDate = c.UpdateDate
                               }
                               where c.OpeType == "I" && c.PurOrderNO == purOrderNO && c.WHID == warId
                               select p1;
                    var stockrecord = list.OrderByDescending(u => u.updateDate).FirstOrDefault();
                    #endregion

                    int i = 1;

                    inwarrecord record = new inwarrecord();
                    record.InWarNo = no;
                    record.PurOrderNO = purOrderNO;
                    record.SupID = stockrecord.supID == null ? "" : stockrecord.supID;
                    record.SupName = stockrecord.supName == null ? "" : stockrecord.supName;
                    record.InWarDate = stockrecord.updateDate;
                    record.Contact = stockrecord.contact == null ? "" : stockrecord.contact;
                    record.Phone = stockrecord.phone == null ? "" : stockrecord.phone;
                    record.UpdateName = stockrecord.updateID == null ? "" : stockrecord.updateID;
                    record.WHName = stockrecord.whName == null ? "" : stockrecord.whName;

                    inwarrecords.Add(record);

                    foreach (var entity in listWr)
                    {
                        inwarrecorddtl inwarrecorddtl = new inwarrecorddtl();
                        inwarrecorddtl.InWarNo = no;
                        inwarrecorddtl.PdtType = entity.pdtType;
                        inwarrecorddtl.PdtID = entity.pdtID;
                        inwarrecorddtl.SeqNo = i;
                        inwarrecorddtl.Spec = entity.spec;
                        inwarrecorddtl.MakeIn = entity.makeIn;
                        inwarrecorddtl.Unit = entity.unit;
                        inwarrecorddtl.PurPrice = entity.purPrice;
                        inwarrecorddtl.Num = entity.num;

                        inwarrecorddtls.Add(inwarrecorddtl);

                        i++;
                    }

                    db.inwarrecord.AddRange(inwarrecords);
                    db.inwarrecorddtl.AddRange(inwarrecorddtls);

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }




        public void SaveOutWarInfo(string saleOrderNo, string warId, string deliveryDate, List<OutboundOrder> listOdo, string no)
        {
            List<outwarrecord> outwarrecords = new List<outwarrecord>();
            List<outwarrecorddtl> outwarrecorddtls = new List<outwarrecorddtl>();
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    #region 获取详细信息
                    var list = from c in db.twhstockrecords
                               join p in db.mproduct on c.PdtID equals p.PdtID into pp
                               from p in pp.DefaultIfEmpty()
                               join o in db.tsaleorderdtl on new { c.SaleOrderNO, c.PdtID } equals new { o.SaleOrderNO, o.PdtID } into oo
                               from o in oo.DefaultIfEmpty()
                               join m in db.mcustomerprodprice on new { o.CusID, o.PdtID, o.Model } equals new { m.CusID, m.PdtID, m.Model } into mm
                               from m in mm.DefaultIfEmpty()
                               join s in db.mcustomer on o.CusID equals s.CusID into ss
                               from s in ss.DefaultIfEmpty()
                               join w in db.mwarehouse on c.WHID equals w.WHID into ww
                               from w in ww.DefaultIfEmpty()
                               let p1 = new
                               {
                                   cusName = s.CusName,
                                   saleOrderNO = o.SaleOrderNO,
                                   whName = w.WHName,
                                   updateID = c.UpdateID,
                                   updateDate = c.UpdateDate,
                                   contact = s.Contact,
                                   phone = s.Phone,
                                   address = s.Address
                               }
                               where c.OpeType == "O" && c.SaleOrderNO == saleOrderNo && c.WHID == warId
                               select p1;
                    var stockrecord = list.OrderByDescending(u => u.updateDate).FirstOrDefault();
                    #endregion

                    int i = 1;

                    outwarrecord record = new outwarrecord();
                    record.OutWarNo = no;
                    record.SaleOrderNO = saleOrderNo;
                    record.CusName = stockrecord.cusName == null ? "" : stockrecord.cusName;
                    record.DeliveryDate = stockrecord.updateDate;
                    record.Contact = stockrecord.contact == null ? "" : stockrecord.contact;
                    record.Phone = stockrecord.phone == null ? "" : stockrecord.phone;
                    record.UpdateName = stockrecord.updateID == null ? "" : stockrecord.updateID;
                    record.WHName = stockrecord.whName == null ? "" : stockrecord.whName;
                    record.Address = stockrecord.address == null ? "" : stockrecord.address;

                    outwarrecords.Add(record);

                    foreach (var entity in listOdo)
                    {
                        outwarrecorddtl outwarrecorddtl = new outwarrecorddtl();
                        outwarrecorddtl.OutWarNo = no;
                        outwarrecorddtl.PdtId = entity.pdtID;
                        outwarrecorddtl.SeqNo = i;
                        outwarrecorddtl.PdtName = entity.pdtName;
                        outwarrecorddtl.Model = entity.model;
                        outwarrecorddtl.MakeIn = entity.makeIn;
                        outwarrecorddtl.MgrInfo = entity.mgrInfo;
                        outwarrecorddtl.Spec = entity.spec;
                        outwarrecorddtl.Unit = entity.unit;
                        outwarrecorddtl.OrderNum = entity.orderNum;
                        outwarrecorddtl.Num = entity.num;
                        outwarrecorddtl.Remark = entity.remark;

                        outwarrecorddtls.Add(outwarrecorddtl);

                        i++;
                    }

                    db.outwarrecord.AddRange(outwarrecords);
                    db.outwarrecorddtl.AddRange(outwarrecorddtls);

                    db.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }
    }
}