using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LauncherMvvmLight.Domain;

using LauncherMvvmLight.Infrastructure.Util;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;


namespace LauncherMvvmLight.View.ShellViews.DeviceDataView
{
    public class DeviceDataViewModel : ViewModelBase
    {
        #region Private Fields
        private static string ViewTitle = "Device Data";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DispatcherTimer timer = null;
        private bool _isBusy = false;
        //private readonly IModuleInfoRepo _moduleInfoRepo;
      



        #endregion

        #region Public Properties  
        [Provide(PropertyName = WellKnownProperties.SelectedDevice)]
        public virtual string SelectedDevice { get; set; }
        public string WindowTitle { get; private set; }
        public List<String> DevicesNames { get; private set; }
        public bool EnableDisableSettings { get; private set; }

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

        public RelayCommand RelayCommandSample { get; private set; }

  

        #endregion

        #region Constructor
        public DeviceDataViewModel()
        {
            


            RelayCommandSample = new RelayCommand(RelayCommandSampleExcFunc, RelayCommandSampleCanExcFlag);


            logger.Log(LogLevel.Info, "--- DevicedataViewModel:loaded ---");

            // Get lists of settings objects
            try
            {

                //Devices = ModuleDataModel.Instance.getDeviceLists();

                logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

            // Set default values
            EnableDisableSettings = true;


            // We expect a message with some lists with changes.
            Messenger.Default.Register<MessageCommunicator>(this, MakingNewDeviceChanges);

            logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");

            Init();


        }
        #endregion

     #region Events
        private void TimerTick(object send, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
     #endregion

     #region Public Methods
        

        /// <summary>
        /// Close view windows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
               
                logger.Log(LogLevel.Debug, "SerialPort.Dispose() & SerialPort.Close() are executed on OnWindowClosing() method.");                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
        }
     #endregion

     #region Private Methods
        /// <summary>
        /// Init - specific module
        /// </summary>
        private void Init()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

        }
        /// <summary>
        /// Timer support util
        /// </summary>
        private void StartTimer(int duration)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }


       // [LongRunningOperation(Message = "Scan takes time !!! ")]
        void RelayCommandSampleExcFunc()
        {
            //Devices = ModuleDataModel.Instance.getDeviceLists();
            //OnPropertyChanged("Devices");

        }

        private bool RelayCommandSampleCanExcFlag()
        {
            return true;//SelectedCategory != null;
        }

        private void MakingNewDeviceChanges(MessageCommunicator changes)
        {
           
            //handle change
        }
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        #endregion

   
    }
}
