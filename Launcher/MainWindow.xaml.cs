using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using TAWLauncher;

namespace TawLauncher
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      try
      {
        InitializeComponent();
        Title = "TAW Launcher";
        UpdateCore.mainWindowInstance = this;
        LogoImage.Source = new BitmapImage(new Uri(Config.LoadingLogo));
        UpdateCore.ReadConfigFile();
        if (UpdateCore.AutomaticallyUpdate) UpdateAfterDelay();
        else RunLauncherAfterDelay();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    public async void RunLauncherAfterDelay(int delay = 1000)
    {
      await Task.Delay(delay);
      StartLauncher();
    }

    private async void UpdateAfterDelay(int delay = 1000)
    {
      await Task.Delay(delay);
      UpdateCore.Update();
    }

    private void StartLauncher()
    {
      Launcher launcher = new Launcher();
      launcher.Show();
      Close();
    }
  }
}