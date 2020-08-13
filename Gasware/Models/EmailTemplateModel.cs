using System;

namespace Gasware.Models
{
    public class EmailTemplateModel
    {
        public int EmailTemplateId { get; set; }
        public string EmailId { get; set; }
        public string DisplayName{ get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailType { get; set; }
        public DateTime CreateDate { get; set; }
        public String CreateBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

    }
}
