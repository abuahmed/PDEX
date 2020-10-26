using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IDocumentService : IDisposable
    {
        IEnumerable<DocumentDTO> GetAll(SearchCriteria<DocumentDTO> criteria = null);
        DocumentDTO Find(string financialAccountId);
        string InsertOrUpdate(DocumentDTO financialAccount);
        string Disable(DocumentDTO financialAccount);
        int Delete(string financialAccountId);
    }
}