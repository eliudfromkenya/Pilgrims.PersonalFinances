using System.ComponentModel.DataAnnotations;

namespace Pilgrims.PersonalFinances.Core.Models;

/// <summary>
/// Represents an attachment (receipt, document) associated with a transaction
/// </summary>
public class TransactionAttachment : BaseEntity
{
    private string _transactionId = string.Empty;
    private Transaction? _transaction;
    private string _fileName = string.Empty;
    private string _originalFileName = string.Empty;
    private string _contentType = string.Empty;
    private long _fileSize;
    private string? _filePath;
    private string? _description;
    private DateTime _uploadedDate = DateTime.UtcNow;

    /// <summary>
    /// ID of the associated transaction
    /// </summary>
    [Required]
    public string TransactionId
    {
        get => _transactionId;
        set => SetProperty(ref _transactionId, value);
    }

    /// <summary>
    /// Navigation property to the associated transaction
    /// </summary>
    public Transaction? Transaction
    {
        get => _transaction;
        set => SetProperty(ref _transaction, value);
    }

    /// <summary>
    /// Stored file name (may be different from original)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FileName
    {
        get => _fileName;
        set => SetProperty(ref _fileName, value);
    }

    /// <summary>
    /// Original file name as uploaded by user
    /// </summary>
    [Required]
    [StringLength(255)]
    public string OriginalFileName
    {
        get => _originalFileName;
        set => SetProperty(ref _originalFileName, value);
    }

    /// <summary>
    /// MIME content type of the file
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ContentType
    {
        get => _contentType;
        set => SetProperty(ref _contentType, value);
    }

    /// <summary>
    /// File size in bytes
    /// </summary>
    [Required]
    public long FileSize
    {
        get => _fileSize;
        set => SetProperty(ref _fileSize, value);
    }

    /// <summary>
    /// Path to the stored file (relative or absolute)
    /// </summary>
    [StringLength(500)]
    public string? FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    /// <summary>
    /// Optional description of the attachment
    /// </summary>
    [StringLength(200)]
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Date and time when the file was uploaded
    /// </summary>
    [Required]
    public DateTime UploadedDate
    {
        get => _uploadedDate;
        set => SetProperty(ref _uploadedDate, value);
    }

    /// <summary>
    /// Gets whether this attachment is an image
    /// </summary>
    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this attachment is a PDF
    /// </summary>
    public bool IsPdf => ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the file extension from the original filename
    /// </summary>
    public string FileExtension
    {
        get
        {
            var extension = Path.GetExtension(OriginalFileName);
            return string.IsNullOrEmpty(extension) ? string.Empty : extension.ToLowerInvariant();
        }
    }

    /// <summary>
    /// Gets a human-readable file size
    /// </summary>
    public string FormattedFileSize
    {
        get
        {
            if (FileSize < 1024)
                return $"{FileSize} B";
            
            if (FileSize < 1024 * 1024)
                return $"{FileSize / 1024.0:F1} KB";
            
            if (FileSize < 1024 * 1024 * 1024)
                return $"{FileSize / (1024.0 * 1024.0):F1} MB";
            
            return $"{FileSize / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }
    }

    /// <summary>
    /// Validates the attachment data
    /// </summary>
    /// <returns>List of validation errors</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(TransactionId))
            errors.Add("Transaction ID is required");

        if (string.IsNullOrWhiteSpace(FileName))
            errors.Add("File name is required");

        if (string.IsNullOrWhiteSpace(OriginalFileName))
            errors.Add("Original file name is required");

        if (string.IsNullOrWhiteSpace(ContentType))
            errors.Add("Content type is required");

        if (FileSize <= 0)
            errors.Add("File size must be greater than 0");

        if (FileSize > 10 * 1024 * 1024) // 10 MB limit
            errors.Add("File size cannot exceed 10 MB");

        if (FileName.Length > 255)
            errors.Add("File name cannot exceed 255 characters");

        if (OriginalFileName.Length > 255)
            errors.Add("Original file name cannot exceed 255 characters");

        if (ContentType.Length > 100)
            errors.Add("Content type cannot exceed 100 characters");

        if (Description?.Length > 200)
            errors.Add("Description cannot exceed 200 characters");

        if (FilePath?.Length > 500)
            errors.Add("File path cannot exceed 500 characters");

        // Validate allowed file types
        var allowedTypes = new[]
        {
            "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp",
            "application/pdf",
            "text/plain",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        if (!allowedTypes.Contains(ContentType.ToLowerInvariant()))
            errors.Add("File type is not supported");

        return errors;
    }
}
