//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyKhoGiaoHangNhanh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaiKhoan
    {
        public int Id { get; set; }
        public int IdNhanVien { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; }
        public string TrangThai { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
    }
}
