using System;
using System.Collections.Generic;
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
    public class PaymentService : IPaymentService
    {
        #region Fields

        private IDbContext _iDbContext;
        private IUnitOfWork _unitOfWork;
        private IRepository<PaymentDTO> _paymentRepository;
        private IRepository<DeliveryLineDTO> _transactionRepository;
        private IRepository<TaskProcessDTO> _taskProcessRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public PaymentService()
        {
            InitializeDbContext();
        }
        public PaymentService(IDbContext iDbContext)
        {
            _iDbContext = iDbContext;
            _unitOfWork = new UnitOfWork(_iDbContext);
            _paymentRepository = _unitOfWork.Repository<PaymentDTO>();
            _transactionRepository = _unitOfWork.Repository<DeliveryLineDTO>();
            _taskProcessRepository = _unitOfWork.Repository<TaskProcessDTO>();
            
        }
        public PaymentService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            _iDbContext = DbContextUtil.GetDbContextInstance();
            _unitOfWork = new UnitOfWork(_iDbContext);
            _paymentRepository = _unitOfWork.Repository<PaymentDTO>();
            _transactionRepository = _unitOfWork.Repository<DeliveryLineDTO>();
            _taskProcessRepository = _unitOfWork.Repository<TaskProcessDTO>();
        }

        #endregion

        #region Common Methods
        public IRepositoryQuery<PaymentDTO> Get()
        {
            var piList = _paymentRepository
                .Query()
                .Include(c => c.TaskProcess,
                         c => c.DeliveryLine,
                         c => c.DeliveryLine.DeliveryHeader,
                         c => c.DeliveryLine.PaymentsandExpenses,
                         s => s.DeliveryLine.DeliveryHeader.OrderByClient)
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piList;
            
        }
        
        public IEnumerable<PaymentDTO> GetAll(SearchCriteria<PaymentDTO> criteria = null)
        {
            IEnumerable<PaymentDTO> piList = new List<PaymentDTO>();
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
                            pdto.FilterList(p => p.PaymentDate >= beginDate);
                        }

                        if (criteria.EndingDate != null)
                        {
                            var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                                criteria.EndingDate.Value.Day, 23, 59, 59);
                            pdto.FilterList(p => p.PaymentDate <= endDate);
                        }

                        #endregion
                        
                        #region By Payment Method
                        if (criteria.PaymentMethodType != -1)
                        {
                            switch ((PaymentMethods)criteria.PaymentMethodType)
                            {
                                case PaymentMethods.Cash:
                                    pdto.FilterList(p => p.Method == PaymentMethods.Cash);
                                    break;
                                case PaymentMethods.Credit:
                                    pdto.FilterList(p => p.Method == PaymentMethods.Credit);
                                    break;
                                case PaymentMethods.Check:
                                    pdto.FilterList(p => p.Method == PaymentMethods.Check);
                                    break;
                            }
                        }
                        #endregion

                        #region By Payment Type
                        if (criteria.PaymentType != -1)
                        {
                            switch (criteria.PaymentType)
                            {
                                case 2:
                                    pdto.FilterList(p => p.Type == PaymentTypes.CashOut);
                                    break;
                                case 5:
                                    pdto.FilterList(p => p.Type == PaymentTypes.CashIn);
                                    break;
                            }
                        }
                        #endregion

                        piList = piList.Concat(pdto.GetList().ToList());
                    
                }
                else
                {
                    piList = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return piList;

        }

        public PaymentDTO Find(string paymentId)
        {
            var orgDto = _paymentRepository.FindById(Convert.ToInt32(paymentId));
            if (_disposeWhenDone)
                Dispose();
            return orgDto;
        }

        public PaymentDTO GetByName(string displayName)
        {
            var cat = _paymentRepository.Query().Filter(c => c.PersonName == displayName).Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(PaymentDTO payment)
        {
            try
            {
                var validate = Validate(payment);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(payment))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists + Environment.NewLine +
                           "With the same Name Exists";

                _paymentRepository.InsertUpdate(payment);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(PaymentDTO payment)
        {

            if (payment == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            try
            {
                _paymentRepository.Update(payment);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            return stat;
        }

        public int Delete(string paymentId)
        {
            try
            {
                _paymentRepository.Delete(paymentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception exception)
            {
                return -1;
            }
        }

        public bool ObjectExists(PaymentDTO payment)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<PaymentDTO>(iDbContext);
            //    var catExists = catRepository.GetAll()
            //        .FirstOrDefault(bp => (bp.DisplayName == payment.DisplayName ||
            //        (!string.IsNullOrEmpty(bp.TinNumber) && bp.TinNumber == payment.TinNumber)) &&
            //        bp.Id != payment.Id);


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

        public string Validate(PaymentDTO payment)
        {
            if (null == payment)
                return GenericMessages.ObjectIsNull;
            
            if (String.IsNullOrEmpty(payment.Reason))
                return payment.Reason + " " + GenericMessages.StringIsNullOrEmpty;

            if (String.IsNullOrEmpty(payment.PersonName))
                return payment.PersonName + " " + GenericMessages.StringIsNullOrEmpty;

            return string.Empty;
        }

        #endregion

        #region Private Methods
        public PaymentDTO GetNewPayment(DeliveryLineDTO selectedTransaction, DateTime paymentDate)
        {
            if (selectedTransaction != null)
            {
                return new PaymentDTO
                {
                    //PaymentMethod = PaymentMethods.Check,
                    Status = PaymentStatus.NotCleared,
                    PaymentDate = paymentDate,
                    PersonName = selectedTransaction.DeliveryHeader.OrderByClient.ToString(),
                    //Reason = selectedTransaction.TransactionType.ToString() + "-" +
                    //         selectedTransaction.TransactionNumber + "-" +
                    //         selectedTransaction.TransactionDateString
                };
            }
            return null;
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