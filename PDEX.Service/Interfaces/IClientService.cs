using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PDEX.Core;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IClientService : IDisposable
    {
        IEnumerable<ClientDTO> GetAll(SearchCriteria<ClientDTO> criteria=null);
        IEnumerable<ClientDTO> GetAll(SearchCriteria<ClientDTO> criteria, out int totalCount);
        ClientDTO Find(string clientId);
        ClientDTO GetByName(string displayName);
        string InsertOrUpdate(ClientDTO client);
        string Disable(ClientDTO client);
        int Delete(string clientId);
        string GetClientCode();
    }
}