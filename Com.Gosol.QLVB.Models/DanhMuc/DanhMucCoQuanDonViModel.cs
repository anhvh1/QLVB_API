using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Gosol.QLVB.Models.DanhMuc
{
    public class DanhMucCoQuanDonViModel
    {
        [Required]
        public int CoQuanID { get; set; }

        [StringLength(255)]
        [Required]
        public string TenCoQuan { get; set; }
        public int? CoQuanChaID { get; set; }
        public int? CapID { get; set; }
        public int? ThamQuyenID { get; set; }
        public int? TinhID { get; set; }
        public int? HuyenID { get; set; }

        public int? ID { get; set; }
        public string Ten { get; set; }
        public int? Cap { get; set; }
        public int? ParentID { get; set; }

        public int? XaID { get; set; }
        public bool? CapUBND { get; set; }
        public bool? CapThanhTra { get; set; }
        public bool? CQCoHieuLuc { get; set; }
        public bool? SuDungPM { get; set; }
        public string MaCQ { get; set; }
        public bool? SuDungQuyTrinh { get; set; }
        [StringLength(50)]
        public string MappingCode { get; set; }
        public bool? IsDisable { get; set; }
        public int? TTChiaTachSapNhap { get; set; }
        public int? ChiaTachSapNhapDenCQID { get; set; }
        public bool? IsStatus { get; set; }
        public int? Highlight { get; set; }
        public int? CoQuanSuDungPhanMemID { get; set; }
        public int? WorkFlowID { get; set; }
        public List<DanhMucCoQuanDonViModel> Children { get; set; }
        public DanhMucCoQuanDonViModel() { }
        public DanhMucCoQuanDonViModel(int CoQuanID, string TenCoQuan, int CoQuanChaID, int CapID, int ThamQuyenID, int TinhID, int HuyenID, int XaID, bool CapUBND, bool CapThanhTra, bool CQCoHieuLuc, bool SuDungPM, string MaCQ, bool SuDungQuyTrinh, string MappingCode, bool IsDisable, int TTChiaTachSapNhap, int ChiaTachSapNhapDenCQID, bool IsStatus)
        {

            this.CoQuanID = CoQuanID;
            this.TenCoQuan = TenCoQuan;
            this.CoQuanChaID = CoQuanChaID;
            this.CapID = CapID;
            this.ThamQuyenID = ThamQuyenID;
            this.TinhID = TinhID;
            this.HuyenID = HuyenID;
            this.XaID = XaID;
            this.CapUBND = CapUBND;
            this.CapThanhTra = CapThanhTra;
            this.CQCoHieuLuc = CQCoHieuLuc;
            this.SuDungPM = SuDungPM;
            this.MaCQ = MaCQ;
            this.SuDungQuyTrinh = SuDungQuyTrinh;
            this.MappingCode = MappingCode;
            this.IsDisable = IsDisable;
            this.TTChiaTachSapNhap = TTChiaTachSapNhap;
            this.ChiaTachSapNhapDenCQID = ChiaTachSapNhapDenCQID;
            this.IsStatus = IsStatus;
        }
    }
    public class DanhMucCoQuanDonViModelPartial
    {

        public int ID { get; set; }
        public string Ten { get; set; }
        public int? MaCQ { get; set; }
        public int? Highlight { get; set; }
        public int? Cap { get; set; }
        public int? ParentID { get; set; }
        public List<DanhMucCoQuanDonViModelPartial> Children { get; set; }
        public List<DanhMucCoQuanDonViModelPartial> Parent { get; set; }
        public DanhMucCoQuanDonViModelPartial()
        {

        }
        public DanhMucCoQuanDonViModelPartial(int ID, string Ten, int MaCQ, int Highlight, int Cap, int ParentID, List<DanhMucCoQuanDonViModelPartial> Children, List<DanhMucCoQuanDonViModelPartial> Parent)
        {
            this.ID = ID;
            this.Ten = Ten;
            this.MaCQ = MaCQ;
            this.Highlight = Highlight;
            this.Cap = Cap;
            this.ParentID = ParentID;
            this.Children = Children;
            this.Parent = Parent;
        }
    }
    public class DanhMucCoQuanPar : DanhMucCoQuanDonViModel
    {
        public List<DanhMucCoQuanDonViModel> Children { get; set; }
        public List<DanhMucCoQuanPar> Parent { get; set; }
        public DanhMucCoQuanPar()
        {

        }
        public DanhMucCoQuanPar(List<DanhMucCoQuanDonViModel> Children, List<DanhMucCoQuanPar> Parent)
        {
           this.Children = Children;
            this.Parent = Parent;
        }


    }
    public class DanhMucCoQuanDonViPartialModel : DanhMucCoQuanDonViModel
    {
        public List<DanhMucCoQuanDonViPartialModel> Children { get; set; }
        public DanhMucCoQuanDonViPartialModel(
            int CoQuanID, string TenCoQuan, int CoQuanChaID,
            string MaCQ, List<DanhMucCoQuanDonViPartialModel> Children)
        {
            this.CoQuanID = CoQuanID;
            this.TenCoQuan = TenCoQuan;
            this.CoQuanChaID = CoQuanChaID;
            this.MaCQ = MaCQ;
            this.Children = Children;
        }
    }
    public class DanhMucCoQuanDonViPartialNew : DanhMucCoQuanDonViModel
    {
        public string DiaChi { get; set; }
        public string TenCoQuanCha { get; set; }
        public int CoQuanSuDungPhanMemID { get; set; }
        public DanhMucCoQuanDonViPartialNew()
        {

        }
        public DanhMucCoQuanDonViPartialNew(string DiaChi, string TenCoQuanCha)
        {
            this.DiaChi = DiaChi;
            this.TenCoQuanCha = TenCoQuanCha;
        }
    }

}
