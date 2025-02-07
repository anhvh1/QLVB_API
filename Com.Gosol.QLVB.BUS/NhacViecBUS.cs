using Com.Gosol.QLVB.DAL;
using Com.Gosol.QLVB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS
{
    public interface INhacViecBUS
    {
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID);
       
        public List<ThongBaoModel> GetAllThongBao();
        public BaseResultModel Edit_ThongBao(ThongBaoModel ThongBaoModel);
        public BaseResultModel SendMailAuto();
        public BaseResultModel SendMail(List<string> MailTo, string TieuDe, string NoiDung);
    }
    public class NhacViecBUS : INhacViecBUS
    {
        private INhacViecDAL _INhacViecDAL;
        public NhacViecBUS(INhacViecDAL nhacViecDAL)
        {
            this._INhacViecDAL = nhacViecDAL;
        }
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID)
        {
            return _INhacViecDAL.GetViecLam(NguoiDungID,CanBoID, CoQuanID);
        }

       
        public BaseResultModel Edit_ThongBao(ThongBaoModel ThongBaoModel)
        {
            return _INhacViecDAL.Edit_ThongBao(ThongBaoModel);
        }
        public List<ThongBaoModel> GetAllThongBao()
        {
            return _INhacViecDAL.GetAllThongBao();
        }
        public BaseResultModel SendMailAuto()
        {
            return _INhacViecDAL.SendMailAuto();
        }

        public BaseResultModel SendMail(List<string> MailTo, string TieuDe, string NoiDung)
        {
            return _INhacViecDAL.SendMail(MailTo, TieuDe, NoiDung);
        }
    }
}
