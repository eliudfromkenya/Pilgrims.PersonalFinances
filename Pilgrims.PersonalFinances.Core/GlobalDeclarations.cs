using Microsoft.Extensions.DependencyInjection;
using Pilgrims.PersonalFinances.Core.Messaging.Interfaces;
using Pilgrims.PersonalFinances.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core
{
    public static class GlobalDeclarations
    {
        public const string ApplicationName = "Pilgrims Personal Finances";
        public const string ApplicationVersion = "1.0.0";
        public const string CompanyName = "Pilgrims Systems";
        public const string Website = "https://pilgrimssoft.com";
        public const string SupportEmail = "";
        public static readonly object SyncRoot = new();
        public static IServiceCollection? CurrentServiceCollection { get; internal set; }
        public static IServiceProvider? CurrentServiceProvider { get; internal set; }
        public static Func<IDbConnection> GetDbConnection { get; set; } = null!;
        public static string ApplicationDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PilgrimsFinances");
        public static string GeneralApplicationHashKey = "This is the general Hash Key";
        public static string CurrentDeviceNumber { get; set; } = "AAAAAA";
        public static string? DbConnectionString { get; internal set; }
        public static IMessagingService? AppMessagingService { get; set; }
        public static BehaviorSubject<User> LoggedInUser { get; } = new(new());
        public static BehaviorSubject<UserSession> LoginUserSession { get; } = new(new());
        public static BehaviorSubject<string> ComputerId { get; } = new("");
        public static BehaviorSubject<string> CurrentTitle { get; } = new("Pilgrims Personal Finances");
        public static BehaviorSubject<double> FontSize { get; } = new(18);
        public static BehaviorSubject<bool> SystemLoggedIn { get; } = new(false);
        public static BehaviorSubject<bool> SystemLocked { get; } = new(false);
    }
}
