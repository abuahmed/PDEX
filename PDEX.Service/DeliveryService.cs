using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public class DeliveryService : IDeliveryService
    {
        #region Fields
        private IDbContext _iDbContext;
        private IUnitOfWork _unitOfWork;
        private IRepository<DeliveryHeaderDTO> _deliveryRepository;
        private IRepository<DeliveryLineDTO> _deliveryLineRepository;
        private IRepository<MessageDTO> _deliveryLineMessageRepository;
        private IRepository<DeliveryRouteDTO> _deliveryLineDeliveryRouteRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor

        public DeliveryService()
        {
            InitializeDbContext();
        }
        public DeliveryService(IDbContext dbContext)
        {
            _iDbContext = dbContext;
            _deliveryRepository = new Repository<DeliveryHeaderDTO>(_iDbContext);
            _deliveryLineRepository = new Repository<DeliveryLineDTO>(_iDbContext);
            _deliveryLineMessageRepository = new Repository<MessageDTO>(_iDbContext);
            _deliveryLineDeliveryRouteRepository = new Repository<DeliveryRouteDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);
        }
        public DeliveryService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }
        public void InitializeDbContext()
        {

            _iDbContext = DbContextUtil.GetDbContextInstance();
            _deliveryRepository = new Repository<DeliveryHeaderDTO>(_iDbContext);
            _deliveryLineRepository = new Repository<DeliveryLineDTO>(_iDbContext);
            _deliveryLineMessageRepository = new Repository<MessageDTO>(_iDbContext);
            _deliveryLineDeliveryRouteRepository = new Repository<DeliveryRouteDTO>(_iDbContext);
            _unitOfWork = new UnitOfWork(_iDbContext);

        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<DeliveryHeaderDTO> Get()
        {
            var piList = _deliveryRepository
              .Query()
              .Include(p => p.OrderByClient, p => p.OrderByClient.Address, p => p.DeliveryLines)
              .OrderBy(q => q.OrderByDescending(c => c.OrderDate));
            return piList;
        }

        public IEnumerable<DeliveryHeaderDTO> GetAll(SearchCriteria<DeliveryHeaderDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<DeliveryHeaderDTO> GetAll(SearchCriteria<DeliveryHeaderDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<DeliveryHeaderDTO> piList = new List<DeliveryHeaderDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration
                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        pdto.FilterList(p => p.DateRecordCreated >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        pdto.FilterList(p => p.DateRecordCreated <= endDate);
                    }

                    #endregion

                    #region For Client

                    if (criteria.BusinessPartnerId != null && criteria.BusinessPartnerId != -1)
                    {
                        pdto.FilterList(w => w.OrderByClientId == criteria.BusinessPartnerId);
                    }

                    #endregion

                    IList<DeliveryHeaderDTO> pdtoList;
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
                    piList = piList.Concat(pdtoList).ToList();

                }
                else
                    piList = Get().Get().ToList();

                #region For Eager Loading Childs
                foreach (var deliveryHeaderDTO in piList)
                {
                    var deliveryLineDtos = (ICollection<DeliveryLineDTO>)GetChilds(deliveryHeaderDTO.Id, false);
                }
                #endregion

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public DeliveryHeaderDTO Find(string deliveryId)
        {
            return _deliveryRepository.FindById(Convert.ToInt32(deliveryId));
        }

        public string InsertOrUpdate(DeliveryHeaderDTO delivery)
        {
            try
            {
                var validate = Validate(delivery);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(delivery))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _deliveryRepository.InsertUpdate(delivery);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(DeliveryHeaderDTO delivery2)
        {
            if (delivery2 == null)
                return GenericMessages.ObjectIsNull;

            var iDbContextTemp = DbContextUtil.GetDbContextInstance();
            var unitOfWorkTemp = new UnitOfWork(iDbContextTemp);
            string stat;
            try
            {
                var delivery = unitOfWorkTemp.Repository<DeliveryHeaderDTO>().Query()
                    .Include(t => t.DeliveryLines)
                    .Filter(t => t.Id == delivery2.Id)
                    .Get()
                    .FirstOrDefault();
                var deliveryRepository2 = unitOfWorkTemp.Repository<DeliveryHeaderDTO>();
                var deliveryLineRepository2 = unitOfWorkTemp.Repository<DeliveryLineDTO>();
                var deliveryLineMessageRepository2 = unitOfWorkTemp.Repository<MessageDTO>();

                if (delivery != null)
                {
                    foreach (var deliveryLine in delivery.DeliveryLines.Where(t => t.Enabled))
                    {
                        deliveryLine.Enabled = false;
                        deliveryLineRepository2.Update(deliveryLine);

                        var line = deliveryLine;
                        var messages = unitOfWorkTemp.Repository<MessageDTO>().Query()
                            .Filter(t => t.DeliveryLineId == line.Id)
                            .Get().ToList();

                        foreach (var message in messages)
                        {
                            message.Enabled = false;
                            deliveryLineMessageRepository2.Update(message);
                        }
                    }
                    delivery.Enabled = false;
                    deliveryRepository2.Update(delivery);
                }
                unitOfWorkTemp.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContextTemp.Dispose();
            }
            return stat;
        }

        public int Delete(string deliveryId)
        {
            try
            {
                _deliveryRepository.Delete(deliveryId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(DeliveryHeaderDTO delivery)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<DeliveryHeaderDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.OrderDate == delivery.OrderDate && bp.Id != delivery.Id)
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

        public string Validate(DeliveryHeaderDTO delivery)
        {
            if (null == delivery)
                return GenericMessages.ObjectIsNull;

            //if (delivery.WarehouseId == 0)
            //    return "Warehouse " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(delivery.Number))
                return delivery.Number + " " + GenericMessages.StringIsNullOrEmpty;

            if (delivery.Number.Length > 50)
                return delivery.Number + " can not be more than 50 characters ";

            return string.Empty;
        }

        #endregion

        #region Child Methods
        public IEnumerable<DeliveryLineDTO> GetChilds(int parentId, bool disposeWhenDone)
        {
            IEnumerable<DeliveryLineDTO> piList;
            try
            {
                piList = _deliveryLineRepository
                    .Query()
                    .Include(a => a.DeliveryHeader, a => a.Messages, a => a.FromClient, a => a.FromAddress, a => a.ToAddress, a => a.ToClient, a => a.ToStaff)
                    .Get()
                    .OrderBy(i => i.Id)
                    .ToList();

                if (parentId != 0)
                    piList = piList.Where(l => l.DeliveryHeaderId == parentId).ToList();

                #region For Eager Loading Childs
                foreach (var deliveryHeaderDTO in piList)
                {
                    var messageDtos = (ICollection<MessageDTO>)GetMessageChilds(deliveryHeaderDTO.Id, false);
                    var routeDtos = (ICollection<DeliveryRouteDTO>)GetDeliveryRouteChilds(deliveryHeaderDTO.Id, false);
                }
                #endregion
            }
            finally
            {
                Dispose(disposeWhenDone);
            }

            return piList;

        }

        public IRepositoryQuery<DeliveryLineDTO> GetChildsQuery()
        {
            var piList = _deliveryLineRepository
              .Query()
              .Include(i => i.Messages, s => s.DeliveryHeader,
              s => s.FromClient, s => s.FromClient.Address,
              s => s.ToClient, s => s.ToClient.Address,
              s => s.FromAddress, s => s.ToAddress,
              s => s.DeliveryHeader.OrderByClient,
              s => s.DeliveryHeader.OrderByClient.Address, s => s.DeliveryRoutes)
              .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<DeliveryLineDTO> GetAllChilds(SearchCriteria<DeliveryLineDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<DeliveryLineDTO> piList = new List<DeliveryLineDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {

                    var pdto = GetChildsQuery();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration

                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated >= beginDate);
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated <= endDate);
                    }

                    #endregion

                    #region For Business Partner

                    if (criteria.BusinessPartnerId != null && criteria.BusinessPartnerId != -1)
                    {
                        pdto.FilterList(w => w.DeliveryHeader.OrderByClientId == criteria.BusinessPartnerId);
                    }

                    #endregion

                    IList<DeliveryLineDTO> pdtoList;
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
                    piList = GetChildsQuery().Get().ToList();

                #region For Eager Loading Childs
                foreach (var deliveryHeaderDTO in piList)
                {
                    var messageDtos = (ICollection<MessageDTO>)GetMessageChilds(deliveryHeaderDTO.Id, false);
                    var routeDtos = (ICollection<DeliveryRouteDTO>)GetDeliveryRouteChilds(deliveryHeaderDTO.Id, false);
                }
                #endregion
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public DeliveryLineDTO FindChild(string deliveryLineId)
        {
            return _deliveryLineRepository.FindById(Convert.ToInt32(deliveryLineId));
        }

        public string InsertOrUpdateChild(DeliveryLineDTO deliveryLine)
        {
            try
            {
                var validate = ValidateChild(deliveryLine);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExistsChild(deliveryLine))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                //insert delivery route for each delivery line
                var deliveryRoute = GetDeliveryRouteChilds(deliveryLine.Id, false).FirstOrDefault();
                if (deliveryRoute == null)
                {
                    deliveryRoute = new DeliveryRouteDTO();
                    if (deliveryLine.Id != 0)
                        deliveryRoute.DeliveryLineId = deliveryLine.Id;
                    else
                        deliveryRoute.DeliveryLine = deliveryLine;

                    deliveryRoute.DeliveryType = deliveryLine.DeliveryType;
                    deliveryRoute.FromAddressId = null;
                    deliveryRoute.ToAddressId = null;
                    deliveryRoute.StartedTime = null;
                    deliveryRoute.EndedTime = null;
                    deliveryRoute.Id = 0;

                    _deliveryLineDeliveryRouteRepository.InsertUpdate(deliveryRoute);
                }

                _deliveryLineRepository.InsertUpdate(deliveryLine);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableChild(DeliveryLineDTO deliveryLine)
        {
            string stat;
            try
            {
                var dline = FindChild(deliveryLine.Id.ToString(CultureInfo.InvariantCulture));
                dline.Enabled = false;
                _deliveryLineRepository.Update(dline);

                var messages = GetMessageChilds(dline.Id, false);
                foreach (var messageDTO in messages)
                {
                    messageDTO.Enabled = false;
                    _deliveryLineMessageRepository.Update(messageDTO);
                }
                var routes = GetDeliveryRouteChilds(dline.Id, false);
                foreach (var routeDTO in routes)
                {
                    routeDTO.Enabled = false;
                    _deliveryLineDeliveryRouteRepository.Update(routeDTO);
                }
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

        public bool ObjectExistsChild(DeliveryLineDTO deliveryLine)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<DeliveryLineDTO>(iDbContext);
            //    var catExists = catRepository.GetAll()
            //        .FirstOrDefault(bp => bp.DeliveryId == deliveryLine.DeliveryId && bp.ItemId == deliveryLine.ItemId && bp.Id != deliveryLine.Id);
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            return false;// objectExists;
        }

        public string ValidateChild(DeliveryLineDTO deliveryLine)
        {
            if (null == deliveryLine)
                return GenericMessages.ObjectIsNull;

            //if (deliveryLine.DeliveryHeader == null)
            //    return "Delivery " + GenericMessages.ObjectIsNull;

            //if (deliveryLine.ItemId == 0)
            //    return "No item is found in the physical inventory line";

            //if (deliveryLine.Unit < 0)
            //    return deliveryLine.Unit + " can not be less than 0 ";

            return string.Empty;
        }
        #endregion

        #region Delivery Route Childs
        public IRepositoryQuery<DeliveryRouteDTO> GetDeliveryRouteChildsQuery()
        {
            var piList = _deliveryLineDeliveryRouteRepository
              .Query()
              .Include(i => i.AssignedToStaff, s => s.FromAddress, s => s.ToAddress, s => s.Receiver, s => s.StaffReceiver, s => s.DeliveryLine, s => s.GPSData)
              .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<DeliveryRouteDTO> GetAllDeliveryRouteChilds(SearchCriteria<DeliveryRouteDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<DeliveryRouteDTO> piList = new List<DeliveryRouteDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {

                    var pdto = GetDeliveryRouteChildsQuery();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration

                    //if (criteria.BeginingDate != null)
                    //{
                    //    var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                    //        criteria.BeginingDate.Value.Day, 0, 0, 0);
                    //    pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated >= beginDate);
                    //}

                    //if (criteria.EndingDate != null)
                    //{
                    //    var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                    //        criteria.EndingDate.Value.Day, 23, 59, 59);
                    //    pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated <= endDate);
                    //}

                    #endregion

                    #region For Business Partner

                    //if (criteria.BusinessPartnerId != null && criteria.BusinessPartnerId != -1)
                    //{
                    //    pdto.FilterList(w => w.DeliveryHeader.SenderId == criteria.BusinessPartnerId);
                    //}

                    #endregion

                    IList<DeliveryRouteDTO> pdtoList;
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
                    piList = GetDeliveryRouteChildsQuery().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public IEnumerable<DeliveryRouteDTO> GetDeliveryRouteChilds(int parentId, bool disposeWhenDone)
        {
            IEnumerable<DeliveryRouteDTO> piList;
            try
            {
                piList = _deliveryLineDeliveryRouteRepository
                        .Query()
                        .Include(i => i.AssignedToStaff, s => s.FromAddress, s => s.ToAddress, s => s.Receiver, s => s.StaffReceiver, s => s.DeliveryLine, s => s.GPSData)
                        .Get()
                        .OrderBy(i => i.Id)
                        .ToList();

                if (parentId != 0)
                    piList = piList.Where(l => l.DeliveryLineId == parentId).ToList();
            }
            finally
            {
                Dispose(disposeWhenDone);
            }

            return piList;

        }

        public DeliveryRouteDTO FindDeliveryRouteChild(string deliveryLineDeliveryRouteId)
        {
            return _deliveryLineDeliveryRouteRepository.FindById(Convert.ToInt32(deliveryLineDeliveryRouteId));
        }

        public string InsertOrUpdateDeliveryRouteChild(DeliveryRouteDTO deliveryRoute)
        {
            string stat;
            try
            {
                //var validate = ValidateChild(deliveryLine);
                if (deliveryRoute == null)
                    return "DeliveryRoute is Null";

                //if (ObjectExistsChild(deliveryLine))
                //    return GenericDeliveryRoutes.DatabaseErrorRecordAlreadyExists;

                _deliveryLineDeliveryRouteRepository.InsertUpdate(deliveryRoute);
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

        public string DisableDeliveryRouteChild(DeliveryRouteDTO deliveryRoute)
        {
            string stat;
            try
            {
                var msg = FindDeliveryRouteChild(deliveryRoute.Id.ToString(CultureInfo.InvariantCulture));
                msg.Enabled = false;
                _deliveryLineDeliveryRouteRepository.Update(msg);
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
        #endregion

        #region Message Childs
        public IRepositoryQuery<MessageDTO> GetMessageChildsQuery()
        {
            var piList = _deliveryLineMessageRepository
              .Query()
              .Include(i => i.Category, s => s.UnitOfMeasure, s => s.StorageBin, s => s.DeliveryLine)
              .OrderBy(q => q.OrderByDescending(c => c.Id));
            return piList;
        }

        public IEnumerable<MessageDTO> GetAllMessageChilds(SearchCriteria<MessageDTO> criteria, out int totalCount)
        {
            totalCount = 0;
            IEnumerable<MessageDTO> piList = new List<MessageDTO>();
            try
            {
                if (criteria != null && criteria.CurrentUserId != -1)
                {

                    var pdto = GetMessageChildsQuery();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration

                    //if (criteria.BeginingDate != null)
                    //{
                    //    var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                    //        criteria.BeginingDate.Value.Day, 0, 0, 0);
                    //    pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated >= beginDate);
                    //}

                    //if (criteria.EndingDate != null)
                    //{
                    //    var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                    //        criteria.EndingDate.Value.Day, 23, 59, 59);
                    //    pdto.FilterList(p => p.DeliveryHeader.DateRecordCreated <= endDate);
                    //}

                    #endregion

                    #region For Business Partner

                    //if (criteria.BusinessPartnerId != null && criteria.BusinessPartnerId != -1)
                    //{
                    //    pdto.FilterList(w => w.DeliveryHeader.SenderId == criteria.BusinessPartnerId);
                    //}

                    #endregion

                    IList<MessageDTO> pdtoList;
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
                    piList = GetMessageChildsQuery().Get().ToList();
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public IEnumerable<MessageDTO> GetMessageChilds(int parentId, bool disposeWhenDone)
        {
            IEnumerable<MessageDTO> piList;
            try
            {
                piList = _deliveryLineMessageRepository
                    .Query()
                    .Include(a => a.DeliveryLine, a => a.Category, a => a.UnitOfMeasure, a => a.StorageBin)
                    .Get()
                    .OrderBy(i => i.Id)
                    .ToList();

                if (parentId != 0)
                    piList = piList.Where(l => l.DeliveryLineId == parentId).ToList();
            }
            finally
            {
                Dispose(disposeWhenDone);
            }

            return piList;

        }

        public MessageDTO FindMessageChild(string deliveryLineMessageId)
        {
            return _deliveryLineMessageRepository.FindById(Convert.ToInt32(deliveryLineMessageId));
        }

        public string InsertOrUpdateMessageChild(MessageDTO messsage)
        {
            string stat;
            try
            {
                //var validate = ValidateChild(deliveryLine);
                if (messsage == null)
                    return "Message is Null";

                //if (ObjectExistsChild(deliveryLine))
                //    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _deliveryLineMessageRepository.InsertUpdate(messsage);
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

        public string DisableMessageChild(MessageDTO message)
        {
            string stat;
            try
            {
                var msg = FindMessageChild(message.Id.ToString(CultureInfo.InvariantCulture));
                msg.Enabled = false;
                _deliveryLineMessageRepository.Update(msg);
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
