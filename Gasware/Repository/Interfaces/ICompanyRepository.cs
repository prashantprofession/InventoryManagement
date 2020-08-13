using Gasware.Models;
using System;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        CompanyModel Get(int id);
        List<CompanyModel> GetCompanies();
        void Update(CompanyModel companyModel);
        int Create(CompanyModel companyModel);
        void Delete(CompanyModel companyModel);
        string GetLicense(int id);
        DateTime GetLicenseExpiryDate(int id);
        decimal GetTaxRate(int id, string type);
    }
}
