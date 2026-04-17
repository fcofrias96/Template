using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
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
    public partial class ExportService<TEntity> : IExportService<TEntity>
        where TEntity : class, IEntityBase
    {
        private readonly IReadRepository<TEntity> _repositoryAsync;

        public ExportService(IReadRepository<TEntity> repository)
        {
            _repositoryAsync = repository;
        }

        public async Task<byte[]> ExportToCSVAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)
        {
            var data = await _repositoryAsync.GetProjectedForExportAsync<TDestination>(specification, cancellationToken);

            return GenerateCSV(title, data);
        }

        public async Task<byte[]> ExportToCSVAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _repositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
            return GenerateCSV(title, data);
        }

        public async Task<byte[]> ExportToPdfAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)

        {
            var data = await _repositoryAsync.GetProjectedForExportAsync<TDestination>(specification, cancellationToken);
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToPdfAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _repositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
            return GeneratePdf(title, data);
        }

        public async Task<byte[]> ExportToDocxAsync<TDestination>(string title, ISpecification<TEntity> specification, CancellationToken cancellationToken)

        {
            var data = await _repositoryAsync.GetProjectedForExportAsync<TDestination>(specification, cancellationToken);
            return GenerateDoc(title, data);
        }

        public async Task<byte[]> ExportToDocxAsync<TDestination>(string title, CancellationToken cancellationToken)
        {
            var data = await _repositoryAsync.GetProjectedAsync<TDestination>(cancellationToken);
            return GenerateDoc(title, data);
        }

        #region Private Methods

        private static byte[] GenerateCSV<TDestination>(string title, IReadOnlyList<TDestination> data)
        {
            var properties = ExportCache<TDestination>.Properties;
            var headers = ExportCache<TDestination>.Headers;

            var sb = new StringBuilder();

            sb.Append('"').Append(title).Append('"').AppendLine();

            // Encabezados
            for (int i = 0; i < headers.Length; i++)
            {
                sb.Append(headers[i]);
                if (i < headers.Length - 1)
                    sb.Append(',');
            }

            sb.AppendLine();

            // Datos
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];

                for (int j = 0; j < properties.Length; j++)
                {
                    var value = properties[j].GetValue(item);
                    var friendlyValue = ConvertToFriendlyValue(value);
                    sb.Append(friendlyValue);

                    if (j < properties.Length - 1)
                        sb.Append(',');
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

                    var titleParagraph = new Paragraph(title)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .SimulateBold();
                    document.Add(titleParagraph);

                    document.Add(new Paragraph("\n"));

                    var properties = ExportCache<TDestination>.Properties;
                    var headers = ExportCache<TDestination>.Headers;

                    var table = new Table(UnitValue.CreatePercentArray(properties.Length)).UseAllAvailableWidth();

                    // Header
                    foreach (var header in headers)
                    {
                        table.AddHeaderCell(new Cell().Add(new Paragraph(header)));
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

                    // Total
                    table.AddCell(
                        new Cell(1, properties.Length)
                            .Add(new Paragraph($"Total: {data.Count}"))
                            .SetTextAlignment(TextAlignment.RIGHT));

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
                    document.InsertParagraph(title)
                            .FontSize(18)
                            .Bold()
                            .Alignment = Xceed.Document.NET.Alignment.center;

                    document.InsertParagraph("\n");

                    var properties = ExportCache<TDestination>.Properties;
                    var headers = ExportCache<TDestination>.Headers;

                    var table = document.AddTable(data.Count + 1, properties.Length);

                    // Header
                    for (int i = 0; i < headers.Length; i++)
                    {
                        table.Rows[0].Cells[i].Paragraphs[0].Append(headers[i]).Bold();
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

                    // Total
                    var totalRow = table.InsertRow(data.Count + 1);
                    totalRow.Cells[0].Paragraphs[0].Append($"Total: {data.Count}");
                    totalRow.MergeCells(0, properties.Length - 1);
                    totalRow.Cells[0].Paragraphs[0].Alignment = Xceed.Document.NET.Alignment.right;

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

            return value switch
            {
                DateTimeOffset time => time.ToString("yyyy-MM-dd"),
                bool v => v ? "Si" : "No",
                _ => value.ToString() ?? string.Empty
            };
        }

        #endregion Private Methods
    }
}