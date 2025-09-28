using Microsoft.AspNetCore.Components.Forms;

namespace Pilgrims.PersonalFinances.Services.Interfaces;

/// <summary>
/// Service interface for file operations including upload, download, and management
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Uploads a file and returns the file path
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="folder">The folder to store the file in</param>
    /// <returns>The file path where the file was stored</returns>
    Task<string> UploadFileAsync(IBrowserFile file, string folder);

    /// <summary>
    /// Uploads multiple files and returns their file paths
    /// </summary>
    /// <param name="files">The files to upload</param>
    /// <param name="folder">The folder to store the files in</param>
    /// <returns>A list of file paths where the files were stored</returns>
    Task<List<string>> UploadFilesAsync(IEnumerable<IBrowserFile> files, string folder);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="filePath">The path of the file to delete</param>
    /// <returns>True if the file was deleted successfully</returns>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// Deletes multiple files from storage
    /// </summary>
    /// <param name="filePaths">The paths of the files to delete</param>
    /// <returns>True if all files were deleted successfully</returns>
    Task<bool> DeleteFilesAsync(IEnumerable<string> filePaths);

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="filePath">The path of the file to check</param>
    /// <returns>True if the file exists</returns>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    /// Gets the file size in bytes
    /// </summary>
    /// <param name="filePath">The path of the file</param>
    /// <returns>The file size in bytes</returns>
    Task<long> GetFileSizeAsync(string filePath);

    /// <summary>
    /// Gets the file content as byte array
    /// </summary>
    /// <param name="filePath">The path of the file</param>
    /// <returns>The file content as byte array</returns>
    Task<byte[]> GetFileContentAsync(string filePath);

    /// <summary>
    /// Gets the file stream for reading
    /// </summary>
    /// <param name="filePath">The path of the file</param>
    /// <returns>The file stream</returns>
    Task<Stream> GetFileStreamAsync(string filePath);

    /// <summary>
    /// Validates if the file type is allowed
    /// </summary>
    /// <param name="contentType">The content type of the file</param>
    /// <returns>True if the file type is allowed</returns>
    bool IsFileTypeAllowed(string contentType);

    /// <summary>
    /// Validates if the file size is within limits
    /// </summary>
    /// <param name="fileSize">The size of the file in bytes</param>
    /// <returns>True if the file size is within limits</returns>
    bool IsFileSizeValid(long fileSize);

    /// <summary>
    /// Gets the maximum allowed file size in bytes
    /// </summary>
    long MaxFileSize { get; }

    /// <summary>
    /// Gets the allowed file types
    /// </summary>
    IEnumerable<string> AllowedFileTypes { get; }

    /// <summary>
    /// Generates a unique file name to prevent conflicts
    /// </summary>
    /// <param name="originalFileName">The original file name</param>
    /// <returns>A unique file name</returns>
    string GenerateUniqueFileName(string originalFileName);

    /// <summary>
    /// Gets the file extension from a file name
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <returns>The file extension</returns>
    string GetFileExtension(string fileName);

    /// <summary>
    /// Creates a directory if it doesn't exist
    /// </summary>
    /// <param name="directoryPath">The directory path to create</param>
    Task CreateDirectoryAsync(string directoryPath);

    /// <summary>
    /// Gets the full file path for storage
    /// </summary>
    /// <param name="folder">The folder name</param>
    /// <param name="fileName">The file name</param>
    /// <returns>The full file path</returns>
    string GetFullFilePath(string folder, string fileName);
}