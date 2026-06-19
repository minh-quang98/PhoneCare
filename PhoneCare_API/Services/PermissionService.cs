namespace PhoneCare_API.Services
{
    public static class PermissionService
    {
        public const string Admin = "Admin";
        public const string AdminCoSo = "AdminCS";
        public const string Sale = "Sale";
        public const string KyThuat = "Kỹ thuật";
        public const string Marketing = "Marketing";

        public static IReadOnlyList<string> Roles { get; } = new[]
        {
            Admin,
            AdminCoSo,
            Sale,
            KyThuat,
            Marketing
        };

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý nhân viên hay không.
        /// </summary>
        public static bool CanManageEmployees(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý cửa hàng hay không.
        /// </summary>
        public static bool CanManageStores(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền xem đơn hàng hay không.
        /// </summary>
        public static bool CanViewOrders(string? role)
        {
            return !string.IsNullOrWhiteSpace(role) && !HasRole(role, Marketing);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền chỉnh sửa đơn hàng hay không.
        /// </summary>
        public static bool CanEditOrders(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo) || HasRole(role, Sale) || HasRole(role, KyThuat);
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền quản lý dịch vụ hay không.
        /// </summary>
        public static bool CanManageServices(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo) || HasRole(role, KyThuat);
        }

        /// <summary>
        /// Kiểm tra vai trò hiện tại có khớp với vai trò được yêu cầu hay không.
        /// </summary>
        public static bool HasRole(string? currentRole, string role)
        {
            return string.Equals(currentRole, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
