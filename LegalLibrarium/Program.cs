using System;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        DatabaseInitializer.Initialize();
        Application.Run(new ClaimEntryForm());
    }
}
