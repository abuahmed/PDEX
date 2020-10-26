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
    public class ClientService : IClientService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<ClientDTO> _clientRepository;
        private readonly bool _disposeWhenDone;
        private IDbContext _iDbContext;
        #endregion

        #region Constructor
        public ClientService()
        {
            InitializeDbContext();
        }

        public ClientService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public ClientService(IDbContext iDbContext)
        {
            _clientRepository = new Repository<ClientDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _clientRepository = new Repository<ClientDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<ClientDTO> Get()
        {
            var piList = _clientRepository
                .Query()
                .Include(a => a.Address, a => a.Documents, a => a.Category)
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName).ThenBy(c => c.Code));
            return piList;
        }

        public IEnumerable<ClientDTO> GetAll(SearchCriteria<ClientDTO> criteria = null)
        {
            int totalCount = 0;
            return this.GetAll(criteria, out totalCount);
        }

        public IEnumerable<ClientDTO> GetAll(SearchCriteria<ClientDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<ClientDTO> piList = new List<ClientDTO>();
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }


                    IList<ClientDTO> pdtoList;
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

        public ClientDTO Find(string clientId)
        {
            var bpId = Convert.ToInt32(clientId);
            var bpDto = Get().Filter(b => b.Id == bpId).Get().FirstOrDefault();
            if (_disposeWhenDone)
                Dispose();
            return bpDto;
        }

        public ClientDTO GetByName(string displayName)
        {
            var bp = Get()
                .Filter(b => b.DisplayName == displayName)
                .Get()
                .FirstOrDefault();
            return bp;
        }

        public string InsertOrUpdate(ClientDTO client)
        {
            try
            {
                var validate = Validate(client);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(client))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name/Tin No. Exists";

                _clientRepository.InsertUpdate(client);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(ClientDTO client)
        {
            //if (_unitOfWork.Repository<DeliveryHeaderDTO>().Query().Get().Any(i => i.ClientId == client.Id) ||
            //    _unitOfWork.Repository<DocumentDTO>().Query().Get().Any(i => i.ClientId == client.Id))
            //{
            //    return "Can't delete the item, it is already in use...";
            //}

            if (client == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _clientRepository.Update(client);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string clientId)
        {
            try
            {
                _clientRepository.Delete(clientId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(ClientDTO client)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<ClientDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => bp.Number == client.Number && bp.Id != client.Id)
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

        public string Validate(ClientDTO client)
        {
            if (null == client)
                return GenericMessages.ObjectIsNull;

            if (client.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(client.DisplayName))
                return client.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (client.DisplayName.Length > 255)
                return client.DisplayName + " can not be more than 255 characters ";

            if (!string.IsNullOrEmpty(client.Code) && client.Code.Length > 50)
                return client.Code + " can not be more than 50 characters ";


            return string.Empty;
        }

        #endregion

        #region Private Methods
        public string GetClientCode()
        {
            const string prefix = "C";
            var bpCode = prefix + "0001";

            try
            {
                var bpDto = Get().Get(1)
                    .Where(c=>!c.IsReceiver)
                    .OrderByDescending(d => d.Id)
                    .Count();

                //if (bpDto != null)
                //{
                    var code = 10000 + bpDto + 1;
                    bpCode = prefix + code.ToString(CultureInfo.InvariantCulture).Substring(1);
                //}
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