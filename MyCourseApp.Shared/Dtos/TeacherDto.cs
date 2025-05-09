using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourseApp.Shared.Dtos
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Specialization { get; set; } // للاستخدام لاحقًا
        public string? Biography { get; set; } // للاستخدام لاحقًا
    }
}

