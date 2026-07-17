using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject>? Grades { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName)) throw new ArgumentException("Имя студента не может быть пустым.");
        if (string.IsNullOrWhiteSpace(LastName)) throw new ArgumentException("Фамилия студента не может быть пустой.");
        if (BirthDate > DateTime.Now) throw new ArgumentException("Дата рождения не может быть в будущем.");

        if (Grades != null && Grades.Any(g => g.Grade < 1 || g.Grade > 5))
        {
            throw new ArgumentException("Оценка по предмету должна быть в диапазоне от 1 до 5.");
        }
    }
}

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.ParseExact(reader.GetString()!, Format, null);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(Format));
}

public static class StudentJsonManager
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void SaveToFile(string filePath, Student student)
    {
        student.Validate();
        string jsonString = JsonSerializer.Serialize(student, Options);
        File.WriteAllText(filePath, jsonString);
    }

    public static Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Файл данных не найден.");
        string jsonString = File.ReadAllText(filePath);

        Student student = JsonSerializer.Deserialize<Student>(jsonString, Options)
                          ?? throw new JsonException("Не удалось десериализовать объект.");

        student.Validate();
        return student;
    }
}
