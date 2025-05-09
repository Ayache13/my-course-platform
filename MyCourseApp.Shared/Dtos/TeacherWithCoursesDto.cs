﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourseApp.Shared.Dtos
{
    public class TeacherWithCoursesDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<CourseDto> Courses { get; set; } = new();
    }

}
