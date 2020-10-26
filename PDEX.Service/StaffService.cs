using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PDEX.Core;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.DAL;
using PDEX.DAL.Interfaces;
using PDEX.Repository;
using PDEX.Repository.Interfaces;
using PDEX.Service.Interfaces;

namespace PDEX.Service
{
    public class StaffService : IStaffService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<StaffDTO> _staffRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public StaffService()
        {
            InitializeDbContext();
        }

        public StaffService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _staffRepository = new Repository<StaffDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<StaffDTO> Get()
        {
            var piList = _staffRepository
                .Query()
                .Include(a => a.Address, a => a.ContactPerson, a => a.ContactPerson.Address, a => a.AssignedVehicles)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName).ThenBy(c => c.Code));
            return piList;
        }

        public IEnumerable<StaffDTO> GetAll(SearchCriteria<StaffDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<StaffDTO> GetAll(SearchCriteria<StaffDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<StaffDTO> piList = new List<StaffDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<StaffDTO> pdtoList;
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

        public StaffDTO Find(string staffId)
        {
            var bpId = Convert.ToInt32(staffId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public StaffDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(StaffDTO staff)
        {
            try
            {
                var validate = Validate(staff);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(staff))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same name exists";

                _staffRepository.InsertUpdate(staff);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(StaffDTO staff)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.StaffId == staff.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.StaffId == staff.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (staff == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _staffRepository.Update(staff);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string staffId)
        {
            try
            {
                _staffRepository.Delete(staffId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(StaffDTO staff)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<StaffDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.DisplayName == staff.DisplayName) &&
                    bp.Id != staff.Id)
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

        public string Validate(StaffDTO staff)
        {
            if (null == staff)
                return GenericMessages.ObjectIsNull;

            if (staff.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(staff.DisplayName))
                return staff.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (staff.DisplayName.Length > 255)
                return staff.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(staff.Code) && staff.Code.Length > 50)
                return staff.Code + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetStaffCode()
        {
            const string prefix = "S";
            var bpCode = prefix + "0001";

            try
            {
                var bpDto = Get().Get(1)//All Deleted and notdeleted
                   .OrderByDescending(d => d.Id)
                   .FirstOrDefault();

                if (bpDto != null)
                {
                    var code = 10000 + bpDto.Id + 1;
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