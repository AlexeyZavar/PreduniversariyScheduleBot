#region

using System.Collections.Generic;
using System.Linq;
using MarkBot.Parsers.Entities;
using Serilog;

#endregion

namespace MarkBot.Parsers;

public static class MainParser
{
    public static List<StudentDifference> FindDifferences(HashSet<Student> updated, HashSet<Student> outdated)
    {
        var diffs = new List<StudentDifference>();

        foreach (var updatedStudent in updated)
        {
            var outdatedFound = outdated.TryGetValue(updatedStudent, out var outdatedStudent);
            if (!outdatedFound)
            {
                Log.Warning("Possibly new student? {Student}", updatedStudent);
                continue;
            }

            var difference = new StudentDifference
            {
                Student = updatedStudent
            };

            foreach (var updatedMark in updatedStudent.Marks)
            {
                var outdatedMark = outdatedStudent!.Marks.FirstOrDefault(x => x.Id == updatedMark.Id);
                if (outdatedMark == null)
                {
                    difference.MarksAdd.Add(updatedMark);
                    continue;
                }

                if (updatedMark.Value != outdatedMark.Value || updatedMark.Description != outdatedMark.Description)
                {
                    difference.MarksChange.Add(new MarkDifference
                    {
                        Outdated = outdatedMark,
                        Updated = updatedMark
                    });
                }
            }

            foreach (var outdatedMark in outdatedStudent!.Marks)
            {
                var updatedMark = updatedStudent.Marks.FirstOrDefault(x => x.Id == outdatedMark.Id);
                if (updatedMark == null)
                {
                    difference.MarksRemove.Add(outdatedMark);
                }
            }

            if (difference.MarksAdd.Count != 0 || difference.MarksRemove.Count != 0 ||
                difference.MarksChange.Count != 0)
            {
                diffs.Add(difference);
            }
        }

        return diffs;
    }
}
