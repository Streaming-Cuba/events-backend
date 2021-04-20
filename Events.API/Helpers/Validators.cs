using System.Text.RegularExpressions;

namespace Events.API.Helpers {
    public static class Validators {
        public static bool IsEmail(this string value) =>
            Regex.IsMatch(value, @"^[a-zA-z0-9\.]+@([a-z0-9]+\.)+([a-z0-9]+)");

        public static bool IsUrl(this string value) =>
            Regex.IsMatch(value, @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)");
    }
}