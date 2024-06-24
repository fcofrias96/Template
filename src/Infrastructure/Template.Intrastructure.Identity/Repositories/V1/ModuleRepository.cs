using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Template.Application.Models.DTOs.V1.Module;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.DbContext.V1;
using Template.Identity.Entities;

namespace Template.Identity.Repositories.V1
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IdentityContext _dbContext;
        private readonly IConfigurationProvider _configurationProvider;

        public ModuleRepository(IdentityContext DbContext, IConfigurationProvider configurationProvider)
        {
            _dbContext = DbContext;
            _configurationProvider = configurationProvider;
        }

        public async Task CreateAsync(Module entity)
        {
            await _dbContext.Modules.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Module entity)
        {
            _dbContext.Modules.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ModuleListDTO>> GetAllProjectedAsync()
        {
            return await _dbContext.Modules
                                   .ProjectTo<ModuleListDTO>(_configurationProvider)
                                   .ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            return await _dbContext.Modules.FindAsync(id);
        }

        public async Task UpdateAsync(Module entity)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ModuleDetailDTO?> GetByIdProjectedAsync(int id)
        {
            var query = _dbContext.Modules.AsQueryable();
            return await query.Where(x => x.Id.Equals(id))
                              .ProjectTo<ModuleDetailDTO>(_configurationProvider)
                              .FirstOrDefaultAsync();
        }
    }
}