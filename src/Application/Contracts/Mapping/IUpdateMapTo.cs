namespace Template.Application.Contracts.Mapping
{
    public interface IUpdateMapTo<in TEntity>
    {
        void ApplyTo(TEntity entity);
    }
}