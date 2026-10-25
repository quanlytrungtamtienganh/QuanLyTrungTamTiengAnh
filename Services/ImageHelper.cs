using System;

namespace TrungTamNgoaiNgu.Services
{
    public static class ImageHelper
    {
        // 1. Nhóm ảnh Tiếng Hàn (5 ảnh thực tế người dùng cung cấp)
        private static readonly string[] KoreanImages = new[]
        {
            "/images/anhtienganh/tienghan.jpg",
            "/images/anhtienganh/tienghan1.jpg",
            "/images/anhtienganh/tienghan3.jpg",
            "/images/anhtienganh/tienghan4.jpeg",
            "/images/anhtienganh/tienghan2.gif"
        };

        // 2. Nhóm ảnh Tiếng Trung (4 ảnh thực tế người dùng cung cấp)
        private static readonly string[] ChineseImages = new[]
        {
            "/images/anhtienganh/tiengtrung1.jpg",
            "/images/anhtienganh/tiengtrung2.jpg",
            "/images/anhtienganh/tiengtrung3.jpg",
            "/images/anhtienganh/tiengtrung4.jpg"
        };

        // 3. Nhóm ảnh Tiếng Anh (8 ảnh thực tế người dùng cung cấp, không đổi tên prefix)
        private static readonly string[] EnglishImages = new[]
        {
            "/images/anhtienganh/trung-tam-tieng-anh-wall-street-english.jpg",
            "/images/anhtienganh/trung-tam-tieng-anh-idt-1342029.jpg",
            "/images/anhtienganh/ttta-1.jpg",
            "/images/anhtienganh/thiet-ke-trung-tam-ngoai-ngu-5.jpg",
            "/images/anhtienganh/Thiet-ke-noi-that-truong-mam-non-Maple-Bear-Sunshine-Garden-10.jpg",
            "/images/anhtienganh/unnamed.jpg",
            "/images/anhtienganh/03189ca0b0afb55ec9642fe00fa5b885_1628568387.jpg",
            "/images/anhtienganh/image-56.png"
        };

        // Nhóm ảnh Tiếng Nhật
        private static readonly string[] JapaneseImages = new[]
        {
            "/images/banners/facility-jaxtina.webp",
            "/images/banners/facility-lounge.webp",
            "/images/banners/classroom-activity.jpg"
        };

        /// <summary>
        /// Lấy ảnh đại diện khóa học chính xác theo ngôn ngữ và phân bổ đều tránh trùng lặp
        /// </summary>
        public static string GetCourseImage(string? ngoaiNgu, string? tenKhoaHoc, int id = 0)
        {
            ngoaiNgu = ngoaiNgu?.Trim() ?? "";
            tenKhoaHoc = tenKhoaHoc?.Trim() ?? "";

            // Tiếng Hàn
            if (ngoaiNgu.Contains("Hàn", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("Hàn", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("TOPIK", StringComparison.OrdinalIgnoreCase))
            {
                int idx = Math.Abs(id > 0 ? (id % KoreanImages.Length) : (tenKhoaHoc.GetHashCode() % KoreanImages.Length));
                return KoreanImages[idx];
            }

            // Tiếng Trung
            if (ngoaiNgu.Contains("Trung", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("Trung", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("HSK", StringComparison.OrdinalIgnoreCase))
            {
                int idx = Math.Abs(id > 0 ? (id % ChineseImages.Length) : (tenKhoaHoc.GetHashCode() % ChineseImages.Length));
                return ChineseImages[idx];
            }

            // Tiếng Nhật
            if (ngoaiNgu.Contains("Nhật", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("Nhật", StringComparison.OrdinalIgnoreCase) ||
                tenKhoaHoc.Contains("JLPT", StringComparison.OrdinalIgnoreCase))
            {
                int idx = Math.Abs(id > 0 ? (id % JapaneseImages.Length) : (tenKhoaHoc.GetHashCode() % JapaneseImages.Length));
                return JapaneseImages[idx];
            }

            // Tiếng Anh (Ưu tiên theo tên khóa học cụ thể)
            if (tenKhoaHoc.Contains("IELTS", StringComparison.OrdinalIgnoreCase) || tenKhoaHoc.Contains("6.5", StringComparison.OrdinalIgnoreCase))
            {
                return "/images/anhtienganh/trung-tam-tieng-anh-idt-1342029.jpg";
            }
            if (tenKhoaHoc.Contains("Giao tiếp", StringComparison.OrdinalIgnoreCase) || tenKhoaHoc.Contains("Phản xạ", StringComparison.OrdinalIgnoreCase))
            {
                return "/images/anhtienganh/ttta-1.jpg";
            }
            if (tenKhoaHoc.Contains("Trẻ em", StringComparison.OrdinalIgnoreCase) || tenKhoaHoc.Contains("Thiếu nhi", StringComparison.OrdinalIgnoreCase) || tenKhoaHoc.Contains("Mầm non", StringComparison.OrdinalIgnoreCase))
            {
                return "/images/anhtienganh/Thiet-ke-noi-that-truong-mam-non-Maple-Bear-Sunshine-Garden-10.jpg";
            }

            // Các khóa học tiếng Anh khác phân phối xoay vòng
            int enIdx = Math.Abs(id > 0 ? (id % EnglishImages.Length) : (tenKhoaHoc.GetHashCode() % EnglishImages.Length));
            return EnglishImages[enIdx];
        }

        // Ảnh sảnh đón tiếp & Campus 5 sao
        public static string GetCampusHeroImage() => "/images/anhtienganh/trung-tam-tieng-anh-wall-street-english.jpg";

        // Ảnh không gian học tập hiện đại
        public static string GetClassroomFacilityImage() => "/images/anhtienganh/thiet-ke-trung-tam-ngoai-ngu-5.jpg";

        // Ảnh hoạt động nhóm & Quảng cáo tuyển sinh (Sponsored Ad)
        public static string GetSponsoredAdImage() => "/images/anhtienganh/unnamed.jpg";

        // Ảnh học viên tương tác
        public static string GetStudentActivityImage() => "/images/anhtienganh/03189ca0b0afb55ec9642fe00fa5b885_1628568387.jpg";
    }
}
