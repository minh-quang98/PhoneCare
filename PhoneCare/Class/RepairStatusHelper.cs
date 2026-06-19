using PhoneCare.Models;

namespace PhoneCare.Class
{
    public static class RepairStatusHelper
    {
        /// <summary>
        /// Chuyển giá trị trạng thái sửa chữa thành nội dung hiển thị.
        /// </summary>
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

        /// <summary>
        /// Kiểm tra trạng thái hiện tại có cho phép chỉnh sửa đơn hàng hay không.
        /// </summary>
        public static bool CanEditOrder(int value)
        {
            return value == (int)RepairStatus.ChoSua
                || value == (int)RepairStatus.DangSua
                || value == (int)RepairStatus.DaSua;
        }
    }
}
