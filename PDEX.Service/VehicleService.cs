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
    public class VehicleService : IVehicleService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<VehicleDTO> _vehicleRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public VehicleService()
        {
            InitializeDbContext();
        }

        public VehicleService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _vehicleRepository = new Repository<VehicleDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<VehicleDTO> Get()
        {
            var piList = _vehicleRepository
                .Query()
                .Include(a => a.AssignedDriver)
                .Filter(a => !string.IsNullOrEmpty(a.PlateNumber))
                .OrderBy(q => q.OrderBy(c => c.PlateNumber));
            return piList;
        }

        public IEnumerable<VehicleDTO> GetAll(SearchCriteria<VehicleDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<VehicleDTO> GetAll(SearchCriteria<VehicleDTO> criteria, out int totalCount)//, 
        {
            totalCount = 0;
            IEnumerable<VehicleDTO> piList = new List<VehicleDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<VehicleDTO> pdtoList;
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

        public VehicleDTO Find(string vehicleId)
        {
            var bpId = Convert.ToInt32(vehicleId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public VehicleDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.PlateNumber == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(VehicleDTO vehicle)
        {
            try
            {
                var validate = Validate(vehicle);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(vehicle))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name/Tin No. Exists";

                _vehicleRepository.InsertUpdate(vehicle);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(VehicleDTO vehicle)
        {
            if (vehicle == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _vehicleRepository.Update(vehicle);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string vehicleId)
        {
            try
            {
                _vehicleRepository.Delete(vehicleId);
                _unitOfWork.Commit();
                return 0;
            }
            catch 
            {
                return -1;
            }
        }

        public bool ObjectExists(VehicleDTO vehicle)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<VehicleDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => (bp.PlateNumber == vehicle.PlateNumber) && bp.Id != vehicle.Id)
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

        public string Validate(VehicleDTO vehicle)
        {
            if (null == vehicle)
                return GenericMessages.ObjectIsNull;
            
            if (String.IsNullOrEmpty(vehicle.PlateNumber))
                return vehicle.PlateNumber + " " + GenericMessages.StringIsNullOrEmpty;
            
            return string.Empty;
        }

        #endregion
        
        #region Private Methods
        public string GetVehicleCode()
        {
            const string prefix = "V"; 
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