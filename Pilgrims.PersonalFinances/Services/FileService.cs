using Microsoft.AspNetCore.Components.Forms;
using Pilgrims.PersonalFinances.Services.Interfaces;

namespace Pilgrims.PersonalFinances.Services;

/// <summary>
/// Service for file operations including upload, download, and management
/// </summary>
public class FileService : IFileService
{
    private readonly string _baseStoragePath;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    private readonly HashSet<string> _allowedFileTypes = new()
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp",
        "application/pdf",
        "text/plain",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    public FileService()
    {
        _baseStoragePath = Path.Combine(FileSystem.AppDataDirectory, "Documents");
        Directory.CreateDirectory(_baseStoragePath);
    }

    public long MaxFileSize => MaxFileSizeBytes;

    public IEnumerable<string> AllowedFileTypes => _allowedFileTypes;

    public async Task<string> UploadFileAsync(IBrowserFile file, string folder)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        if (!IsFileTypeAllowed(file.ContentType))
            throw new InvalidOperationException($"File type '{file.ContentType}' is not allowed");

        if (!IsFileSizeValid(file.Size))
            throw new InvalidOperationException($"File size exceeds the maximum limit of {MaxFileSize / (1024 * 1024)}MB");

        var folderPath = Path.Combine(_baseStoragePath, folder);
        await CreateDirectoryAsync(folderPath);

        var uniqueFileName = GenerateUniqueFileName(file.Name);
        var filePath = Path.Combine(folderPath, uniqueFileName);

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var browserFileStream = file.OpenReadStream(MaxFileSize);
            await browserFileStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            // Clean up the file if upload failed
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            throw new InvalidOperationException($"Failed to upload file: {ex.Message}", ex);
        }

        // Return relative path from base storage
        return Path.GetRelativePath(_baseStoragePath, filePath);
    }

    public async Task<List<string>> UploadFilesAsync(IEnumerable<IBrowserFile> files, string folder)
    {
        var uploadedFiles = new List<string>();
        var exceptions = new List<Exception>();

        foreach (var file in files)
        {
            try
            {
                var filePath = await UploadFileAsync(file, folder);
                uploadedFiles.Add(filePath);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Any())
        {
            // Clean up successfully uploaded files if any failed
            foreach (var uploadedFile in uploadedFiles)
            {
                try
                {
                    await DeleteFileAsync(uploadedFile);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }

            throw new AggregateException("One or more files failed to upload", exceptions);
        }

        return uploadedFiles;
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        try
        {
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_baseStoragePath, filePath);
            
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
        }

        return false;
    }

    public async Task<bool> DeleteFilesAsync(IEnumerable<string> filePaths)
    {
        var allDeleted = true;

        foreach (var filePath in filePaths)
        {
            var deleted = await DeleteFileAsync(filePath);
            if (!deleted)
                allDeleted = false;
        }

        return allDeleted;
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        return await Task.Run(() =>
        {
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_baseStoragePath, filePath);
            return File.Exists(fullPath);
        });
    }

    public async Task<long> GetFileSizeAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return 0;

        return await Task.Run(() =>
        {
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_baseStoragePath, filePath);
            
            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                return fileInfo.Length;
            }
            
            return 0;
        });
    }

    public async Task<byte[]> GetFileContentAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_baseStoragePath, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task<Stream> GetFileStreamAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_baseStoragePath, filePath);
        
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await Task.FromResult(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }

    public bool IsFileTypeAllowed(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        return _allowedFileTypes.Contains(contentType.ToLowerInvariant());
    }

    public bool IsFileSizeValid(long fileSize)
    {
        return fileSize > 0 && fileSize <= MaxFileSize;
    }

    public string GenerateUniqueFileName(string originalFileName)
    {
        if (string.IsNullOrEmpty(originalFileName))
            return Guid.NewGuid().ToString();

        var extension = GetFileExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueId = Guid.NewGuid().ToString("N")[..8]; // First 8 characters

        return $"{nameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
    }

    public string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;

        return Path.GetExtension(fileName);
    }

    public async Task CreateDirectoryAsync(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
            return;

        await Task.Run(() =>
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        });
    }

    public string GetFullFilePath(string folder, string fileName)
    {
        if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
            throw new ArgumentException("Folder and file name cannot be null or empty");

        return Path.Combine(_baseStoragePath, folder, fileName);
    }

    /// <summary>
    /// Gets the display name for a file type
    /// </summary>
    public string GetFileTypeDisplayName(string contentType)
    {
        return contentType?.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => "JPEG Image",
            "image/png" => "PNG Image",
            "image/gif" => "GIF Image",
            "image/bmp" => "Bitmap Image",
            "image/webp" => "WebP Image",
            "application/pdf" => "PDF Document",
            "text/plain" => "Text File",
            "application/msword" => "Word Document",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "Word Document",
            "application/vnd.ms-excel" => "Excel Spreadsheet",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "Excel Spreadsheet",
            _ => "Unknown File Type"
        };
    }

    /// <summary>
    /// Gets the icon class for a file type (for UI display)
    /// </summary>
    public string GetFileTypeIcon(string contentType)
    {
        return contentType?.ToLowerInvariant() switch
        {
            var type when type.StartsWith("image/") => "🖼️",
            "application/pdf" => "📄",
            "text/plain" => "📝",
            var type when type.Contains("word") => "📝",
            var type when type.Contains("excel") => "📊",
            _ => "📎"
        };
    }

    /// <summary>
    /// Formats file size for display
    /// </summary>
    public string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 B";

        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}