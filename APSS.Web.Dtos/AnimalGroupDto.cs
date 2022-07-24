using APSS.Domain.Entities;
using APSS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APSS.Web.Dtos
{
    internal class AnimalGroupDto : BaseAuditbleDto
    {
        public string Type { get; set; } = null!;

        public int Quantity { get; set; }

        public AnimalSex Sex { get; set; }
    }
}