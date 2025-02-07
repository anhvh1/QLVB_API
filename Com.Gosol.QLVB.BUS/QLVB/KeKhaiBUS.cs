using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    public interface IKeKhaiBUS
    {
        //edited by ChungNN 22/1/2021
        public KeKhaiModel GetByID(int KeKhaiID);
    }
    public class KeKhaiBUS : IKeKhaiBUS
    {
        private IKeKhaiDAL _KeKhaiDAL;
        public KeKhaiBUS(IKeKhaiDAL keKhaiDAL)
        {
            this._KeKhaiDAL = keKhaiDAL;
        }

        public KeKhaiModel GetByID(int KeKhaiID)
        {
            return _KeKhaiDAL.GetByID(KeKhaiID);
        }
    }
}
