using PhoneCare.Models;

namespace PhoneCare.Class
{
    public static class RepairStatusHelper
    {
        public static string GetText(int value)
        {
            switch ((RepairStatus)value)
            {
                case RepairStatus.ChoSua:
                    return "Chờ sửa";
                case RepairStatus.DangSua:
                    return "Đang sửa";
                case RepairStatus.DaSua:
                    return "Đã sửa";
                case RepairStatus.KhongSuaDuoc:
                    return "Không sửa được";
                case RepairStatus.KhachKhongSua:
                    return "Khách không sửa";
                case RepairStatus.DaTraKhach:
                    return "Đã trả khách";
                default:
                    return value.ToString();
            }
        }

        public static bool CanEditOrder(int value)
        {
            return value == (int)RepairStatus.ChoSua
                || value == (int)RepairStatus.DangSua
                || value == (int)RepairStatus.DaSua;
        }
    }
}
