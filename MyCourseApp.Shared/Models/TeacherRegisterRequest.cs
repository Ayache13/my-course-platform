﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourseApp.Shared.Models
{
    public class TeacherRegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public string Biography { get; set; } = string.Empty;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
