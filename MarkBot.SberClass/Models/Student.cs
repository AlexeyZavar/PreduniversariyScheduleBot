#region

using System.Text.Json.Serialization;

#endregion

namespace MarkBot.SberClass.Models;

public class StudentResponse
{
    [JsonPropertyName("student")] public Student Student { get; set; }
}

public class Student
{
    [JsonPropertyName("getCurrentAcademicYearStudyPeriods")]
    public GetCurrentAcademicYearStudyPeriod[] GetCurrentAcademicYearStudyPeriods { get; set; }

    [JsonPropertyName("getStudentDiary")] public GetStudentDiary GetStudentDiary { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class GetStudentDiary
{
    [JsonPropertyName("lessons")] public Lesson[] Lessons { get; set; }

    [JsonPropertyName("events")] public object[] Events { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class Lesson
{
    [JsonPropertyName("lessonId")] public string LessonId { get; set; }

    [JsonPropertyName("number")] public string Number { get; set; }

    [JsonPropertyName("classRoomName")] public string ClassRoomName { get; set; }

    [JsonPropertyName("start")] public DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")] public DateTimeOffset End { get; set; }

    [JsonPropertyName("subject")] public Subject Subject { get; set; }

    [JsonPropertyName("marks")] public Mark[] Marks { get; set; }

    [JsonPropertyName("homeworks")] public object[] Homeworks { get; set; }

    [JsonPropertyName("lessonPlan")] public LessonPlan LessonPlan { get; set; }

    [JsonPropertyName("hasControlWork")] public bool HasControlWork { get; set; }

    [JsonPropertyName("visitJournalValue")]
    public string VisitJournalValue { get; set; }

    [JsonPropertyName("onlineLesson")] public bool OnlineLesson { get; set; }

    [JsonPropertyName("module")] public Module Module { get; set; }

    [JsonPropertyName("comments")] public object[] Comments { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class LessonPlan
{
    [JsonPropertyName("note")] public string Note { get; set; }

    [JsonPropertyName("numberOfTasks")] public long NumberOfTasks { get; set; }

    [JsonPropertyName("amountOfLaborCost")]
    public long AmountOfLaborCost { get; set; }

    [JsonPropertyName("completed")] public bool Completed { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class Mark
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("governmentMark")] public long GovernmentMark { get; set; }

    [JsonPropertyName("comment")] public string Comment { get; set; }

    [JsonPropertyName("learningActivity")] public LearningActivity LearningActivity { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class LearningActivity
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("weight")] public decimal Weight { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class Module
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class Subject
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("fullName")] public string FullName { get; set; }

    [JsonPropertyName("colour")] public string Colour { get; set; }

    [JsonPropertyName("avatar")] public string Avatar { get; set; }

    [JsonPropertyName("orgUnitId")] public Guid OrgUnitId { get; set; }

    [JsonPropertyName("tenantShortName")] public string TenantShortName { get; set; }

    [JsonPropertyName("subjectType")] public string SubjectType { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}

public class GetCurrentAcademicYearStudyPeriod
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("start")] public DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")] public DateTimeOffset End { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("isCurrent")] public bool IsCurrent { get; set; }

    [JsonPropertyName("isWorkDays")] public bool IsWorkDays { get; set; }

    [JsonPropertyName("createTime")] public DateTimeOffset CreateTime { get; set; }

    [JsonPropertyName("modifyTime")] public DateTimeOffset ModifyTime { get; set; }

    [JsonPropertyName("isActive")] public bool IsActive { get; set; }

    [JsonPropertyName("__typename")] public string Typename { get; set; }
}
