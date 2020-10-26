using System;
using System.Collections.Generic;
using System.Linq;
using PDEX.Core;
using PDEX.Core.Models;
using PDEX.DAL;
using PDEX.Repository;
using PDEX.Repository.Interfaces;
using PDEX.Service.Interfaces;

namespace PDEX.Service
{
    public class DocumentService : IDocumentService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<DocumentDTO> _financialAccountRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor

        public DocumentService()
        {
            InitializeDbContext();
        }
        public DocumentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _financialAccountRepository = new Repository<DocumentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<DocumentDTO> Get()
        {
            var piList = _financialAccountRepository
                .Query()
                .Include(a => a.Client, a => a.Category)
                .Filter(a => !string.IsNullOrEmpty(a.Description))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
        }

        public IEnumerable<DocumentDTO> GetAll(SearchCriteria<DocumentDTO> criteria = null)
        {
            IEnumerable<DocumentDTO> accountList = new List<DocumentDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {

                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<DocumentDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoList = pdto.GetList().ToList();

                    accountList = accountList.Concat(pdtoList).ToList();

                }
                else
                    accountList = Get().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return accountList;
        }

        public DocumentDTO Find(string financialAccountId)
        {
            return _financialAccountRepository.FindById(Convert.ToInt32(financialAccountId));
        }



        public string InsertOrUpdate(DocumentDTO financialAccount)
        {
            try
            {
                var validate = Validate(financialAccount);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(financialAccount))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _financialAccountRepository.InsertUpdate(financialAccount);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(DocumentDTO financialAccount)
        {
            if (financialAccount == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _financialAccountRepository.Update(financialAccount);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string financialAccountId)
        {
            try
            {
                _financialAccountRepository.Delete(financialAccountId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(DocumentDTO financialAccount)
        {
            var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<DocumentDTO>(iDbContext);
            //    var catExists = catRepository
            //        .Query()
            //        .Filter(bp => bp.BankName == financialAccount.BankName && bp.AccountNumber == financialAccount.AccountNumber && bp.Id != financialAccount.Id)
            //        .Get()
            //        .FirstOrDefault();
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return objectExists;
        }

        public string Validate(DocumentDTO financialAccount)
        {
            if (null == financialAccount)
                return GenericMessages.ObjectIsNull;

            if (financialAccount.ClientId == 0)
                return "Client is null/disabled ";

            return string.Empty;
        }

        #endregion

        #region Disposing
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}