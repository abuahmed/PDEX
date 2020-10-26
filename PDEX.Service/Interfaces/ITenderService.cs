using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface ITenderService : IDisposable
    {
        IEnumerable<TenderDTO> GetAll(SearchCriteria<TenderDTO> criteria = null);
        IEnumerable<TenderDTO> GetAll(SearchCriteria<TenderDTO> criteria, out int totalCount);
        TenderDTO Find(string tenderId);
        TenderDTO GetByName(string displayName);
        string InsertOrUpdate(TenderDTO tender);
        string Disable(TenderDTO tender);
        int Delete(string tenderId);
    }
}