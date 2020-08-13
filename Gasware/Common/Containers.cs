using Gasware.Billing;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;

namespace Gasware.Common
{
    public static class Containers
    {
        public static UnityContainer GetAllContaines()
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IAddressRepository, AddressRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IBillingRepository, BillingRepository>(new InjectionConstructor("admin"));
            container.RegisterType<ICompanyRepository, CompanyRepository>(new InjectionConstructor("admin"));
            container.RegisterType<ICustomerRepository, CustomerRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IDeliveryPersonRepository, DeliveryPersonRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IProductRepository, ProductRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IStockEntryRepository, StockEntryRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IUserRespository, UserRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IVendorRepository, VendorRepository>(new InjectionConstructor("admin"));

            container.RegisterType<IBillingService, BillingServiceProvider>( new InjectionConstructor("admin"));
            container.RegisterType<ICustomerService, CustomerServiceProvider>(new InjectionConstructor("admin"));
            container.RegisterType<IDeliveryPersonService, DeliveryPersonService>(new InjectionConstructor("admin"));
            container.RegisterType<IProductService, ProductServiceProvider>(new InjectionConstructor("admin"));
            container.RegisterType<IVendorService, VendorServiceProvider>(new InjectionConstructor("admin"));
            container.RegisterType<IDailyReportRepository, DailyReportRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IInvoiceRepository, InvoiceRepository>(new InjectionConstructor("admin"));
            container.RegisterType<IEmailTemplateRepository, EmailTemplateRepository>(new InjectionConstructor("admin"));
            return container;
        }
    }
}
