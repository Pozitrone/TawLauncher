using System;
using System.Collections.Generic;
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
    private static string _versionFileUrl;
    private static string _updateFileUrl;
    public static bool _automaticallyUpdate { get; private set; } = true;
    private static string _exeToRun;
    public static bool _runAfterUpdate { get; private set; } = true;
    private static bool _keepZip;
    private static bool _keepLauncherOpen;

    public static bool _updateAvailable { get; private set; }

    public static Version CheckForUpdates()
    {
      Version currentVersion = CheckCurrentVersion();
      Version newVersion = CheckNewVersion();
      
      if (newVersion is null)
      {
        MessageBox.Show("Couldn't get updates. Please, check your connection.", "Taw Launcher");
        Application.Current.Shutdown();
      }

      if (currentVersion is null || currentVersion < newVersion)
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

      _updateAvailable = false;
    }

    public static void Run()
    {
      Process.Start(Path.GetFullPath("Data/Application/" + _exeToRun));
      if (!_keepLauncherOpen) Application.Current.Shutdown();
    }

    public static Version CheckCurrentVersion()
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
      if (!File.Exists(path)) return null;
      return new Version(File.ReadLines(path).First());
    }

    public static void ReadConfigFile()
    {
      if (!File.Exists("taw.conf"))
      {
        MessageBox.Show("Config file not found. Generating blank config file.", "Taw Launcher");
        GenerateConfigFile();
        Application.Current.Shutdown();
      }

      string[] rawConfig = File.ReadAllLines("taw.conf");
      //MessageBox.Show(rawConfig.Where(line => line != "" && line[0] != '#').ToList().Aggregate((x , y) => x + "\n" + y).ToString());
      List<string> config = rawConfig.Where(line => line != "" && line[0] != '#').Select(x => x.Split('=')[1]).ToList();

      try
      {
        _versionFileUrl = config[0];
        _updateFileUrl = config[1];
        _automaticallyUpdate = bool.Parse(config[2]);
        _exeToRun = config[3].Contains(".exe") ? config[3] : config[3] + ".exe";
        _runAfterUpdate = bool.Parse(config[4]);
        _keepZip = bool.Parse(config[5]);
        _keepLauncherOpen = bool.Parse(config[6]);
      }
      catch
      {
        MessageBox.Show("Config file invalid. Please, check the values.", "Taw Launcher");
        Application.Current.Shutdown();
      }

      ValidateConfig();
    }

    private static void GenerateConfigFile()
    {
      FileStream configFile = File.Create("taw.conf");
      configFile.Close();

      string[] config = new string[]
      {
        "# Url where the app downloads the version.txt from",
        "VERSION_FILE_URL=\n",

        "# Url to the zipped project",
        "APPLICATION_ZIP_URL=\n",

        "# Whether the app should automatically update upon startup (default FALSE)",
        "AUTOMATICALLY_UPDATE=FALSE\n",

        "# Name of the .exe file within the zip",
        "EXE_TO_RUN=\n",

        "# Automatically start the app after finishing the download (default FALSE)",
        "RUN_AFTER_UPDATE_FINISHED=FALSE\n",

        "# Whether the app should keep the zip file (default FALSE)",
        "KEEP_ZIP_FILE=FALSE\n",

        "# Whether the launcher should be kept open after starting the app (default FALSE)",
        "KEEP_LAUNCHER_OPEN=FALSE\n"
      };
      File.WriteAllLines("taw.conf", config);
    }

    private static void ValidateConfig()
    {
      try
      {
        if (_versionFileUrl == String.Empty) throw new Exception("VERSION_FILE_URL");
        if (_updateFileUrl == String.Empty) throw new Exception("APPLICATION_ZIP_URL");
        //_automaticallyUpdate
        if (_exeToRun == ".exe") throw new Exception("EXE_TO_RUN");
        //_runAfterUpdate
        //_keepZip
        //_keepLauncherOpen
      }
      catch (Exception ex)
      {
        MessageBox.Show("Config file invalid. Please, check the values. \n Invalid value: " + ex.Message,
          "Taw Launcher");
        Application.Current.Shutdown();
      }
    }
  }
}