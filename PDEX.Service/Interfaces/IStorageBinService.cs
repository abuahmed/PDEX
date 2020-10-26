using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IStorageBinService : IDisposable
    {
        IEnumerable<StorageBinDTO> GetAll(SearchCriteria<StorageBinDTO> criteria = null);
        IEnumerable<StorageBinDTO> GetAll(SearchCriteria<StorageBinDTO> criteria, out int totalCount);
        StorageBinDTO Find(string storageBinId);
        string InsertOrUpdate(StorageBinDTO storageBin);
        string Disable(StorageBinDTO storageBin);
        int Delete(string storageBinId);
        string GetStorageBinCode();
    }
}