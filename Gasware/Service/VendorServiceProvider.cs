using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    public class VendorServiceProvider : IVendorService
    {


        private static string _username;
        private IVendorRepository _vendorRepository;

       
        public VendorServiceProvider(IUnityContainer container, string username)
        {
            _username = username;
            _vendorRepository = container.Resolve<IVendorRepository>();
        }

        public VendorServiceProvider(string username)
        {
           _username = username;
        }


        

        public int Create(VendorViewModel vendorViewModel)
        {
            if (_vendorRepository == null)
            {
                _vendorRepository = new VendorRepository("admin");
            }
            return _vendorRepository.Create(GetVendorModel(vendorViewModel));
        }

        public void Delete(VendorViewModel vendorViewModel)
        {
            if (_vendorRepository == null)
            {
                _vendorRepository = new VendorRepository("admin");
            }
            _vendorRepository.Delete(GetVendorModel(vendorViewModel));
        }

        public VendorViewModel Get(int id)
        {
            if (_vendorRepository == null)
            {
                _vendorRepository = new VendorRepository("admin");
            }
            return GetViewModel(_vendorRepository.Get(id));
        }

 
        public List<VendorViewModel> GetVendors()
        {
            if (_vendorRepository == null)
            {
                _vendorRepository = new VendorRepository("admin");
            }
            List<VendorModel> vendorModels = _vendorRepository.GetVendors();
            List<VendorViewModel> vendorViewModels = new List<VendorViewModel>();
            foreach(VendorModel vendorModel in vendorModels)
            {
                vendorViewModels.Add(GetViewModel(vendorModel));
            }
            return vendorViewModels;
        }

        public List<VendorReportModel> GetVendorReportModels()
        {
            List<VendorViewModel> vendorViewModels = GetVendors();
            List<VendorReportModel> vendorReports = new List<VendorReportModel>();
            foreach(VendorViewModel vendorViewModel in vendorViewModels)
            {
                vendorReports.Add(new VendorReportModel()
                {
                    AddressId = vendorViewModel.Address.AddressId,
                    SupplierId = vendorViewModel.SupplierId,
                    AddressLine1 = vendorViewModel.Address.AddressLine1,
                    AddressLine2 = vendorViewModel.Address.AddressLine2,
                    City = vendorViewModel.Address.City,
                    Country= vendorViewModel.Address.City,
                    Name = vendorViewModel.Name,
                    PhoneNumber = vendorViewModel.PhoneNumber,
                    PinCode = vendorViewModel.Address.PinCode,
                    State = vendorViewModel.Address.State,
                    Street = vendorViewModel.Address.Street
                });
            }
            return vendorReports;
        }

        public static VendorViewModel GetViewModel(VendorModel vendorModel)
        {
            return new VendorViewModel()
            {
                Address = vendorModel.Address,
                Name = vendorModel.Name,
                PhoneNumber = vendorModel.PhoneNumber,
                SupplierId = vendorModel.SupplierId
            };
        }

        public static VendorModel GetVendorModel(VendorViewModel viewModel)
        {
            return new VendorModel()
            {
                Address = viewModel.Address,
                Name = viewModel.Name,
                PhoneNumber = viewModel.PhoneNumber,
                SupplierId = viewModel.SupplierId
            };
        }

        public void Update(VendorViewModel vendorViewModel)
        {
            if (_vendorRepository == null)
            {
                _vendorRepository = new VendorRepository("admin");
            }
            _vendorRepository.Update(GetVendorModel(vendorViewModel));
        }

        //public void Save()
        //{
        //    if (_vendorRepository == null)
        //    {
        //        _vendorRepository = new VendorRepository("admin");
        //    }
        //    //vendorModel = GetVendorModelFromUIVendor(vendor);
        //    if (vendorModel.SupplierId == 0 || vendorModel == null)
        //    {                
        //        int vendorId = _vendorRepository.Create(vendorModel);
        //        if (vendorId == 0)
        //            return;
        //    }
        //    else
        //    {
        //        _vendorRepository.Update(vendorModel);
        //    }
        //    MessageBox.Show("Vendor details saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //}     
    }
}
