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
      if (other is null) return false;
      return (Major == other.Major && Minor == other.Minor && Patch == other.Patch);
    }
    
    // auto-generated
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == this.GetType() && Equals((Version) obj);
    }

    private static bool Equals(Version x, Version y)
    {
      if (y is null) return false;
      return (x.Major == y.Major && x.Minor == y.Minor && x.Patch == y.Patch);
    }

    // auto-generated
    public override int GetHashCode()
    {
      unchecked
      {
        int hashCode = Major;
        hashCode = (hashCode * 397) ^ Minor;
        hashCode = (hashCode * 397) ^ Patch;
        return hashCode;
      }
    }

    public override string ToString()
    {
      return Major + "." + Minor + "." + Patch;
    }

    public static bool operator ==(Version x, Version y)
    {
      if (x is null && y is null) return true;
      if (x is null ^ y is null) return false;
      return (x.Major == y.Major && x.Minor == y.Minor && x.Patch == y.Patch);
    }

    public static bool operator !=(Version x, Version y)
    {
      if (x is null && y is null) return false;
      if (x is null ^ y is null) return true;
      return !Equals(x, y);
    }

    public static bool operator >(Version x, Version y)
    {
      if (x is null) x = new Version("0.0.0");
      if (y is null) y = new Version("0.0.0");
      
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
  }
}