using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Model;
using NLog;

namespace LauncherMvvmLight.Domain.Utils.Samples
{
    public class SampleViewModel : ViewModelBase
    {
    #region Private Fields
        private readonly IFrameNavigationService _navigationService;
        private static string AppTitle = "Excalibur Sample View Model";
        private static Logger logger = LogManager.GetCurrentClassLogger();        
        private DispatcherTimer timer = null;     
    #endregion
    #region Public Properties
        public string WindowTitle { get; private set; }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    #endregion

    #region Public ICommands
        public RelayCommand SampleCommand { get; set; }
    #endregion
    #region Constructor
        public SampleViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            SampleCommand = new RelayCommand(SampleCommandHandler);
           
            logger.Log(LogLevel.Info, "--- PROGRAM STARTED ---");

            // Get lists of settings objects
            try
            {                
                logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
            //_navigationService.NavigateTo("Config");
        }
    #endregion
    #region Events

    #endregion
    #region Public Methods
        
    #endregion
    #region Private Methods
        /// <summary>
        /// Method to ...
        /// </summary>
        private void SampleCommandHandler()
        {
            try
            {
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    #endregion
    }
}

//process.StartInfo.RedirectStandardOutput = true;
#if false
 if (File.Exists("Start.ps1"))
            {
                File.GetAttributes("Start.ps1");
                string strCmdText =   Path.Combine(Directory.GetCurrentDirectory(), "Start.ps1");
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
                process.StartInfo.Arguments = "\"&'"+strCmdText+"'\"";

                process.Start();
                string s = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                using (StreamWriter outfile = new StreamWriter("StandardOutput.txt", true))
                {
                    outfile.Write(s);
                }

            }

     https://www.google.co.il/search?q=power+shell+.ps1+file+run+c%23&oq=power+shell+.ps1+file+run+c%23&aqs=chrome..69i57j0.14388j0j8&sourceid=chrome&ie=UTF-8
#endif