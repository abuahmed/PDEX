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
    public class TenderService : ITenderService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<TenderDTO> _tenderRepository;
        private IRepository<TenderItemDTO> _tenderItemRepository;
        //private IRepository<TenderTaskDTO> _tenderTaskRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public TenderService()
        {
            InitializeDbContext();
        }

        public TenderService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public TenderService(IDbContext iDbContext)
        {
            _tenderItemRepository = new Repository<TenderItemDTO>(_iDbContext);
            //_tenderTaskRepository = new Repository<TenderTaskDTO>(_iDbContext);
            _tenderRepository = new Repository<TenderDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _tenderItemRepository = new Repository<TenderItemDTO>(_iDbContext);
            //_tenderTaskRepository = new Repository<TenderTaskDTO>(_iDbContext);
            _tenderRepository = new Repository<TenderDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Tender Methods

        public IRepositoryQuery<TenderDTO> Get()
        {
            var piList = _tenderRepository
                .Query()
                .Include(a => a.CompanyAddress, a => a.TenderItems, a => a.PublishedOn)
                .Filter(a => !string.IsNullOrEmpty(a.CompanyName))
                .OrderBy(q => q.OrderBy(c => c.CompanyName));
            return piList;
        }

        public IEnumerable<TenderDTO> GetAll(SearchCriteria<TenderDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<TenderDTO> GetAll(SearchCriteria<TenderDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<TenderDTO> piList = new List<TenderDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<TenderDTO> pdtoList;
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

        public TenderDTO Find(string tenderId)
        {
            var bpId = Convert.ToInt32(tenderId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public TenderDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.TenderNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(TenderDTO tender)
        {
            try
            {
                var validate = Validate(tender);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(tender))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Number";

                _tenderRepository.InsertUpdate(tender);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(TenderDTO tender)
        {

            if (tender == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _tenderRepository.Update(tender);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string tenderId)
        {
            try
            {
                _tenderRepository.Delete(tenderId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(TenderDTO tender)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<TenderDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => bp.TenderNumber == tender.TenderNumber && bp.Id != tender.Id)
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

        public string Validate(TenderDTO tender)
        {
            if (null == tender)
                return GenericMessages.ObjectIsNull;

            if (tender.TenderNumber == null)
                return "Tender Number " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(tender.CompanyName))
                return tender.CompanyName + " " + GenericMessages.StringIsNullOrEmpty;

            if (tender.CompanyName.Length > 255)
                return tender.CompanyName + " can not be more than 255 characters ";


            return string.Empty;
        }

        #endregion

        #region Tender Item Methods
        
        public IEnumerable<TenderItemDTO> GetItemChilds(int parentId, bool disposeWhenDone)
        {
            IEnumerable<TenderItemDTO> piList;
            try
            {
                piList = _tenderItemRepository
                    .Query()
                    .Include(a => a.Tender, a => a.TenderTasks, a => a.TenderItemCategory)
                    .Get()
                    .OrderBy(i => i.Id)
                    .ToList();

                if (parentId != 0)
                    piList = piList.Where(l => l.TenderId == parentId).ToList();

                #region For Eager Loading ItemChilds
                foreach (var tenderDTO in piList)
                {
                    //var taskDtos = (ICollection<TenderTaskDTO>)GetTaskChilds(tenderDTO.Id, false);
                }
                #endregion
            }
            finally
            {
                Dispose(disposeWhenDone);
            }

            return piList;

        }

        public IRepositoryQuery<TenderItemDTO> GetItemChildsQuery()
        {
            var piList = _tenderItemRepository
              .Query()
              .Include(a => a.Tender, a => a.TenderTasks, a => a.TenderItemCategory)
              .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<TenderItemDTO> GetAllItemChilds(SearchCriteria<TenderItemDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<TenderItemDTO> piList = new List<TenderItemDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {

                    var pdto = GetItemChildsQuery();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration

                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        pdto.FilterList(p => p.Tender.DateRecordCreated >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        pdto.FilterList(p => p.Tender.DateRecordCreated <= endDate);
                    }

                    #endregion

                    IList<TenderItemDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount2;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount2).ToList();
                        totalCount = totalCount2;
                    }
                    else
                        pdtoList = pdto.GetList().ToList();

                    piList = piList.Concat(pdtoList).ToList();

                }
                else
                    piList = GetItemChildsQuery().Get().ToList();

                #region For Eager Loading ItemChilds
                foreach (var tenderDTO in piList)
                {
                    //var taskDtos = (ICollection<TenderTaskDTO>)GetTaskChilds(tenderDTO.Id, false);
                }
                #endregion
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public TenderItemDTO FindItemChild(string tenderItemId)
        {
            return _tenderItemRepository.FindById(Convert.ToInt32(tenderItemId));
        }

        public string InsertOrUpdateItemChild(TenderItemDTO tenderItem)
        {
            try
            {
                var validate = ValidateItemChild(tenderItem);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExistsItemChild(tenderItem))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _tenderItemRepository.InsertUpdate(tenderItem);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableItemChild(TenderItemDTO tenderItem)
        {
            string stat;
            try
            {
                var dline = FindItemChild(tenderItem.Id.ToString(CultureInfo.InvariantCulture));
                dline.Enabled = false;
                _tenderItemRepository.Update(dline);

                //var messages = GetMessageItemChilds(dline.Id, false);
                //foreach (var messageDTO in messages)
                //{
                //    messageDTO.Enabled = false;
                //    _tenderItemMessageRepository.Update(messageDTO);
                //}

                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return stat;
        }

        public bool ObjectExistsItemChild(TenderItemDTO tenderItem)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<TenderItemDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => bp.TenderId == tenderItem.TenderId && bp.TenderItemCategoryId == tenderItem.TenderItemCategoryId && bp.Id != tenderItem.Id)
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

        public string ValidateItemChild(TenderItemDTO tenderItem)
        {
            if (null == tenderItem)
                return GenericMessages.ObjectIsNull;

            return string.Empty;
        }
        
        #endregion
        

        #region Private Methods

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