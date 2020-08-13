using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System.Collections.Generic;

namespace Gasware.Service.Interfaces
{
    public interface IDeliveryPersonService
    {
        DeliveryPersonViewModel Get(int id);
        List<DeliveryPersonViewModel> GetDeliveryPersons();
        List<DeliveryPersonReportModel> GetDeliveryPersonReportModels();
        void Update(DeliveryPersonViewModel deliveryPersonViewModel);
        void Add(DeliveryPersonViewModel deliveryPersonViewModel);
        void Delete(DeliveryPersonViewModel deliveryPersonViewModel);
        DeliveryPersonViewModel GetViewModel(DeliveryPersonModel deliveryPersonModel);
        DeliveryPersonModel GetDatabaseModel(DeliveryPersonViewModel deliveryPersonViewModel);
    }
}
