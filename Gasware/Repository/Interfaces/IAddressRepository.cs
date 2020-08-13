using Gasware.Models;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IAddressRepository
    {
        AddressModel Get(int id);
        List<AddressModel> GetAddresses();
        int Create(AddressModel addressModel);
        int Update(AddressModel address);
        void Delete(int id);       
    }
}
