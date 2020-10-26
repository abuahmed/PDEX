using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PDEX.Core;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IVehicleService : IDisposable
    {
        IEnumerable<VehicleDTO> GetAll(SearchCriteria<VehicleDTO> criteria=null);
        IEnumerable<VehicleDTO> GetAll(SearchCriteria<VehicleDTO> criteria, out int totalCount);
        VehicleDTO Find(string vehicleId);
        VehicleDTO GetByName(string displayName);
        string InsertOrUpdate(VehicleDTO vehicle);
        string Disable(VehicleDTO vehicle);
        int Delete(string vehicleId);
        string GetVehicleCode();
    }
}