using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LauncherMvvmLight.Model;


namespace LauncherMvvmLight.Domain.DB
{
    /// <summary>
    /// This class manages a note repository.
    /// The devs will be saved using serialization.
    /// </summary>
    public class DeviceInfoRepository : IDeviceInfoRepository
    {
        private IList<DeviceInfoModel> _devStore;
        private readonly string _dataFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoRepository"/> class.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public DeviceInfoRepository(string fileName)
        {
            fileName = string.Concat(fileName + ".devs");
            _dataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            Deserialize();
        }

        /// <summary>
        /// Saves the specified note.
        /// </summary>
        /// <param name="dev">The note.</param>
        public void Save(DeviceInfoModel dev)
        {
            if (!_devStore.Contains(dev))
                _devStore.Add(dev);

            Serialize();
        }

        /// <summary>
        /// Deletes the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        public void Delete(DeviceInfoModel dev)
        {
            _devStore.Remove(dev);

            Serialize();
        }

        /// <summary>
        /// Resets the repository.
        /// </summary>
        public void RepositoryReset()
        {
            File.Delete(_dataFile);
            _devStore = new List<DeviceInfoModel>();
        }

        /// <summary>
        /// Returns the entire repository.
        /// </summary>
        /// <returns>A List with all Devs</returns>
        public IList<DeviceInfoModel> FindAll()
        {
            return _devStore;
        }

        /// <summary>
        /// Serializes all the notes to a file.
        /// </summary>
        private void Serialize()
        {
            using (var stream = File.Open(_dataFile, FileMode.OpenOrCreate))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _devStore);
            }
        }

        /// <summary>
        /// Deserializes all notes or creates an empty List.
        /// </summary>
        private void Deserialize()
        {
            if (File.Exists(_dataFile))
                using (var stream = File.Open(_dataFile, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    _devStore = (IList<DeviceInfoModel>) formatter.Deserialize(stream);
                }
            else
                _devStore = new List<DeviceInfoModel>();
        }
    }
}
