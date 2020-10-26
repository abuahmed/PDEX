using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PDEX.Core;
using PDEX.Core.Models;
using PDEX.DAL;
using PDEX.DAL.Interfaces;
using PDEX.Repository;
using PDEX.Repository.Interfaces;
using PDEX.Service.Interfaces;

namespace PDEX.Service
{
    public class StorageBinService : IStorageBinService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<StorageBinDTO> _storageBinRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public StorageBinService()
        {
            InitializeDbContext();
        }

        public StorageBinService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _storageBinRepository = new Repository<StorageBinDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<StorageBinDTO> Get()
        {
            var piList = _storageBinRepository
                .Query()
                .Include(a => a.Warehouse);
            return piList;
        }

        public IEnumerable<StorageBinDTO> GetAll(SearchCriteria<StorageBinDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<StorageBinDTO> GetAll(SearchCriteria<StorageBinDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<StorageBinDTO> piList = new List<StorageBinDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<StorageBinDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0 && criteria.PaymentListType == -1)
                    {
                        int totalCount2;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                        totalCount = totalCount2;
                    }
                    else
                    {
                        pdtoList = pdto.GetList().ToList();
                        totalCount = pdtoList.Count;
                    }

                    piList = pdtoList.ToList();
                }
                else
                    piList = Get().Get().OrderBy(i => i.Id).ToList();

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return piList;
        }

        public StorageBinDTO Find(string storageBinId)
        {
            var bpId = Convert.ToInt32(storageBinId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

 

        public string InsertOrUpdate(StorageBinDTO storageBin)
        {
            try
            {
                var validate = Validate(storageBin);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(storageBin))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name/Tin No. Exists";

                _storageBinRepository.InsertUpdate(storageBin);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(StorageBinDTO storageBin)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.StorageBinId == storageBin.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.StorageBinId == storageBin.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (storageBin == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _storageBinRepository.Update(storageBin);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string storageBinId)
        {
            try
            {
                _storageBinRepository.Delete(storageBinId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(StorageBinDTO storageBin)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<StorageBinDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.Shelve == storageBin.Shelve || bp.BoxNumber == storageBin.BoxNumber) &&
                                  bp.Id != storageBin.Id)
                    .Get()
                    .FirstOrDefault();


                if (catExists != null)
                    objectExists = true;
            }
            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(StorageBinDTO storageBin)
        {
            if (null == storageBin)
                return GenericMessages.ObjectIsNull;

            if (storageBin.Warehouse == null)
                return "Warehouse " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(storageBin.Shelve))
                return storageBin.Shelve + " " + GenericMessages.StringIsNullOrEmpty;

            if (String.IsNullOrEmpty(storageBin.BoxNumber))
                return storageBin.BoxNumber + " " + GenericMessages.StringIsNullOrEmpty;
            
            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetStorageBinCode()
        {
            const string prefix = "C";
            var bpCode = prefix + "0001";

            try
            {
                var bpDto = Get().Get(1)
                    .OrderByDescending(d => d.Id)
                    .FirstOrDefault();

                if (bpDto != null)
                {
                    var code = 10000 + bpDto
                        .Id + 1;
                    bpCode = prefix + code.ToString(CultureInfo.InvariantCulture).Substring(1);
                }
            }
            catch
            {
                return "";
            }
            return bpCode;
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