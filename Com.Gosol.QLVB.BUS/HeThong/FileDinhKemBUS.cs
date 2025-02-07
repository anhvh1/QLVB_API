using Com.Gosol.QLVB.DAL.HeThong;
using Com.Gosol.QLVB.Models;
using Com.Gosol.QLVB.Models.HeThong;
using Com.Gosol.QLVB.Ultilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.BUS.HeThong
{
    public class FileDinhKemBUS
    {
        private FileDinhKemDAL _FileDinhKemDAL;
        public FileDinhKemBUS()
        {
            this._FileDinhKemDAL = new FileDinhKemDAL();
        }
        public BaseResultModel Delete(List<FileDinhKemModel> ListFileDinhKem)
        {
            return _FileDinhKemDAL.Delete(ListFileDinhKem);
        }

        public FileDinhKemModel GetByID(int FileDinhKemID, int FileType)
        {
            return _FileDinhKemDAL.GetByID(FileDinhKemID, FileType);
        }

        public List<FileDinhKemModel> GetByNgiepVuID(int NghiepVuID, int FileType)
        {
            return _FileDinhKemDAL.GetByNgiepVuID(NghiepVuID, FileType);
        }

        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel)
        {
            BaseResultModel baseResultModel = new BaseResultModel();
            var Result = _FileDinhKemDAL.Insert(FileDinhKemModel);
            var FileID = Utils.ConvertToInt32(Result.Data, 0);
            baseResultModel.Status = 1;
            baseResultModel.Data = FileID;
            return baseResultModel;
        }
    }
}
