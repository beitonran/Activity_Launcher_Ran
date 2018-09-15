using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using GalaSoft.MvvmLight.Messaging;
using LauncherMvvmLight.Domain.Messages;
using LauncherMvvmLight.Domain.Services.UtilServices;
using LauncherMvvmLight.MessageInfrastructure;
using LauncherMvvmLight.Model;
using Microsoft.Win32;
using System.IO;
using System.Management;
using System.Text;
using System.Runtime.InteropServices;
using LauncherMvvmLight.Domain.DataLayer.DB;
using System.Reflection;
using LauncherMvvmLight.Domain.Utils.Helpers;

namespace LauncherMvvmLight.Domain.Services.DeviceCollectService
{
    /// <summary>
    /// The Interface defining methods for Create Employee and Read All Employees  
    /// </summary>
    public interface IDBSrv
    {
        List<DeviceTableDbModel> GetActiveDeviceTableData();
        List<DeviceTableDbModel> GetDeviceTableData();
        List<ModuleTableDbModel> GetModuleTableData();
        List<DeviceConfigTableModel> GetConfigTypeList();
    }
    /// <summary>
    /// Class implementing IDataAccessService interface and implementing
    /// its methods by making call to the Entities using CompanyEntities object
    /// </summary>
    public class DBSrv : IDBSrv
    {
        // First create a runspace
        // You really only need to do this once. Each pipeline you create can run in this runspace.
        private String uProcessId;
        private DataExcellLayer mDataExcellLayer = null;
        public List<DataRow> DevicesTableDB;
        public List<DataRow> ModulesTableDB;
       

        private List<ModuleTableDbModel> _modulesListDB;
        private List<DeviceTableDbModel> _devicesListDB;
        private List<DeviceConfigTableModel> _devicesConfigTypeList;

        

        /*
        
        private List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        */
        public string[] PropertiesFromType(object atype)
        {
            if (atype == null) return new string[] { };
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            List<string> propNames = new List<string>();
            foreach (PropertyInfo prp in props)
            {
                propNames.Add(prp.Name);
            }
            return propNames.ToArray();
        }

        private void ConvertDeviceDataTable(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if ((dr.ItemArray[0].ToString() == "Device")&&(dr.ItemArray[1].ToString() == "active"))
                {
                    DeviceTableDbModel deviceTableDbModel = new DeviceTableDbModel();
                    foreach (DataColumn column in dr.Table.Columns)
                    {
                        foreach (PropertyInfo pro in typeof(DeviceTableDbModel).GetProperties())
                        {
                            if (pro.Name == column.ColumnName)
                            {
                                pro.SetValue(deviceTableDbModel, dr[column.ColumnName], null);

                            }
                            else
                                continue;
                        }
                    }
                    _devicesListDB.Add(deviceTableDbModel);
                }
            }
        }

        private void SetDevicesConfigTypeList()
        {
            IEnumerable<DeviceTableDbModel> distinctList = _devicesListDB.IEnumDistinctBy(x => x.DeviceAlias);

            foreach (DeviceTableDbModel item in distinctList)
            {
                DeviceConfigTableModel deviceConfigTableModel = new DeviceConfigTableModel();

                deviceConfigTableModel.DeviceTypeName = item.DeviceAlias;
                deviceConfigTableModel.DeviceTypeDisplayConfig = item.DeviceDisplayConfig;
                deviceConfigTableModel.DipConfig = item.DipConfig;
                _devicesConfigTypeList.Add(deviceConfigTableModel);
            }
           
        }


        public DBSrv()
        {

            _devicesListDB = new List<DeviceTableDbModel>();
            _modulesListDB = new List<ModuleTableDbModel>();
            _devicesConfigTypeList = new List<DeviceConfigTableModel>();

            mDataExcellLayer = new DataExcellLayer();
            bool rc = mDataExcellLayer.LoadTableData("Devices", @"C:\Excalibur\ExcaliburDB\ExcellDB\ExcDB.xlsx");

            if (rc)
            {
                ConvertDeviceDataTable(mDataExcellLayer.GetDeviceDataTable());
                SetDevicesConfigTypeList();
            }          
        }

        private void Init()
        {
            //DevicesTableDB
            foreach (var p in DevicesTableDB)
            {
                Console.WriteLine(p.ItemArray[1]);
            }
        }

        public List<DeviceTableDbModel> GetActiveDeviceTableData()
        {
            return (_devicesListDB);
        }

        public List<DeviceTableDbModel> GetDeviceTableData()
        {
            return (_devicesListDB);          
        }

        public List<ModuleTableDbModel> GetModuleTableData()
        {
            return (_modulesListDB);
        }

        public List<DeviceConfigTableModel> GetConfigTypeList()
        {
            return (_devicesConfigTypeList);
        }
     
    }    
}
