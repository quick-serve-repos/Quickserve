namespace QuickServe.Application.Utils.Payments.Model
{
    public class VNPayConfigModel
    {
        /// <summary>
        /// Terminal ID/Mã Website (vnp_TmnCode)
        /// This value get from partner code of payment config table
        /// </summary>
        public string TerminalId { get; set; }

        /// <summary>
        /// Secret Key/Chuỗi bí mật tạo checksum (vnp_HashSecret)
        /// </summary>
        public string SecretKey { get; set; }
    }
}
