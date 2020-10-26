using System;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface ICompanyService : IDisposable
    {
        CompanyDTO GetCompany();
        string InsertOrUpdate(CompanyDTO client);
        //string Disable(CompanyDTODTO client);
        //int Delete(string companyDTOId);
    }
}