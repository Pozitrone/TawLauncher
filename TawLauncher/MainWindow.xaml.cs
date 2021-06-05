using System;
using System.Threading.Tasks;
using System.Windows;

namespace TawLauncher
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      try
      {
        InitializeComponent();
              UpdateCore.ReadConfigFile();
              RunLauncherAfterDelay();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
      
    }

    private async void RunLauncherAfterDelay(int delay = 1000)
    {
      await Task.Delay(delay);
      StartLauncher();
    }
    
    private void StartLauncher()
    {
      Launcher launcher = new Launcher();
      launcher.Show();
      this.Close();
    }
  }
}