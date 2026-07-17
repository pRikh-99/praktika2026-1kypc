using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;
using task13;

namespace task13tests;

public class JsonSerializationTests
{
    [Fact]
    public void Student_Serialization_AppliesCorrectDateFormatAndIgnoresNull()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateTime(2005, 5, 15),
            Grades = null
        };

        string json = JsonSerializer.Serialize(student);

        Assert.Contains("\"BirthDate\":\"2005-05-15\"", json);
        Assert.DoesNotContain("\"Grades\"", json);
    }

    [Fact]
    public void Student_Validation_ThrowsException_OnInvalidGrades()
    {
        var student = new Student
        {
            FirstName = "Петр",
            LastName = "Петров",
            BirthDate = new DateTime(2004, 10, 10),
            Grades = new List<Subject> { new() { Name = "Математика", Grade = 10 } }
        };

        Assert.Throws<ArgumentException>(() => student.Validate());
    }

    [Fact]
    public void StudentJsonManager_SaveAndLoad_WorksCorrectlyWithFiles()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        var originalStudent = new Student
        {
            FirstName = "Алексей",
            LastName = "Смирнов",
            BirthDate = new DateTime(2006, 1, 1),
            Grades = new List<Subject> { new() { Name = "Физика", Grade = 5 } }
        };

        try
        {
            StudentJsonManager.SaveToFile(tempFile, originalStudent);
            Student loadedStudent = StudentJsonManager.LoadFromFile(tempFile);

            Assert.Equal(originalStudent.FirstName, loadedStudent.FirstName);
            Assert.Equal(originalStudent.LastName, loadedStudent.LastName);
            Assert.Equal(originalStudent.BirthDate, loadedStudent.BirthDate);
            Assert.NotNull(loadedStudent.Grades);
            Assert.Single(loadedStudent.Grades);
            Assert.Equal(5, loadedStudent.Grades[0].Grade);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
