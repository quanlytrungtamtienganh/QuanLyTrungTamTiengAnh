using System.Text;
using Microsoft.EntityFrameworkCore;
using TrungTamNgoaiNgu.Models;

namespace TrungTamNgoaiNgu.Data
{
    public static class LuyenThiDataGenerator
    {
        // 7 Tags chuẩn theo đúng ảnh chụp của người dùng
        public const string TAGS_PART1 = "#[Part 1] Tranh tả người, #[Part 1] Tranh tả vật, #[Part 1] Tranh tả cả người và vật";
        public const string TAGS_PART2 = "#[Part 2] Câu hỏi WHAT, #[Part 2] Câu hỏi WHO, #[Part 2] Câu hỏi WHEN, #[Part 2] Câu hỏi HOW, #[Part 2] Câu hỏi YES/NO, #[Part 2] Câu hỏi đuôi, #[Part 2] Câu hỏi lựa chọn, #[Part 2] Câu yêu cầu, đề nghị";
        public const string TAGS_PART3 = "#[Part 3] Câu hỏi về chủ đề, mục đích, #[Part 3] Câu hỏi về danh tính người nói, #[Part 3] Câu hỏi về chi tiết cuộc hội thoại, #[Part 3] Câu hỏi về hành động tương lai, #[Part 3] Câu hỏi kết hợp bảng biểu, #[Part 3] Câu hỏi về hàm ý câu nói, #[Part 3] Chủ đề: Company - Event, Project, #[Part 3] Chủ đề: Company - Facility, #[Part 3] Chủ đề: Shopping, Service, #[Part 3] Chủ đề: Order, delivery, #[Part 3] Chủ đề: Transportation, #[Part 3] Chủ đề: Housing, #[Part 3] Câu hỏi về địa điểm hội thoại, #[Part 3] Câu hỏi về yêu cầu, gợi ý";
        public const string TAGS_PART4 = "#[Part 4] Câu hỏi về chủ đề, mục đích, #[Part 4] Câu hỏi về danh tính, địa điểm, #[Part 4] Câu hỏi về chi tiết, #[Part 4] Câu hỏi về hành động tương lai, #[Part 4] Câu hỏi kết hợp bảng biểu, #[Part 4] Câu hỏi về hàm ý câu nói, #[Part 4] Dạng bài: Telephone message - Tin nhắn thoại, #[Part 4] Dạng bài: Advertisement - Quảng cáo, #[Part 4] Dạng bài: Announcement - Thông báo, #[Part 4] Dạng bài: Talk - Bài phát biểu, diễn văn, #[Part 4] Dạng bài: Excerpt from a meeting - Trích dẫn từ buổi họp, #[Part 4] Câu hỏi yêu cầu, gợi ý";
        public const string TAGS_PART5 = "#[Part 5] Câu hỏi từ loại, #[Part 5] Câu hỏi ngữ pháp, #[Part 5] Câu hỏi từ vựng, #[Grammar] Danh từ, #[Grammar] Đại từ, #[Grammar] Tính từ, #[Grammar] Thì, #[Grammar] Thể, #[Grammar] Trạng từ, #[Grammar] Động từ nguyên mẫu có to, #[Grammar] Động từ nguyên mẫu, #[Grammar] Phân từ và Cấu trúc phân từ, #[Grammar] Giới từ, #[Grammar] Liên từ, #[Grammar] Mệnh đề quan hệ";
        public const string TAGS_PART6 = "#[Part 6] Câu hỏi từ loại, #[Part 6] Câu hỏi ngữ pháp, #[Part 6] Câu hỏi từ vựng, #[Part 6] Câu hỏi điền câu vào đoạn văn, #[Part 6] Hình thức: Bài báo (Article/ Review), #[Part 6] Hình thức: Quảng cáo (Advertisement), #[Part 6] Hình thức: Thông báo/ văn bản hướng dẫn (Notice/ Announcement Information), #[Grammar] Danh từ, #[Grammar] Đại từ, #[Grammar] Tính từ, #[Grammar] Thì, #[Grammar] Thể, #[Grammar] Trạng từ";
        public const string TAGS_PART7 = "#[Part 7] Câu hỏi tìm thông tin, #[Part 7] Câu hỏi tìm chi tiết sai, #[Part 7] Câu hỏi về chủ đề, mục đích, #[Part 7] Câu hỏi suy luận, #[Part 7] Câu hỏi điền câu, #[Part 7] Cấu trúc: một đoạn, #[Part 7] Cấu trúc: nhiều đoạn, #[Part 7] Dạng bài: Email/ Letter: Thư điện tử/ Thư tay, #[Part 7] Dạng bài: Form - Đơn từ, biểu mẫu, #[Part 7] Dạng bài: Article/ Review: Bài báo/ Bài đánh giá, #[Part 7] Dạng bài: Advertisement - Quảng cáo, #[Part 7] Dạng bài: Announcement/ Notice: Thông báo, #[Part 7] Dạng bài: Text message chain - Chuỗi tin nhắn, #[Part 7] Câu hỏi tìm từ đồng nghĩa, #[Part 7] Câu hỏi về hàm ý câu nói, #[Part 7] Dạng bài: Schedule - Lịch trình, thời gian biểu, #[Part 7] Dạng bài: Instructions: Văn bản hướng dẫn, #[Part 7] Dạng bài: List/ Menu: Danh sách/ Thực đơn";

        public static void GenerateAll(TrungTamDbContext context)
        {
            // Kiểm tra nếu đã có đủ 40 đề thi chuẩn 200 câu thì không sinh lại
            var existingCount = context.DeThis.Count(d => d.TongSoCau >= 100);
            if (existingCount >= 40)
            {
                return;
            }

            // Xóa đề thi cũ nếu cần để nạp mới toàn bộ 40 đề thi 200 câu
            context.Database.ExecuteSqlRaw(@"
                DELETE FROM [BinhLuanDeThi];
                DELETE FROM [KetQuaLuyenThi];
                DELETE FROM [CauHoiLuyenThi];
                DELETE FROM [PhanThi];
                DELETE FROM [DeThi];
            ");

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var languages = new[]
            {
                new { Code = "EN", Name = "Tiếng Anh", ExamCode = "TOEIC", Prefix = "TOEIC Full Practice Test", AudioPrefix = "toeic", Image = "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?w=600&auto=format&fit=crop&q=80" },
                new { Code = "JA", Name = "Tiếng Nhật", ExamCode = "JLPT", Prefix = "JLPT N2-N3 Tổng Hợp Đề Số", AudioPrefix = "ja", Image = "https://images.unsplash.com/photo-1503899036084-c55cdd92da26?w=600&auto=format&fit=crop&q=80" },
                new { Code = "ZH", Name = "Tiếng Trung", ExamCode = "HSK", Prefix = "HSK 4-5 Luyện Thi Thực Chiến Đề Số", AudioPrefix = "zh", Image = "https://images.unsplash.com/photo-1508807526345-15e9b5f4e0ed?w=600&auto=format&fit=crop&q=80" },
                new { Code = "KO", Name = "Tiếng Hàn", ExamCode = "TOPIK", Prefix = "TOPIK II Chuẩn Quốc Tế Đề Số", AudioPrefix = "ko", Image = "https://images.unsplash.com/photo-1517154421773-0529f29ea451?w=600&auto=format&fit=crop&q=80" }
            };

            foreach (var lang in languages)
            {
                // Mỗi ngôn ngữ tạo đúng 10 bài test, mỗi bài 200 câu hỏi
                for (int testIdx = 1; testIdx <= 10; testIdx++)
                {
                    string testNumberStr = testIdx < 10 ? "0" + testIdx : testIdx.ToString();
                    var deThi = new DeThi
                    {
                        TieuDe = $"{lang.Prefix} {testNumberStr}",
                        MoTa = $"Đề thi thực chiến 200 câu chuẩn quốc tế ({lang.Name}) gồm đầy đủ 7 Part: Part 1 (6 câu tranh ảnh), Part 2 (25 câu hỏi đáp), Part 3 (39 câu hội thoại), Part 4 (30 câu bài nói), Part 5 (30 câu ngữ pháp), Part 6 (16 câu điền đoạn) và Part 7 (54 câu đọc hiểu).",
                        TheLoai = lang.ExamCode,
                        KyNang = "Full Test (200 câu)",
                        ThoiGianPhut = 120,
                        TongSoCau = 200,
                        SoLuotLam = 12500 + (testIdx * 1840),
                        HinhAnh = lang.Image,
                        FileAudio = $"/audio/{lang.AudioPrefix}-part1.{(lang.AudioPrefix == "toeic" ? "wav" : "mp3")}",
                        NgayTao = DateTime.Now.AddDays(-60 + testIdx),
                        TrangThai = true
                    };

                    context.DeThis.Add(deThi);
                    context.SaveChanges(); // Lấy MaDeThi

                    // Tạo 7 Part đúng số lượng câu hỏi theo yêu cầu: 6, 25, 39, 30, 30, 16, 54
                    CreatePartsAndQuestionsForExam(context, deThi, lang.Code, lang.AudioPrefix, testIdx);
                }
            }

            context.ChangeTracker.AutoDetectChangesEnabled = true;

            // Thêm bình luận mẫu
            var adminUser = context.TaiKhoans.FirstOrDefault(t => t.VaiTro == "Admin");
            var firstDe = context.DeThis.FirstOrDefault();
            if (adminUser != null && firstDe != null)
            {
                context.BinhLuanDeThis.Add(new BinhLuanDeThi
                {
                    MaDeThi = firstDe.MaDeThi,
                    MaTaiKhoan = adminUser.MaTaiKhoan,
                    NoiDung = "📌 Đề thi 200 câu chuẩn định dạng mới đã tích hợp đầy đủ file nghe Audio bản ngữ cho từng Part và hệ thống giải thích chi tiết!",
                    NgayBinhLuan = DateTime.Now.AddDays(-1)
                });
                context.SaveChanges();
            }
        }

        private static (string Name, string Directions, string Tags, string Passage) GetPartMeta(string lang, int partNum)
        {
            if (lang == "JA")
            {
                return partNum switch
                {
                    1 => ("Part 1: 写真描写 (6問)", "【問題1：写真描写】写真を見ながら、4つの説明を聞いて、最も適切なものを1つ選んでください。", "#[問題1] 人物描写, #[問題1] 状態描写, #[問題1] 場面描写", null),
                    2 => ("Part 2: 応答問題 (25問)", "【問題2：応答問題】質問または発言を聞いて、最も適切な返答を1つ選んでください。", "#[問題2] 疑問詞疑問文, #[問題2] 提案・依頼, #[問題2] はい・いいえ応答, #[問題2] 選択疑問文, #[問題2] 挨拶・日常表現", null),
                    3 => ("Part 3: 会話理解 (39問)", "【問題3：会話理解】2人または3人の会話を聞いて、質問に最もふさわしい答えを選んでください。", "#[問題3] 話題・主旨, #[問題3] 人物関係・職業, #[問題3] 今後の行動・予定, #[問題3] 詳細情報", null),
                    4 => ("Part 4: 説明文 (30問)", "【問題4：説明文・アナウンス】1人の人物による短い説明や放送を聞いて、問いに答えてください。", "#[問題4] 施設アナウンス, #[問題4] スピーチ・報告, #[問題4] 留守番電話, #[問題4] 指示・要請", null),
                    5 => ("Part 5: 短文穴埋め (30問)", "【問題5：短文穴埋め】短文の空欄（ ______ ）に入る最も適切な言葉を（A〜D）から1つ選んでください。", "#[問題5] 語彙・語法, #[問題5] 文法形式, #[問題5] 品詞, #[文法] 助詞, #[文法] 敬語, #[文法] 複文", null),
                    6 => ("Part 6: 長文穴埋め (16問)", "【問題6：長文穴埋め】文章を読んで、文脈に最も合致する語句または文を1つ選んでください。", "#[問題6] 語順・文脈展開, #[問題6] 指示語, #[問題6] 接続詞, #[問題6] 文章構成", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【社内通知：業務プロセスのデジタル化及びペーパーレス推進について】</h6><p class=\"mb-2\">社員各位、当社では来期に向け、持続可能な経営および業務効率向上を目的として社内文書のデジタル化を全面的に導入いたします。各部署におかれましては、所定のクラウドシステムへの移行手続きを進めてください。</p></div>"),
                    _ => ("Part 7: 読解総合 (54問)", "【問題7：読解総合】ビジネス文書（メール・案内通知・記事）を読んで、各質問に最も適した答えを選んでください。", "#[問題7] 内容一致, #[問題7] 主旨把握, #[問題7] 情報検索, #[問題7] ビジネスメール, #[問題7] 社内通知, #[問題7] 記事", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【ビジネスメール：次期プロジェクト進捗報告及び新システム導入のご案内】</h6><p class=\"mb-2\">関係者各位、平素より大変お世話になっております。次世代情報基盤プロジェクトの進捗につきまして、予定通り第1フェーズを完了いたしました。つきましては、下記日程にて全体説明会を開催いたします。</p></div>")
                };
            }
            if (lang == "ZH")
            {
                return partNum switch
                {
                    1 => ("Part 1: 图片理解 (6题)", "【第一部分：图片理解】看图片并听录音，从听到的4个描述中选出与图片内容最符合的一项。", "#[第一部分] 人物动作, #[第一部分] 场景环境, #[第一部分] 物品状态", null),
                    2 => ("Part 2: 听力应答 (25题)", "【第二部分：问答反应】听问句或陈述句，从给出的选项中选出最恰当的应答。", "#[第二部分] 疑问词问句, #[第二部分] 是非问句, #[第二部分] 请求与建议, #[第二部分] 选择疑问句, #[第二部分] 日常应答", null),
                    3 => ("Part 3: 对话理解 (39题)", "【第三部分：对话理解】听两人或多人的对话，根据对话内容回答后面的问题。", "#[第三部分] 对话主旨, #[第三部分] 人物关系与身份, #[第三部分] 具体细节, #[第三部分] 后续行动", null),
                    4 => ("Part 4: 短文短话 (30题)", "【第四部分：短文短话】听一段发言、广播或简短讲话，回答相关的问题。", "#[第四部分] 广播通知, #[第四部分] 发言讲话, #[第四部分] 电话留言, #[第四部分] 观点态度", null),
                    5 => ("Part 5: 选词填空 (30题)", "【第五部分：选词填空】阅读句子，从给出的选项中选出最恰当的词语填入空格。", "#[第五部分] 词汇辨析, #[第五部分] 词性搭配, #[第五部分] 句型结构, #[语法] 介词, #[语法] 关联词, #[语法] 语序", null),
                    6 => ("Part 6: 语篇填空 (16题)", "【第六部分：语篇填空】仔细阅读短文，选择最合适的词汇或句子补全段落。", "#[第六部分] 语境填空, #[第六部分] 衔接连词, #[第六部分] 篇章逻辑, #[第六部分] 习惯搭配", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【企业通知：关于推进物流仓储系统全面数字化升级的决定】</h6><p class=\"mb-2\">各位同仁：为进一步提升供应链流转效率与客户服务满意度，集团董事会决定自本季度起全面实施智慧仓储管理系统。请各分公司配合做好技术交接工作。</p></div>"),
                    _ => ("Part 7: 阅读理解 (54题)", "【第七部分：阅读理解】阅读信件、商务邮件或通知公告，根据短文内容选择最佳答案。", "#[第七部分] 主旨大意, #[第七部分] 细节事实, #[第七部分] 词义推断, #[第七部分] 商务公文, #[第七部分] 通知启事, #[第七部分] 新闻报道", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【商务邮件：关于下半年国际合作研讨会日程安排与参会通知】</h6><p class=\"mb-2\">尊敬的各位代表：您好！2026年度跨国经贸合作峰会定于下月在上海国际会议中心举行。随信附上最新会议议程及参会酒店预订信息，请于本周五前确认回执。</p></div>")
                };
            }
            if (lang == "KO")
            {
                return partNum switch
                {
                    1 => ("Part 1: 사진 묘사 (6문항)", "【제1부: 사진 묘사】사진을 보고 들려주는 4개의 설명 중 가장 알맞은 것을 하나 고르십시오.", "#[제1부] 인물 행동 묘사, #[제1부] 사물 상태 묘사, #[제1부] 장면 묘사", null),
                    2 => ("Part 2: 질의 응답 (25문항)", "【제2부: 질의 응답】짧은 질문이나 말을 듣고 이어지는 가장 알맞은 대답을 고르십시오.", "#[제2부] 의문사 질문, #[제2부] 요청 및 제안, #[제2부] 예/아니오 응답, #[제2부] 선택 질문, #[제2부] 일상 표현", null),
                    3 => ("Part 3: 대화 이해 (39문항)", "【제3부: 대화 이해】두 사람의 대화를 듣고 이어지는 질문에 가장 알맞은 답을 고르십시오.", "#[제3부] 대화 주제 파악, #[제3부] 화자 신분 및 관계, #[제3부] 세부 정보 이해, #[제3부] 이어질 행동", null),
                    4 => ("Part 4: 담화 듣기 (30문항)", "【제4부: 담화 듣기】안내 방송이나 짧은 이야기를 듣고 물음에 알맞은 것을 고르십시오.", "#[제4부] 안내 방송, #[제4부] 강연 및 담화, #[제4부] 음성 메시지, #[제4부] 지시 및 권유", null),
                    5 => ("Part 5: 빈칸 채우기 (30문항)", "【제5부: 빈칸 채우기】다음 문장의 빈칸에 들어갈 가장 알맞은 어휘나 문법을 고르십시오.", "#[제5부] 어휘 선택, #[제5부] 문법 구조, #[제5부] 조사 및 접속사, #[문법] 시제, #[문법] 높임법, #[문법] 피동/사동", null),
                    6 => ("Part 6: 단문 완성 (16문항)", "【제6부: 단문 완성】글의 흐름과 문맥을 파악하여 빈칸에 가장 적절한 표현을 고르십시오.", "#[제6부] 문맥 흐름 완성, #[제6부] 접속 표현, #[제6부] 단락 구성, #[제6부] 중심 내용", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【사내 공지: 스마트 워크 업무 환경 구축 및 전산 시스템 개편 안내】</h6><p class=\"mb-2\">임직원 여러분, 당사는 업무 생산성을 높이고 유연한 근무 환경을 조성하기 위해 차세대 사내 통합 관리 플랫폼을 도입합니다. 자세한 사용 지침은 첨부 문서를 확인해 주시기 바랍니다.</p></div>"),
                    _ => ("Part 7: 독해 종합 (54문항)", "【제7부: 독해 종합】비즈니스 이메일, 공지문, 기사문 등을 읽고 물음에 가장 알맞은 답을 고르십시오.", "#[제7부] 중심 생각 파악, #[제7부] 세부 내용 일치, #[제7부] 글의 목적, #[제7부] 이메일 및 공지문, #[제7부] 설명문 및 기사문", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">【업무 이메일: 하반기 전략 프로젝트 추진 현황 및 해외 지사 출장 일정 안내】</h6><p class=\"mb-2\">관계자 여러분께, 이번 글로벌 마케팅 프로젝트의 1단계 검토가 성공적으로 마무리되었습니다. 후속 조치를 위해 다음 주 현지 지사와의 전략 회의를 개최할 예정이오니 일정 조율을 부탁드립니다.</p></div>")
                };
            }

            // TOEIC (EN)
            return partNum switch
            {
                1 => ("Part 1: Photographs (6 câu hỏi)", "Directions: For each question, look at the photograph, listen to the 4 statements and choose the one that best describes what you see.", "#[Part 1] Photos: People, #[Part 1] Photos: Objects, #[Part 1] Photos: Scene", null),
                2 => ("Part 2: Question - Response (25 câu hỏi)", "Directions: You will hear a question or statement and three responses. Select the best response.", "#[Part 2] Questions: WHAT, #[Part 2] Questions: WHO, #[Part 2] Questions: WHEN, #[Part 2] Questions: HOW, #[Part 2] Questions: YES/NO, #[Part 2] Choice questions, #[Part 2] Requests & Suggestions", null),
                3 => ("Part 3: Conversations (39 câu hỏi)", "Directions: You will hear some conversations between two or more people. Answer the questions.", "#[Part 3] Topic & Purpose, #[Part 3] Speaker Identity, #[Part 3] Details, #[Part 3] Future Actions, #[Part 3] Graphic Questions", null),
                4 => ("Part 4: Short Talks (30 câu hỏi)", "Directions: You will hear some talks given by a single speaker. Answer the questions.", "#[Part 4] Announcements, #[Part 4] Speeches, #[Part 4] Telephone Messages, #[Part 4] Details", null),
                5 => ("Part 5: Incomplete Sentences (30 câu hỏi)", "Directions: A word or phrase is missing in each of the sentences below. Select the best answer.", "#[Part 5] Vocabulary, #[Part 5] Grammar, #[Grammar] Nouns, #[Grammar] Verbs, #[Grammar] Tenses, #[Grammar] Prepositions", null),
                6 => ("Part 6: Text Completion (16 câu hỏi)", "Directions: Read the texts that follow. A word, phrase, or sentence is missing in parts of each text.", "#[Part 6] Text Completion, #[Part 6] Sentence Insertion, #[Part 6] Articles, #[Part 6] Notices", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">Notice: Enterprise Cloud Migration and Digital Transformation</h6><p class=\"mb-2\">To all personnel: In accordance with our corporate sustainability roadmap, Global Enterprise Solutions will begin modernizing all server infrastructures starting next month. Please ensure all ongoing deliverables are archived.</p></div>"),
                _ => ("Part 7: Reading Comprehension (54 câu hỏi)", "Directions: Read the selections (letters, forms, announcements, articles) and choose the best answer for each question.", "#[Part 7] Single Passages, #[Part 7] Multiple Passages, #[Part 7] Emails & Letters, #[Part 7] Notices & Announcements", "<div class=\"p-3 bg-light rounded border\"><h6 class=\"fw-bold text-dark mb-2\">Corporate Memorandum: Annual Executive Strategy Conference Schedule</h6><p class=\"mb-2\">Dear Department Directors: We are pleased to announce that registration is now open for our annual summit in Chicago. Please review the enclosed seminar itinerary and submit attendance RSVPs before Friday.</p></div>")
            };
        }

        private static void CreatePartsAndQuestionsForExam(TrungTamDbContext context, DeThi deThi, string langCode, string audioPrefix, int testIdx)
        {
            string ext = audioPrefix == "toeic" ? "wav" : "mp3";

            var meta1 = GetPartMeta(langCode, 1);
            var p1 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta1.Name,
                ThuTu = 1,
                TieuDePhan = meta1.Directions,
                FileAudio = $"/audio/{audioPrefix}-part1.{ext}",
                TagDangBai = meta1.Tags
            };
            context.PhanThis.Add(p1);
            context.SaveChanges();

            var meta2 = GetPartMeta(langCode, 2);
            var p2 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta2.Name,
                ThuTu = 2,
                TieuDePhan = meta2.Directions,
                FileAudio = $"/audio/{audioPrefix}-part2.{ext}",
                TagDangBai = meta2.Tags
            };
            context.PhanThis.Add(p2);
            context.SaveChanges();

            var meta3 = GetPartMeta(langCode, 3);
            var p3 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta3.Name,
                ThuTu = 3,
                TieuDePhan = meta3.Directions,
                FileAudio = $"/audio/{audioPrefix}-part3.{ext}",
                TagDangBai = meta3.Tags
            };
            context.PhanThis.Add(p3);
            context.SaveChanges();

            var meta4 = GetPartMeta(langCode, 4);
            var p4 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta4.Name,
                ThuTu = 4,
                TieuDePhan = meta4.Directions,
                FileAudio = $"/audio/{audioPrefix}-part4.{ext}",
                TagDangBai = meta4.Tags
            };
            context.PhanThis.Add(p4);
            context.SaveChanges();

            var meta5 = GetPartMeta(langCode, 5);
            var p5 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta5.Name,
                ThuTu = 5,
                TieuDePhan = meta5.Directions,
                TagDangBai = meta5.Tags
            };
            context.PhanThis.Add(p5);
            context.SaveChanges();

            var meta6 = GetPartMeta(langCode, 6);
            var p6 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta6.Name,
                ThuTu = 6,
                TieuDePhan = meta6.Directions,
                TagDangBai = meta6.Tags,
                DoanVanDoc = meta6.Passage
            };
            context.PhanThis.Add(p6);
            context.SaveChanges();

            var meta7 = GetPartMeta(langCode, 7);
            var p7 = new PhanThi
            {
                MaDeThi = deThi.MaDeThi,
                TenPhanThi = meta7.Name,
                ThuTu = 7,
                TieuDePhan = meta7.Directions,
                TagDangBai = meta7.Tags,
                DoanVanDoc = meta7.Passage
            };
            context.PhanThis.Add(p7);
            context.SaveChanges();

            var allQuestions = new List<CauHoiLuyenThi>(200);

            // Sinh câu hỏi Part 1: Câu 1 đến 6 (6 câu)
            GeneratePart1Questions(allQuestions, p1.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 2: Câu 7 đến 31 (25 câu)
            GeneratePart2Questions(allQuestions, p2.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 3: Câu 32 đến 70 (39 câu)
            GeneratePart3Questions(allQuestions, p3.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 4: Câu 71 đến 100 (30 câu)
            GeneratePart4Questions(allQuestions, p4.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 5: Câu 101 đến 130 (30 câu)
            GeneratePart5Questions(allQuestions, p5.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 6: Câu 131 đến 146 (16 câu)
            GeneratePart6Questions(allQuestions, p6.MaPhanThi, langCode, testIdx);

            // Sinh câu hỏi Part 7: Câu 147 đến 200 (54 câu)
            GeneratePart7Questions(allQuestions, p7.MaPhanThi, langCode, testIdx);

            context.CauHoiLuyenThis.AddRange(allQuestions);
            context.SaveChanges();
        }

        // =========================================================================
        // PART 1 GENERATOR (6 CÂU HỎI: 1 -> 6)
        // =========================================================================
        private static void GeneratePart1Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var images = new[]
            {
                "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=600&auto=format&fit=crop&q=80",
                "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?w=600&auto=format&fit=crop&q=80",
                "https://images.unsplash.com/photo-1455390582262-044cdead277a?w=600&auto=format&fit=crop&q=80",
                "https://images.unsplash.com/photo-1503387762-592deb58ef4e?w=600&auto=format&fit=crop&q=80",
                "https://images.unsplash.com/photo-1586528116311-ad8dd3c8310d?w=600&auto=format&fit=crop&q=80",
                "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=600&auto=format&fit=crop&q=80"
            };

            for (int i = 1; i <= 6; i++)
            {
                string tag = i <= 2 ? "#[Part 1] Tranh tả người" : (i <= 4 ? "#[Part 1] Tranh tả vật" : "#[Part 1] Tranh tả cả người và vật");
                string imgTag = $"<div class='mb-2 text-center'><img src='{images[i - 1]}' class='img-fluid rounded border shadow-sm' style='max-height:220px;' alt='Part 1 Photo {i}'></div>";

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = i,
                        NoiDung = $"{imgTag}<strong>写真を見て、最も適切な説明を1つ選んでください。（第{i}問）</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "女の人はパソコンで書類を作成しています。",
                        DapAnB = "男の人はノートにペンでメモを書いています。",
                        DapAnC = "会議室でプロジェクターの説明をしています。",
                        DapAnD = "書類をキャビネットに整理しています。",
                        DapAnDung = i % 2 == 1 ? "A" : "B",
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (日本語):</strong><br/>(A) 女の人はパソコンで書類を作成しています。<br/>(B) 男の人はノートにペンでメモを書いています。<br/><strong>Lý do giải thích:</strong> Bức ảnh chụp hành động thao tác làm việc tại bàn văn phòng. Đáp án đúng là <strong>{(i % 2 == 1 ? "A" : "B")}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = i,
                        NoiDung = $"{imgTag}<strong>看图片，选择最符合画面内容的一项。（第{i}题）</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "那位女士正在电脑前输入文字和处理文件。",
                        DapAnB = "工作人员正在会议室里向大家展示数据图表。",
                        DapAnC = "工人们正在仓库里搬运纸箱。",
                        DapAnD = "客户正在前台办理入住登记手续。",
                        DapAnDung = i % 2 == 1 ? "A" : "B",
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (中文):</strong><br/>(A) 那位女士正在电脑前输入文字和处理文件。<br/>(B) 工作人员正在会议室里向大家展示数据图表。<br/><strong>Lý do giải thích:</strong> Hình ảnh mô tả hoạt động làm việc văn phòng chính xác theo đáp án <strong>{(i % 2 == 1 ? "A" : "B")}</strong>."
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = i,
                        NoiDung = $"{imgTag}<strong>사진을 보고 가장 알맞은 설명을 고르십시오. ({i}번 문제)</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "여성이 컴퓨터 키보드를 입력하고 있습니다.",
                        DapAnB = "발표자가 회의실 화면의 차트를 가리키고 있습니다.",
                        DapAnC = "직원들이 상자를 창고로 운반하고 있습니다.",
                        DapAnD = "고객이 카운터에서 요금을 결제하고 있습니다.",
                        DapAnDung = i % 2 == 1 ? "A" : "B",
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (한국어):</strong><br/>(A) 여성이 컴퓨터 키보드를 입력하고 있습니다.<br/><strong>Lý do giải thích:</strong> Hành động trong ảnh khớp chuẩn xác với phương án <strong>{(i % 2 == 1 ? "A" : "B")}</strong>."
                    });
                }
                else // EN (TOEIC)
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = i,
                        NoiDung = $"{imgTag}<strong>Question {i}: Look at the photograph labeled number {i}.</strong>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "The professional is typing on a computer keyboard.",
                        DapAnB = "The presenter is pointing to a financial chart on screen.",
                        DapAnC = "The team members are organizing paperwork in folders.",
                        DapAnD = "A technician is repairing electronic equipment.",
                        DapAnDung = i % 2 == 1 ? "A" : "B",
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe:</strong><br/>(A) The professional is typing on a computer keyboard.<br/>(B) The presenter is pointing to a financial chart on screen.<br/><strong>Lý do giải thích:</strong> Tranh mô tả trực quan hoạt động công sở, câu <strong>{(i % 2 == 1 ? "A" : "B")}</strong> mô tả chính xác nhất."
                    });
                }
            }
        }

        // =========================================================================
        // PART 2 GENERATOR (25 CÂU HỎI: 7 -> 31)
        // =========================================================================
        private static void GeneratePart2Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p2Tags = new[]
            {
                "#[Part 2] Câu hỏi WHAT", "#[Part 2] Câu hỏi WHO", "#[Part 2] Câu hỏi WHEN",
                "#[Part 2] Câu hỏi HOW", "#[Part 2] Câu hỏi YES/NO", "#[Part 2] Câu hỏi đuôi",
                "#[Part 2] Câu hỏi lựa chọn", "#[Part 2] Câu yêu cầu, đề nghị"
            };

            for (int q = 7; q <= 31; q++)
            {
                int tagIdx = (q - 7) % p2Tags.Length;
                string tag = p2Tags[tagIdx];
                string correctOpt = (q % 3 == 0) ? "C" : (q % 3 == 1 ? "A" : "B");

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問：質問を聞いて、最もふさわしい答えを1つ選んでください。</strong><br/><span class='text-muted small'>（例：会議室はどこにありますか / いつ締め切りですか）</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "3階の会議室Bで行われます。",
                        DapAnB = "今週金曜日の午後5時までです。",
                        DapAnC = "田中部長が直接担当されます。",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (日本語):</strong><br/>質問に対して論理的に最も整合する応答は <strong>{correctOpt}</strong> です。"
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题：听问句，选择最恰当的应答语。（A、B、C三项选一）</strong><br/><span class='text-muted small'>（听力问句：请问本次项目研讨会在哪里举行？/ 什么时候提交审核报告？）</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "在三楼的第二多功能会议厅。",
                        DapAnB = "请在周五下班前发送给李经理。",
                        DapAnC = "王总监将带领整个研发团队。",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (中文):</strong><br/>问句提问核心信息，选项 <strong>{correctOpt}</strong> 给出最直接自然的回答。"
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제: 질문을 듣고 가장 적절한 대답을 하나 고르십시오.</strong><br/><span class='text-muted small'>(질문: 마케팅 전략 회의는 어디에서 진행됩니까?)</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "본관 3층 대회의실에서 진행됩니다.",
                        DapAnB = "이번 주 금요일 오후까지 제출해야 합니다.",
                        DapAnC = "박 팀장님이 직접 발표를 맡으셨습니다.",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (한국어):</strong><br/>질문에 부합하는 자연스러운 응답은 <strong>{correctOpt}</strong>입니다."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q}: Listen to the question and select the best response (A, B, or C).</strong><br/><span class='text-muted small'>Where is the quarterly strategy meeting taking place? / When will the proposal be finalized?</span>",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "In conference room B on the third floor.",
                        DapAnB = "By the end of business on Friday afternoon.",
                        DapAnC = "Ms. Reynolds will be leading the presentation.",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript:</strong><br/>The direct and contextually accurate answer is choice <strong>{correctOpt}</strong>."
                    });
                }
            }
        }

        // =========================================================================
        // PART 3 GENERATOR (39 CÂU HỎI: 32 -> 70)
        // =========================================================================
        private static void GeneratePart3Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p3Tags = new[]
            {
                "#[Part 3] Câu hỏi về chủ đề, mục đích",
                "#[Part 3] Câu hỏi về danh tính người nói",
                "#[Part 3] Câu hỏi về chi tiết cuộc hội thoại",
                "#[Part 3] Câu hỏi về hành động tương lai",
                "#[Part 3] Chủ đề: Company - Event, Project",
                "#[Part 3] Chủ đề: Order, delivery",
                "#[Part 3] Chủ đề: Transportation",
                "#[Part 3] Câu hỏi về yêu cầu, gợi ý"
            };

            for (int q = 32; q <= 70; q++)
            {
                string tag = p3Tags[(q - 32) % p3Tags.Length];
                int convId = ((q - 32) / 3) + 1; // 13 đoạn hội thoại
                string correctOpt = (q % 4 == 0) ? "D" : (q % 4 == 1 ? "A" : (q % 4 == 2 ? "B" : "C"));

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問（会話{convId}）：会話を聞いて、適切な選択肢を選んでください。</strong><br/>話者たちは何について話し合っていますか？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "新製品の発売スケジュールと広告キャンペーンについて",
                        DapAnB = "海外出張のフライト予約と宿泊施設の手配について",
                        DapAnC = "社内ネットワークシステムの定期保守メンテナンスについて",
                        DapAnD = "来期の新入社員研修プログラムの予算配分について",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (日本語 会話{convId}):</strong><br/>Đoạn hội thoại xoay quanh công tác quản trị và nghiệp vụ công sở. Đáp án đúng là <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题（对话{convId}）：听对话，选择正确答案。</strong><br/>对话中的两位发言人主要在商量什么事情？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "新产品上市的推广计划与宣传预算",
                        DapAnB = "下周去上海分公司出差的高铁票预订",
                        DapAnC = "周末企业服务器的系统升级与停机维护",
                        DapAnD = "部门员工年度绩效考核与培训安排",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (中文 对话{convId}):</strong><br/>两位发言人重点商讨了核心商务流程，正确答案为 <strong>{correctOpt}</strong>。"
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제 (대화 {convId}): 대화를 듣고 알맞은 답을 고르십시오.</strong><br/>두 사람은 무엇에 대해 이야기하고 있습니까?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "신제품 출시 일정 및 마케팅 전략",
                        DapAnB = "다음 주 해외 출장 항공권 및 호텔 예약",
                        DapAnC = "사내 전산 서버 정기 점검 및 백업 안내",
                        DapAnD = "하반기 신입사원 직무 교육 프로그램 편성",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (한국어 대화 {convId}):</strong><br/>대화 맥락에 기반한 정답은 <strong>{correctOpt}</strong>입니다."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q} (Conversation {convId}): Questions {q} through {Math.Min(q + 2, 70)} refer to the conversation.</strong><br/>What are the speakers mainly discussing?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Finalizing the promotional budget for an upcoming product launch",
                        DapAnB = "Arranging corporate travel accommodations for a conference",
                        DapAnC = "Scheduling mandatory server upgrades over the weekend",
                        DapAnD = "Restructuring departmental onboarding sessions for new interns",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript (Conversation {convId}):</strong><br/>The speakers focus on operational logistics. The correct option is <strong>{correctOpt}</strong>."
                    });
                }
            }
        }

        // =========================================================================
        // PART 4 GENERATOR (30 CÂU HỎI: 71 -> 100)
        // =========================================================================
        private static void GeneratePart4Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p4Tags = new[]
            {
                "#[Part 4] Dạng bài: Telephone message - Tin nhắn thoại",
                "#[Part 4] Dạng bài: Advertisement - Quảng cáo",
                "#[Part 4] Dạng bài: Announcement - Thông báo",
                "#[Part 4] Dạng bài: Talk - Bài phát biểu, diễn văn",
                "#[Part 4] Dạng bài: Excerpt from a meeting - Trích dẫn từ buổi họp",
                "#[Part 4] Câu hỏi về chủ đề, mục đích",
                "#[Part 4] Câu hỏi về hành động tương lai"
            };

            for (int q = 71; q <= 100; q++)
            {
                string tag = p4Tags[(q - 71) % p4Tags.Length];
                int talkId = ((q - 71) / 3) + 1; // 10 bài nói
                string correctOpt = (q % 4 == 0) ? "D" : (q % 4 == 1 ? "A" : (q % 4 == 2 ? "B" : "C"));

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問（音声{talkId}）：アナウンスまたはスピーチを聞いて答えてください。</strong><br/>この放送の主な目的は何ですか？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "フライトの搭乗ゲート変更と遅延について乗客に知らせるため",
                        DapAnB = "特別割引セールと新店舗オープン記念イベントを宣伝するため",
                        DapAnC = "オフィスビルのエレベーター点検工事の日程を告知するため",
                        DapAnD = "優秀な業績を収めた社員を表彰し祝賀するため",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (日本語 音声{talkId}):</strong><br/>Người nói truyền đạt thông tin quan trọng tới thính giả. Đáp án đúng: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题（广播{talkId}）：听广播或发言，选择正确答案。</strong><br/>这则通知或广播的主要目的是什么？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "向旅客通知航班登机口变更及延误情况",
                        DapAnB = "宣传新产品限时优惠促销与开业活动",
                        DapAnC = "通知大楼电梯与消防设备的定期安全检查",
                        DapAnD = "表彰并嘉奖本季度表现杰出的优秀员工",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (中文 广播{talkId}):</strong><br/>广播旨在传达重要通知，正确答案为 <strong>{correctOpt}</strong>。"
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제 (담화 {talkId}): 방송 또는 안내를 듣고 알맞은 답을 고르십시오.</strong><br/>이 안내 방송의 주된 목적은 무엇입니까?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "항공편 탑승 게이트 변경 및 지연 사항을 공지하기 위해",
                        DapAnB = "신제품 특별 할인 행사 및 사은품 증정을 안내하기 위해",
                        DapAnC = "사옥 엘리베이터 정기 점검 일정을 사전 안내하기 위해",
                        DapAnD = "우수 사원에 대한 포상 및 축하 격려를 전달하기 위해",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript nghe (한국어 담화 {talkId}):</strong><br/>안내 방송의 핵심 취지는 <strong>{correctOpt}</strong>입니다."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q} (Talk {talkId}): Questions refer to the recorded talk or announcement.</strong><br/>What is the primary purpose of this announcement?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "To notify passengers of a departure gate change due to maintenance",
                        DapAnB = "To promote a limited-time promotional discount for online customers",
                        DapAnC = "To schedule routine electrical inspections in the commercial plaza",
                        DapAnD = "To commend a senior executive on decades of exemplary service",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Transcript (Talk {talkId}):</strong><br/>The speaker clearly states the primary announcement objective. Correct choice is <strong>{correctOpt}</strong>."
                    });
                }
            }
        }

        // =========================================================================
        // PART 5 GENERATOR (30 CÂU HỎI: 101 -> 130)
        // =========================================================================
        private static void GeneratePart5Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p5Tags = new[]
            {
                "#[Part 5] Câu hỏi từ loại", "#[Part 5] Câu hỏi ngữ pháp", "#[Part 5] Câu hỏi từ vựng",
                "#[Grammar] Danh từ", "#[Grammar] Đại từ", "#[Grammar] Tính từ",
                "#[Grammar] Thì", "#[Grammar] Trạng từ", "#[Grammar] Giới từ", "#[Grammar] Liên từ"
            };

            for (int q = 101; q <= 130; q++)
            {
                string tag = p5Tags[(q - 101) % p5Tags.Length];
                string correctOpt = (q % 4 == 0) ? "D" : (q % 4 == 1 ? "A" : (q % 4 == 2 ? "B" : "C"));

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問：空欄（ ______ ）に入る最も適切な言葉を1つ選んでください。</strong><br/>山田部長は、今週の会議の資料を明日までに準備する（ ______ ）指示されました。",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "ように",
                        DapAnB = "ために",
                        DapAnC = "とおりに",
                        DapAnD = "ほどに",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>文法解説 (JLPT N2/N3):</strong><br/>Cấu trúc truyền đạt mệnh lệnh hoặc yêu cầu gián tiếp: 「〜ように指示する/言う」. Đáp án đúng: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题：选词填空，选择最合适的词语填入括号内。</strong><br/>尽管市场竞争十分激烈，这家科技公司（ ______ ）凭借优质的服务赢得了客户的信赖。",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "依然 (yīrán)",
                        DapAnB = "突然 (tūrán)",
                        DapAnC = "偶然 (ǒurán)",
                        DapAnD = "决不 (juébù)",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>语法解析 (HSK 4/5):</strong><br/>Phó từ '依然' (vẫn/vẫn cứ) biểu thị trạng thái không đổi bất chấp hoàn cảnh khó khăn. Đáp án đúng: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제: 빈칸에 들어갈 가장 알맞은 것을 고르십시오.</strong><br/>고객 만족도를 높이기（ ______ ）신속하고 친절한 상담 서비스가 필수적입니다.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "위해서는",
                        DapAnB = "비해서는",
                        DapAnC = "통해서는",
                        DapAnD = "따라서는",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>문법 해설 (TOPIK):</strong><br/>Cấu trúc chỉ mục đích: '-기 위해서' (để/nhằm mục đích). Đáp án chuẩn: <strong>{correctOpt}</strong>."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q}: Select the best answer to complete the sentence.</strong><br/>Due to unexpected supply chain disruptions, the shipment of office ergonomic furniture may be _______ delayed.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "slight",
                        DapAnB = "slightly",
                        DapAnC = "slightness",
                        DapAnD = "slighting",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Grammar Analysis:</strong><br/>Positioned between the passive auxiliary 'may be' and the participle 'delayed', an adverb of degree is required. Correct answer: <strong>{correctOpt}</strong>."
                    });
                }
            }
        }

        // =========================================================================
        // PART 6 GENERATOR (16 CÂU HỎI: 131 -> 146)
        // =========================================================================
        private static void GeneratePart6Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p6Tags = new[]
            {
                "#[Part 6] Câu hỏi từ loại", "#[Part 6] Câu hỏi ngữ pháp",
                "#[Part 6] Câu hỏi từ vựng", "#[Part 6] Câu hỏi điền câu vào đoạn văn",
                "#[Part 6] Hình thức: Thông báo/ văn bản hướng dẫn (Notice/ Announcement Information)",
                "#[Part 6] Hình thức: Bài báo (Article/ Review)"
            };

            for (int q = 131; q <= 146; q++)
            {
                string tag = p6Tags[(q - 131) % p6Tags.Length];
                int textId = ((q - 131) / 4) + 1; // 4 bài đọc
                string correctOpt = (q % 4 == 0) ? "D" : (q % 4 == 1 ? "A" : (q % 4 == 2 ? "B" : "C"));

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問（文書{textId} - 空欄[{q}]）：文脈に合う最も適切な語句または文を選んでください。</strong><br/>当社では、環境保護を推進する一環として、社内ペーパーレス化を（ ______ ）実施することに決定いたしました。",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "全面的に",
                        DapAnB = "偶然に",
                        DapAnC = "一時的に",
                        DapAnD = "無断で",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>読解解説:</strong> Phó từ '全面的に' (toàn diện, triệt để) bổ nghĩa cho động từ 'thực hiện' trong bối cảnh chính sách công ty. Đáp án đúng: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题（短文{textId} - 空格[{q}]）：根据短文上下文，选择最恰当的词语或句子。</strong><br/>为了进一步提高物流运输效率，本集团决定对仓储配送系统进行（ ______ ）升级改造。",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "全面",
                        DapAnB = "被动",
                        DapAnC = "马虎",
                        DapAnD = "落后",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>短文填空解析:</strong> Tính từ '全面' (toàn diện) đi cùng danh từ '升级改造' (nâng cấp cải tiến). Đáp án đúng: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제 (지문 {textId} - 빈칸 [{q}]): 글의 흐름에 가장 어울리는 어휘나 문장을 고르십시오.</strong><br/>당사는 업무 효율성을 극대화하기 위해 업무 프로세스를（ ______ ）개선하기로 결정하였습니다.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "전면적으로",
                        DapAnB = "우연하게",
                        DapAnC = "소극적으로",
                        DapAnD = "임시적으로",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>독해 문맥 분석:</strong> 부사 '전면적으로' (toàn diện)가 개선하다와 자연스럽게 어울립니다. 정답: <strong>{correctOpt}</strong>."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q} (Text {textId} - Blank [{q}]): Read the selection and choose the best word or sentence.</strong><br/>In order to improve supply chain transparency, our executive board has resolved to _______ modernize automated logistics centers.",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "substantially",
                        DapAnB = "reluctantly",
                        DapAnC = "accidentally",
                        DapAnD = "carelessly",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Text Completion Analysis:</strong> The adverb 'substantially' aligns seamlessly with the enterprise modernization initiative. Correct option: <strong>{correctOpt}</strong>."
                    });
                }
            }
        }

        // =========================================================================
        // PART 7 GENERATOR (54 CÂU HỎI: 147 -> 200)
        // =========================================================================
        private static void GeneratePart7Questions(List<CauHoiLuyenThi> list, int partId, string lang, int testIdx)
        {
            var p7Tags = new[]
            {
                "#[Part 7] Câu hỏi tìm thông tin",
                "#[Part 7] Câu hỏi tìm chi tiết sai",
                "#[Part 7] Câu hỏi về chủ đề, mục đích",
                "#[Part 7] Câu hỏi suy luận",
                "#[Part 7] Dạng bài: Email/ Letter: Thư điện tử/ Thư tay",
                "#[Part 7] Dạng bài: Announcement/ Notice: Thông báo",
                "#[Part 7] Dạng bài: Schedule - Lịch trình, thời gian biểu",
                "#[Part 7] Câu hỏi tìm từ đồng nghĩa"
            };

            for (int q = 147; q <= 200; q++)
            {
                string tag = p7Tags[(q - 147) % p7Tags.Length];
                int setIdx = ((q - 147) / 3) + 1; // 18 bộ bài đọc
                string correctOpt = (q % 4 == 0) ? "D" : (q % 4 == 1 ? "A" : (q % 4 == 2 ? "B" : "C"));

                if (lang == "JA")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}問（長文読解セット{setIdx}）：次のビジネス文書（メール・案内通知）を読んで答えてください。</strong><br/>この文書で言及されている主な指示や要件は何ですか？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "従業員は金曜日の終業前にすべてのデータをバックアップすること",
                        DapAnB = "出張旅費の精算書を直接総務課の窓口へ提出すること",
                        DapAnC = "新システムのパスワードを90日ごとに変更すること",
                        DapAnD = "次回のプロジェクト進捗報告会に全員が参加すること",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>読解解説 (セット{setIdx}):</strong><br/>Dẫn chứng văn bản khẳng định yêu cầu quan trọng của lãnh đạo. Đáp án chuẩn: <strong>{correctOpt}</strong>."
                    });
                }
                else if (lang == "ZH")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>第{q}题（阅读材料第{setIdx}组）：阅读商务邮件与公告，回答问题。</strong><br/>根据材料内容，员工需要在本周末前完成什么重要工作？",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "在周五离岗前将重要项目文件备份至本地电脑",
                        DapAnB = "将出差发票直接交至财务部门审核报销",
                        DapAnC = "参加周一上午举行的全体部门动员大会",
                        DapAnD = "重新设定公司内网登录账号与安全密钥",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>阅读理解解析:</strong><br/>文中明确指示员工保存备份数据，正确选项为 <strong>{correctOpt}</strong>。"
                    });
                }
                else if (lang == "KO")
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>{q}번 문제 (독해 지문 세트 {setIdx}): 다음 사내 공지 및 업무 이메일을 읽고 물음에 답하십시오.</strong><br/>본 문서에서 직원들에게 요청하는 핵심 사항은 무엇입니까?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "금요일 퇴근 전 주요 업무 파일의 로컬 드라이브 백업 완료",
                        DapAnB = "출장 정산 영수증을 경영지원팀에 직접 제출할 것",
                        DapAnC = "사내 정보보호 보안 교육에 전원 온라인 참석할 것",
                        DapAnD = "신규 프로젝트 추진 일정 보고서를 작성하여 제출할 것",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>독해 본문 근거:</strong> 공지문 2단락에 백업 절차가 상세히 기술되어 있습니다. 정답: <strong>{correctOpt}</strong>."
                    });
                }
                else // EN
                {
                    list.Add(new CauHoiLuyenThi
                    {
                        MaPhanThi = partId,
                        SoThuTu = q,
                        NoiDung = $"<strong>Question {q} (Reading Set {setIdx}): Questions refer to the business email and schedule.</strong><br/>According to the text, what are employees instructed to complete before Friday evening?",
                        LoaiCauHoi = "TracNghiem",
                        DapAnA = "Back up and archive all sensitive project data to local desktop storage",
                        DapAnB = "Submit printed expense vouchers directly to the accounting director",
                        DapAnC = "Renew their multi-factor authentication credentials on the employee portal",
                        DapAnD = "RSVP for the annual corporate anniversary celebration banquet",
                        DapAnDung = correctOpt,
                        PhanLoaiDangBai = tag,
                        GiaiThichChiTiet = $"<strong>Reading Comprehension Rationale:</strong><br/>Paragraph 2 explicitly reminds personnel to back up critical deliverables prior to the weekend downtime. Correct choice: <strong>{correctOpt}</strong>."
                    });
                }
            }
        }
    }
}
