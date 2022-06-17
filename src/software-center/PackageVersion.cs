using System.Linq;
using System.Text.RegularExpressions;

namespace software_center
{
    public class PackageVersion
    {
        public int Major { get; }
        public int Minor { get; }
        public int Build { get; }
        public int Revision { get; }
        public bool IsValid { get; }

        public PackageVersion(string data)
        {
            var parts = Regex.Split(
                data,
                "[^\\d]+"
            ).Where(IsNumeric)
                .ToArray();
            IsValid = parts.Any();
            Major = TryParseInt(parts.At(0));
            Minor = TryParseInt(parts.At(1));
            Build = TryParseInt(parts.At(2));
            Revision = TryParseInt(parts.At(3));
        }

        private bool IsNumeric(string arg)
        {
            return arg is not null && Regex.IsMatch(arg, "\\d+");
        }

        private PackageVersion(
            int major,
            int minor,
            int build,
            int revision
        )
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
        }

        private int TryParseInt(string str)
        {
            return int.TryParse(str, out var result)
                ? result
                : 0;
        }

        public static PackageVersion Parse(string data)
        {
            var result = new PackageVersion(data);
            return result.IsValid
                ? result
                : null;
        }

        public static bool TryParse(
            string data,
            out PackageVersion version
        )
        {
            try
            {
                version = new PackageVersion(data);
                version = version.IsValid
                    ? version
                    : null;
                return version is not null;
            }
            catch
            {
                version = default;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not PackageVersion pkgVer)
            {
                return false;
            }

            return Major == pkgVer.Major &&
                Minor == pkgVer.Minor &&
                Build == pkgVer.Build &&
                Revision == pkgVer.Revision;
        }

        public static bool operator ==(PackageVersion a, PackageVersion b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator >(PackageVersion a, PackageVersion b)
        {
            if (a is null || b is null)
            {
                return false;
            }

            if (a.Major > b.Major)
            {
                return true;
            }

            if (a.Minor > b.Minor)
            {
                return true;
            }

            if (a.Build > b.Build)
            {
                return true;
            }

            return a.Revision > b.Revision;
        }

        public static bool operator >=(PackageVersion a, PackageVersion b)
        {
            if (a is null || b is null)
            {
                return false;
            }

            return a > b || a == b;
        }

        public static bool operator <=(PackageVersion a, PackageVersion b)
        {
            if (a is null || b is null)
            {
                return false;
            }

            return a < b || a == b;
        }

        public static bool operator <(PackageVersion a, PackageVersion b)
        {
            return b > a;
        }

        public static bool operator !=(PackageVersion a, PackageVersion b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Build;
                hashCode = (hashCode * 397) ^ Revision;
                return hashCode;
            }
        }

        public PackageVersion Clone()
        {
            return new PackageVersion(
                Major,
                Minor,
                Build,
                Revision
            );
        }
    }
}