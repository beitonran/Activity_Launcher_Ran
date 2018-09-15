using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using LauncherMvvmLight.Domain.Services.SyncService;
using LauncherMvvmLight.Domain.Services.TaskService;
using NLog;

namespace LauncherMvvmLight.View.PageViews.TestView
{
    public class TestViewModel : ViewModelBase
    {
    #region Private Fields
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ITaskSrv _taskSrvProxy;
        private ISyncSrv _syncSrvProxy;
        private IAsyncSrv _asyncSrvProxy;

        #endregion
        #region Public Fields
        public RelayCommand<string> TaskServTest { get; set; }
        public RelayCommand<string> SyncServTest { get; set; }
        public RelayCommand<string> AsyncServTest { get; set; }
        #endregion
        #region Private Methods
        public TestViewModel(ITaskSrv taskServProxy, ISyncSrv syncServProxy, IAsyncSrv asyncServProxy)
        {
            _taskSrvProxy = taskServProxy;
            _syncSrvProxy = syncServProxy;
            _asyncSrvProxy = asyncServProxy;

            TaskServTest = new RelayCommand<string>(TaskServTestHandler);
            SyncServTest = new RelayCommand<string>(SyncServTestHandler);
            AsyncServTest = new RelayCommand<string>(AsyncServTestHandler);

            Init();
        }
        #endregion
        #region Private Methods
        private void Init()
        {
            try
            {
                // specific logic initialization;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

        }

        private void TaskServTestHandler(string param)
        {
            _taskSrvProxy.RunSerivceTest(param);
        }
        private void SyncServTestHandler(string param)
        {
            _syncSrvProxy.RunSerivceTest(param);
        }
        private void AsyncServTestHandler(string param)
        {
            _asyncSrvProxy.RunSerivceTest(param);
        }
        #endregion
        #region Public Methods

        #endregion
    }
}
