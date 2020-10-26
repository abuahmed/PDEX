using System;
using System.Collections.Generic;
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
    public class TaskProcessService : ITaskProcessService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<TaskProcessDTO> _taskProcessRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public TaskProcessService()
        {
            InitializeDbContext();
        }

        public TaskProcessService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public TaskProcessService(IDbContext iDbContext)
        {
            _taskProcessRepository = new Repository<TaskProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _taskProcessRepository = new Repository<TaskProcessDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region TaskProcess Methods

        public IRepositoryQuery<TaskProcessDTO> Get()
        {
            var piList = _taskProcessRepository
                .Query()
                .Include(a => a.Client, a => a.CompanyAddress, a => a.TaskProcessCategory, a => a.TenderItem,a => a.PaymentsandExpenses, a => a.AssignedTo);
            return piList;
        }

        public IEnumerable<TaskProcessDTO> GetAll(SearchCriteria<TaskProcessDTO> criteria = null)
        {
            int totalCount;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<TaskProcessDTO> GetAll(SearchCriteria<TaskProcessDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<TaskProcessDTO> piList;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<TaskProcessDTO> pdtoList;
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

        public TaskProcessDTO Find(string taskProcessId)
        {
            var bpId = Convert.ToInt32(taskProcessId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public TaskProcessDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.Description == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(TaskProcessDTO taskProcess)
        {
            try
            {
                var validate = Validate(taskProcess);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(taskProcess))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Number";

                _taskProcessRepository.InsertUpdate(taskProcess);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(TaskProcessDTO taskProcess)
        {

            if (taskProcess == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _taskProcessRepository.Update(taskProcess);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string taskProcessId)
        {
            try
            {
                _taskProcessRepository.Delete(taskProcessId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(TaskProcessDTO taskProcess)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<TaskProcessDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.Description == taskProcess.Description && bp.Id != taskProcess.Id)
            //        .Get()
            //        .FirstOrDefault();

            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            //return objectExists;
            return false;
        }

        public string Validate(TaskProcessDTO taskProcess)
        {
            if (null == taskProcess)
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