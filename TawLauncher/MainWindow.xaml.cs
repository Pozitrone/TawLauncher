using System;
using System.Threading.Tasks;
using System.Windows;

namespace TawLauncher
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      try
      {
        InitializeComponent();
        UpdateCore.mainWindowInstance = this;
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