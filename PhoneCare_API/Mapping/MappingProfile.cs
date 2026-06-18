using AutoMapper;
using PhoneCare.Models;
using PhoneCare_API.Models.DTO.CoSoCuaHang;
using PhoneCare_API.Models.DTO.DichVu;
using PhoneCare_API.Models.DTO.DonHang;
using PhoneCare_API.Models.DTO.NhanVien;

namespace PhoneCare_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CoSoCuaHang, CoSoCuaHangDTO>().ReverseMap();
            CreateMap<DichVu, DichVuDTO>().ReverseMap();
            CreateMap<DonHang, DonHangDTO>().ReverseMap();
            CreateMap<NhanVien, NhanVienDTO>().ReverseMap();
        }
    }
}
