using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IPaymentService : IDisposable
    {
        IEnumerable<PaymentDTO> GetAll(SearchCriteria<PaymentDTO> criteria = null);
        PaymentDTO Find(string paymentId);
        PaymentDTO GetByName(string displayName);
        string InsertOrUpdate(PaymentDTO payment);
        string Disable(PaymentDTO payment);
        int Delete(string paymentId);

    }
}