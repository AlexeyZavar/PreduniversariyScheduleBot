#region

using System.Text;
using MarkBot.SberClass;

#endregion

Console.OutputEncoding = Encoding.UTF8;

// var client = new BebraClassClient();
//
// await client.RefreshToken().ConfigureAwait(false);
//
// var res = await client.FetchPeriods().ConfigureAwait(false);
// foreach (var period in res) Console.WriteLine(period.Name);
//
// var res2 = await client.FetchMarks().ConfigureAwait(false);
// foreach (var mark in res2) Console.WriteLine(mark);

var client = new BebraClassClient();
var res = await client.Authorize("REDACTED", "REDACTED");

Console.WriteLine(res);

var marks = await client.FetchMarks();
foreach (var mark in marks)
{
    Console.WriteLine(mark);
}
