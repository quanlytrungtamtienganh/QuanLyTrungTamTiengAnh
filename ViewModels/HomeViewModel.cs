using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.ViewModels
{
    public class HomeViewModel
    {
        public List<KhoaHoc> FeaturedCourses { get; set; } = new List<KhoaHoc>();
        public List<LopHoc> RecruitingClasses { get; set; } = new List<LopHoc>();
        public int TotalCourses { get; set; }
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
    }
}
