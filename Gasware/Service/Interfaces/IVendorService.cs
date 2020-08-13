using Gasware.ReportModels;
using Gasware.ViewModels;
using System.Collections.Generic;

namespace Gasware.Service.Interfaces
{
    public interface IVendorService
    {
        VendorViewModel Get(int id);
        List<VendorViewModel> GetVendors();
        List<VendorReportModel> GetVendorReportModels();
        void Update(VendorViewModel vendorViewModel);
        int Create(VendorViewModel vendorViewModel);
        void Delete(VendorViewModel vendorViewModel);
    }
}
