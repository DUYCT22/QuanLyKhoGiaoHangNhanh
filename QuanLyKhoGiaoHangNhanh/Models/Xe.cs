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
    
    public partial class Xe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Xe()
        {
            this.GiaoHangs = new HashSet<GiaoHang>();
        }
    
        public int Id { get; set; }
        public int IdNhanVienGiaoHang { get; set; }
        public string Ten { get; set; }
        public Nullable<double> TaiTrong { get; set; }
        public Nullable<double> TheTich { get; set; }
        public string ThongSoKyThuat { get; set; }
        public Nullable<int> GiaoHang { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoHang> GiaoHangs { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
