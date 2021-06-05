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
      InitializeComponent();
      UpdateCore.ReadConfigFile();
      RunLauncherAfterDelay();
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