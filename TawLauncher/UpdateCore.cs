using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows;

namespace TawLauncher
{
  public static class UpdateCore
  {
    private static string _versionFileUrl = "C:\\Users\\TheAshenWolf\\Desktop\\TawLtest\\version.txt";
    private static string _updateFileUrl = "C:\\Users\\TheAshenWolf\\Desktop\\TawLtest\\app.zip";
    private static bool _automaticallyUpdate = true;
    private static string _exeToRun = "MinecraftClient.exe"; // TODO: Validate if exe
    private static bool _runAfterUpdate = true;
    private static bool _keepZip = false;
    private static bool _keepLauncherOpen = false;

    private static bool _updateAvailable = false;

    public static Version CheckForUpdates()
    {
      Version currentVersion = CheckCurrentVersion();
      Version newVersion = CheckNewVersion();

      if (newVersion == null) throw new Exception("Couldn't get updates. Please, check your connection.");

      if (currentVersion == null || currentVersion < newVersion)
      {
        _updateAvailable = true;
        if (_automaticallyUpdate) Update();
        else return newVersion;
      }

      if (Directory.Exists("temp")) Directory.Delete("temp", true);
      return null;
    }

    public static void Update()
    {
      if (!_updateAvailable) return;
      
      if (Directory.Exists("Data")) Directory.Delete("Data", true);
      DownloadUpdateFile(_updateFileUrl);
      ZipFile.ExtractToDirectory("Data/Application.zip", "Data/Application");
      if (!_keepZip) File.Delete("Data/Application.zip");
      
      File.Copy("temp/version.txt", "Data/version.txt");
      Directory.Delete("temp", true);
      
      if (_runAfterUpdate) Run();
    }

    public static void Run()
    {
      Process.Start(Path.GetFullPath("Data/Application/" + _exeToRun));
      if (!_keepLauncherOpen) Application.Current.Shutdown();
    }

    private static Version CheckCurrentVersion()
    {
      return !Directory.Exists("Data") ? null : ReadVersionFile();
    }

    private static Version CheckNewVersion()
    {
      if (_versionFileUrl == null) return null;

      DownloadVersionFile(_versionFileUrl);
      return ReadVersionFile("temp/version.txt");
    }

    private static void CreateTempDirectory()
    {
      if (Directory.Exists("temp")) Directory.Delete("temp", true);
      Directory.CreateDirectory("temp");
    }

    private static void CreateDirectoryTree()
    {
      if (Directory.Exists("Data")) return;
      Directory.CreateDirectory("Data");
    }

    private static void DownloadVersionFile(string url)
    {
      CreateTempDirectory();

      WebClient webClient = new WebClient();
      try
      {
        webClient.DownloadFile(url, "temp/version.txt");
      }
      catch
      {
        throw new Exception("Couldn't download version file.");
      }
    }

    private static void DownloadUpdateFile(string url)
    {
      CreateDirectoryTree();

      WebClient webClient = new WebClient();
      try
      {
        webClient.DownloadFile(url, "Data/Application.zip");
      }
      catch
      {
        throw new Exception("Couldn't download update.");
      }
    }

    private static Version ReadVersionFile(string path = "Data/version.txt")
    {
      return new Version(File.ReadLines(path).First());
    }

    private static void ReadConfigFile()
    {
      // TODO:
    }
  }
}