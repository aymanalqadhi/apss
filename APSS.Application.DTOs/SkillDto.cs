using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APSS.Application.DTOs
{
    public class SkillDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Field { get; set; } = null!;
        public string NameIndividual { get; set; } = null!;
    }
}