using PhoneCare.Models;

namespace PhoneCare_API.Services
{
    public static class RepairStatusService
    {
        public static string GetText(int value)
        {
            return (RepairStatus)value switch
            {
                RepairStatus.ChoSua => "Chờ sửa",
                RepairStatus.DangSua => "Đang sửa",
                RepairStatus.DaSua => "Đã sửa",
                RepairStatus.KhongSuaDuoc => "Không sửa được",
                RepairStatus.KhachKhongSua => "Khách không sửa",
                RepairStatus.DaTraKhach => "Đã trả khách",
                _ => value.ToString()
            };
        }

        public static bool CanEditOrder(int value)
        {
            return value == (int)RepairStatus.ChoSua
                || value == (int)RepairStatus.DangSua
                || value == (int)RepairStatus.DaSua;
        }

        public static bool IsValid(int value)
        {
            return Enum.IsDefined(typeof(RepairStatus), value);
        }
    }
}
