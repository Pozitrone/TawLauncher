using System;

namespace TawLauncher
{
  public class Version
  {
    private int Major { get; }
    private int Minor { get; }
    private int Patch { get; }

    // Construct

    public Version(string version, char delimiter = '.')
    {
      string[] versionSplit = version.Split(delimiter);
      if (versionSplit.Length != 3)
        throw new Exception("Version has to be in format Major.Minor.Patch. (Eg: 1.0.2)");

      Major = int.Parse(versionSplit[0]);
      Minor = int.Parse(versionSplit[1]);
      Patch = int.Parse(versionSplit[2]);
    }

    public Version(int major, int minor, int patch)
    {
      Major = major;
      Minor = minor;
      Patch = patch;
    }

    // Methods

    private bool Equals(Version other)
    {
      return (IsNull == other.IsNull) || (Major == other.Major && Minor == other.Minor && Patch == other.Patch);
    }
    
    // auto-generated
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((Version) obj);
    }

    // auto-generated
    public override int GetHashCode()
    {
      unchecked
      {
        int hashCode = Major;
        hashCode = (hashCode * 397) ^ Minor;
        hashCode = (hashCode * 397) ^ Patch;
        hashCode = (hashCode * 397) ^ IsNull.GetHashCode();
        return hashCode;
      }
    }

    public override string ToString()
    {
      return Major + "." + Minor + "." + Patch;
    }

    public static bool operator ==(Version x, Version y)
    {
      return Equals(x, y);
    }

    public static bool operator !=(Version x, Version y)
    {
      return !Equals(x, y);
    }

    public static bool operator >(Version x, Version y)
    {
      if (x.Major > y.Major) return true;
      if (x.Major < y.Major) return false;

      if (x.Minor > y.Minor) return true;
      if (x.Minor < y.Minor) return false;

      return x.Patch > y.Patch;
    }

    public static bool operator >=(Version x, Version y)
    {
      return x == y || x > y;
    }

    public static bool operator <=(Version x, Version y)
    {
      return !(x > y);
    }

    public static bool operator <(Version x, Version y)
    {
      return !(x >= y);
    }

    public bool IsNull { get; }
  }
}