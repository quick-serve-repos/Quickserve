using QuickServe.Application.DTOs;

namespace QuickServe.Application.Helpers
{
    public static class TranslatorMessages
    {
        public static class AccountMessages
        {
            public static TranslatorMessageDto Tài_khoản_không_tìm_thấy_với_UserName(string userName) => new(nameof(Tài_khoản_không_tìm_thấy_với_UserName), [userName]);
            public static TranslatorMessageDto Tài_khoản_không_tìm_thấy_với_Email(string email) => new(nameof(Tài_khoản_không_tìm_thấy_với_Email), [email]);
            public static TranslatorMessageDto Tên_đăng_nhập_đã_được_sử_dụng(string userName) => new(nameof(Tên_đăng_nhập_đã_được_sử_dụng), [userName]);
            public static string Mật_khẩu_không_hợp_lệ() => nameof(Mật_khẩu_không_hợp_lệ);
            public static string Không_xác_nhận() => new(nameof(Không_xác_nhận));
            public static TranslatorMessageDto Tài_khoản_đã_tồn_tại_với_Email(string email) => new(nameof(Tài_khoản_đã_tồn_tại_với_Email), [email]);
        }

        public static class ProductMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_sản_phẩm(long id)
                => new(nameof(Không_tìm_thấy_sản_phẩm), [id.ToString()]);
        }

        public static class StoreMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_cửa_hàng(long id)
                => new(nameof(Không_tìm_thấy_cửa_hàng), [id.ToString()]);
        }

        public static class CategoryMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_danh_mục(long id)
                => new(nameof(Không_tìm_thấy_danh_mục), [id.ToString()]);
            public static TranslatorMessageDto Danh_mục_tồn_tại_mẫu_sản_phẩm(long id)
                => new(nameof(Danh_mục_tồn_tại_mẫu_sản_phẩm), [id.ToString()]);
            public static TranslatorMessageDto Tên_danh_mục_đã_tồn_tại(string name)
                => new(nameof(Tên_danh_mục_đã_tồn_tại), [name]);

        }
        public static class RequestMessage
        {
            public static TranslatorMessageDto Trường_id_không_hợp_lệ(long id)
               => new(nameof(Trường_id_không_hợp_lệ), [id.ToString()]);
        }
        public static class IngredientTypeMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_loại_nguyên_liệu(long id)
                => new(nameof(Không_tìm_thấy_loại_nguyên_liệu), [id.ToString()]);
            public static TranslatorMessageDto Loại_nguyên_liệu_có_tồn_tại_nguyên_liệu(long id)
                => new(nameof(Loại_nguyên_liệu_có_tồn_tại_nguyên_liệu), [id.ToString()]);
            public static TranslatorMessageDto Tên_loại_nguyên_liệu_đã_tồn_tại(string name)
                => new(nameof(Tên_loại_nguyên_liệu_đã_tồn_tại), [name]);
        }

        public static class IngredientMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_nguyên_liệu(long id)
                => new(nameof(Không_tìm_thấy_nguyên_liệu), [id.ToString()]);
            public static TranslatorMessageDto Nguyên_liệu_tồn_tại_trong_sản_phẩm_và_ca_làm_việc(long id)
                => new(nameof(Nguyên_liệu_tồn_tại_trong_sản_phẩm_và_ca_làm_việc), [id.ToString()]);
            public static TranslatorMessageDto Nguyên_liệu_tồn_tại_các_dinh_dưỡng(long id)
                => new(nameof(Nguyên_liệu_tồn_tại_các_dinh_dưỡng), [id.ToString()]);
            public static TranslatorMessageDto Tên_nguyên_liệu_đã_tồn_tại(string name)
                => new(nameof(Tên_nguyên_liệu_đã_tồn_tại), [name]);
            public static TranslatorMessageDto Nguyên_liệu_không_có_các_dinh_dưỡng(long id)
                => new(nameof(Nguyên_liệu_không_có_các_dinh_dưỡng), [id.ToString()]);
        }

        public static class ProductTemplateMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_mẫu_sản_phẩm(long id)
                => new(nameof(Không_tìm_thấy_mẫu_sản_phẩm), [id.ToString()]);
            public static TranslatorMessageDto Tên_mẫu_sản_phẩm_đã_tồn_tại(string name)
               => new(nameof(Tên_mẫu_sản_phẩm_đã_tồn_tại), [name]);
            public static TranslatorMessageDto Mẫu_sản_phẩm_tồn_tại_bước_không_hoạt_động(long id)
              => new(nameof(Mẫu_sản_phẩm_tồn_tại_bước_không_hoạt_động), [id.ToString()]);
            public static TranslatorMessageDto Mẫu_sản_phẩm_tồn_tại_sản_phẩm_và_bước_mẫu(long id)
                => new(nameof(Mẫu_sản_phẩm_tồn_tại_sản_phẩm_và_bước_mẫu), [id.ToString()]);
        }

        public static class TemplateStepMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_bước_mẫu(long id)
                => new(nameof(Không_tìm_thấy_bước_mẫu), [id.ToString()]);
            public static TranslatorMessageDto Tên_bước_mẫu_đã_tồn_tại(string name)
               => new(nameof(Tên_bước_mẫu_đã_tồn_tại), [name]);
            public static TranslatorMessageDto Bước_mẫu_trạng_thái_không_đúng(int status)
               => new(nameof(Bước_mẫu_trạng_thái_không_đúng), [status.ToString()]);
            public static TranslatorMessageDto Bước_mẫu_tồn_tại_loại_nguyên_liệu(long id)
               => new(nameof(Bước_mẫu_tồn_tại_loại_nguyên_liệu), [id.ToString()]);
        }
        public static class NutritionMessages
        {
            public static TranslatorMessageDto Không_tìm_thấy_dinh_dưỡng(long id)
                => new(nameof(Không_tìm_thấy_dinh_dưỡng), [id.ToString()]);
            public static TranslatorMessageDto Tên_dinh_dưỡng_đã_tồn_tại(string name)
              => new(nameof(Tên_dinh_dưỡng_đã_tồn_tại), [name]);
            public static TranslatorMessageDto Dinh_dưỡng_tồn_tại_trong_danh_sách_dinh_dưỡng_của_nguyên_liệu(long id)
             => new(nameof(Dinh_dưỡng_tồn_tại_trong_danh_sách_dinh_dưỡng_của_nguyên_liệu), [id.ToString()]);
        }
        public static class SessionMessage
        {
            public static TranslatorMessageDto Không_tìm_thấy_ca_làm_việc(long id)
                => new(nameof(Không_tìm_thấy_ca_làm_việc), [id.ToString()]);
            public static TranslatorMessageDto Tên_ca_làm_việc_đã_tồn_tại(string name)
              => new(nameof(Tên_ca_làm_việc_đã_tồn_tại), [name]);
            public static TranslatorMessageDto Thời_gian_làm_việc_đã_có_trong_ca_khác(long id)
              => new(nameof(Thời_gian_làm_việc_đã_có_trong_ca_khác), [id.ToString()]);
            public static TranslatorMessageDto Ca_làm_việc_tồn_tại_các_nguyên_liệu(long id)
                => new(nameof(Ca_làm_việc_tồn_tại_các_nguyên_liệu), [id.ToString()]);
            public static TranslatorMessageDto Thời_gian_bắt_đầu_phải_trước_thời_gian_kết_thúc(long id)
                => new(nameof(Thời_gian_bắt_đầu_phải_trước_thời_gian_kết_thúc), [id.ToString()]);
            public static TranslatorMessageDto Ca_làm_việc_không_tồn_tại_các_nguyên_liệu(long id)
                => new(nameof(Ca_làm_việc_không_tồn_tại_các_nguyên_liệu), [id.ToString()]);
        }
    }
}
