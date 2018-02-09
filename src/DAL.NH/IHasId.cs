namespace DAL.NH
{
    public interface IHasId
    {
        object Id { get; }
    }

    public interface IHasId<out TKey> : IHasId
    {
        new TKey Id { get; }
    }
}