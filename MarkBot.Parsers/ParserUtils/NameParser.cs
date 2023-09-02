#region

using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace MarkBot.Parsers.ParserUtils;

public static class NameParser
{
    private static readonly HashSet<string> _firstnames;
    private static readonly HashSet<string> _middlenames;
    private static readonly HashSet<string> _lastnames;

    static NameParser()
    {
        _firstnames = new HashSet<string>(File.ReadAllLines(Path.Combine("ParserUtils", "data", "firstnames.txt")));
        _middlenames =
            new HashSet<string>(File.ReadAllLines(Path.Combine("ParserUtils", "data", "middlenames.txt")));
        _lastnames = new HashSet<string>(File.ReadAllLines(Path.Combine("ParserUtils", "data", "lastnames.txt")));
    }

    public static (string? firstname, string? middlename, string? lastname) Parse(string[] valueSplit)
    {
        string? firstname = null;
        string? middlename = null;
        string? lastname = null;

        if (valueSplit.Length == 2)
        {
            foreach (var s in valueSplit.Select(x => x.ToUpper()))
            {
                if (lastname == null && _lastnames.Contains(s))
                {
                    lastname = s;
                }
                else if (firstname == null && _firstnames.Contains(s))
                {
                    firstname = s;
                }
            }
        }
        else if (valueSplit.Length == 3)
        {
            foreach (var s in valueSplit.Select(x => x.ToUpper()))
            {
                if (lastname == null && _lastnames.Contains(s))
                {
                    lastname = s;
                }
                else if (firstname == null && _firstnames.Contains(s))
                {
                    firstname = s;
                }
                else if (middlename == null && _middlenames.Contains(s))
                {
                    middlename = s;
                }
            }

            firstname ??= valueSplit[1];
            middlename ??= valueSplit[2];
            lastname ??= valueSplit[0];
        }
        else if (valueSplit.Length == 4)
        {
            firstname = valueSplit[1];
            lastname = valueSplit[0];
            middlename = valueSplit[2] + " " + valueSplit[3];
        }

        return (firstname?.ToUpper(), middlename?.ToUpper(), lastname?.ToUpper());
    }
}
