using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentStream.Core.DTOs.jobApplication
{
    public class UpdateApplicationStatusDto
    {
        public string Status { get; set; } = string.Empty; // "Reviewing", "Accepted", "Rejected"
    }
}