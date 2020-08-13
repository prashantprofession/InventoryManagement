using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using Unity;

namespace Gasware.Service
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly string _username;
        private IDeliveryPersonRepository _deliveryPersonRepository;

        //public DeliveryPersonService(IDeliveryPersonRepository localdeliveryPersonRepository)
        //{
        //    deliveryPersonRepository = localdeliveryPersonRepository;
        //}

        public DeliveryPersonService(IDeliveryPersonRepository deliveryPersonRepository)
        {
            _deliveryPersonRepository = deliveryPersonRepository;
        }


        public DeliveryPersonService(IUnityContainer container, string username)
        {
            _username = username;
            _deliveryPersonRepository = container.Resolve<IDeliveryPersonRepository>();
        }

        public DeliveryPersonService(string username)
        {
            _username = username;
        }

        public DeliveryPersonViewModel Get(int id)
        {
            if (_deliveryPersonRepository == null)
            {
                _deliveryPersonRepository = new DeliveryPersonRepository(_username);
            }

            DeliveryPersonModel deliveryPersonModel = _deliveryPersonRepository.Get(id);
            if (deliveryPersonModel == null)
            {
                return null;
            }
            return GetViewModel(deliveryPersonModel);
        }

        public List<DeliveryPersonViewModel> GetDeliveryPersons()
        {
            if (_deliveryPersonRepository == null)
            {
                _deliveryPersonRepository = new DeliveryPersonRepository(_username);
            }
            List<DeliveryPersonModel> deliveryPersonModels = _deliveryPersonRepository.GetDeliveryPersons();
            List<DeliveryPersonViewModel> deliveryPersonViewModels = new List<DeliveryPersonViewModel>();
            foreach (DeliveryPersonModel deliveryPersonModel in deliveryPersonModels)
            {
                deliveryPersonViewModels.Add(GetViewModel(deliveryPersonModel));
            }
            return deliveryPersonViewModels;
        }

        public List<DeliveryPersonReportModel> GetDeliveryPersonReportModels()
        {
            List<DeliveryPersonReportModel> deliveryPersonReportModels = new List<DeliveryPersonReportModel>();
            List<DeliveryPersonViewModel> deliveryPersonViewModels = GetDeliveryPersons();
            foreach(DeliveryPersonViewModel deliveryPersonViewModel in deliveryPersonViewModels)
            {
                deliveryPersonReportModels.Add(new DeliveryPersonReportModel()
                {
                    AddressId = deliveryPersonViewModel.Address.AddressId,
                    AddressLine1 = deliveryPersonViewModel.Address.AddressLine1,
                    AddressLine2= deliveryPersonViewModel.Address.AddressLine2,
                    City = deliveryPersonViewModel.Address.City,
                    Country = deliveryPersonViewModel.Address.Country,
                    DeliveryPersonId = deliveryPersonViewModel.DeliveryPersonId,
                    Name = deliveryPersonViewModel.Name,
                    PhoneNumber = deliveryPersonViewModel.PhoneNumber,
                    PinCode = deliveryPersonViewModel.Address.PinCode,
                    State = deliveryPersonViewModel.Address.State,
                    Street  = deliveryPersonViewModel.Address.Street
                });
            }
            return deliveryPersonReportModels;
        }

        public DeliveryPersonViewModel GetViewModel(DeliveryPersonModel deliveryPersonModel)
        {
            return new DeliveryPersonViewModel()
            {
                Address = deliveryPersonModel.Address,
                DeliveryPersonId = deliveryPersonModel.DeliveryPersonId,
                Name = deliveryPersonModel.Name,
                PhoneNumber = deliveryPersonModel.PhoneNumber
            };
        }

        public DeliveryPersonModel GetDatabaseModel(DeliveryPersonViewModel deliveryPersonViewModel)
        {
            return new DeliveryPersonModel()
            {
                Address = deliveryPersonViewModel.Address,
                DeliveryPersonId = deliveryPersonViewModel.DeliveryPersonId,
                Name = deliveryPersonViewModel.Name,
                PhoneNumber = deliveryPersonViewModel.PhoneNumber
            };
        }


        public void Add(DeliveryPersonViewModel deliveryPersonViewModel)
        {
            if (_deliveryPersonRepository == null)
            {
                _deliveryPersonRepository = new DeliveryPersonRepository(_username);
            }
            DeliveryPersonModel deliveryPersonModel = GetDatabaseModel(deliveryPersonViewModel);
            _deliveryPersonRepository.Create(deliveryPersonModel);
        }

        public void Update(DeliveryPersonViewModel deliveryPersonViewModel)
        {
            if (_deliveryPersonRepository == null)
            {
                _deliveryPersonRepository = new DeliveryPersonRepository(_username);
            }
            DeliveryPersonModel deliveryPersonModel = GetDatabaseModel(deliveryPersonViewModel);
            _deliveryPersonRepository.Update(deliveryPersonModel);
        }

        public void Delete(DeliveryPersonViewModel deliveryPersonViewModel)
        {
            if (_deliveryPersonRepository == null)
            {
                _deliveryPersonRepository = new DeliveryPersonRepository(_username);
            }
            DeliveryPersonModel deliveryPersonModel = GetDatabaseModel(deliveryPersonViewModel);
            _deliveryPersonRepository.Delete(deliveryPersonModel);
        }
    }
}
