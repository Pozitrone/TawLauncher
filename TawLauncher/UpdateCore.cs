using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace TawLauncher
{
  public class UpdateCore
  {
    private string _versionFileUrl = null;
    private string _updateFileUrl = null;
    private bool _automaticallyUpdate = false;
    private bool _hideConfigFile = false;
    private string _exeToRun = null;
    private bool _runAfterUpdate = false;
    private bool _keepZip = false;

    public Version CheckForUpdates()
    {
      Version currentVersion = CheckCurrentVersion();
      Version newVersion = CheckNewVersion();

      if (newVersion == null) throw new Exception("Couldn't get updates. Please, check your connection.");
      if (currentVersion == null) return newVersion;

      if (currentVersion < newVersion) return newVersion;

      Directory.Delete(".tmp");
      return null;
    }

    public void Update()
    {
      if (Directory.Exists("Data")) Directory.Delete("Data");
      DownloadUpdateFile(_updateFileUrl);
      ZipFile.ExtractToDirectory("Data/Application.zip", "Data/Application");
      if (!_keepZip) File.Delete("Data/Application.zip");
      
      File.Copy(".tmp/version.txt", "Data/version.txt");
      Directory.Delete(".tmp");
    }

    private Version CheckCurrentVersion()
    {
      return !Directory.Exists("Data") ? null : ReadVersionFile();
    }

    private Version CheckNewVersion()
    {
      if (_versionFileUrl == null) return null;

      DownloadVersionFile(_versionFileUrl);
      return ReadVersionFile(".tmp/version.txt");
    }

    private void CreateTempDirectory()
    {
      if (Directory.Exists(".tmp")) Directory.Delete(".tmp");
      Directory.CreateDirectory(".tmp");
    }

    private void CreateDirectoryTree()
    {
      if (Directory.Exists("Data")) return;
      Directory.CreateDirectory("Data");
    }

    private void DownloadVersionFile(string url)
    {
      CreateTempDirectory();

      WebClient webClient = new WebClient();
      try
      {
        webClient.DownloadFile(url, ".tmp/version.txt");
      }
      catch
      {
        throw new Exception("Couldn't download version file.");
      }
    }

    private void DownloadUpdateFile(string url)
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

    private Version ReadVersionFile(string path = "Data/version.txt")
    {
      return new Version(File.ReadLines(path).First());
    }

    private void ReadConfigFile()
    {
      // TODO:
    }
  }
}