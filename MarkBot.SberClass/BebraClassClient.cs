#region

using System.Globalization;
using System.Net;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using MarkBot.SberClass.Models;
using Mark = MarkBot.Parsers.Entities.Mark;

#endregion

namespace MarkBot.SberClass;

public class BebraClassClient
{
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _httpClient;
    private readonly GraphQLHttpClient _qlClient;

    public BebraClassClient()
    {
        _handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        _httpClient = new HttpClient(_handler);

        _httpClient.DefaultRequestHeaders.Add("userrole", "STUDENT");
        _httpClient.DefaultRequestHeaders.Add("schoolid", "REDACTED");

        _qlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
        {
            EndPoint = new Uri("https://beta.sberclass.ru/services/graphql")
        }, new SystemTextJsonSerializer(), _httpClient);
    }

    public string Username { get; set; }
    public string Password { get; set; }

    public async ValueTask<bool> Authorize(string username, string password)
    {
        _handler.CookieContainer = new CookieContainer();

        var res = await _httpClient.GetAsync(
                                             "https://newschool.sberclass.ru/services/auth/check?returnTo=https://newschool.sberclass.ru/");
        var txt = await res.Content.ReadAsStringAsync();

        var url =
            "https://auth.sberclass.ru/auth/realms/EduPowerKeycloak/login-actions/authenticate?" +
            txt
                .Split("https://auth.sberclass.ru/auth/realms/EduPowerKeycloak/login-actions/authenticate?")[1]
                .Split("\"")[0].Replace("&amp;", "&");

        var form = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password }
        };
        var res2 = await _httpClient.PostAsync(url, new FormUrlEncodedContent(form));

        Username = username;
        Password = password;

        return res2.RequestMessage?.RequestUri?.ToString().Contains("newschool.sberclass.ru") ?? false;
    }

    public async ValueTask<bool> RefreshToken()
    {
        var res = await _httpClient.GetAsync(
                                             "https://beta.sberclass.ru/services/auth/check?returnTo=https://beta.sberclass.ru/diary");
        if (!res.IsSuccessStatusCode)
        {
            var res2 = await Authorize(Username, Password);
            if (!res2)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<GetCurrentAcademicYearStudyPeriod[]> FetchPeriods()
    {
        var req = new GraphQLRequest
        {
            Query = @"
                query getCurrentAcademicYearStudyPeriods {
                  student {
                    getCurrentAcademicYearStudyPeriods {
                      ...Mfe_StudyPeriod_1_
                      __typename
                    }
                    __typename
                  }
                }

                fragment Mfe_StudyPeriod_1_ on StudyPeriod {
                  id
                  start
                  end
                  name
                  isCurrent
                  isWorkDays
                  createTime
                  modifyTime
                  isActive
                  __typename
                }",
            OperationName = "getCurrentAcademicYearStudyPeriods"
        };

        var res = await _qlClient.SendQueryAsync<StudentResponse>(req);

        return res.Data.Student.GetCurrentAcademicYearStudyPeriods;
    }

    public async Task<Mark[]> FetchMarksByDate(DateTime from, DateTime to)
    {
        var req = new GraphQLRequest
        {
            Query = @"
query StudentDiaryQuery($fromDate: Date!, $toDate: Date!) {
  student {
    getStudentDiary(fromDate: $fromDate, toDate: $toDate) {
      lessons {
        lessonId
        number
        classRoomName
        start
        end
        subject {
          id
          name
          fullName
          colour
          avatar
          orgUnitId
          tenantShortName
          subjectType
          __typename
        }
        marks {
          id
          governmentMark
          comment
          learningActivity {
            name
            weight
            __typename
          }
          __typename
        }
        homeworks {
          homeworkId
          note
          numberOfTasks
          amountOfLaborCost
          completed
          __typename
        }
        lessonPlan {
          note
          numberOfTasks
          amountOfLaborCost
          completed
          __typename
        }
        hasControlWork
        visitJournalValue
        onlineLesson
        module {
          id
          name
          __typename
        }
        comments {
          id
          comment
          teacherAvatarUrl
          teacherFullName
          teacherId
          __typename
        }
        __typename
      }
      events {
        id
        name
        description
        startTime
        endTime
        allDayEvent
        location
        __typename
      }
      __typename
    }
    __typename
  }
}
",
            OperationName = "StudentDiaryQuery",
            Variables = new
            {
                fromDate = from.ToString("yyyy-MM-dd"),
                toDate = to.ToString("yyyy-MM-dd")
            }
        };

        var res = await _qlClient.SendQueryAsync<StudentResponse>(req);

        return res.Data.Student.GetStudentDiary.Lessons.SelectMany(x => x.Marks.Select(y => new Mark
        {
            Id = y.Id.ToString(),
            Value =
                $"{y.GovernmentMark}{((int)y.LearningActivity.Weight == 1 ? "" : "^" + (int)y.LearningActivity.Weight)}",
            Description = string.Join("; ",
                                      new[] { y.Comment, y.LearningActivity.Name }
                                          .Where(s => !string.IsNullOrWhiteSpace(s))),
            Subject = x.Subject.Name,
            Date = x.Start
        })).ToArray();
    }

    public async Task<Mark[]> FetchMarks()
    {
        return await FetchMarksByDate(DateTime.Parse("01.09.2022", CultureInfo.GetCultureInfo("ru-RU")),
                                      DateTime.Parse("01.09.2023", CultureInfo.GetCultureInfo("ru-RU")));
    }
}
