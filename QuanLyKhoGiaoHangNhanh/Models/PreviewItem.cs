using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenNhutDuy_2122110447.Models
{
    public class PreviewItem
    {
        public int Id { get; set; }
        public string DiaChi { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; }
        public double CanNang { get; set; }
        public double TheTich { get; set; }
        public string TenNguoiNhan { get; set; }
        public string Sdt { get; set; }
        public decimal GiaTriDonHang { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public decimal TongTien { get; set; }
        public string GhiChu { get; set; }
    }
}