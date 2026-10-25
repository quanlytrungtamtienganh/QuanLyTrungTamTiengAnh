using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Data
{
    public static partial class DbInitializer
    {
        public static void SeedLuyenThiIfEmpty(TrungTamDbContext context)
        {
            try
            {
                // 1. Tự động tạo các bảng nếu chưa có
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DeThi' and xtype='U')
                    BEGIN
                        CREATE TABLE [DeThi] (
                            [MaDeThi] int NOT NULL IDENTITY,
                            [TieuDe] nvarchar(255) NOT NULL,
                            [MoTa] nvarchar(max) NULL,
                            [TheLoai] nvarchar(50) NOT NULL DEFAULT 'IELTS Academic',
                            [KyNang] nvarchar(50) NOT NULL DEFAULT 'Listening',
                            [ThoiGianPhut] int NOT NULL DEFAULT 40,
                            [TongSoCau] int NOT NULL DEFAULT 40,
                            [SoLuotLam] int NOT NULL DEFAULT 0,
                            [HinhAnh] nvarchar(255) NULL,
                            [FileAudio] nvarchar(500) NULL,
                            [NgayTao] datetime2 NOT NULL DEFAULT GETDATE(),
                            [TrangThai] bit NOT NULL DEFAULT 1,
                            CONSTRAINT [PK_DeThi] PRIMARY KEY ([MaDeThi])
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PhanThi' and xtype='U')
                    BEGIN
                        CREATE TABLE [PhanThi] (
                            [MaPhanThi] int NOT NULL IDENTITY,
                            [MaDeThi] int NOT NULL,
                            [TenPhanThi] nvarchar(100) NOT NULL,
                            [ThuTu] int NOT NULL DEFAULT 1,
                            [TieuDePhan] nvarchar(255) NULL,
                            [DoanVanDoc] nvarchar(max) NULL,
                            [FileAudio] nvarchar(500) NULL,
                            [TagDangBai] nvarchar(100) NULL,
                            CONSTRAINT [PK_PhanThi] PRIMARY KEY ([MaPhanThi]),
                            CONSTRAINT [FK_PhanThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CauHoiLuyenThi' and xtype='U')
                    BEGIN
                        CREATE TABLE [CauHoiLuyenThi] (
                            [MaCauHoi] int NOT NULL IDENTITY,
                            [MaPhanThi] int NOT NULL,
                            [SoThuTu] int NOT NULL DEFAULT 1,
                            [NoiDung] nvarchar(max) NOT NULL,
                            [LoaiCauHoi] nvarchar(30) NOT NULL DEFAULT 'TracNghiem',
                            [DapAnA] nvarchar(max) NULL,
                            [DapAnB] nvarchar(max) NULL,
                            [DapAnC] nvarchar(max) NULL,
                            [DapAnD] nvarchar(max) NULL,
                            [DapAnDung] nvarchar(255) NOT NULL,
                            [GiaiThichChiTiet] nvarchar(max) NULL,
                            [PhanLoaiDangBai] nvarchar(100) NULL,
                            [Diem] float NOT NULL DEFAULT 1.0,
                            CONSTRAINT [PK_CauHoiLuyenThi] PRIMARY KEY ([MaCauHoi]),
                            CONSTRAINT [FK_CauHoiLuyenThi_PhanThi_MaPhanThi] FOREIGN KEY ([MaPhanThi]) REFERENCES [PhanThi] ([MaPhanThi]) ON DELETE CASCADE
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BinhLuanDeThi' and xtype='U')
                    BEGIN
                        CREATE TABLE [BinhLuanDeThi] (
                            [MaBinhLuan] int NOT NULL IDENTITY,
                            [MaDeThi] int NOT NULL,
                            [MaTaiKhoan] int NOT NULL,
                            [NoiDung] nvarchar(1000) NOT NULL,
                            [NgayBinhLuan] datetime2 NOT NULL DEFAULT GETDATE(),
                            [MaBinhLuanCha] int NULL,
                            CONSTRAINT [PK_BinhLuanDeThi] PRIMARY KEY ([MaBinhLuan]),
                            CONSTRAINT [FK_BinhLuanDeThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE,
                            CONSTRAINT [FK_BinhLuanDeThi_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE CASCADE
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KetQuaLuyenThi' and xtype='U')
                    BEGIN
                        CREATE TABLE [KetQuaLuyenThi] (
                            [MaKetQua] int NOT NULL IDENTITY,
                            [MaDeThi] int NOT NULL,
                            [MaTaiKhoan] int NULL,
                            [CheDoLam] nvarchar(50) NOT NULL DEFAULT 'LuyenTap',
                            [CacPhanDaLam] nvarchar(255) NULL,
                            [SoCauDung] int NOT NULL DEFAULT 0,
                            [SoCauSai] int NOT NULL DEFAULT 0,
                            [SoCauBoQua] int NOT NULL DEFAULT 0,
                            [TongSoCau] int NOT NULL DEFAULT 0,
                            [DiemSo] float NOT NULL DEFAULT 0,
                            [DoChinhXac] float NOT NULL DEFAULT 0,
                            [ThoiGianLamGiay] int NOT NULL DEFAULT 0,
                            [NgayNop] datetime2 NOT NULL DEFAULT GETDATE(),
                            [ChiTietBaiLamJson] nvarchar(max) NULL,
                            CONSTRAINT [PK_KetQuaLuyenThi] PRIMARY KEY ([MaKetQua]),
                            CONSTRAINT [FK_KetQuaLuyenThi_DeThi_MaDeThi] FOREIGN KEY ([MaDeThi]) REFERENCES [DeThi] ([MaDeThi]) ON DELETE CASCADE,
                            CONSTRAINT [FK_KetQuaLuyenThi_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
                        );
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PhanThi_MaDeThi')
                        CREATE NONCLUSTERED INDEX [IX_PhanThi_MaDeThi] ON [PhanThi] ([MaDeThi]);

                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CauHoiLuyenThi_MaPhanThi')
                        CREATE NONCLUSTERED INDEX [IX_CauHoiLuyenThi_MaPhanThi] ON [CauHoiLuyenThi] ([MaPhanThi]);

                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BinhLuanDeThi_MaDeThi')
                        CREATE NONCLUSTERED INDEX [IX_BinhLuanDeThi_MaDeThi] ON [BinhLuanDeThi] ([MaDeThi]);

                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_KetQuaLuyenThi_MaDeThi')
                        CREATE NONCLUSTERED INDEX [IX_KetQuaLuyenThi_MaDeThi] ON [KetQuaLuyenThi] ([MaDeThi]);

                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DeThi_TrangThai')
                        CREATE NONCLUSTERED INDEX [IX_DeThi_TrangThai] ON [DeThi] ([TrangThai], [SoLuotLam] DESC);
                ");

                // Khởi tạo 40 đề thi 200 câu chuẩn quốc tế cho cả 4 ngôn ngữ (Anh, Nhật, Trung, Hàn)
                LuyenThiDataGenerator.GenerateAll(context);
                return;

                // =========================================================================
                // ĐỀ 1: IELTS Collection 1 Listening Test 1 (20 câu hỏi liên tục - Đủ 4 Section)
                // =========================================================================
                var deIelts1 = new DeThi
                {
                    TieuDe = "IELTS Collection 1 Listening Test 1",
                    MoTa = "Bộ đề thi thực hành IELTS Listening chuẩn định dạng Cambridge với 4 Section: Form Completion, Multiple Choice, Discussion và Academic Lecture.",
                    TheLoai = "IELTS Academic",
                    KyNang = "Listening",
                    ThoiGianPhut = 30,
                    TongSoCau = 20,
                    SoLuotLam = 349023,
                    HinhAnh = "https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=600&auto=format&fit=crop&q=80",
                    FileAudio = "/audio/ielts-listening-part1.wav",
                    NgayTao = DateTime.Now.AddDays(-60),
                    TrangThai = true
                };
                context.DeThis.Add(deIelts1);
                context.SaveChanges();

                // ── Phần 1: Section 1 (Câu 1 - 5) ──
                var rec1 = new PhanThi
                {
                    MaDeThi = deIelts1.MaDeThi,
                    TenPhanThi = "Section 1: Eco-Farm Booking",
                    ThuTu = 1,
                    TieuDePhan = "Questions 1-5: Complete the notes below. Write ONE WORD AND/OR A NUMBER for each answer.",
                    TagDangBai = "#Listening|Form Completion",
                    FileAudio = "/audio/ielts-listening-part1.wav",
                    DoanVanDoc = @"
<div class='alert alert-light border small py-2 mb-3'>
    <div class='fw-bold text-primary mb-1'><i class='bi bi-info-circle me-1'></i>Tình huống bài nghe:</div>
    <em>Helen Greendale gọi điện thoại đến lễ tân Eco-Farm để đặt phòng nghỉ dưỡng cuối tuần cho gia đình. Hãy lắng nghe đoạn băng và điền thông tin chính xác vào các chỗ trống bên dưới.</em>
</div>"
                };
                context.PhanThis.Add(rec1);
                context.SaveChanges();

                var questionsRec1 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec1.MaPhanThi,
                        SoThuTu = 1,
                        NoiDung = "Customer surname: Helen (1) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "Greendale",
                        PhanLoaiDangBai = "[Listening] Form Completion",
                        GiaiThichChiTiet = "<strong>Vị trí nghe (Transcript):</strong> 'My first name is Helen, and my surname is Greendale, that is spelled G-R-E-E-N-D-A-L-E.'<br/><strong>Lý do giải thích:</strong> Người nói đánh vần rõ ràng từng chữ cái họ của mình là G-R-E-E-N-D-A-L-E. Do đó từ cần điền là <strong>Greendale</strong> (chú ý viết hoa chữ cái đầu)."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec1.MaPhanThi,
                        SoThuTu = 2,
                        NoiDung = "Work email address: helen123@(2) _______ .com",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "freemail",
                        PhanLoaiDangBai = "[Listening] Form Completion",
                        GiaiThichChiTiet = "<strong>Vị trí nghe (Transcript):</strong> 'Could I also take down your work email address? It is helen123 at freemail dot com.'<br/><strong>Lý do giải thích:</strong> Tên miền hộp thư được phát âm là 'freemail'. Đáp án chính xác là <strong>freemail</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec1.MaPhanThi,
                        SoThuTu = 3,
                        NoiDung = "Residential address: (3) _______ Road, Sheffield",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "16 Green",
                        PhanLoaiDangBai = "[Listening] Form Completion",
                        GiaiThichChiTiet = "<strong>Vị trí nghe (Transcript):</strong> 'We live at sixteen Green Road, Sheffield.'<br/><strong>Lý do giải thích:</strong> Đề bài yêu cầu điền số nhà và tên đường trước từ 'Road'. Người nói trả lời là '16 Green Road'. Đáp án chuẩn là <strong>16 Green</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec1.MaPhanThi,
                        SoThuTu = 4,
                        NoiDung = "Information source: Found website while browsing the (4) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "internet",
                        PhanLoaiDangBai = "[Listening] Form Completion",
                        GiaiThichChiTiet = "<strong>Vị trí nghe (Transcript):</strong> 'I found your official website while browsing the internet.'<br/><strong>Lý do giải thích:</strong> Người gọi xác nhận nguồn biết đến trang trại là thông qua 'the internet'. Từ cần điền là <strong>internet</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec1.MaPhanThi,
                        SoThuTu = 5,
                        NoiDung = "Loyalty membership number: (5) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "74390",
                        PhanLoaiDangBai = "[Listening] Form Completion",
                        GiaiThichChiTiet = "<strong>Vị trí nghe (Transcript):</strong> 'What is your Northern Hotel loyalty membership number? It is 74390.'<br/><strong>Lý do giải thích:</strong> Dãy số được phát âm rõ 'seven-four-three-nine-zero'. Đáp án là <strong>74390</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(questionsRec1);
                context.SaveChanges();

                // ── Phần 2: Section 2 (Câu 6 - 10) ──
                var rec2 = new PhanThi
                {
                    MaDeThi = deIelts1.MaDeThi,
                    TenPhanThi = "Section 2: Rivervale Park",
                    ThuTu = 2,
                    TieuDePhan = "Questions 6-10: Choose the correct letter, A, B, or C.",
                    TagDangBai = "#Listening|Multiple Choice",
                    FileAudio = "/audio/ielts-listening-part2.wav",
                    DoanVanDoc = @"
<div class='alert alert-light border small py-2 mb-3'>
    <div class='fw-bold text-primary mb-1'><i class='bi bi-info-circle me-1'></i>Tình huống bài nghe:</div>
    <em>Người hướng dẫn thuyết minh giới thiệu về lịch sử, công trình mới và các quy định tham quan tại Công viên Sinh thái Rivervale Nature Park.</em>
</div>"
                };
                context.PhanThis.Add(rec2);
                context.SaveChanges();

                var questionsRec2 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec2.MaPhanThi,
                        SoThuTu = 6,
                        NoiDung = "What was the original purpose of the park land in the 19th century?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "A private botanical garden",
                        DapAnB = "An active limestone quarry",
                        DapAnC = "A cattle grazing pasture",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí nghe:</strong> 'Back in the nineteenth century, this entire expanse was excavated as an active limestone quarry before being restored.'<br/><strong>Lý do giải thích:</strong> Vùng đất ban đầu là một mỏ khai thác đá vôi (limestone quarry). Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec2.MaPhanThi,
                        SoThuTu = 7,
                        NoiDung = "The new modern visitor pavilion was mainly funded by:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "An 80% grant from the national lottery fund",
                        DapAnB = "Charitable donations from regional businesses",
                        DapAnC = "Local municipal property tax revenues",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí nghe:</strong> '...made possible primarily by an eighty percent grant from the national lottery fund.'<br/><strong>Lý do giải thích:</strong> Nguồn kinh phí chính yếu đến từ quỹ xổ số quốc gia (national lottery fund). Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec2.MaPhanThi,
                        SoThuTu = 8,
                        NoiDung = "Why is the northern lakeside trail temporarily closed during spring?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Heavy rain caused dangerous mudslides",
                        DapAnB = "Rare wetland birds are currently nesting among the reeds",
                        DapAnC = "Timber cutting operations are underway",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí nghe:</strong> '...the northern lakeside trail is temporarily closed because rare wetland birds are currently nesting among the reeds.'<br/><strong>Lý do giải thích:</strong> Đường mòn đóng cửa do các loài chim quý hiếm đang làm tổ trong mùa sinh sản. Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec2.MaPhanThi,
                        SoThuTu = 9,
                        NoiDung = "Children under 12 can participate in the weekend insect safari if they:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Are accompanied by a supervising adult",
                        DapAnB = "Bring their own personal magnifying tools",
                        DapAnC = "Have registered online at least two days in advance",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí nghe:</strong> '...children under twelve are welcome on our insect safari, provided they are accompanied by a supervising adult.'<br/><strong>Lý do giải thích:</strong> Điều kiện cần là phải có người lớn đi kèm giám sát (accompanied by an adult). Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec2.MaPhanThi,
                        SoThuTu = 10,
                        NoiDung = "What discount is granted at the gift shop for annual pass holders?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "5% on all items",
                        DapAnB = "10% on educational books",
                        DapAnC = "15% discount on all items",
                        DapAnDung = "C",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí nghe:</strong> '...where annual pass holders enjoy a fifteen percent discount on all items.'<br/><strong>Lý do giải thích:</strong> Giảm giá 15% cho người có thẻ hội viên thường niên. Đáp án đúng: <strong>C</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(questionsRec2);
                context.SaveChanges();

                // ── Phần 3: Section 3 (Câu 11 - 15) ──
                var rec3 = new PhanThi
                {
                    MaDeThi = deIelts1.MaDeThi,
                    TenPhanThi = "Section 3: Project Discussion",
                    ThuTu = 3,
                    TieuDePhan = "Questions 11-15: Choose the correct letter, A, B, or C.",
                    TagDangBai = "#Listening|Academic Discussion",
                    FileAudio = "/audio/ielts-listening-part3.wav",
                    DoanVanDoc = @"
<div class='alert alert-light border small py-2 mb-3'>
    <div class='fw-bold text-primary mb-1'><i class='bi bi-info-circle me-1'></i>Tình huống bài nghe:</div>
    <em>Hai sinh viên Jack và Sarah thảo luận về tiến độ đồ án tốt nghiệp ngành Kỹ thuật Năng lượng tái tạo và cuộc khảo sát thực địa ở Cornwall.</em>
</div>"
                };
                context.PhanThis.Add(rec3);
                context.SaveChanges();

                var questionsRec3 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec3.MaPhanThi,
                        SoThuTu = 11,
                        NoiDung = "Why did Jack choose offshore wind farms over solar energy for his dissertation?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Technological advances have drastically cut turbine production costs",
                        DapAnB = "His academic supervisor strongly recommended wind energy",
                        DapAnC = "He had previously completed an internship at a marine firm",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '...because recent technological advancements have drastically cut turbine production costs.'<br/><strong>Lý do giải thích:</strong> Jack chọn điện gió ngoài khơi vì chi phí sản xuất tuabin giảm mạnh. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec3.MaPhanThi,
                        SoThuTu = 12,
                        NoiDung = "What main obstacle did Sarah face during her field survey in Cornwall?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Residents refused to take part in phone interviews",
                        DapAnB = "Bureaucratic delay getting environmental clearance took six weeks",
                        DapAnC = "Statistical software generated contradictory regression results",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Obtaining environmental clearance permits from the regional council took six weeks longer than anticipated.'<br/><strong>Lý do giải thích:</strong> Trì hoãn thủ tục xin giấy phép môi trường kéo dài thêm 6 tuần. Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec3.MaPhanThi,
                        SoThuTu = 13,
                        NoiDung = "How did local residents describe their sentiment toward the wind project?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Extremely encouraging",
                        DapAnB = "Strongly resistant",
                        DapAnC = "Completely indifferent",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'However, the data gathered from local residents was extremely encouraging.'<br/><strong>Lý do giải thích:</strong> Dữ liệu khảo sát từ người dân địa phương rất tích cực và đáng khích lệ. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec3.MaPhanThi,
                        SoThuTu = 14,
                        NoiDung = "What is the key advantage of offshore wind over onshore installations?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Winds are stronger, more consistent and avoid visual clutter on land",
                        DapAnB = "Marine turbines require no periodic maintenance",
                        DapAnC = "Underwater cables are much cheaper to install",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Lý do giải thích:</strong> Điện gió ngoài khơi có sức gió mạnh và đều đặn hơn, không chiếm diện tích đất đai khu dân cư. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec3.MaPhanThi,
                        SoThuTu = 15,
                        NoiDung = "What milestone do the two students aim to accomplish before the end of the semester?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Publish a joint peer-reviewed conference paper",
                        DapAnB = "Submit complete field datasets and finish prototype simulation",
                        DapAnC = "Secure funding from commercial energy companies",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[Listening] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Lý do giải thích:</strong> Mục tiêu trọng tâm là hoàn thành thu thập dữ liệu và chạy mô phỏng mẫu thử nghiệm. Đáp án đúng: <strong>B</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(questionsRec3);
                context.SaveChanges();

                // ── Phần 4: Section 4 (Câu 16 - 20) ──
                var rec4 = new PhanThi
                {
                    MaDeThi = deIelts1.MaDeThi,
                    TenPhanThi = "Section 4: Acoustic Architecture",
                    ThuTu = 4,
                    TieuDePhan = "Questions 16-20: Complete the summary below. Write ONE WORD ONLY for each answer.",
                    TagDangBai = "#Listening|Lecture Summary",
                    FileAudio = "/audio/ielts-listening-part4.wav",
                    DoanVanDoc = @"
<div class='alert alert-light border small py-2 mb-3'>
    <div class='fw-bold text-primary mb-1'><i class='bi bi-info-circle me-1'></i>Tình huống bài giảng:</div>
    <em>Bài giảng học thuật về nghệ thuật âm học trong kiến trúc cổ đại: Nhà hát Hy Lạp tại Epidaurus và kỹ thuật khuếch đại giọng nói của người La Mã.</em>
</div>"
                };
                context.PhanThis.Add(rec4);
                context.SaveChanges();

                var questionsRec4 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec4.MaPhanThi,
                        SoThuTu = 16,
                        NoiDung = "The theater at Epidaurus carried voices to the highest tier without modern (16) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "amplifiers",
                        PhanLoaiDangBai = "[Listening] Summary Completion",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '...how actors' voices could carry to the topmost tier without modern amplifiers.'<br/><strong>Lý do giải thích:</strong> Từ chỉ thiết bị khuếch đại âm thanh là <strong>amplifiers</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec4.MaPhanThi,
                        SoThuTu = 17,
                        NoiDung = "The key acoustic filter was the rows of carved (17) _______ seats.",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "limestone",
                        PhanLoaiDangBai = "[Listening] Summary Completion",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Acoustic analysis revealed that the rows of carved limestone seats acted as an acoustic filter.'<br/><strong>Lý do giải thích:</strong> Chất liệu của các hàng ghế là đá vôi: <strong>limestone</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec4.MaPhanThi,
                        SoThuTu = 18,
                        NoiDung = "The seats effectively suppressed low-frequency ambient (18) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "murmurs",
                        PhanLoaiDangBai = "[Listening] Summary Completion",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '...suppressing low-frequency ambient murmurs.'<br/><strong>Lý do giải thích:</strong> Từ cần điền là tiếng xì xào, tạp âm tần số thấp: <strong>murmurs</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec4.MaPhanThi,
                        SoThuTu = 19,
                        NoiDung = "Roman builders placed hollow bronze (19) _______ below the stage floorboards.",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "vessels",
                        PhanLoaiDangBai = "[Listening] Summary Completion",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Centuries later, Roman builders placed hollow bronze vessels below stage floorboards.'<br/><strong>Lý do giải thích:</strong> Những chiếc bình đồng rỗng: <strong>vessels</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = rec4.MaPhanThi,
                        SoThuTu = 20,
                        NoiDung = "These bronze vessels were engineered to (20) _______ with actors' vocal sounds.",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "resonate",
                        PhanLoaiDangBai = "[Listening] Summary Completion",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '...to resonate with actors' voices.'<br/><strong>Lý do giải thích:</strong> Động từ chỉ sự cộng hưởng âm thanh là <strong>resonate</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(questionsRec4);
                context.SaveChanges();

                // =========================================================================
                // ĐỀ 2: TOEIC Full Practice Test ETS 2026 (25 câu liên tục - Cả Nghe & Đọc)
                // =========================================================================
                var deToeic = new DeThi
                {
                    TieuDe = "TOEIC Full Practice Test ETS 2026",
                    MoTa = "Đề thi TOEIC format mới chuẩn ETS gồm cả 2 kỹ năng Listening & Reading: Part 1 Tranh ảnh, Part 2 Hỏi đáp, Part 3 Hội thoại, Part 5 Điền câu và Part 7 Đọc hiểu Email.",
                    TheLoai = "TOEIC",
                    KyNang = "Listening & Reading",
                    ThoiGianPhut = 35,
                    TongSoCau = 25,
                    SoLuotLam = 187650,
                    HinhAnh = "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?w=600&auto=format&fit=crop&q=80",
                    FileAudio = "/audio/toeic-part1.wav",
                    NgayTao = DateTime.Now.AddDays(-30),
                    TrangThai = true
                };
                context.DeThis.Add(deToeic);
                context.SaveChanges();

                // ── Part 1: Photographs (Listening) (Câu 1 - 5) ──
                var toeicPart1 = new PhanThi
                {
                    MaDeThi = deToeic.MaDeThi,
                    TenPhanThi = "Part 1: Photographs (Listening)",
                    ThuTu = 1,
                    TieuDePhan = "Directions: For each question, look at the photograph, listen to the 4 statements and choose the one that best describes what you see.",
                    TagDangBai = "#TOEIC|Photographs #Listening",
                    FileAudio = "/audio/toeic-part1.wav"
                };
                context.PhanThis.Add(toeicPart1);
                context.SaveChanges();

                var toeicP1Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart1.MaPhanThi,
                        SoThuTu = 1,
                        NoiDung = "<div class='mb-2 text-center'><img src='https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=600&auto=format&fit=crop&q=80' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Question 1 Photo'></div><strong>Question 1: Look at the photograph.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "The woman is typing on a computer keyboard.",
                        DapAnB = "The woman is writing notes in a spiral notebook.",
                        DapAnC = "The woman is speaking into a telephone handset.",
                        DapAnD = "The woman is filing paperwork in a cabinet.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 1 - Photographs",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>(A) The woman is typing on a computer keyboard.<br/>(B) The woman is writing notes in a spiral notebook.<br/>(C) The woman is speaking into a telephone handset.<br/>(D) The woman is filing paperwork in a cabinet.<br/><strong>Lý do giải thích:</strong> Trong bức ảnh, người phụ nữ đang ngồi trước màn hình và đặt hai tay gõ bàn phím máy tính. Phương án <strong>A</strong> mô tả hoàn toàn chuẩn xác hành động."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart1.MaPhanThi,
                        SoThuTu = 2,
                        NoiDung = "<div class='mb-2 text-center'><img src='https://images.unsplash.com/photo-1542744173-8e7e53415bb0?w=600&auto=format&fit=crop&q=80' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Question 2 Photo'></div><strong>Question 2: Look at the photograph.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "The colleagues are shaking hands across the table.",
                        DapAnB = "The presenter is pointing at a financial chart on the screen.",
                        DapAnC = "The team is packing cardboard boxes in the office.",
                        DapAnD = "The workers are cleaning the conference room window.",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 1 - Photographs",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>(A) The colleagues are shaking hands across the table.<br/>(B) The presenter is pointing at a financial chart on the screen.<br/>(C) The team is packing cardboard boxes in the office.<br/>(D) The workers are cleaning the conference room window.<br/><strong>Lý do giải thích:</strong> Bức ảnh chụp buổi thuyết trình, người diễn giả đang đứng chỉ vào biểu đồ phân tích trên màn hình chiếu. Phương án <strong>B</strong> là đáp án đúng."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart1.MaPhanThi,
                        SoThuTu = 3,
                        NoiDung = "<div class='mb-2 text-center'><img src='https://images.unsplash.com/photo-1455390582262-044cdead277a?w=600&auto=format&fit=crop&q=80' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Question 3 Photo'></div><strong>Question 3: Look at the photograph.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "A pen is being used to write on a document.",
                        DapAnB = "A book is being placed on a bookshelf.",
                        DapAnC = "An envelope is being sealed with glue.",
                        DapAnD = "A laptop is being repaired by a technician.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 1 - Photographs",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>(A) A pen is being used to write on a document.<br/>(B) A book is being placed on a bookshelf.<br/>(C) An envelope is being sealed with glue.<br/>(D) A laptop is being repaired by a technician.<br/><strong>Lý do giải thích:</strong> Cận cảnh một người cầm bút bi viết lên trang giấy. Câu <strong>A</strong> mô tả đúng bản chất hành động."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart1.MaPhanThi,
                        SoThuTu = 4,
                        NoiDung = "<div class='mb-2 text-center'><img src='https://images.unsplash.com/photo-1503387762-592deb58ef4e?w=600&auto=format&fit=crop&q=80' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Question 4 Photo'></div><strong>Question 4: Look at the photograph.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "The man is reviewing architectural blueprints.",
                        DapAnB = "The man is painting the interior wall.",
                        DapAnC = "The man is climbing an aluminum ladder.",
                        DapAnD = "The man is purchasing construction materials.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 1 - Photographs",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>(A) The man is reviewing architectural blueprints.<br/>(B) The man is painting the interior wall.<br/>(C) The man is climbing an aluminum ladder.<br/>(D) The man is purchasing construction materials.<br/><strong>Lý do giải thích:</strong> Kỹ sư đội mũ bảo hộ đang trải bản vẽ kiến trúc ra bàn làm việc để kiểm tra. Đáp án đúng là <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart1.MaPhanThi,
                        SoThuTu = 5,
                        NoiDung = "<div class='mb-2 text-center'><img src='https://images.unsplash.com/photo-1586528116311-ad8dd3c8310d?w=600&auto=format&fit=crop&q=80' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Question 5 Photo'></div><strong>Question 5: Look at the photograph.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Items are being arranged on warehouse shelves.",
                        DapAnB = "A truck is being loaded with heavy cargo.",
                        DapAnC = "The customer is paying for purchases at the counter.",
                        DapAnD = "The store entrance is being locked.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 1 - Photographs",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>(A) Items are being arranged on warehouse shelves.<br/>(B) A truck is being loaded with heavy cargo.<br/>(C) The customer is paying for purchases at the counter.<br/>(D) The store entrance is being locked.<br/><strong>Lý do giải thích:</strong> Hàng hóa và thùng hàng đang được xếp ngay ngắn trên các kệ kho bãi logistics. Đáp án chuẩn: <strong>A</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(toeicP1Questions);
                context.SaveChanges();

                // ── Part 2: Question-Response (Listening) (Câu 6 - 10) ──
                var toeicPart2 = new PhanThi
                {
                    MaDeThi = deToeic.MaDeThi,
                    TenPhanThi = "Part 2: Question-Response (Listening)",
                    ThuTu = 2,
                    TieuDePhan = "Directions: You will hear a question or statement and three responses. Select the best response.",
                    TagDangBai = "#TOEIC|Question-Response #Listening",
                    FileAudio = "/audio/toeic-part2.wav"
                };
                context.PhanThis.Add(toeicPart2);
                context.SaveChanges();

                var toeicP2Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart2.MaPhanThi,
                        SoThuTu = 6,
                        NoiDung = "<strong>Question 6:</strong> Where is the quarterly marketing strategy meeting taking place?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Yes, yesterday afternoon.",
                        DapAnB = "In conference room B on the third floor.",
                        DapAnC = "Ms. Rodriguez will lead the team.",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 2 - Where Question",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>Question: Where is the quarterly marketing strategy meeting taking place?<br/>(A) Yes, yesterday afternoon. (Sai vì câu hỏi 'Where' không trả lời Yes/No)<br/>(B) In conference room B on the third floor. (Chỉ đúng địa điểm phòng họp B tầng 3)<br/>(C) Ms. Rodriguez will lead the team. (Chỉ người, không phải nơi chốn)<br/>-> Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart2.MaPhanThi,
                        SoThuTu = 7,
                        NoiDung = "<strong>Question 7:</strong> When will the revised financial audit report be finalized?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "By the end of business on Friday.",
                        DapAnB = "At the local printing facility.",
                        DapAnC = "Fifteen new accounting templates.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 2 - When Question",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>Question: When will the revised financial audit report be finalized?<br/>(A) By the end of business on Friday. (Chỉ mốc thời gian hoàn thành trước cuối giờ chiều thứ Sáu)<br/>(B) At the local printing facility. (Chỉ nơi chốn)<br/>(C) Fifteen new accounting templates. (Chỉ số lượng đồ vật)<br/>-> Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart2.MaPhanThi,
                        SoThuTu = 8,
                        NoiDung = "<strong>Question 8:</strong> Why was the flight to Chicago delayed?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Gate thirty-four.",
                        DapAnB = "Severe weather conditions at O'Hare airport.",
                        DapAnC = "Flight attendant training.",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 2 - Why Question",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>Question: Why was the flight to Chicago delayed?<br/>(A) Gate thirty-four. (Chỉ cổng ra máy bay)<br/>(B) Severe weather conditions at O'Hare airport. (Giải thích lý do thời tiết xấu tại sân bay Chicago)<br/>(C) Flight attendant training.<br/>-> Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart2.MaPhanThi,
                        SoThuTu = 9,
                        NoiDung = "<strong>Question 9:</strong> Would you prefer to take the express train or rent a company car?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "The train is much faster during peak morning hours.",
                        DapAnB = "Yes, I rented an apartment downtown.",
                        DapAnC = "Tomorrow morning at eight.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 2 - Choice Question",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>Question: Would you prefer to take the express train or rent a company car?<br/>(A) The train is much faster during peak morning hours. (Lựa chọn đi tàu hỏa vì nhanh hơn trong giờ cao điểm)<br/>(B) Yes... (Câu hỏi lựa chọn 'Or' không trả lời Yes/No)<br/>-> Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart2.MaPhanThi,
                        SoThuTu = 10,
                        NoiDung = "<strong>Question 10:</strong> Has the purchase order for new laptops been approved yet?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Mr. Tanaka signed off on it this morning.",
                        DapAnB = "Twenty gigabytes of internal memory.",
                        DapAnC = "On the top desk shelf.",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 2 - Yes/No Question",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>Question: Has the purchase order for new laptops been approved yet?<br/>(A) Mr. Tanaka signed off on it this morning. ('signed off' = approved: Đã được ông Tanaka ký duyệt sáng nay)<br/>(B) Twenty gigabytes... (Thông số kỹ thuật)<br/>-> Đáp án đúng: <strong>A</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(toeicP2Questions);
                context.SaveChanges();

                // ── Part 3: Short Conversations (Listening) (Câu 11 - 15) ──
                var toeicPart3 = new PhanThi
                {
                    MaDeThi = deToeic.MaDeThi,
                    TenPhanThi = "Part 3: Short Conversations (Listening)",
                    ThuTu = 3,
                    TieuDePhan = "Questions 11-15: Listen to the dialogue and select the best answer for each question.",
                    TagDangBai = "#TOEIC|Conversations #Listening",
                    FileAudio = "/audio/toeic-part3.wav"
                };
                context.PhanThis.Add(toeicPart3);
                context.SaveChanges();

                var toeicP3Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart3.MaPhanThi,
                        SoThuTu = 11,
                        NoiDung = "What department does Maria work in?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Human resources",
                        DapAnB = "Accounting and finance",
                        DapAnC = "Legal compliance",
                        DapAnD = "Customer public relations",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 3 - Conversations",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Hello Mr. Chen, this is Maria from human resources.'<br/><strong>Lý do giải thích:</strong> Maria tự giới thiệu mình đến từ phòng Nhân sự (Human Resources). Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart3.MaPhanThi,
                        SoThuTu = 12,
                        NoiDung = "What position are the speakers currently discussing?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Lead software architect",
                        DapAnB = "Marketing campaign director",
                        DapAnC = "Senior financial auditor",
                        DapAnD = "Head office receptionist",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 3 - Conversations",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'I am reviewing the applications for the lead software architect position.'<br/><strong>Lý do giải thích:</strong> Vị trí tuyển dụng là Kiến trúc sư phần mềm trưởng (lead software architect). Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart3.MaPhanThi,
                        SoThuTu = 13,
                        NoiDung = "Why does candidate David Miller stand out to Mr. Chen?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "He graduated top of his class from university",
                        DapAnB = "He has eight years of cloud architecture experience",
                        DapAnC = "He is fluent in four international languages",
                        DapAnD = "He requested the lowest salary in the applicant pool",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 3 - Conversations",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Candidate David Miller stands out because he has eight years of cloud architecture experience and glowing references...'<br/><strong>Lý do giải thích:</strong> Ứng viên nổi bật vì có 8 năm kinh nghiệm kiến trúc đám mây. Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart3.MaPhanThi,
                        SoThuTu = 14,
                        NoiDung = "When will the interview with David Miller most likely take place?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Next Tuesday at 10:00 AM",
                        DapAnB = "This Friday afternoon at 2:00 PM",
                        DapAnC = "Next Monday morning at 9:00 AM",
                        DapAnD = "In three weeks after budget approval",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 3 - Conversations",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> 'Should I schedule an in-person interview for next Tuesday morning? - Yes please, ten AM would work perfectly...'<br/><strong>Lý do giải thích:</strong> Phỏng vấn được chốt vào sáng thứ Ba tuần tới lúc 10 giờ. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart3.MaPhanThi,
                        SoThuTu = 15,
                        NoiDung = "What will Maria most likely do next?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Contact David Miller to confirm the interview slot",
                        DapAnB = "Cancel the online job advertisement",
                        DapAnC = "Negotiate the starting remuneration package",
                        DapAnD = "Send rejection letters to all other applicants",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Listening] Part 3 - Conversations",
                        GiaiThichChiTiet = "<strong>Lý do giải thích:</strong> Sau khi thống nhất lịch với ông Chen, bước tiếp theo của Maria là liên hệ với ứng viên để xếp lịch hẹn. Đáp án đúng: <strong>A</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(toeicP3Questions);
                context.SaveChanges();

                // ── Part 5: Incomplete Sentences (Reading) (Câu 16 - 20) ──
                var toeicPart5 = new PhanThi
                {
                    MaDeThi = deToeic.MaDeThi,
                    TenPhanThi = "Part 5: Incomplete Sentences (Reading)",
                    ThuTu = 4,
                    TieuDePhan = "Directions: Select the best answer to complete each sentence.",
                    TagDangBai = "#TOEIC|Grammar & Vocabulary"
                };
                context.PhanThis.Add(toeicPart5);
                context.SaveChanges();

                var toeicP5Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart5.MaPhanThi,
                        SoThuTu = 16,
                        NoiDung = "Mr. Henderson requested that all expense reports be submitted directly to _______ by Friday afternoon.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "he",
                        DapAnB = "him",
                        DapAnC = "his",
                        DapAnD = "himself",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Reading] Pronouns",
                        GiaiThichChiTiet = "<strong>Phân tích ngữ pháp:</strong> Sau giới từ 'to' cần đại từ tân ngữ (Object Pronoun) 'him'.<br/><strong>Dịch nghĩa:</strong> Ông Henderson yêu cầu nộp báo cáo chi phí trực tiếp cho ông ấy.<br/>-> Đáp án: <strong>B (him)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart5.MaPhanThi,
                        SoThuTu = 17,
                        NoiDung = "Due to unprecedented demand, the delivery of your customized office furniture may be _______ delayed.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "slight",
                        DapAnB = "slightly",
                        DapAnC = "slightness",
                        DapAnD = "slighted",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Reading] Word Form",
                        GiaiThichChiTiet = "<strong>Phân tích:</strong> Đứng giữa 'may be' và phân từ 'delayed' cần một trạng từ (Adverb) 'slightly' bổ nghĩa cho 'delayed'.<br/>-> Đáp án: <strong>B (slightly)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart5.MaPhanThi,
                        SoThuTu = 18,
                        NoiDung = "The newly appointed CEO is widely _______ for turning around struggling startups within six months.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "praised",
                        DapAnB = "applied",
                        DapAnC = "invited",
                        DapAnD = "located",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Vocabulary",
                        GiaiThichChiTiet = "<strong>Cụm từ:</strong> 'be praised for something' = được ca ngợi, khen thưởng vì thành tích.<br/>-> Đáp án: <strong>A (praised)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart5.MaPhanThi,
                        SoThuTu = 19,
                        NoiDung = "Employees wishing to enroll in the advanced leadership workshop must submit their application _______ October 30.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "into",
                        DapAnB = "before",
                        DapAnC = "during",
                        DapAnD = "among",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Reading] Prepositions",
                        GiaiThichChiTiet = "<strong>Giới từ thời gian:</strong> 'before + mốc thời gian' = trước ngày 30 tháng 10.<br/>-> Đáp án: <strong>B (before)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart5.MaPhanThi,
                        SoThuTu = 20,
                        NoiDung = "The customer technical assistance hotline operates _______ 24 hours a day to assist overseas clients.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "continuous",
                        DapAnB = "continuously",
                        DapAnC = "continuation",
                        DapAnD = "continuity",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[TOEIC Reading] Word Form",
                        GiaiThichChiTiet = "<strong>Phân tích:</strong> Bổ nghĩa cho động từ 'operates' (hoạt động) cần một trạng từ 'continuously' (liên tục).<br/>-> Đáp án: <strong>B (continuously)</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(toeicP5Questions);
                context.SaveChanges();

                // ── Part 7: Reading Comprehension (Reading) (Câu 21 - 25) ──
                var toeicPart7 = new PhanThi
                {
                    MaDeThi = deToeic.MaDeThi,
                    TenPhanThi = "Part 7: Reading Comprehension (Reading)",
                    ThuTu = 5,
                    TieuDePhan = "Questions 21-25 refer to the following internal company memorandum.",
                    TagDangBai = "#TOEIC|Reading #Email/Memo",
                    DoanVanDoc = @"
<div class='p-3 bg-white rounded border shadow-sm mb-3' style='font-size:13.5px; line-height:1.6;'>
    <div class='border-bottom pb-2 mb-2'>
        <strong>MEMORANDUM</strong><br/>
        <strong>TO:</strong> All Staff - NexaTech Solutions<br/>
        <strong>FROM:</strong> Jason Gallagher, Chief Information Officer (CIO)<br/>
        <strong>DATE:</strong> October 14, 2026<br/>
        <strong>SUBJECT:</strong> Scheduled Enterprise Server Upgrade & Network Maintenance
    </div>
    <p>Please be advised that the Information Technology department will conduct a major upgrade of our primary data storage servers this coming weekend. The maintenance period will commence on <strong>Friday, October 17 at 11:00 PM</strong> and conclude no later than <strong>Sunday, October 19 at 6:00 PM</strong>.</p>
    <p>During this window, internal network drives, corporate email servers, and remote VPN access will experience unavoidable interruptions. Employees are strongly advised to save all active project files and back up critical deliverables to their local desktop drives prior to leaving the office on Friday afternoon.</p>
    <p>If your department handles urgent client emergencies over the weekend, an on-call IT emergency helpdesk team can be reached via hotline extension 4402.</p>
</div>"
                };
                context.PhanThis.Add(toeicPart7);
                context.SaveChanges();

                var toeicP7Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart7.MaPhanThi,
                        SoThuTu = 21,
                        NoiDung = "What is the primary purpose of the memorandum?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "To announce a scheduled server upgrade and network maintenance window",
                        DapAnB = "To introduce a newly hired chief information officer",
                        DapAnC = "To permanently deactivate employee remote VPN access",
                        DapAnD = "To report a serious corporate cybersecurity breach",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Main Purpose",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> 'SUBJECT: Scheduled Enterprise Server Upgrade & Network Maintenance'.<br/><strong>Lý do giải thích:</strong> Mục đích thông báo kế hoạch nâng cấp máy chủ vào dịp cuối tuần. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart7.MaPhanThi,
                        SoThuTu = 22,
                        NoiDung = "When is the maintenance operation scheduled to begin?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Friday, October 17 at 11:00 PM",
                        DapAnB = "Saturday, October 18 at 8:00 AM",
                        DapAnC = "Sunday, October 19 at 6:00 PM",
                        DapAnD = "Monday morning at normal business opening",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Detail Question",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...commence on Friday, October 17 at 11:00 PM...'<br/><strong>Lý do giải thích:</strong> 'commence' = begin: Bắt đầu lúc 11:00 tối thứ Sáu ngày 17/10. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart7.MaPhanThi,
                        SoThuTu = 23,
                        NoiDung = "What are employees instructed to do before leaving on Friday afternoon?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Save and back up critical files locally on their desktop drives",
                        DapAnB = "Unplug all monitors and office printers",
                        DapAnC = "Change their company network login passwords",
                        DapAnD = "Submit overtime work applications for the weekend",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Detail Question",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...strongly advised to save all active project files and back up critical deliverables to their local desktop drives...'<br/><strong>Lý do giải thích:</strong> Nhân viên được yêu cầu lưu và sao lưu tài liệu quan trọng về máy tính trước khi về. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart7.MaPhanThi,
                        SoThuTu = 24,
                        NoiDung = "How can staff obtain emergency IT assistance during the maintenance window?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "By calling the on-call emergency helpdesk hotline at extension 4402",
                        DapAnB = "By contacting Jason Gallagher directly on his private cell phone",
                        DapAnC = "By visiting the third floor server room in person",
                        DapAnD = "By waiting until Monday morning to file a support ticket",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Detail Question",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...an on-call IT emergency helpdesk team can be reached via hotline extension 4402.'<br/><strong>Lý do giải thích:</strong> Gọi số máy lẻ đường dây nóng 4402. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = toeicPart7.MaPhanThi,
                        SoThuTu = 25,
                        NoiDung = "In paragraph 2, the word 'unavoidable' is closest in meaning to:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "inevitable",
                        DapAnB = "optional",
                        DapAnC = "temporary",
                        DapAnD = "unacceptable",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[TOEIC Reading] Vocabulary in Context",
                        GiaiThichChiTiet = "<strong>Phân tích ngữ cảnh:</strong> 'experience unavoidable interruptions' = chịu những sự gián đoạn không thể tránh khỏi.<br/>- 'inevitable' (tính từ: chắc chắn xảy ra, không thể tránh khỏi) đồng nghĩa hoàn toàn với 'unavoidable'.<br/>-> Đáp án đúng: <strong>A (inevitable)</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(toeicP7Questions);
                context.SaveChanges();

                // =========================================================================
                // ĐỀ 3: IELTS Academic Reading Test 1 (14 câu liên tục)
                // =========================================================================
                var deIeltsReading = new DeThi
                {
                    TieuDe = "IELTS Collection 1 Reading Test 1",
                    MoTa = "Đề thi đọc hiểu IELTS Academic chuẩn với 2 Passage chuyên sâu: Lịch sử Trồng trà & Đột phá AI trong Y học hiện đại.",
                    TheLoai = "IELTS Academic",
                    KyNang = "Reading",
                    ThoiGianPhut = 40,
                    TongSoCau = 14,
                    SoLuotLam = 241248,
                    HinhAnh = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=600&auto=format&fit=crop&q=80",
                    NgayTao = DateTime.Now.AddDays(-45),
                    TrangThai = true
                };
                context.DeThis.Add(deIeltsReading);
                context.SaveChanges();

                // ── Passage 1: The Evolution of Tea Cultivation (Câu 1 - 7) ──
                var pass1 = new PhanThi
                {
                    MaDeThi = deIeltsReading.MaDeThi,
                    TenPhanThi = "Passage 1: Evolution of Tea",
                    ThuTu = 1,
                    TieuDePhan = "Questions 1-7: Read the passage below and answer the questions.",
                    TagDangBai = "#Reading|True/False/Not Given #Reading|Multiple Choice",
                    DoanVanDoc = @"
<div class='reading-passage p-3 bg-light rounded border mb-3' style='font-size:13.5px; line-height:1.6;'>
    <h5 class='fw-bold text-center text-primary mb-3'>THE EVOLUTION OF TEA CULTIVATION</h5>
    <p>Tea is the most widely consumed beverage in the world after water. The story of tea begins in ancient China, over 4,000 years ago. According to legend, in 2737 BC, Chinese Emperor Shen Nong was sitting beneath a wild Camellia sinensis tree while his servant boiled drinking water, when dry leaves blew into the boiling pot, creating a fragrant infusion.</p>
    <p>Throughout the Tang Dynasty (618-907 AD), tea drinking evolved from an obscure medicinal tonic into an esteemed national pastime. The scholar Lu Yu penned 'The Classic of Tea' (Cha Jing), the first known definitive monograph documenting cultivation methods, ceramic teaware, and philosophical etiquette.</p>
    <p>During the 17th century, Dutch maritime merchants first introduced Chinese green and black tea to Europe, where it initially remained an extravagant luxury reserved exclusively for royal courts and wealthy aristocrats. It was not until the British East India Company established intensive plantations in Assam and Darjeeling in northern India during the 1830s that tea became an affordable everyday household staple for the working class.</p>
</div>"
                };
                context.PhanThis.Add(pass1);
                context.SaveChanges();

                var qPass1 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 1,
                        NoiDung = "According to legend, Emperor Shen Nong deliberately invented tea as an herbal medicine.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "TRUE",
                        DapAnB = "FALSE",
                        DapAnC = "NOT GIVEN",
                        DapAnDung = "FALSE",
                        PhanLoaiDangBai = "[Reading] True/False/Not Given",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...when dry leaves blew into the boiling pot, creating a fragrant infusion.'<br/><strong>Lý do giải thích:</strong> Truyền thuyết kể rằng lá trà tình cờ bị gió thổi rơi vào nồi nước sôi (tình cờ), chứ không phải do Hoàng đế cố tình (deliberately) chế tạo. Đáp án đúng: <strong>FALSE</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 2,
                        NoiDung = "Lu Yu's 'Cha Jing' was the earliest written treatise devoted exclusively to tea.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "TRUE",
                        DapAnB = "FALSE",
                        DapAnC = "NOT GIVEN",
                        DapAnDung = "TRUE",
                        PhanLoaiDangBai = "[Reading] True/False/Not Given",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...the first known definitive monograph documenting cultivation methods...'<br/><strong>Lý do giải thích:</strong> 'The first known definitive monograph' trùng khớp với 'earliest written treatise devoted exclusively'. Đáp án đúng: <strong>TRUE</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 3,
                        NoiDung = "Dutch traders exported tea to South America during the 17th century.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "TRUE",
                        DapAnB = "FALSE",
                        DapAnC = "NOT GIVEN",
                        DapAnDung = "NOT GIVEN",
                        PhanLoaiDangBai = "[Reading] True/False/Not Given",
                        GiaiThichChiTiet = "<strong>Lý do giải thích:</strong> Đoạn văn chỉ nhắc đến thương nhân Hà Lan mang trà đến châu Âu (Europe), hoàn toàn không đề cập đến Nam Mỹ (South America). Đáp án đúng: <strong>NOT GIVEN</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 4,
                        NoiDung = "During the Tang Dynasty, tea transformed from a medicinal remedy into:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "An esteemed national pastime",
                        DapAnB = "An imperial tax currency",
                        DapAnC = "A secret religious elixir",
                        DapAnD = "An export monopoly to Japan",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...evolved from an obscure medicinal tonic into an esteemed national pastime.'<br/><strong>Lý do:</strong> Đáp án đúng là <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 5,
                        NoiDung = "In 17th-century Europe, tea was initially accessible only to:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Royal courts and wealthy aristocrats",
                        DapAnB = "Merchant sailors and ship captains",
                        DapAnC = "Working-class factory laborers",
                        DapAnD = "Monks in remote monasteries",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...reserved exclusively for royal courts and wealthy aristocrats.'<br/><strong>Lý do:</strong> Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 6,
                        NoiDung = "Tea became widely affordable to British commoners primarily because:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Tariffs on Chinese tea imports were abolished",
                        DapAnB = "Large-scale plantations were established in Assam and Darjeeling",
                        DapAnC = "Steamships reduced transportation times across the ocean",
                        DapAnD = "Domestic tea gardens were cultivated in Southern England",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...established intensive plantations in Assam and Darjeeling... that tea became an affordable everyday household staple...'<br/><strong>Lý do:</strong> Đáp án đúng: <strong>B</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass1.MaPhanThi,
                        SoThuTu = 7,
                        NoiDung = "The botanical Latin name of the tea plant mentioned in paragraph 1 is:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Camellia sinensis",
                        DapAnB = "Coffea arabica",
                        DapAnC = "Mentha piperita",
                        DapAnD = "Theobroma cacao",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí bài đọc:</strong> '...beneath a wild Camellia sinensis tree...'<br/><strong>Lý do:</strong> Đáp án đúng là <strong>A</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(qPass1);
                context.SaveChanges();

                // ── Passage 2: AI in Healthcare (Câu 8 - 14) ──
                var pass2 = new PhanThi
                {
                    MaDeThi = deIeltsReading.MaDeThi,
                    TenPhanThi = "Passage 2: AI in Healthcare",
                    ThuTu = 2,
                    TieuDePhan = "Questions 8-14: Read the passage below and answer the questions.",
                    TagDangBai = "#Reading|Multiple Choice #Reading|Summary Completion",
                    DoanVanDoc = @"
<div class='reading-passage p-3 bg-light rounded border mb-3' style='font-size:13.5px; line-height:1.6;'>
    <h5 class='fw-bold text-center text-primary mb-3'>ARTIFICIAL INTELLIGENCE IN CLINICAL DIAGNOSTICS</h5>
    <p>The integration of machine learning algorithms into clinical healthcare has yielded remarkable advancements in diagnostic accuracy. Modern deep convolutional neural networks, trained on millions of high-resolution radiological scans, are capable of discerning microscopic malignant tissue anomalies long before they manifest visually to human clinicians.</p>
    <p>In a landmark 2023 multi-hospital clinical evaluation, a deep learning diagnostic model achieved a diagnostic sensitivity rate of 94.8% in detecting early-stage pulmonary nodules, performing on par with senior board-certified radiologists while reducing scan evaluation time by two-thirds.</p>
    <p>Nevertheless, significant ethical and technical challenges persist. Most notably, the 'black box' phenomenon poses a serious hurdle: algorithmic models rarely offer transparent interpretability for their decision-making pathways. Consequently, healthcare regulatory authorities worldwide maintain that artificial intelligence must remain a supportive assistive tool rather than an autonomous medical decision maker.</p>
</div>"
                };
                context.PhanThis.Add(pass2);
                context.SaveChanges();

                var qPass2 = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 8,
                        NoiDung = "Deep learning convolutional networks in radiology are primarily designed to:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Discern microscopic malignant tissue anomalies at an early stage",
                        DapAnB = "Replace senior board-certified radiologists entirely",
                        DapAnC = "Reduce the cost of manufacturing radiological hardware",
                        DapAnD = "Automate patient billing and insurance claims",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...capable of discerning microscopic malignant tissue anomalies long before they manifest visually...'<br/><strong>Lý do:</strong> Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 9,
                        NoiDung = "In the 2023 clinical evaluation, the diagnostic sensitivity of the deep learning model was:",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "94.8%",
                        DapAnB = "85.2%",
                        DapAnC = "72.4%",
                        DapAnD = "99.9%",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...achieved a diagnostic sensitivity rate of 94.8%...'<br/><strong>Lý do:</strong> Tỷ lệ phát hiện chính xác là 94.8%. Đáp án: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 10,
                        NoiDung = "What ethical challenge is described as the 'black box' phenomenon?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Algorithmic models lack transparent interpretability in their decisions",
                        DapAnB = "Medical hardware consumes excessive electrical power",
                        DapAnC = "Patients express strong distrust toward computer screens",
                        DapAnD = "Hospital insurance policies refuse to cover digital treatment",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[Reading] Multiple Choice",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...algorithmic models rarely offer transparent interpretability for their decision-making pathways.'<br/><strong>Lý do:</strong> Hiện tượng 'hộp đen' là do thiếu tính giải thích minh bạch trong quá trình đưa ra quyết định của thuật toán. Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 11,
                        NoiDung = "AI algorithms are trained on extensive datasets of (11) _______ scans to detect early tissue anomalies.",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "radiological",
                        PhanLoaiDangBai = "[Reading] Summary Completion",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...trained on millions of high-resolution radiological scans...'<br/><strong>Từ cần điền:</strong> <strong>radiological</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 12,
                        NoiDung = "The model cut scan evaluation time by two (12) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "thirds",
                        PhanLoaiDangBai = "[Reading] Summary Completion",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...reducing scan evaluation time by two-thirds.'<br/><strong>Từ cần điền:</strong> <strong>thirds</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 13,
                        NoiDung = "The core technical hurdle of AI models is the lack of decision (13) _______",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "interpretability",
                        PhanLoaiDangBai = "[Reading] Summary Completion",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...rarely offer transparent interpretability for their decision-making...'<br/><strong>Từ cần điền:</strong> <strong>interpretability</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = pass2.MaPhanThi,
                        SoThuTu = 14,
                        NoiDung = "Regulatory bodies state AI should function as an assistive (14) _______ rather than an autonomous decision maker.",
                        LoaiCauHoi = "DienTu",
                        DapAnDung = "tool",
                        PhanLoaiDangBai = "[Reading] Summary Completion",
                        GiaiThichChiTiet = "<strong>Vị trí:</strong> '...must remain a supportive assistive tool rather than an autonomous medical decision maker.'<br/><strong>Từ cần điền:</strong> <strong>tool</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(qPass2);
                context.SaveChanges();

                // =========================================================================
                // ĐỀ 4: HSK 3 Thi thử Chuẩn Quốc Tế (10 câu liên tục - Cả Nghe & Đọc)
                // =========================================================================
                var deHsk = new DeThi
                {
                    TieuDe = "HSK 3 Luyện thi thử Đề số 1 (汉语水平考试)",
                    MoTa = "Đề luyện thi HSK 3 chuẩn theo khung Hán ngữ quốc tế gồm 2 kỹ năng: Phần 1 Nghe hiểu hội thoại thường nhật và Phần 2 Đọc hiểu ngữ pháp 600 từ vựng cốt lõi.",
                    TheLoai = "HSK 3",
                    KyNang = "Listening & Reading",
                    ThoiGianPhut = 25,
                    TongSoCau = 10,
                    SoLuotLam = 34500,
                    HinhAnh = "https://images.unsplash.com/photo-1508807526345-15e9b5f4e0ed?w=600&auto=format&fit=crop&q=80",
                    FileAudio = "/audio/toeic-part2.wav",
                    NgayTao = DateTime.Now.AddDays(-20),
                    TrangThai = true
                };
                context.DeThis.Add(deHsk);
                context.SaveChanges();

                // ── Phần 1: Nghe hiểu HSK 3 (Câu 1 - 5) ──
                var hskPart1 = new PhanThi
                {
                    MaDeThi = deHsk.MaDeThi,
                    TenPhanThi = "Phần 1: Nghe hiểu (第一部分: 听力)",
                    ThuTu = 1,
                    TieuDePhan = "听短对话，选择正确的答案 (Nghe đoạn đối thoại ngắn và chọn câu trả lời đúng).",
                    TagDangBai = "#HSK3|Listening #听力",
                    FileAudio = "/audio/toeic-part2.wav"
                };
                context.PhanThis.Add(hskPart1);
                context.SaveChanges();

                var hskP1Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart1.MaPhanThi,
                        SoThuTu = 1,
                        NoiDung = "<strong>Câu 1:</strong> 他们明天打算什么时候在图书馆门口见面？<br/><span class='text-muted small'>(Tāmen míngtiān dǎsuàn shénme shíhou zài túshūguǎn ménkǒu jiànmiàn?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "明天上午八点 (Sáng mai 8h)",
                        DapAnB = "明天下午两点 (Chiều mai 2h)",
                        DapAnC = "明天晚上七点 (Tối mai 7h)",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[HSK 3 Nghe] Thời gian",
                        GiaiThichChiTiet = "<strong>Transcript nghe:</strong><br/>男：我们明天什么时候见面？<br/>女：明天下午两点在图书馆门口见，好吗？<br/>-> Đáp án đúng: <strong>B (Chiều mai 2h)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart1.MaPhanThi,
                        SoThuTu = 2,
                        NoiDung = "<strong>Câu 2:</strong> 男的今天为什么没去公司上班？<br/><span class='text-muted small'>(Nán de jīntiān wèishénme méi qù gōngsī shàngbān?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "他身体不舒服发烧了 (Anh ấy bị sốt, người không khỏe)",
                        DapAnB = "他去外地旅游了 (Anh ấy đi du lịch)",
                        DapAnC = "今天公司放假 (Hôm nay công ty nghỉ)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3 Nghe] Nguyên nhân",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '男的昨天淋雨了，今天发烧身体不舒服，所以在家里休息。'<br/>-> Đáp án đúng: <strong>A</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart1.MaPhanThi,
                        SoThuTu = 3,
                        NoiDung = "<strong>Câu 3:</strong> 这件红色的衣服打折后多少钱？<br/><span class='text-muted small'>(Zhè jiàn hóngsè de yīfu dǎzhé hòu duōshǎo qián?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "100 块 (100 tệ)",
                        DapAnB = "200 块 (200 tệ)",
                        DapAnC = "300 块 (300 tệ)",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[HSK 3 Nghe] Giá cả",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '这件红色的原价三百，今天打折只要两百块。'<br/>-> Đáp án: <strong>B (200 tệ)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart1.MaPhanThi,
                        SoThuTu = 4,
                        NoiDung = "<strong>Câu 4:</strong> 他们打算怎么去首都国际机场？<br/><span class='text-muted small'>(Tāmen dǎsuàn zěnme qù shǒudū guójì jīchǎng?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "坐地铁去 (Đi tàu điện ngầm)",
                        DapAnB = "打出租车去 (Đi taxi)",
                        DapAnC = "坐公共汽车去 (Đi xe buýt)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3 Nghe] Phương tiện",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '现在路上经常堵车，我们还是坐地铁去机场比较快。'<br/>-> Đáp án: <strong>A (Đi tàu điện ngầm)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart1.MaPhanThi,
                        SoThuTu = 5,
                        NoiDung = "<strong>Câu 5:</strong> 女的想请服务员拿什么饮料？<br/><span class='text-muted small'>(Nǚ de xiǎng qǐng fúwùyuán ná shénme yǐnliào?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "一杯热绿茶 (Một cốc trà xanh nóng)",
                        DapAnB = "一杯冰咖啡 (Một cốc cà phê đá)",
                        DapAnC = "一瓶矿泉水 (Một chai nước khoáng)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3 Nghe] Đồ uống",
                        GiaiThichChiTiet = "<strong>Transcript:</strong> '服务员，请给我一杯热绿茶，谢谢。'<br/>-> Đáp án đúng: <strong>A</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(hskP1Questions);
                context.SaveChanges();

                // ── Phần 2: Đọc hiểu HSK 3 (Câu 6 - 10) ──
                var hskPart2 = new PhanThi
                {
                    MaDeThi = deHsk.MaDeThi,
                    TenPhanThi = "Phần 2: Đọc hiểu (第二部分: 阅读)",
                    ThuTu = 2,
                    TieuDePhan = "选词填空，选择正确的答案完成句子 (Chọn từ đúng hoàn thành câu).",
                    TagDangBai = "#HSK3|Reading #阅读"
                };
                context.PhanThis.Add(hskPart2);
                context.SaveChanges();

                var hskP2Questions = new List<CauHoiLuyenThi>
                {
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart2.MaPhanThi,
                        SoThuTu = 6,
                        NoiDung = "虽然外面在下大雪，_______ 房间里非常暖和。<br/><span class='text-muted small'>(Suīrán wàimiàn zài xià dà xuě, _______ fángjiān lǐ fēicháng nuǎnhuo.)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "因为 (yīnwèi)",
                        DapAnB = "但是 (dànshì)",
                        DapAnC = "所以 (suǒyǐ)",
                        DapAnD = "或者 (huòzhě)",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[HSK 3] Liên từ chuyển ngoặt",
                        GiaiThichChiTiet = "<strong>Cấu trúc:</strong> '虽然……，但是……' (Tuy rằng... nhưng mà...).<br/><strong>Dịch nghĩa:</strong> Mặc dù bên ngoài đang tuyết rơi to, nhưng trong phòng lại rất ấm áp.<br/>-> Đáp án đúng: <strong>B (但是)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart2.MaPhanThi,
                        SoThuTu = 7,
                        NoiDung = "桌子上放着一 _______ 刚买回来的新鲜水果。<br/><span class='text-muted small'>(Zhuōzi shàng fàngzhe yì _______ gāng mǎi huílái de xīnxiān shuǐguǒ.)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "盘 (pán - đĩa)",
                        DapAnB = "本 (běn - quyển)",
                        DapAnC = "辆 (liàng - chiếc xe)",
                        DapAnD = "只 (zhī - con vật)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3] Lượng từ",
                        GiaiThichChiTiet = "<strong>Ngữ pháp:</strong> Lượng từ cho hoa quả để trên bàn là '盘' (đĩa).<br/>-> Đáp án: <strong>A (一盘新鲜水果 - một đĩa hoa quả tươi)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart2.MaPhanThi,
                        SoThuTu = 8,
                        NoiDung = "他不但汉语说得非常流利，_______ 还会写很多漂亮的汉字。<br/><span class='text-muted small'>(Tā búdàn hànyǔ shuō de fēicháng liúlì, _______ hái huì xiě hěn duō piàoliang de hànzì.)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "但是 (dànshì)",
                        DapAnB = "而且 (érqiě)",
                        DapAnC = "如果 (rúguǒ)",
                        DapAnD = "还是 (háishì)",
                        DapAnDung = "B",
                        PhanLoaiDangBai = "[HSK 3] Liên từ tăng tiến",
                        GiaiThichChiTiet = "<strong>Cấu trúc tăng tiến:</strong> '不但……，而且……' (Không những... mà còn...).<br/>-> Đáp án đúng: <strong>B (而且)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart2.MaPhanThi,
                        SoThuTu = 9,
                        NoiDung = "你先等我五分钟，我把这些作业 _______ 完就跟你一起去超市。<br/><span class='text-muted small'>(Nǐ xiān děng wǒ wǔ fēnzhōng, wǒ bǎ zhèxiē zuòyè _______ wán jiù gēn nǐ yìqǐ qù chāoshì.)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "写 (xiě - viết)",
                        DapAnB = "喝 (hē - uống)",
                        DapAnC = "跑 (pǎo - chạy)",
                        DapAnD = "跳 (tiào - nhảy)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3] Động từ câu chữ 把",
                        GiaiThichChiTiet = "<strong>Phân tích:</strong> Tân ngữ là '作业' (bài tập về nhà), động từ phù hợp là '写完' (viết/làm xong).<br/>-> Đáp án đúng: <strong>A (写)</strong>."
                    },
                    new CauHoiLuyenThi
                    {
                        MaPhanThi = hskPart2.MaPhanThi,
                        SoThuTu = 10,
                        NoiDung = "这家餐馆的北京烤鸭在城市里很 _______，每天都有许多顾客排队。<br/><span class='text-muted small'>(Zhè jiā cānguǎn de Běijīng kǎoyā zài chéngshì lǐ hěn _______, měitiān dōu yǒu xǔduō gùkè páiduì.)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "有名 (yǒumíng - nổi tiếng)",
                        DapAnB = "难过 (nánguò - buồn rầu)",
                        DapAnC = "生气 (shēngqì - tức giận)",
                        DapAnD = "害怕 (hàipà - sợ hãi)",
                        DapAnDung = "A",
                        PhanLoaiDangBai = "[HSK 3] Tính từ miêu tả",
                        GiaiThichChiTiet = "<strong>Dịch nghĩa:</strong> Món vịt quay Bắc Kinh của quán ăn này rất nổi tiếng (有名), ngày nào cũng có đông đảo thực khách xếp hàng.<br/>-> Đáp án đúng: <strong>A (有名)</strong>."
                    }
                };
                context.CauHoiLuyenThis.AddRange(hskP2Questions);
                context.SaveChanges();

                // ── Bình luận mẫu ──
                var adminUser = context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin");
                if (adminUser != null)
                {
                    context.BinhLuanDeThis.AddRange(new List<BinhLuanDeThi>
                    {
                        new BinhLuanDeThi
                        {
                            MaDeThi = deIelts1.MaDeThi,
                            MaTaiKhoan = adminUser.MaTaiKhoan,
                            NoiDung = "📌 Đề thi Listening đã được nạp đầy đủ Audio chất lượng cao cho cả 4 Section. Hãy dùng nút chỉnh tốc độ (0.8x - 1.5x) nếu cần luyện tai nghe nhé!",
                            NgayBinhLuan = DateTime.Now.AddDays(-5)
                        },
                        new BinhLuanDeThi
                        {
                            MaDeThi = deToeic.MaDeThi,
                            MaTaiKhoan = adminUser.MaTaiKhoan,
                            NoiDung = "💡 Bộ đề TOEIC 2026 đã cập nhật đầy đủ hình ảnh Part 1 và Audio đàm thoại Part 2, Part 3. Chúc các bạn đạt điểm cao!",
                            NgayBinhLuan = DateTime.Now.AddDays(-2)
                        }
                    });
                    context.SaveChanges();
                }

                Console.WriteLine("✅ Đã khởi tạo thành công 100% hệ thống Đề thi online với đầy đủ phần Nghe Audio và câu hỏi đa dạng!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi nạp dữ liệu Luyện thi: " + ex.Message);
            }
        }
    }
}
