namespace Template.Application.Extensions
{
    public static class EnumExtension
    {
        public static string ToName(this Enum value)
        {
            return  value.ToString();
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

    }
}