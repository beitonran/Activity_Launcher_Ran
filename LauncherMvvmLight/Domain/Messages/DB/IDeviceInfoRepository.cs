using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LauncherMvvmLight.Model;


namespace LauncherMvvmLight.Domain.DB
{
    public interface IDeviceInfoRepository
    {
        /// <summary>
        /// Saves the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        void Save(DeviceInfoModel note);

        /// <summary>
        /// Deletes the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        void Delete(DeviceInfoModel note);

        /// <summary>
        /// Resets the repository.
        /// </summary>
        void RepositoryReset();

        /// <summary>
        /// Returns the entire repository.
        /// </summary>
        /// <returns>A List with all Notes</returns>
        IList<DeviceInfoModel> FindAll();
    }
}
