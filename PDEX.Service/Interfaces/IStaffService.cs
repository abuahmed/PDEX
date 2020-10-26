using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PDEX.Core;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IStaffService : IDisposable
    {
        IEnumerable<StaffDTO> GetAll(SearchCriteria<StaffDTO> criteria=null);
        IEnumerable<StaffDTO> GetAll(SearchCriteria<StaffDTO> criteria, out int totalCount);
        StaffDTO Find(string staffId);
        StaffDTO GetByName(string displayName);
        string InsertOrUpdate(StaffDTO staff);
        string Disable(StaffDTO staff);
        int Delete(string staffId);
        string GetStaffCode();
    }
}