using System.ComponentModel;

namespace QuickServe.Utils.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static long GenerateUniqueId()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
