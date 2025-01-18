using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NguyenNhutDuy_2122110447.Models
{
    public class PreviewItemSupply
    {
        public int IdVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string Loai { get; set; }
        public int SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public string ThongSoKyThuat { get; set; }
        public decimal GiaNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string TinhTrang { get; set; }
        public decimal GiaKhauHao { get; set; }
    }
}