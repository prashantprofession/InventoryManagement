using Gasware.Models;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IVendorRepository
    {
        VendorModel Get(int id);
        List<VendorModel> GetVendors();
        void Update(VendorModel vendorModel);
        int Create(VendorModel vendorModel);
        void Delete(VendorModel vendorModel);
    }
}
