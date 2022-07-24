using APSS.Domain.Entities;
using APSS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APSS.Web.Dtos
{
    internal class AnimalProductDto : BaseAuditbleDto
    {
        public string Name { get; set; } = null!;

        public AnimalProductUnit Unit { get; set; } = null!;

        public double Quantity { get; set; }

        public TimeSpan PeriodTaken { get; set; }

        public AnimalGroupDto Producer { get; set; } = null!;
    }
}