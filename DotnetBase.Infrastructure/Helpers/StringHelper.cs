using System;

namespace DotnetBase.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string GenerateOtpNumber(int length)
        {
            const string numbers = "1234567890";
            var otp = string.Empty;
            for (var i = 0; i < length; i++)
            {
                string character;
                do
                {
                    var index = new Random().Next(0, numbers.Length);
                    character = numbers.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character, StringComparison.Ordinal) != -1);
                otp += character;
            }

            return otp;
        }
    }
}
