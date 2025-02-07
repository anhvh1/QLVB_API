using Com.Gosol.QLVB.DAL.KeKhai;
using Com.Gosol.QLVB.Models.KeKhai;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.KeKhai
{
    //Edited by CHungNN 22/1/2021
    public interface IFileDinhKemLogBUS
    {
        public int Insert(FileDinhKemLogModel fileDinhKemLog, int ThaoTac);
    }
    public class FileDinhhKemLogBUS : IFileDinhKemLogBUS
    {
        private IFileDinhKemLogDAL _FileDinhKemLogDAL;
        public FileDinhhKemLogBUS(IFileDinhKemLogDAL FileDinhKemLogDAL)
        {
            this._FileDinhKemLogDAL = FileDinhKemLogDAL;
        }
        public int Insert(FileDinhKemLogModel fileDinhKemLog, int ThaoTac)
        {
            return _FileDinhKemLogDAL.Insert(fileDinhKemLog, ThaoTac);
        }
    }
}
