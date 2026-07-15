namespace task02;

public class StudentService
{
    using System;
using System.Collections.Generic;
using System.Linq;

namespace task02;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students ?? throw new ArgumentNullException(nameof(students));

    public IEnumerable<Student> GetStudentsByFaculty(string faculty) =>
        _students.Where(s => s.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade) =>
        _students.Where(s => s.Grades.Any() && s.Grades.Average() >= minAverageGrade);

    public IEnumerable<Student> GetStudentsOrderedByName() =>
        _students.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase);

    public ILookup<string, Student> GroupStudentsByFaculty() =>
        _students.ToLookup(s => s.Faculty);

    public string GetFacultyWithHighestAverageGrade() =>
        _students
            .Where(s => s.Grades.Any())
            .GroupBy(s => s.Faculty)
            .Select(g => new { Faculty = g.Key, Avg = g.SelectMany(s => s.Grades).Average() })
            .OrderByDescending(f => f.Avg)
            .Select(f => f.Faculty)
            .FirstOrDefault() ?? string.Empty;
}

}
