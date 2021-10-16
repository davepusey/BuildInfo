using System;
using System.Text.RegularExpressions;

namespace SetAssemblyProperties
{
    public class ProductVersion
    {
        // Basic pattern to ensure we have four numbers seperated by periods, and is therefore safe to pass to string.Split
        private const string REGEX_BASIC = "^[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+$";

        // Advanced patterns to ensure each split part is within valid ranges
        private const string REGEX_MAJOR_MINOR = "[0-1]?[0-9][0-9]?|2[0-4][0-9]|25[0-5]"; // 0 to 255
        private const string REGEX_BUILD = "([1-5][0-9]|6[0-5])(00[1-9]|0[1-9][0-9]|[1-2][0-9][0-9]|3[0-5][0-9]|36[0-6])"; // AABBB - where A is 10 to 65, and B is 1 to 366 with leading zeros
        private const string REGEX_REVISION = "([0-1][0-9]|2[0-3])[0-5][0-9]"; // AABB - where A is 0 to 23 with leading zeros, and B is 0 to 59 with leading zeros

        public ProductVersion(byte major = 1, byte minor = 0) : this(major, minor, DateTime.UtcNow) { }

        private ProductVersion(byte major, byte minor, DateTime buildDate)
        {
            if ((buildDate.Year < 2010) || (buildDate.Year > 2065)) throw new ArgumentOutOfRangeException(nameof(buildDate.Year), buildDate.Year, "Year must be between 2010 and 2065 inclusive.");

            Major = major;
            Minor = minor;

            BuildDate = buildDate;

            Build = (ushort)buildDate.Year;
            Build %= 100;
            Build *= 1000;
            Build += (ushort)buildDate.DayOfYear;

            Revision = (ushort)buildDate.Hour;
            Revision *= 100;
            Revision += (ushort)buildDate.Minute;

        }

        public byte Major { get; }

        public byte Minor { get; }

        public DateTime BuildDate { get; }

        public ushort Build { get; }

        public ushort Revision { get; }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2:D5}.{3:D4}", Major, Minor, Build, Revision);
        }

        public static bool TryParse(string s)
        {
            return TryParse(s, out _);
        }
        
        public static bool TryParse(string s, out ProductVersion p)
        {
            try
            {
                p = Parse(s);
                return true;
            }
            catch (Exception)
            {
                p = null;
                return false;
            }
        }

        public static ProductVersion Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s));

            if (Regex.IsMatch(s, REGEX_BASIC) == false) throw new ArgumentException("Invalid format. Must be four numbers seperated by periods.", nameof(s));

            string[] sParts = s.Split('.');

            if (Regex.IsMatch(sParts[0], REGEX_MAJOR_MINOR) == false) throw new ArgumentOutOfRangeException("MAJOR", sParts[0], "MAJOR is not within valid range.");
            if (Regex.IsMatch(sParts[1], REGEX_MAJOR_MINOR) == false) throw new ArgumentOutOfRangeException("MINOR", sParts[1], "MINOR is not within valid range.");
            if (Regex.IsMatch(sParts[2], REGEX_BUILD) == false) throw new ArgumentOutOfRangeException("BUILD", sParts[2], "BUILD is not within valid range.");
            if (Regex.IsMatch(sParts[3], REGEX_REVISION) == false) throw new ArgumentOutOfRangeException("REVISION", sParts[3], "REVISION is not within valid range.");

            byte major = Convert.ToByte(sParts[0]);
            byte minor = Convert.ToByte(sParts[1]);
            ushort build = Convert.ToUInt16(sParts[2]);
            ushort revision = Convert.ToUInt16(sParts[3]);

            int buildYear = 2000 + Math.DivRem(build, 1000, out int buildDayOfYear);
            int buildHour = Math.DivRem(revision, 100, out int buildMinute);

            DateTime buildDate = new DateTime(buildYear, 1, 1, buildHour, buildMinute, 0, DateTimeKind.Utc).AddDays(buildDayOfYear - 1);

            return new ProductVersion(major, minor, buildDate);
        }
    }
}