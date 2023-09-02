namespace MarkBot.Repositories;

public abstract class RepositoryBase
{
    public RepositoryBase(MarkDatabase database)
    {
        Database = database;
    }

    protected MarkDatabase Database { get; }
}
