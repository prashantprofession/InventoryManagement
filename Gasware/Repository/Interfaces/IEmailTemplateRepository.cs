using Gasware.Models;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IEmailTemplateRepository
    {
        EmailTemplateModel Get(int id);
        List<EmailTemplateModel> GetEmailTemplates();
        void Update(EmailTemplateModel emailTemplateModel);
        int Create(EmailTemplateModel emailTemplateModel);
        void Delete(EmailTemplateModel emailTemplateModel);
    }
}
