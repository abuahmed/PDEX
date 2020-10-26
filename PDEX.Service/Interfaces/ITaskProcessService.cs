using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface ITaskProcessService : IDisposable
    {
        IEnumerable<TaskProcessDTO> GetAll(SearchCriteria<TaskProcessDTO> criteria = null);
        IEnumerable<TaskProcessDTO> GetAll(SearchCriteria<TaskProcessDTO> criteria, out int totalCount);
        TaskProcessDTO Find(string taskProcessId);
        TaskProcessDTO GetByName(string displayName);
        string InsertOrUpdate(TaskProcessDTO taskProcess);
        string Disable(TaskProcessDTO taskProcess);
        int Delete(string taskProcessId);
    }
}