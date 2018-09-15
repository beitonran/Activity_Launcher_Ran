using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LauncherMvvmLight.Model;


namespace LauncherMvvmLight.Domain.Services.UtilServices
{
    public interface ISystemInfoRepository
    {
        /// <summary>
        /// Saves the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        void Save(SystemInfoModel note);

        /// <summary>
        /// Deletes the specified note.
        /// </summary>
        /// <param name="note">The note.</param>
        void Delete(SystemInfoModel note);

        /// <summary>
        /// Resets the repository.
        /// </summary>
        void RepositoryReset();

        /// <summary>
        /// Returns the entire repository.
        /// </summary>
        /// <returns>A List with all Notes</returns>
        IList<SystemInfoModel> GetAllData();
    }
}
