using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models.HeThong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public interface INguoiDungBUS
    {
        //NguoiDungModel GetInfoByLogin(string UserName,string Password);
        bool VerifyUser(string UserName, string Password, string Email, ref NguoiDungModel NguoiDung);
        public NguoiDungModel GetByTenNguoiDung(string TenNguoiDung);
        public int UpdateThoiGianlogin(NguoiDungModel nguoiDungModel, ref string message);
    }
    public class NguoiDungBUS : INguoiDungBUS
    {
        private INguoiDungDAL _NguoiDungDAL;
        public NguoiDungBUS(INguoiDungDAL NguoiDungDAL)
        {
            _NguoiDungDAL = NguoiDungDAL;
        }

        private NguoiDungModel GetInfoByLogin(string UserName, string Password)
        {
            return _NguoiDungDAL.GetInfoByLogin(UserName, Password);
        }
        private NguoiDungModel GetInfoByLoginCAS(string Mail)
        {
            return _NguoiDungDAL.GetInfoByLoginCAS(Mail);
        }

        public bool VerifyUser(string UserName, string Password, string Email, ref NguoiDungModel NguoiDung)
        {
            //if(Email != null && Email != "")
            //{
            //    NguoiDung = GetInfoByLoginCAS(Email);
            //}
            //else 
                NguoiDung = GetInfoByLogin(UserName, Password);
            if (NguoiDung != null && NguoiDung.TrangThai == 1)
            {
                return true;
            }
            return false;
        }

        public NguoiDungModel GetByTenNguoiDung(string TenNguoiDung)
        {
            return _NguoiDungDAL.GetByTenNguoiDung(TenNguoiDung);
        }

        public int UpdateThoiGianlogin(NguoiDungModel nguoiDungModel, ref string message)
        {
            return _NguoiDungDAL.UpdateThoiGianlogin(nguoiDungModel,ref message);
        }
    }
}
