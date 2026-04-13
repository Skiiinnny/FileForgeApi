using System.IO.Compression;
using System.Xml;
using FileForgeApi.Shared.Documents;
using FileForgeApi.Shared.Results;

namespace FileForgeApi.Features.ExcelMetadata;

public sealed class ExcelMetadataService(ILogger<ExcelMetadataService> logger, IDocumentFetchService documentFetchService) : IExcelMetadataService
{
    public async Task<Result<ExcelMetadataResponse>> GetMetadataAsync(ExcelMetadataRequest? request)
    {
        var validation = ExcelMetadataValidator.Validate(request);
        if (!validation.IsSuccess)
            return Result<ExcelMetadataResponse>.Failure(validation.Error!);

        var (fileBytes, useUrl) = validation.Value!;

        if (useUrl)
        {
            var fetchResult = await documentFetchService.FetchAsync(request!.DocumentUrl!);
            if (!fetchResult.IsSuccess)
                return Result<ExcelMetadataResponse>.Failure(fetchResult.Error!);
            fileBytes = fetchResult.Value;
        }

        try
        {
            using var zip = new ZipArchive(new MemoryStream(fileBytes!), ZipArchiveMode.Read);

            var coreEntry = zip.GetEntry("docProps/core.xml");
            if (coreEntry is null)
                return Result<ExcelMetadataResponse>.Success(new ExcelMetadataResponse());

            using var stream = coreEntry.Open();
            var response = ParseCoreProperties(stream);
            return Result<ExcelMetadataResponse>.Success(response);
        }
        catch (Exception ex)
        {
            ExcelMetadataServiceLogging.MetadataExtractionFailed(logger, ex);
            return Result<ExcelMetadataResponse>.Failure($"No se pudo extraer metadatos: {ex.Message}");
        }
    }

    private static ExcelMetadataResponse ParseCoreProperties(Stream stream)
    {
        string? creator = null;
        string? created = null;
        string? modified = null;
        string? lastModifiedBy = null;
        string? title = null;
        string? subject = null;
        string? keywords = null;
        string? description = null;

        var settings = new XmlReaderSettings { Async = true };
        using var reader = XmlReader.Create(stream, settings);

        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
                continue;

            var localName = reader.LocalName;
            var value = reader.ReadElementContentAsString();

            switch (localName)
            {
                case "creator":
                    creator = value;
                    break;
                case "created":
                    created = value;
                    break;
                case "modified":
                    modified = value;
                    break;
                case "lastModifiedBy":
                    lastModifiedBy = value;
                    break;
                case "title":
                    title = value;
                    break;
                case "subject":
                    subject = value;
                    break;
                case "keywords":
                    keywords = value;
                    break;
                case "description":
                    description = value;
                    break;
            }
        }

        return new ExcelMetadataResponse(
            Creator: creator,
            Created: created,
            Modified: modified,
            LastModifiedBy: lastModifiedBy,
            Title: title,
            Subject: subject,
            Keywords: keywords,
            Description: description);
    }
}
