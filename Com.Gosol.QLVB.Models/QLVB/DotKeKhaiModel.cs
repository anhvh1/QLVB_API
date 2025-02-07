using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.QLVB.Models.KeKhai
{
    public class DotKeKhaiModel
    {
        public int DotKeKhaiID { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int? ApDungCho { get; set; }
        public bool? TrangThai { get; set; }
        public int? NamKeKhai { get; set; }
        public int LoaiDotKeKhai { get; set; }
        public string TenDotKeKhai { get; set; }
        public string MoTaDotKeKhai { get; set; }
        public int CapQuanLy { get; set; }
        public bool QuyenChinhSua { get; set; }
        public int? CoQuanTao { get; set; }
        public DotKeKhaiModel()
        {

        }
        public DotKeKhaiModel(int DotKeKhaiID, DateTime TuNgay, DateTime DenNgay, bool TrangThai, int? ApDungCho, int? NamKeKhai, int LoaiDotKeKhai, string TenDotKeKhai, int CapQuanLy, bool QuyenChinhSua)
        {
            this.DotKeKhaiID = DotKeKhaiID;
            this.TuNgay = TuNgay;
            this.DenNgay = DenNgay;
            this.TrangThai = TrangThai;
            this.ApDungCho = ApDungCho;
            this.NamKeKhai = NamKeKhai;
            this.LoaiDotKeKhai = LoaiDotKeKhai;
            this.TenDotKeKhai = TenDotKeKhai;
            this.CapQuanLy = CapQuanLy;
            this.QuyenChinhSua = QuyenChinhSua;
        }

    }
    public class DotKeKhaiPartial : DotKeKhaiModel
    {
        public int? CoQuanID { get; set; }
        public int? CanBoID { get; set; }
        public string TenCoQuan { get; set; }
        public string TenCanBo { get; set; }

        public int? KeKhaiID { get; set; }
        public List<int> DanhSachCoQuan { set; get; }
        public List<int> DanhSachCanBo { set; get; }
        public bool CoBanKeKhai { get; set; }


        public DotKeKhaiPartial()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

        }
        public DotKeKhaiPartial(int CoQuanID, int CanBoID, string TenCoQuan, string TenCanBo)
        {

            this.CoQuanID = CoQuanID;
            this.CanBoID = CanBoID;
            this.TenCoQuan = TenCoQuan;
            this.TenCanBo = TenCanBo;

        }

        public object Select(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }

    public class NV001
    {
        public int NV0011 { get; set; }
        public DateTime NV0012 { get; set; }
        public DateTime NV0013 { get; set; }
        public int? NV0017 { get; set; }
        public bool? NV0014 { get; set; }
        public int? NV0015 { get; set; }
        public int NV0016 { get; set; }
        public string NV0019 { get; set; }
        public int NV00110 { get; set; }
        public string NV0018 { get; set; }
        public bool QuyenChinhSua { get; set; }
        public int? NV00111 { get; set; }
    }

    public class NV001Partial : NV001
    {
        public int? NV0023 { get; set; }
        public int? NV0024 { get; set; }
        public string TenCoQuan { get; set; }
        public string TenCanBo { get; set; }
        public int? NV0031 { get; set; }
        public List<int> DanhSachCoQuan { set; get; }
        public List<int> DanhSachCanBo { set; get; }
        public bool CoBanKeKhai { get; set; }

    }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DotKeKhaiModel, NV001>()
                .ForMember(des => des.NV0011, x => x.MapFrom(sourse => sourse.DotKeKhaiID))
                .ForMember(des => des.NV0012, x => x.MapFrom(sourse => sourse.TuNgay))
                .ForMember(des => des.NV0013, x => x.MapFrom(sourse => sourse.DenNgay))
                .ForMember(des => des.NV0014, x => x.MapFrom(sourse => sourse.TrangThai))
                .ForMember(des => des.NV0015, x => x.MapFrom(sourse => sourse.NamKeKhai))
                .ForMember(des => des.NV0016, x => x.MapFrom(sourse => sourse.LoaiDotKeKhai))
                .ForMember(des => des.NV0017, x => x.MapFrom(sourse => sourse.ApDungCho))
                .ForMember(des => des.NV0018, x => x.MapFrom(sourse => sourse.TenDotKeKhai))
                .ForMember(des => des.NV0019, x => x.MapFrom(sourse => sourse.MoTaDotKeKhai))
                .ForMember(des => des.NV00110, x => x.MapFrom(sourse => sourse.CapQuanLy))
                .ForMember(des => des.NV00111, x => x.MapFrom(sourse => sourse.CoQuanTao));

            CreateMap<DotKeKhaiPartial, NV001Partial>()
                 .ForMember(des => des.NV0023, x => x.MapFrom(sourse => sourse.CoQuanID))
                 .ForMember(des => des.NV0024, x => x.MapFrom(sourse => sourse.CanBoID))
                 .ForMember(des => des.TenCoQuan, x => x.MapFrom(sourse => sourse.TenCoQuan))
                 .ForMember(des => des.TenCanBo, x => x.MapFrom(sourse => sourse.TenCanBo))
                 .ForMember(des => des.NV0031, x => x.MapFrom(sourse => sourse.KeKhaiID))
                 .ForMember(des => des.DanhSachCoQuan, x => x.MapFrom(sourse => sourse.DanhSachCoQuan))
                 .ForMember(des => des.DanhSachCanBo, x => x.MapFrom(sourse => sourse.DanhSachCanBo))
                 .ForMember(des => des.CoBanKeKhai, x => x.MapFrom(sourse => sourse.CoBanKeKhai));
        }
    }



}
