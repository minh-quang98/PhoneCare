using PhoneCare.Models;

namespace PhoneCare_API.Services
{
    public static class RepairStatusService
    {
        /// <summary>
        /// Chuyển giá trị trạng thái sửa chữa thành nội dung hiển thị.
        /// </summary>
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

        /// <summary>
        /// Kiểm tra trạng thái hiện tại có cho phép chỉnh sửa đơn hàng hay không.
        /// </summary>
        public static bool CanEditOrder(int value)
        {
            return value == (int)RepairStatus.ChoSua
                || value == (int)RepairStatus.DangSua
                || value == (int)RepairStatus.DaSua;
        }

        /// <summary>
        /// Kiểm tra giá trị trạng thái có nằm trong tập trạng thái hợp lệ hay không.
        /// </summary>
        public static bool IsValid(int value)
        {
            return Enum.IsDefined(typeof(RepairStatus), value);
        }
    }
}
