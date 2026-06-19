using System;

namespace PhoneCare.Class
{
    public static class PermissionService
    {
        public const string Admin = "Admin";
        public const string AdminCoSo = "AdminCS";
        public const string Sale = "Sale";
        public const string KyThuat = "Kỹ thuật";
        public const string Marketing = "Marketing";

        /// <summary>
        /// Kiểm tra người dùng hiện tại có vai trò quản trị hay không.
        /// </summary>
        public static bool IsAdmin()
        {
            return HasRole(Admin);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý nhân viên hay không.
        /// </summary>
        public static bool CanManageEmployees()
        {
            return HasRole(Admin) || HasRole(AdminCoSo);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý cửa hàng hay không.
        /// </summary>
        public static bool CanManageStores()
        {
            return HasRole(Admin) || HasRole(AdminCoSo);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền xem đơn hàng hay không.
        /// </summary>
        public static bool CanViewOrders()
        {
            return CurrentUser.Id != 0 && !HasRole(Marketing);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền chỉnh sửa đơn hàng hay không.
        /// </summary>
        public static bool CanEditOrders()
        {
            return HasRole(Admin) || HasRole(AdminCoSo) || HasRole(Sale) || HasRole(KyThuat);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý dịch vụ hay không.
        /// </summary>
        public static bool CanManageServices()
        {
            return HasRole(Admin) || HasRole(AdminCoSo) || HasRole(KyThuat);
        }

        /// <summary>
        /// Kiểm tra vai trò hiện tại có khớp với vai trò được yêu cầu hay không.
        /// </summary>
        public static bool HasRole(string role)
        {
            return string.Equals(CurrentUser.LoaiNhanVien, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
