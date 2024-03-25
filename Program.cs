using System;
using System.Net;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("User32")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr FindWindowW([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName);

    readonly static MD5 md5 = MD5.Create();

    private static string GetMD5Hash(string path)
    {
        try { return Convert.ToBase64String(md5.ComputeHash(File.ReadAllBytes(path))); }
        catch { return default; }
    }

    private static string GetMD5Hash(Stream stream)
    {
        return Convert.ToBase64String(md5.ComputeHash(stream));
    }

    static void Main()
    {
        Console.WriteLine("[INFO] Northstar Updater by Aetopia.");
        Console.WriteLine("[INFO] GitHub Repository: https://github.com/Aetopia/Northstar-Updater");
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        using WebClient webClient = new();
        webClient.Headers.Add(HttpRequestHeader.UserAgent, "_");

        try
        {
            Console.WriteLine("[INFO] Obtain latest release information.");
            string input = webClient.DownloadString("https://api.github.com/repos/R2Northstar/Northstar/releases/latest");
            string requestUriString = ((new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(input)["assets"] as ArrayList)[0] as Dictionary<string, object>)["browser_download_url"] as string;

            WebRequest webRequest = WebRequest.Create(requestUriString);
            webRequest.Method = "HEAD";

            using WebResponse webResponse = webRequest.GetResponse();
            if (!File.Exists("Northstar.zip") || GetMD5Hash("Northstar.zip") != webResponse.Headers["Content-MD5"])
            {
                Console.WriteLine("[INFO] Couldn't validate cached release.");
                try { webClient.DownloadFileTaskAsync(requestUriString, "Northstar.zip").GetAwaiter().GetResult(); }
                catch (Exception e) { Console.WriteLine($"[ERROR] {e.Message}"); }
            }
            else
                Console.WriteLine("[INFO] Validated cached release.");
        }
        catch (Exception e) { Console.WriteLine($"[ERROR] {e.Message}"); }

        try
        {
            using ZipArchive zipFile = ZipFile.OpenRead("Northstar.zip");
            for (int i = 0; i < zipFile.Entries.Count; i++)
            {
                if (zipFile.Entries[i].Length == 0)
                    Directory.CreateDirectory(zipFile.Entries[i].FullName);
                else
                {
                    using Stream stream = zipFile.Entries[i].Open();
                    if (GetMD5Hash(stream) != (File.Exists(zipFile.Entries[i].FullName) ? GetMD5Hash(zipFile.Entries[i].FullName) : default))
                    {
                        Console.WriteLine($"[INFO] {zipFile.Entries[i].FullName} : Extract");
                        zipFile.Entries[i].ExtractToFile(@$".\\{zipFile.Entries[i].FullName}", true);
                    }
                    else Console.WriteLine($"[INFO] {zipFile.Entries[i].FullName} : Pass");
                }
            }
        }
        catch (Exception e) { Console.WriteLine($"[ERROR] {e.Message}"); }

        ProcessStartInfo startInfo = new()
        {
            FileName = "NorthstarLauncher.exe",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        try
        {
            string executableFileName = Environment.GetCommandLineArgs()[0];
            startInfo.Arguments = Environment.CommandLine.Remove(Environment.CommandLine.IndexOf(executableFileName), executableFileName.Length).TrimStart('"').Substring(1);
        }
        catch { }
        try { Process.Start(startInfo).Dispose(); }
        catch (Exception e) { Console.WriteLine(e); }
    }
}