using APSS.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APSS.Web.Dtos
{
    internal class ProductExpenseDto : BaseAuditbleDto
    {
        public string Type { get; set; } = null!;

        public decimal Price { get; set; }

        public ProductDto SpentOn { get; set; } = null!;
    }
}