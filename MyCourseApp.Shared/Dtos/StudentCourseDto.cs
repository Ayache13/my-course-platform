public class StudentCourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }


    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }

    public DateTime EnrollmentDate { get; set; }

  
    public List<string> Tags { get; set; } = new();
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
}
