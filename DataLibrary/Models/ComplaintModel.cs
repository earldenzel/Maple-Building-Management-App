using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class ComplaintModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int PropertyManagerId { get; set; }
        public DateTime IncidentDate { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Details { get; set; }
        public int ComplaintTypeId { get; set; }
        public int ComplaintStatusId { get; set; }
    }
}
