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
    
    public partial class BanGiaoVatTu
    {
        public int Id { get; set; }
        public int IdVatTu { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public int IdNhanVien { get; set; }
        public System.DateTime NgayBanGiao { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
        public virtual VatTu VatTu { get; set; }
    }
}
