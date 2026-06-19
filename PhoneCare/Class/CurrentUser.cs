using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Class
{
    public static class CurrentUser
    {
        public static int Id { get; set; }
        public static string UserName { get; set; }
        public static string FullName { get; set; }
        public static int CoSoCuaHangId { get; set; }
        public static string LoaiNhanVien { get; set; }

        /// <summary>
        /// Xóa thông tin người dùng đang đăng nhập khỏi phiên hiện tại.
        /// </summary>
        public static void Clear()
        {
            Id = 0;
            UserName = null;
            FullName = null;
            CoSoCuaHangId = 0;
            LoaiNhanVien = null;
        }
    }
}
