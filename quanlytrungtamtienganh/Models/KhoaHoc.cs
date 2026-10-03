
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace quanlytrungtamtienganh.Models;
{

     [Table("KhoaHoc")]
    public class KhoaHoc
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = " Mã khóa học ")]
    public int MaKhoaHoc { get; set; }


    [Required(ErrorMessage = "Tên Khóa học không được để trống")]
    [StringLength(150, ErrorMessage = "Tên Khóa học không quá 150 ký tự ")]
    [Display(Name ="Têm khóa học ")]
    public string TenKhoaHoc { get; set; } = string.Empty;


    [Required(ErrorMessage = "cấp độ không được để trống ")]
    [StringLength(50)]
    [Display(Name = "Cấp độ")]
    public string CapDo { get; set; } = string.Empty;


    [Required(ErrorMessage = "Ngoại ngữ không được để trống  ")]
    [StringLength (50)]
    [Display(Name ="Ngoại ngữ")]
    public string NgoaiNgu {  get; set; } = string.Empty;


    [Required(ErrorMessage ="số buổi học không được để trống")]
    [Range(0,500,ErrorMessage ="số buổi học phải từ 1 đến 500 buổi")]
    [Display(Name ="số buổi")]
    public string? SoBuoi {  get; set; }

    [Required(ErrorMessage ="học phí không được để trống")]
    [Range(1,1000000000000,ErrorMessage ="học phí lớn hơn hoặc bằng 0")]
    [Column (TypeName = "decimal(18,2)")]
    [Display(Name ="số buổi")]
    public decimal HocPhi { get; set; }


    [Display(Name ="mô tả khóa học ")]
    public string? MoTa { get; set; }


    [Required]
    [StringLength(50)]
    [Display(Name = "Trạng Thái ")]
    public string TrangThai { get; set; } = "Đang mở ";

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}
