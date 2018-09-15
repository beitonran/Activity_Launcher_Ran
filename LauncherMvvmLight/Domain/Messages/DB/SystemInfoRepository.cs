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
    public class SystemInfoRepository : ISystemInfoRepository
    {
        private IList<SystemInfoModel> _dataStore;
        private readonly string _dataFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoRepository"/> class.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public SystemInfoRepository(string fileName)
        {
            fileName = string.Concat(fileName + ".isys");
            _dataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            Deserialize();
        }

        /// <summary>
        /// Saves the specified note.
        /// </summary>
        /// <param name="dev">The note.</param>
        public void Save(SystemInfoModel data)
        {
            if (!_dataStore.Contains(data))
                _dataStore.Add(data);

            Serialize();
        }

        /// <summary>
        /// Deletes the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        public void Delete(SystemInfoModel data)
        {
            _dataStore.Remove(data);

            Serialize();
        }

        /// <summary>
        /// Resets the repository.
        /// </summary>
        public void RepositoryReset()
        {
            File.Delete(_dataFile);
            _dataStore = new List<SystemInfoModel>();
        }

        /// <summary>
        /// Returns the entire repository.
        /// </summary>
        /// <returns>A List with all Devs</returns>
        public IList<SystemInfoModel> GetAllData()
        {
            return _dataStore;
        }

        /// <summary>
        /// Serializes all the notes to a file.
        /// </summary>
        private void Serialize()
        {
            using (var stream = File.Open(_dataFile, FileMode.OpenOrCreate))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _dataStore);
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
                    _dataStore = (IList<SystemInfoModel>) formatter.Deserialize(stream);
                }
            else
                _dataStore = new List<SystemInfoModel>();
        }
    }
}
