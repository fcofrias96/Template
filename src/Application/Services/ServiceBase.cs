using AutoMapper;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Template.Application.Attributes;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.V1;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Domain.Contracts;
using Xceed.Words.NET;



namespace Template.Application.Services
{
    public class ServiceBase<TEntity> : IAsyncService<TEntity> where TEntity : IEntityBase
    {
        #region Private Variable

        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<TEntity> _repository;

        #endregion Private Variable

        public ServiceBase(IAsyncRepository<TEntity> repository, IMapper mapper, IHttpContextAccessor httpContext, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContext = httpContext;
            _cache = cache;
        }

        #region Public Methods

        public virtual async Task<TDestination> CreateAsync<TSource, TDestination>(TSource source)
        {
            var entity = _mapper.Map<TEntity>(source);
            await _repository.CreateAsync(entity);
            _cache.InvalidateCache<TEntity>();
            return _mapper.Map<TDestination>(entity);
        }

        public async Task DeleteLogicalAsync<TKey>(TKey id)
        {
            var entity = await GetByIdAsync(id);
            entity.Active = false;
            await _repository.UpdateAsync(entity);
            _cache.InvalidateCache<TEntity>();
        }

        public virtual async Task DeletePermanentAsync<TKey>(TKey id)
        {
            var entity = await GetByIdAsync(id);
            await _repository.DeleteAsync(entity);
            _cache.InvalidateCache<TEntity>();
        }
        public async Task<byte[]> ExportToExcelAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationCreatorInstance<TSpecification>(queryParameters, false);
            var data = await _repository.GetAllProjectedToExportAsync<TDestination>(spec);

            return GenerateExcel(title, data);

        }


        public async Task<byte[]> ExportToExcelAsync<TDestination>(string title)                      
        {
            var data = await _repository.GetAllProjectedAsync<TDestination>();
            return GenerateExcel(title, data);
        }




        public async Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationCreatorInstance<TSpecification>(queryParameters, false);
            var data = await _repository.GetAllProjectedToExportAsync<TDestination>(spec);
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToPdfAsync<TDestination>(string title)
        {
            var data = await _repository.GetAllProjectedAsync<TDestination>();
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToWordAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationCreatorInstance<TSpecification>(queryParameters, false);
            var data = await _repository.GetAllProjectedToExportAsync<TDestination>(spec);
            return GenerateWord(title, data);
        }

        public async Task<byte[]> ExportToWordAsync<TDestination>(string title)
        {
            var data = await _repository.GetAllProjectedAsync<TDestination>();
            return GenerateWord(title, data);
        }

        public async Task<ApiResponse<List<TDestination>>> GetAllProjectedAsync<TDestination>()
        {
            string cacheKey = $"{typeof(TEntity).FullName}Cache";

            if (!_cache.TryGetValue(cacheKey, out List<TDestination> mappedEntities))
            {
                mappedEntities = await _repository.GetAllProjectedAsync<TDestination>();
                _cache.Set(cacheKey, mappedEntities, TimeSpan.FromHours(24));
            }

            return new ApiResponse<List<TDestination>>(mappedEntities);
        }

        public virtual async Task<ApiResponse<PagedCollection<TDestination>>> GetAllProjectedWithPaginationAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters)
        where TSpecification : ISpecification<TEntity>
        {

            var spec = SpecificationCreatorInstance<TSpecification>(queryParameters);

            if (spec.Skip % spec.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({spec.Skip}) must be either zero or a multiple of the 'limit' value({spec.Take}).");
            }

            var total = await _repository.GetTotalCountAsync(spec.Criteria);

            if (total < spec.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({spec.Take}).");
            }

            var items = await _repository.GetAllProjectedWithPaginationAsync<TDestination>(spec);
            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl()!;
            var next = UrlUtile.GetNextURL(href, spec.Take, spec.Skip, total);
            var prev = UrlUtile.GetPrevURL(href, spec.Take, spec.Skip);

            var pagedCollection = new PagedCollection<TDestination>
            {
                HRef = href,
                Elements = items,
                Limit = spec.Take,
                Next = next,
                Offset = spec.Skip,
                Prev = prev,
                Total = total
            };

            return new ApiResponse<PagedCollection<TDestination>>(pagedCollection);
        }

        public virtual async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            var mappedEntity = await _repository.GetByIdProjectedAsync<TKey, TDestination>(id);
            if (mappedEntity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return new ApiResponse<TDestination>(mappedEntity);
        }

        public virtual async Task UpdateAsync<TKey, TSource>(TKey id, TSource source)
        {
            var propertyInfo = typeof(TSource).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new BadRequestException("The entity does not have an Id property");
            }

            var sourceId = propertyInfo.GetValue(source);

            if (!EqualityComparer<TKey>.Default.Equals(id, (TKey?)sourceId))
            {
                throw new BadRequestException("Invalid Id used in request");
            }

            var entity = await GetByIdAsync(id);
            _mapper.Map(source, entity);
            _cache.InvalidateCache<TEntity>();
            await _repository.UpdateAsync(entity);
        }


        #endregion Public Methods

        #region Private Methods

        private byte[] GenerateExcel<TDestination>(string title, IList<TDestination> data)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(title);
                var properties = typeof(TDestination).GetProperties();

                // Header
                for (int i = 0; i < properties.Length; i++)
                {

                    var propertyName = GetDisplayName(properties[i]);
                    worksheet.Cell(1, i + 1).Value = propertyName;
                }

                // Data
                for (int i = 0; i < data.Count; i++)
                {
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var value = properties[j].GetValue(data[i]);
                        worksheet.Cell(i + 2, j + 1).Value = ConvertToFriendlyValue(value);
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }

        }


        private byte[] GeneratePdf<TDestination>(string title, IList<TDestination> data)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                {
                    var pdf = new PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf);

                    // Add title
                    var titleParagraph = new Paragraph(title)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .SetBold();
                    document.Add(titleParagraph);

                    // Add some space between title and table
                    document.Add(new Paragraph("\n"));

                    var properties = typeof(TDestination).GetProperties();
                    var table = new Table(UnitValue.CreatePercentArray(properties.Length)).UseAllAvailableWidth();

                    // Header
                    foreach (var property in properties)
                    {
                        var propertyName = GetDisplayName(property);
                        table.AddHeaderCell(new Cell().Add(new Paragraph(propertyName)));
                    }

                    // Data
                    foreach (var item in data)
                    {
                        foreach (var property in properties)
                        {
                            var value = ConvertToFriendlyValue(property.GetValue(item));
                            table.AddCell(new Cell().Add(new Paragraph(value)));
                        }
                    }

                    // Add total count row
                    table.AddCell(new Cell(1, properties.Length).Add(new Paragraph($"Total: {data.Count}")).SetTextAlignment(TextAlignment.RIGHT));

                    document.Add(table);
                    document.Close();
                }

                return stream.ToArray();
            }
        }
        private byte[] GenerateWord<TDestination>(string title, IList<TDestination> data)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = DocX.Create(stream))
                {
                    // Add title as header
                    document.InsertParagraph(title).FontSize(18)
                                                   .Bold()
                                                   .Alignment = Xceed.Document.NET.Alignment.center;

                    document.InsertParagraph("\n");

                    var properties = typeof(TDestination).GetProperties();
                    var table = document.AddTable(data.Count + 1, properties.Length);

                    // Header
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var propertyName = GetDisplayName(properties[i]);
                        table.Rows[0].Cells[i].Paragraphs[0].Append(propertyName).Bold();
                    }

                    // Data
                    for (int i = 0; i < data.Count; i++)
                    {
                        var item = data[i];
                        for (int j = 0; j < properties.Length; j++)
                        {
                            var value = ConvertToFriendlyValue(properties[j].GetValue(item));
                            table.Rows[i + 1].Cells[j].Paragraphs[0].Append(value);
                        }
                    }

                    // Add total count row
                    var totalCell = table.InsertRow(data.Count + 1);
                    totalCell.Cells[0].Paragraphs[0].Append($"Total: {data.Count}");
                    totalCell.MergeCells(0, properties.Length - 1);
                    totalCell.Cells[0].Paragraphs[0].Alignment = Xceed.Document.NET.Alignment.right;

                    document.InsertTable(table);
                    document.Save();
                }

                return stream.ToArray();
            }
        }
        private string GetDisplayName(PropertyInfo property)
        {
            var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
            return displayName?.Name ?? property.Name;
        }

        private TSpecification SpecificationCreatorInstance<TSpecification>(IQueryParametersBase request, bool applyPagination = true) where TSpecification : ISpecification<TEntity>
        {
            var type = typeof(TSpecification);
            object[] constructorArguments = new object[] { request, applyPagination };
            var spec = (TSpecification)Activator.CreateInstance(type, constructorArguments)!;
            return spec;
        }

        private string ConvertToFriendlyValue(object? value)
        {
            if (value is null)
                return string.Empty;

            if (value is DateTime)
                return ((DateTime)value).ToString("yyyy-MM-dd");

            if (value is bool)
                return (bool)value ? "Si" : "No";

            return value.ToString() ?? string.Empty;
        }

        private async Task<TEntity> GetByIdAsync<TKey>(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            return entity;
        }
        #endregion Private Methods
    }
}