using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace quanlytrungtamtienganh.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage ="Tên đăng nhập không được để trống")]
        [StringLength(50,ErrorMessage ="Tên đăng nhập không vượt quá 50 ký tự ")]
        [Display(Name ="Tên đăng nhập ")]
        public string TaiKhoanName { get; set; }=string.Empty;

        [Required(ErrorMessage ="Mật khẩu không được để trống ")]
        [StringLength(255)]
        [Display(Name ="Mật khẩu ")]
        public string MatKhau { get; set; } = string.Empty;


        [Required(ErrorMessage ="Email không được để trống")]
        [EmailAddress(ErrorMessage ="Email không đúng định dạng ")]
        [StringLength(100)]
        [Display(Name ="Địa chỉ Email")]
        public string Email { get;set; } = string.Empty;


        [Required]
        [StringLength(20)]
        [Display(Name = "vai trò")]
        public string VaiTro { get; set; } = "HocVien";

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng Thái ")]
        public String TrangThai { get; set; } = "Hoatdong";
          
        public virtual HocVien?HocVien { get; set; }

    }
}
