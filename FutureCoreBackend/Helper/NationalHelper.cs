using System.Text.RegularExpressions;

namespace FutureCoreBackend.Helper
{
    public class NationalHelper
    {
        public static DateTime? ExtractBirthdate(string nationalId)
        {
            var regex = new Regex(@"^([1-9]{1})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})[0-9]{3}([0-9]{1})[0-9]{1}$");
            var match = regex.Match(nationalId);

            if (match.Success)
            {
                try
                {
                    int century = (match.Groups[1].Value == "2") ? 1900 : 2000;
                    int year = century + int.Parse(match.Groups[2].Value);
                    int month = int.Parse(match.Groups[3].Value);
                    int day = int.Parse(match.Groups[4].Value);
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

    }
}
