using Gasware.Models;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IDeliveryPersonRepository
    {
        DeliveryPersonModel Get(int id);
        List<DeliveryPersonModel> GetDeliveryPersons();
        void Update(DeliveryPersonModel deliveryPersonModel);
        int Create(DeliveryPersonModel deliveryPersonModel);
        void Delete(DeliveryPersonModel deliveryPersonModel);
    }
}
