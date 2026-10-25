namespace Template.BuildingBlocks.Application.Contracts.Mapping
{
    public interface ICreateMapTo<out TEntity> 
    {
       TEntity ToEntity();
    }
}
