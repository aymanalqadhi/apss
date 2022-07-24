using APSS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APSS.Web.Dtos
{
    internal class ConfirmableDto : BaseAuditbleDto
    {
        public bool? IsConfirmed { get; set; }
    }
}