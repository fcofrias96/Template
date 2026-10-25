namespace Template.Application.Contracts.Mapping
{
    public interface ICreateMapTo<out TEntity> 
    {
       TEntity ToEntity();
    }
}
