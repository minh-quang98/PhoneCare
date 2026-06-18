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

        public static bool IsAdmin()
        {
            return HasRole(Admin);
        }

        public static bool CanManageEmployees()
        {
            return HasRole(Admin) || HasRole(AdminCoSo);
        }

        public static bool CanManageStores()
        {
            return HasRole(Admin) || HasRole(AdminCoSo);
        }

        public static bool CanViewOrders()
        {
            return CurrentUser.Id != 0 && !HasRole(Marketing);
        }

        public static bool CanEditOrders()
        {
            return HasRole(Admin) || HasRole(AdminCoSo) || HasRole(Sale) || HasRole(KyThuat);
        }

        public static bool CanManageServices()
        {
            return HasRole(Admin) || HasRole(AdminCoSo) || HasRole(KyThuat);
        }

        public static bool HasRole(string role)
        {
            return string.Equals(CurrentUser.LoaiNhanVien, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
