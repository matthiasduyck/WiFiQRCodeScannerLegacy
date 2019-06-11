using System;
using WixSharp;

// DON'T FORGET to update NuGet package "WixSharp".
// NuGet console: Update-Package WixSharp
// NuGet Manager UI: updates tab

namespace Wifi_QR_Code_Scanner_Installer
{
    class Program
    {
        static void Main()
        {
            var project = new Project("Wifi QR Code Scanner",
                              new Dir(@"%ProgramFiles%\Matthias Duyck\Wifi QR Code Scanner Legacy",
                                  new File("Program.cs")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}