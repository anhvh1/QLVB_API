using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.HeThong
{
    public class FileDinhKemModel
    {
        public int FileID { get; set; }
        public int? NghiepVuID { get; set; }
        public string TenFile { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int? NguoiCapNhat { get; set; }
        public int FileType { get; set; }
        public int? TrangThai { get; set; }
        public string FolderPath { get; set; }
        public string FileUrl { get; set; }
        public string NoiDung { get; set; }
        public bool? IsBaoMat { get; set; }
        public bool? IsMaHoa { get; set; }

        public FileDinhKemModel()
        {

        }
        public FileDinhKemModel(int FileID)
        {
            this.FileID = FileID;
        }
    }
}
