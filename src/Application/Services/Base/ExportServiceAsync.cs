using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Text;
using Template.Application.Attributes;
using Template.Application.Contracts;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Contracts.Services.Base.Export;
using Template.Application.Utilities;
using Template.Domain.Contracts;
using Xceed.Words.NET;

namespace Template.Application.Services.Base
{
    public class ExportServiceAsync<TEntity, TDbContext> : IExportServiceAsync<TEntity, TDbContext>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly IReadRepositoryAsync<TEntity, TDbContext> _readDepositoryAsync;
        private readonly ILogger _logger;

        public ExportServiceAsync(IReadRepositoryAsync<TEntity, TDbContext> repository, ILogger logger)
        {
            _readDepositoryAsync = repository;
            _logger = logger;
        }

        public async Task<byte[]> ExportToCSVAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationHelper.CreatorInstance<TSpecification, TEntity>(queryParameters, _logger, false);
            var data = await _readDepositoryAsync.GetProjectedForExportAsync<TDestination>(spec, cancellationToken);

            return GenerateCSV(title, data);
        }

        public async Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _readDepositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
            return GenerateCSV(title, data);
        }

        public async Task<byte[]> ExportToPdfAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationHelper.CreatorInstance<TSpecification, TEntity>(queryParameters, _logger, false);
            var data = await _readDepositoryAsync.GetProjectedForExportAsync<TDestination>(spec, cancellationToken);
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _readDepositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToDocxAsync<TDestination, TSpecification>(string title, IQueryParametersBase queryParameters, CancellationToken cancellationToken)
                                        where TSpecification : ISpecification<TEntity>
        {
            var spec = SpecificationHelper.CreatorInstance<TSpecification, TEntity>(queryParameters, _logger, false);
            var data = await _readDepositoryAsync.GetProjectedForExportAsync<TDestination>(spec, cancellationToken);
            return GenerateDoc(title, data);
        }

        public async Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _readDepositoryAsync.GetProjectedAsync<TDestination>(cancellationToken );
            return GenerateDoc(title, data);
        }

        #region Private Methods

        private static byte[] GenerateCSV<TDestination>(string title, IReadOnlyList<TDestination> data)
        {
            var properties = typeof(TDestination).GetProperties();
            var sb = new StringBuilder();

            sb.AppendLine($"\"{title}\"");

            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = GetDisplayName(properties[i]);
                sb.Append(propertyName);

                if (i < properties.Length - 1)
                    sb.Append(",");
            }

            sb.AppendLine();

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    var value = properties[j].GetValue(data[i]);
                    var friendlyValue = ConvertToFriendlyValue(value);
                    sb.Append(friendlyValue);

                    if (j < properties.Length - 1)
                        sb.Append(",");
                }

                sb.AppendLine();
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }



        private static byte[] GeneratePdf<TDestination>(string title, IReadOnlyList<TDestination> data)
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

        private static byte[] GenerateDoc<TDestination>(string title, IReadOnlyList<TDestination> data)
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

        private static string GetDisplayName(PropertyInfo property)
        {
            var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
            return displayName?.Name ?? property.Name;
        }

        private static string ConvertToFriendlyValue(object? value)
        {
            if (value is null)
                return string.Empty;

            if (value is DateTimeOffset time)
                return time.ToString("yyyy-MM-dd");

            if (value is bool v)
                return v ? "Si" : "No";

            return value.ToString() ?? string.Empty;
        }

        #endregion Private Methods
    }
}