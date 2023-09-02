#region

using System.Collections.Generic;
using System.Threading.Tasks;
using MarkBot.Parsers.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace MarkBot.Repositories;

public class SubscriberRepository : RepositoryBase
{
    /// <inheritdoc />
    public SubscriberRepository(MarkDatabase database) : base(database)
    {
    }

    public async Task<IEnumerable<Subscriber>> Fetch()
    {
        return await Database.Subscribers.ToListAsync();
    }

    public async Task<Subscriber> FindOrCreate(long id)
    {
        var sub = await Database.Subscribers.FindAsync(id);
        if (sub != null)
        {
            return sub;
        }

        sub = new Subscriber
        {
            Id = id
        };
        Database.Add(sub);
        await Database.SaveChangesAsync();

        return sub;
    }

    public async Task<Subscriber?> FindByName(string? firstname, string? lastname)
    {
        if (firstname == null || lastname == null)
        {
            return null;
        }

        firstname = firstname.ToLower();
        lastname = lastname.ToLower();

        firstname = char.ToUpper(firstname[0]) + firstname[1..];
        lastname = char.ToUpper(lastname[0]) + lastname[1..];

        var sub = await Database.Subscribers.FirstOrDefaultAsync(x =>
                                                                     x.Name!.Contains(firstname) &&
                                                                     x.Name.Contains(lastname));

        return sub;
    }

    public async Task ChangeName(long id, string name)
    {
        var sub = await FindOrCreate(id);
        sub.Name = name;

        await Database.SaveChangesAsync();
    }

    public async Task Remove(long id)
    {
        var sub = await FindOrCreate(id);

        Database.Subscribers.Remove(sub);
        await Database.SaveChangesAsync();
    }

    public async Task SetBebraClassData(long id, string username, string password)
    {
        var sub = await FindOrCreate(id);

        sub.Username = username;
        sub.Password = password;

        await Database.SaveChangesAsync();
    }
}
