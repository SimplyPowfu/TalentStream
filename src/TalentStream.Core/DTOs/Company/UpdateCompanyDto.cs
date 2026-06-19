using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentStream.Core.DTOs.Company
{
    public class UpdateCompanyDto
    {
        public string? Name { get; set; }
		public string? Industry { get; set; }
		public string? Location { get; set; }
    }
}