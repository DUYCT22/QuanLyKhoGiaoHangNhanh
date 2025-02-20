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
    
    public partial class VatTu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VatTu()
        {
            this.BanGiaoVatTus = new HashSet<BanGiaoVatTu>();
            this.LichSuBanGiaoVatTus = new HashSet<LichSuBanGiaoVatTu>();
        }
    
        public int IdVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string Loai { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public string ThongSoKyThuat { get; set; }
        public Nullable<decimal> GiaNhap { get; set; }
        public System.DateTime NgayNhap { get; set; }
        public string TinhTrang { get; set; }
        public Nullable<decimal> GiaKhauHao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BanGiaoVatTu> BanGiaoVatTus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuBanGiaoVatTu> LichSuBanGiaoVatTus { get; set; }
    }
}
