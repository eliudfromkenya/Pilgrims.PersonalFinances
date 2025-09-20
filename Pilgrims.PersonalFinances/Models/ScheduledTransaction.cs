using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pilgrims.PersonalFinances.Models.Enums;

namespace Pilgrims.PersonalFinances.Models
{
    /// <summary>
    /// Represents a scheduled transaction with recurrence patterns
    /// </summary>
    public class ScheduledTransaction : BaseEntity
    {
        /// <summary>
        /// Name/title for this scheduled transaction
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the scheduled transaction
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Type of transaction (Income, Expense, Transfer, Adjustment)
        /// </summary>
        [Required]
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Account for this transaction
        /// </summary>
        [Required]
        public int AccountId { get; set; }

        /// <summary>
        /// Category for this transaction
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Transfer destination account (for Transfer transactions)
        /// </summary>
        public int? TransferToAccountId { get; set; }

        /// <summary>
        /// Payee information
        /// </summary>
        [MaxLength(200)]
        public string? Payee { get; set; }

        /// <summary>
        /// Additional notes
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Tags (comma-separated)
        /// </summary>
        [MaxLength(1000)]
        public string? Tags { get; set; }

        /// <summary>
        /// Recurrence pattern type
        /// </summary>
        [Required]
        public RecurrenceType RecurrenceType { get; set; }

        /// <summary>
        /// Interval for recurrence (e.g., every 2 weeks, every 3 months)
        /// </summary>
        public int RecurrenceInterval { get; set; } = 1;

        /// <summary>
        /// Days of week for weekly recurrence (bit flags: Sunday=1, Monday=2, etc.)
        /// </summary>
        public int? DaysOfWeek { get; set; }

        /// <summary>
        /// Day of month for monthly recurrence (1-31, or negative for relative dates)
        /// </summary>
        public int? DayOfMonth { get; set; }

        /// <summary>
        /// Month of year for annual recurrence (1-12)
        /// </summary>
        public int? MonthOfYear { get; set; }

        /// <summary>
        /// First occurrence date
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Last date this scheduled transaction was processed
        /// </summary>
        public DateTime? LastProcessedDate { get; set; }

        /// <summary>
        /// Alias for EndType to match expected property name
        /// </summary>
        [NotMapped]
        public RecurrenceEndType RecurrenceEndType 
        { 
            get => EndType; 
            set => EndType = value; 
        }

        /// <summary>
        /// How the recurrence should end
        /// </summary>
        [Required]
        public RecurrenceEndType EndType { get; set; }

        /// <summary>
        /// End date (if EndType is EndDate)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Number of occurrences (if EndType is AfterOccurrences)
        /// </summary>
        public int? MaxOccurrences { get; set; }

        /// <summary>
        /// Current occurrence count
        /// </summary>
        public int CurrentOccurrences { get; set; } = 0;
        
        /// <summary>
        /// Alias for CurrentOccurrences to match expected property name
        /// </summary>
        [NotMapped]
        public int OccurrenceCount 
        { 
            get => CurrentOccurrences; 
            set => CurrentOccurrences = value; 
        }

        /// <summary>
        /// How transactions should be processed
        /// </summary>
        [Required]
        public SchedulingMode SchedulingMode { get; set; }

        /// <summary>
        /// When to send notifications
        /// </summary>
        public NotificationTiming NotificationTiming { get; set; }

        /// <summary>
        /// Whether this scheduled transaction is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date of last generated transaction
        /// </summary>
        public DateTime? LastGeneratedDate { get; set; }

        /// <summary>
        /// Date of next scheduled transaction
        /// </summary>
        public DateTime? NextDueDate { get; set; }

        /// <summary>
        /// List of skipped dates (JSON serialized)
        /// </summary>
        [MaxLength(2000)]
        public string? SkippedDates { get; set; }

        /// <summary>
        /// Whether to adjust for weekends/holidays
        /// </summary>
        public bool AdjustForWeekends { get; set; } = false;

        // Navigation Properties
        /// <summary>
        /// Navigation property to the account
        /// </summary>
        public virtual Account? Account { get; set; }

        /// <summary>
        /// Navigation property to the category
        /// </summary>
        public virtual Category? Category { get; set; }

        /// <summary>
        /// Navigation property to the transfer destination account
        /// </summary>
        public virtual Account? TransferToAccount { get; set; }

        /// <summary>
        /// Collection of generated transactions
        /// </summary>
        public virtual ICollection<Transaction> GeneratedTransactions { get; set; } = new List<Transaction>();

        // Computed Properties
        /// <summary>
        /// Gets whether this scheduled transaction is overdue
        /// </summary>
        [NotMapped]
        public bool IsOverdue => NextDueDate.HasValue && NextDueDate.Value < DateTime.Today && IsActive;

        /// <summary>
        /// Gets whether this scheduled transaction is due today
        /// </summary>
        [NotMapped]
        public bool IsDueToday => NextDueDate.HasValue && NextDueDate.Value.Date == DateTime.Today && IsActive;

        /// <summary>
        /// Gets whether this scheduled transaction is due soon (within 7 days)
        /// </summary>
        [NotMapped]
        public bool IsDueSoon => NextDueDate.HasValue && NextDueDate.Value.Date <= DateTime.Today.AddDays(7) && NextDueDate.Value.Date > DateTime.Today && IsActive;

        /// <summary>
        /// Gets the formatted amount as a string
        /// </summary>
        [NotMapped]
        public string FormattedAmount => Amount.ToString("C2");

        /// <summary>
        /// Gets the recurrence description
        /// </summary>
        [NotMapped]
        public string RecurrenceDescription => GetRecurrenceDescription();

        /// <summary>
        /// Gets the tags as a list
        /// </summary>
        [NotMapped]
        public List<string> TagList => string.IsNullOrEmpty(Tags) 
            ? new List<string>() 
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .Where(t => !string.IsNullOrEmpty(t))
                  .ToList();

        /// <summary>
        /// Gets the skipped dates as a list
        /// </summary>
        [NotMapped]
        public List<DateTime> SkippedDatesList => string.IsNullOrEmpty(SkippedDates) 
            ? new List<DateTime>() 
            : System.Text.Json.JsonSerializer.Deserialize<List<DateTime>>(SkippedDates) ?? new List<DateTime>();

        // Methods
        /// <summary>
        /// Validates the scheduled transaction
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");

            if (Amount <= 0)
                errors.Add("Amount must be greater than 0");

            if (AccountId <= 0)
                errors.Add("Account is required");

            if (TransactionType == TransactionType.Transfer && TransferToAccountId == null)
                errors.Add("Transfer destination account is required for transfer transactions");

            if (TransactionType == TransactionType.Transfer && TransferToAccountId == AccountId)
                errors.Add("Transfer destination account cannot be the same as source account");

            if (StartDate < DateTime.Today)
                errors.Add("Start date cannot be in the past");

            if (EndType == RecurrenceEndType.EndDate && EndDate.HasValue && EndDate.Value <= StartDate)
                errors.Add("End date must be after start date");

            if (EndType == RecurrenceEndType.AfterOccurrences && (!MaxOccurrences.HasValue || MaxOccurrences.Value <= 0))
                errors.Add("Number of occurrences must be greater than 0");

            if (RecurrenceType == RecurrenceType.Weekly && (!DaysOfWeek.HasValue || DaysOfWeek.Value == 0))
                errors.Add("Days of week must be specified for weekly recurrence");

            if (RecurrenceType == RecurrenceType.Monthly && (!DayOfMonth.HasValue || DayOfMonth.Value == 0))
                errors.Add("Day of month must be specified for monthly recurrence");

            return errors;
        }

        /// <summary>
        /// Calculates the next due date based on the recurrence pattern
        /// </summary>
        /// <returns>Next due date or null if no more occurrences</returns>
        public DateTime? CalculateNextDueDate()
        {
            if (!IsActive) return null;

            // Check if we've reached the maximum occurrences
            if (EndType == RecurrenceEndType.AfterOccurrences && 
                MaxOccurrences.HasValue && 
                CurrentOccurrences >= MaxOccurrences.Value)
                return null;

            var baseDate = LastGeneratedDate ?? StartDate;
            DateTime nextDate;

            switch (RecurrenceType)
            {
                case RecurrenceType.Daily:
                    nextDate = baseDate.AddDays(RecurrenceInterval);
                    break;

                case RecurrenceType.Weekly:
                    nextDate = CalculateNextWeeklyDate(baseDate);
                    break;

                case RecurrenceType.BiWeekly:
                    nextDate = baseDate.AddDays(14 * RecurrenceInterval);
                    break;

                case RecurrenceType.Monthly:
                    nextDate = CalculateNextMonthlyDate(baseDate);
                    break;

                case RecurrenceType.Quarterly:
                    nextDate = baseDate.AddMonths(3 * RecurrenceInterval);
                    break;

                case RecurrenceType.SemiAnnually:
                    nextDate = baseDate.AddMonths(6 * RecurrenceInterval);
                    break;

                case RecurrenceType.Annually:
                    nextDate = baseDate.AddYears(RecurrenceInterval);
                    break;

                default:
                    return null;
            }

            // Check if we've passed the end date
            if (EndType == RecurrenceEndType.EndDate && EndDate.HasValue && nextDate > EndDate.Value)
                return null;

            // Adjust for weekends if needed
            if (AdjustForWeekends)
                nextDate = AdjustForWeekend(nextDate);

            return nextDate;
        }

        /// <summary>
        /// Skips a specific occurrence
        /// </summary>
        /// <param name="date">Date to skip</param>
        public void SkipOccurrence(DateTime? date)
        {
            if(date == null) return;
            var skippedList = SkippedDatesList;
            if (!skippedList.Contains(date.Value.Date))
            {
                skippedList.Add(date.Value.Date);
                SkippedDates = System.Text.Json.JsonSerializer.Serialize(skippedList);
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Removes a skipped occurrence
        /// </summary>
        /// <param name="date">Date to unskip</param>
        public void UnskipOccurrence(DateTime date)
        {
            var skippedList = SkippedDatesList;
            if (skippedList.Remove(date.Date))
            {
                SkippedDates = skippedList.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(skippedList) : null;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Marks an occurrence as generated
        /// </summary>
        /// <param name="date">Date that was generated</param>
        public void MarkAsGenerated(DateTime date)
        {
            LastGeneratedDate = date;
            CurrentOccurrences++;
            NextDueDate = CalculateNextDueDate();
            MarkAsDirty();
        }

        /// <summary>
        /// Adds a tag to this scheduled transaction
        /// </summary>
        /// <param name="tag">Tag to add</param>
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;

            var currentTags = TagList;
            if (!currentTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                currentTags.Add(tag.Trim());
                Tags = string.Join(", ", currentTags);
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Removes a tag from this scheduled transaction
        /// </summary>
        /// <param name="tag">Tag to remove</param>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return;

            var currentTags = TagList;
            var removed = currentTags.RemoveAll(t => t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase)) > 0;
            
            if (removed)
            {
                Tags = currentTags.Count > 0 ? string.Join(", ", currentTags) : null;
                MarkAsDirty();
            }
        }

        /// <summary>
        /// Creates a transaction from this scheduled transaction
        /// </summary>
        /// <param name="date">Date for the transaction</param>
        /// <returns>New transaction</returns>
        public Transaction CreateTransaction(DateTime date)
        {
            return new Transaction
            {
                Amount = Amount,
                Date = date,
                Type = TransactionType,
                AccountId = AccountId,
                CategoryId = CategoryId,
                TransferToAccountId = TransferToAccountId,
                Payee = Payee,
                Description = $"{Name} (Scheduled)",
                Notes = Notes,
                Tags = Tags,
                Status = TransactionStatus.Pending
            };
        }

        private string GetRecurrenceDescription()
        {
            return RecurrenceType switch
            {
                RecurrenceType.Daily => RecurrenceInterval == 1 ? "Daily" : $"Every {RecurrenceInterval} days",
                RecurrenceType.Weekly => "Weekly",
                RecurrenceType.BiWeekly => "Bi-weekly",
                RecurrenceType.Monthly => "Monthly",
                RecurrenceType.Quarterly => "Quarterly",
                RecurrenceType.SemiAnnually => "Semi-annually",
                RecurrenceType.Annually => "Annually",
                RecurrenceType.Custom => "Custom",
                _ => "One-time"
            };
        }

        private DateTime CalculateNextWeeklyDate(DateTime baseDate)
        {
            if (!DaysOfWeek.HasValue) return baseDate.AddDays(7);

            var nextDate = baseDate.AddDays(1);
            while (nextDate <= baseDate.AddDays(7))
            {
                var dayFlag = 1 << (int)nextDate.DayOfWeek;
                if ((DaysOfWeek.Value & dayFlag) != 0)
                    return nextDate;
                nextDate = nextDate.AddDays(1);
            }

            return baseDate.AddDays(7 * RecurrenceInterval);
        }

        private DateTime CalculateNextMonthlyDate(DateTime baseDate)
        {
            if (!DayOfMonth.HasValue) return baseDate.AddMonths(RecurrenceInterval);

            var nextMonth = baseDate.AddMonths(RecurrenceInterval);
            var targetDay = DayOfMonth.Value;

            if (targetDay > 0)
            {
                // Specific day of month
                var daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                targetDay = Math.Min(targetDay, daysInMonth);
                return new DateTime(nextMonth.Year, nextMonth.Month, targetDay);
            }
            else
            {
                // Relative day (e.g., last day of month)
                var daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                return new DateTime(nextMonth.Year, nextMonth.Month, daysInMonth + targetDay + 1);
            }
        }

        private DateTime AdjustForWeekend(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
                return date.AddDays(-1); // Move to Friday
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return date.AddDays(1); // Move to Monday
            return date;
        }
    }
}