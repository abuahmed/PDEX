using System;
using System.Collections.Generic;
using PDEX.Core;
using PDEX.Core.Models;

namespace PDEX.Service.Interfaces
{
    public interface IDeliveryService : IDisposable
    {
        IEnumerable<DeliveryHeaderDTO> GetAll(SearchCriteria<DeliveryHeaderDTO> criteria = null);
        IEnumerable<DeliveryHeaderDTO> GetAll(SearchCriteria<DeliveryHeaderDTO> criteria, out int totalCount);
        DeliveryHeaderDTO Find(string transactionHeaderId);
        
        string InsertOrUpdate(DeliveryHeaderDTO transactionHeader);
        string Disable(DeliveryHeaderDTO transactionHeader);
        int Delete(string transactionHeaderId);
        
        IEnumerable<DeliveryLineDTO> GetChilds(int parentId, bool disposeWhenDone);
        IEnumerable<DeliveryLineDTO> GetAllChilds(SearchCriteria<DeliveryLineDTO> criteria, out int totalCount);
        string InsertOrUpdateChild(DeliveryLineDTO transactionLine);
        string DisableChild(DeliveryLineDTO transactionLine);

        IEnumerable<MessageDTO> GetAllMessageChilds(SearchCriteria<MessageDTO> criteria, out int totalCount);
        IEnumerable<MessageDTO> GetMessageChilds(int parentId, bool disposeWhenDone);
        string InsertOrUpdateMessageChild(MessageDTO messsage);
        string DisableMessageChild(MessageDTO message);

        IEnumerable<DeliveryRouteDTO> GetAllDeliveryRouteChilds(SearchCriteria<DeliveryRouteDTO> criteria, out int totalCount);
        IEnumerable<DeliveryRouteDTO> GetDeliveryRouteChilds(int parentId, bool disposeWhenDone);
        string InsertOrUpdateDeliveryRouteChild(DeliveryRouteDTO deliveryRoute);
        string DisableDeliveryRouteChild(DeliveryRouteDTO deliveryRoute);
    }
}