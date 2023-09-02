#region

using System.Collections.Generic;

#endregion

namespace MarkBot;

public class Config
{
    public string Key { get; set; }
    public string ConnectionString { get; set; }
    public int UpdateDelay { get; set; }
    public ICollection<long> Admins { get; set; } = new List<long>();
}
