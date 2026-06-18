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

        public static bool CanManageEmployees(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo);
        }

        public static bool CanManageStores(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo);
        }

        public static bool CanViewOrders(string? role)
        {
            return !string.IsNullOrWhiteSpace(role) && !HasRole(role, Marketing);
        }

        public static bool CanEditOrders(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo) || HasRole(role, Sale) || HasRole(role, KyThuat);
        }

        public static bool CanManageServices(string? role)
        {
            return HasRole(role, Admin) || HasRole(role, AdminCoSo) || HasRole(role, KyThuat);
        }

        public static bool HasRole(string? currentRole, string role)
        {
            return string.Equals(currentRole, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}
