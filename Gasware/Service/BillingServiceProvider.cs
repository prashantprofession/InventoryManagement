using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Gasware.Billing
{
    public class BillingServiceProvider : IBillingService
    {
        private IBillingRepository _billingRepositoryProvider;
        private ICustomerService _customerService;
        private IDeliveryPersonService _deliveryPersonService;
        private IProductService _productService;
        private readonly string _username;
        private IInvoiceRepository _invoiceRepository;

        public BillingServiceProvider(IUnityContainer container, string username)
        {
            _username = username;
            _customerService = container.Resolve<ICustomerService>();
            _deliveryPersonService = container.Resolve<IDeliveryPersonService>();
            _productService = container.Resolve<IProductService>();
            _billingRepositoryProvider = container.Resolve<IBillingRepository>();
            _invoiceRepository = container.Resolve<IInvoiceRepository>();
        }

        public BillingServiceProvider(string username)
        {
            _username = username;
        }
        public BillingViewModel Get(int id)
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            BillingModel billingModel = _billingRepositoryProvider.GetBilling(id);
            return GetViewModel(billingModel);
        }

        public List<BillingViewModel> GetDashboardBillings(string username)
        {
            _billingRepositoryProvider = new BillingRepository(username);
            return GetBillings();
        }

        public List<BillingViewModel> GetBillings()
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            List<BillingModel> billingModels = _billingRepositoryProvider.GetAllBillings();
            List<BillingViewModel> returnBillingViewModels = new List<BillingViewModel>();
            foreach (BillingModel billingModel in billingModels)
            {
                returnBillingViewModels.Add(GetViewModel(billingModel));
            }
            return returnBillingViewModels;
        }

        public List<BillingReportModel> GetBillingReportModels()
        {
            List<BillingViewModel> billingViewModels = GetBillings();
            List<BillingReportModel> billingReportModels = new List<BillingReportModel>();
            foreach (BillingViewModel billing in billingViewModels)
            {
                billingReportModels.Add(GetReportModelForView(billing));
            }
            return billingReportModels;
        }

        public List<BillingReportModel> GetBillingReportModels(TransactionsFilterInputModel filterInputModel)
        {
            List<BillingViewModel> billingViewModels = GetFilteredBills(filterInputModel);
            List<BillingReportModel> billingReportModels = new List<BillingReportModel>();
            foreach (BillingViewModel billing in billingViewModels)
            {
                billingReportModels.Add(GetReportModelForView(billing));
            }
            return billingReportModels;
        }

        public BillingReportModel GetReportModelForView(BillingViewModel billing)
        {
            return new BillingReportModel()
            {
                BilledAmount = billing.BilledAmount,
                BillingDate = billing.BillingDate,
                BillingId = billing.BillingId,
                Customer = billing.Customer.Name,
                DeliveredFullCylinderQty = billing.DeliveredFullCylinderQty,
                DeliveryPerson = billing.DeliveryPerson.Name,
                Invoice = billing?.Invoice?.InvoiceId ?? 0,
                PaidAmount = billing.PaidAmount,
                Product = billing.Product.Name,
                Rate = billing.Rate,
                ReceivedEmptyCylinderQty = billing.ReceivedEmptyCylinderQty,
                Details = billing.Details
            };
        }


        public List<BillingViewModel> GetBillingsForDate(DateTime dateTime)
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            List<BillingModel> billingModels = _billingRepositoryProvider.GetBillingsForDate(dateTime);
            List<BillingViewModel> returnBillingViewModels = new List<BillingViewModel>();
            foreach (BillingModel billingModel in billingModels)
            {
                returnBillingViewModels.Add(GetViewModel(billingModel));
            }
            return returnBillingViewModels;
        }

        public List<BillingViewModel> GetFilteredBills(TransactionsFilterInputModel transactionsFilter)
        {
            if (transactionsFilter == null)
            {
                transactionsFilter = new TransactionsFilterInputModel()
                {
                    FromDate = DateTime.MinValue,
                    ToDate = DateTime.MaxValue
                };
            }
            List<BillingViewModel> billingViews = GetBillings();
            billingViews = billingViews.Where(x => x.BillingDate >= transactionsFilter.FromDate &&
                                                   x.BillingDate <= transactionsFilter.ToDate).ToList();
            if (transactionsFilter.ProductView != null)
            {
                billingViews = billingViews.Where(x => x.Product.Productid ==
                                    transactionsFilter.ProductView.Productid).ToList();
            }

            if (transactionsFilter.CustomerView != null)
            {
                billingViews = billingViews.Where(x => x.Customer.CustomerId ==
                                    transactionsFilter.CustomerView.CustomerId).ToList();
            }

            if (transactionsFilter.DeliveryPersonView != null)
            {
                billingViews = billingViews.Where(x => x.DeliveryPerson.DeliveryPersonId ==
                                    transactionsFilter.DeliveryPersonView.DeliveryPersonId).ToList();
            }
            return billingViews;
        }

        public BillingViewModel GetViewModel(BillingModel billingModel)
        {
            if (_customerService == null)
            {
                _customerService = new CustomerServiceProvider(_username);
            }

            if (_deliveryPersonService == null)
            {
                _deliveryPersonService = new DeliveryPersonService(_username);
            }

            if (_productService == null)
            {
                _productService = new ProductServiceProvider(_username);
            }

            if (_invoiceRepository == null)
            {
                _invoiceRepository = new InvoiceRepository(_username);
            }

            BillingViewModel viewModel = new BillingViewModel()
            {
                BilledAmount = billingModel.BilledAmount,
                BillingId = billingModel.BillingId,
                DeliveredFullCylinderQty = billingModel.DeliveredFullCylinderQty,
                PaidAmount = billingModel.PaidAmount,
                ReceivedEmptyCylinderQty = billingModel.ReceivedEmptyCylinderQty,
                Customer = _customerService.Get(billingModel.Customer.CustomerId),
                DeliveryPerson = _deliveryPersonService.Get(billingModel.DeliveryPerson.DeliveryPersonId),
                Product = _productService.Get(billingModel.Product.Productid),
                BillingDate = billingModel.BillingDate,
                Rate = billingModel.Rate,
                Invoice = _invoiceRepository.Get(billingModel.Invoice.InvoiceId),
                Details = billingModel.Details,
                IsDeleteButtonEnabled = false
            };
            if (!viewModel.Product.IsBillable || viewModel.Product.IsExpense)
            {
                viewModel.Invoice = new InvoiceModel()
                {
                    IsBillRequired = false,                    
                };
                viewModel.IsDeleteButtonEnabled = true;
            }                
            return viewModel;
        }

        public BillingModel GetModelFromView(BillingViewModel viewModel)
        {
            if (_customerService == null)
            {
                _customerService = new CustomerServiceProvider(_username);
            }

            if (_deliveryPersonService == null)
            {
                _deliveryPersonService = new DeliveryPersonService(_username);
            }

            if (_productService == null)
            {
                _productService = new ProductServiceProvider(_username);
            }

            return new BillingModel()
            {
                BilledAmount = viewModel.BilledAmount,
                BillingId = viewModel.BillingId,
                DeliveredFullCylinderQty = viewModel.DeliveredFullCylinderQty,
                PaidAmount = viewModel.PaidAmount,
                ReceivedEmptyCylinderQty = viewModel.ReceivedEmptyCylinderQty,
                Customer = viewModel.Customer == null ? new CustomerViewModel() : 
                                    _customerService.Get(viewModel.Customer.CustomerId),
                Product = _productService.Get(viewModel.Product.Productid),
                DeliveryPerson = _deliveryPersonService.Get(viewModel.DeliveryPerson.DeliveryPersonId),
                BillingDate = viewModel.BillingDate,
                Rate = viewModel.Rate,
                Invoice = viewModel.Invoice,
                Details = viewModel.Details
            };
        }

        public int Add(BillingViewModel billingViewModel)
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            BillingModel billingModel = GetModelFromView(billingViewModel);
            return _billingRepositoryProvider.Create(billingModel);
        }

        public void Update(BillingViewModel billingViewModel)
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            BillingModel billingModel = GetModelFromView(billingViewModel);
            _billingRepositoryProvider.Update(billingModel);
        }

        public void Delete(int billingId)
        {
            if (_billingRepositoryProvider == null)
            {
                _billingRepositoryProvider = new BillingRepository(_username);
            }
            _billingRepositoryProvider.Delete(billingId);
        }
    }
}
