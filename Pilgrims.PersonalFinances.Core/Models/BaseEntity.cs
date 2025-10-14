using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using SQLite;

namespace Pilgrims.PersonalFinances.Core.Models
{
    /// <summary>
    /// Base entity class that provides common properties and INotifyPropertyChanged implementation
    /// for automatic dirty tracking and UpdatedAt updates
    /// </summary>
    public abstract class BaseEntity : INotifyPropertyChanged
    {
        private bool _isDirty;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt;
        private string? updatedBy;
        private string? createdBy;

        [Key]
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            private set => SetProperty(ref _updatedAt, value);
        }

        public string? CreatedBy { get => createdBy; private set => SetProperty(ref createdBy, value); }
        public string? UpdatedBy { get => updatedBy; private set => SetProperty(ref updatedBy, value); }

        /// <summary>
        /// Indicates whether the entity has been modified since creation or last save
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets a property value and raises PropertyChanged event if the value has changed
        /// Automatically marks the entity as dirty and updates the UpdatedAt timestamp
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The name of the property (automatically provided by compiler)</param>
        /// <returns>True if the property value was changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            // Don't mark as dirty for certain system properties
            if (propertyName != nameof(IsDirty) && 
                propertyName != nameof(UpdatedAt) && 
                propertyName != nameof(CreatedAt))
            {
                MarkAsDirty();
            }

            return true;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Marks the entity as dirty and updates the UpdatedAt timestamp
        /// </summary>
        public void MarkAsDirty()
        {
            if (!_isDirty)
            {
                _isDirty = true;
                _updatedAt = DateTime.UtcNow;
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(UpdatedAt));
            }
        }

        /// <summary>
        /// Marks the entity as clean (not dirty)
        /// This should be called after successfully saving the entity
        /// </summary>
        public void MarkAsClean()
        {
            if (_isDirty)
            {
                _isDirty = false;
                OnPropertyChanged(nameof(IsDirty));
            }
        }

        /// <summary>
        /// Forces an update of the UpdatedAt timestamp without marking as dirty
        /// Useful for system updates that shouldn't trigger dirty state
        /// </summary>
        public void TouchUpdatedAt()
        {
            _updatedAt = DateTime.UtcNow;
            OnPropertyChanged(nameof(UpdatedAt));
        }
    }
}
