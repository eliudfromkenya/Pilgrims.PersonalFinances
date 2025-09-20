# UI/UX Design Guidelines
## Personal Finance Manager - .NET MAUI Application

### Table of Contents
1. [Design Philosophy](#design-philosophy)
2. [Color Palette & Theming](#color-palette--theming)
3. [Typography](#typography)
4. [Layout & Spacing](#layout--spacing)
5. [Component Library](#component-library)
6. [Screen-Specific Guidelines](#screen-specific-guidelines)
7. [Animation & Transitions](#animation--transitions)
8. [Accessibility](#accessibility)
9. [Responsive Design](#responsive-design)
10. [Implementation Guidelines](#implementation-guidelines)

---

## Design Philosophy

### Core Principles
- **Financial-Grade Professionalism**: Clean, trustworthy interface that instills confidence
- **Clarity First**: Financial data must be immediately readable and understandable
- **Contextual Color Coding**: Consistent color semantics across all financial elements
- **Smooth Interactions**: Fluid animations that enhance rather than distract
- **Accessibility**: Inclusive design for all users and abilities

### Visual Hierarchy
1. **Primary**: Account balances, critical alerts, main actions
2. **Secondary**: Transaction details, budget progress, navigation
3. **Tertiary**: Metadata, timestamps, supporting information

---

## Color Palette & Theming

### Light Theme
```
Primary Colors:
- Background: #F8FAFC (Light Sky Blue)
- Surface: #FFFFFF (Pure White)
- Primary: #1E40AF (Navy Blue)
- Secondary: #3B82F6 (Blue)

Text Colors:
- Primary Text: #1F2937 (Dark Gray)
- Secondary Text: #6B7280 (Medium Gray)
- Disabled Text: #9CA3AF (Light Gray)

Financial Semantics:
- Positive/Income/Assets: #059669 (Green)
- Negative/Expenses/Debts: #DC2626 (Red)
- Neutral/Transfers: #3B82F6 (Blue)
- Warning/Budget Alert: #F59E0B (Amber)

Accent Colors:
- Success: #10B981 (Emerald)
- Error: #EF4444 (Red)
- Warning: #F59E0B (Amber)
- Info: #3B82F6 (Blue)
```

### Dark Theme
```
Primary Colors:
- Background: #0F172A (Dark Navy)
- Surface: #1E293B (Slate)
- Primary: #60A5FA (Light Blue)
- Secondary: #3B82F6 (Blue)

Text Colors:
- Primary Text: #F8FAFC (Light Gray)
- Secondary Text: #CBD5E1 (Medium Light Gray)
- Disabled Text: #64748B (Muted Gray)

Financial Semantics:
- Positive/Income/Assets: #34D399 (Light Green)
- Negative/Expenses/Debts: #F87171 (Light Red)
- Neutral/Transfers: #60A5FA (Light Blue)
- Warning/Budget Alert: #FBBF24 (Light Amber)

Accent Colors:
- Success: #34D399 (Light Emerald)
- Error: #F87171 (Light Red)
- Warning: #FBBF24 (Light Amber)
- Info: #60A5FA (Light Blue)
```

### Theme Implementation
```xml
<!-- Light Theme Resources -->
<Color x:Key="BackgroundColor">#F8FAFC</Color>
<Color x:Key="SurfaceColor">#FFFFFF</Color>
<Color x:Key="PrimaryColor">#1E40AF</Color>
<Color x:Key="PositiveColor">#059669</Color>
<Color x:Key="NegativeColor">#DC2626</Color>

<!-- Dark Theme Resources -->
<Color x:Key="BackgroundColorDark">#0F172A</Color>
<Color x:Key="SurfaceColorDark">#1E293B</Color>
<Color x:Key="PrimaryColorDark">#60A5FA</Color>
<Color x:Key="PositiveColorDark">#34D399</Color>
<Color x:Key="NegativeColorDark">#F87171</Color>
```

---

## Typography

### Font Hierarchy
```xml
<!-- Primary Font Family -->
<OnPlatform x:TypeArguments="x:String" x:Key="PrimaryFont">
    <On Platform="iOS">SF Pro Display</On>
    <On Platform="Android">Roboto</On>
    <On Platform="WinUI">Segoe UI</On>
</OnPlatform>

<!-- Monospace for Financial Data -->
<OnPlatform x:TypeArguments="x:String" x:Key="MonospaceFont">
    <On Platform="iOS">SF Mono</On>
    <On Platform="Android">Roboto Mono</On>
    <On Platform="WinUI">Consolas</On>
</OnPlatform>
```

### Text Styles
```xml
<!-- Headings -->
<Style x:Key="Heading1" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource PrimaryFont}" />
    <Setter Property="FontSize" Value="32" />
    <Setter Property="FontAttributes" Value="Bold" />
</Style>

<Style x:Key="Heading2" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource PrimaryFont}" />
    <Setter Property="FontSize" Value="24" />
    <Setter Property="FontAttributes" Value="Bold" />
</Style>

<!-- Financial Data -->
<Style x:Key="CurrencyLarge" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource MonospaceFont}" />
    <Setter Property="FontSize" Value="28" />
    <Setter Property="FontAttributes" Value="Bold" />
</Style>

<Style x:Key="CurrencyMedium" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource MonospaceFont}" />
    <Setter Property="FontSize" Value="18" />
    <Setter Property="FontAttributes" Value="None" />
</Style>

<!-- Body Text -->
<Style x:Key="BodyText" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource PrimaryFont}" />
    <Setter Property="FontSize" Value="16" />
</Style>

<Style x:Key="Caption" TargetType="Label">
    <Setter Property="FontFamily" Value="{StaticResource PrimaryFont}" />
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{DynamicResource SecondaryTextColor}" />
</Style>
```

---

## Layout & Spacing

### Grid System
- **Base Unit**: 8px
- **Margins**: 16px (2 units)
- **Padding**: 12px (1.5 units)
- **Card Spacing**: 8px (1 unit)

### Spacing Scale
```xml
<x:Double x:Key="SpacingXS">4</x:Double>
<x:Double x:Key="SpacingS">8</x:Double>
<x:Double x:Key="SpacingM">12</x:Double>
<x:Double x:Key="SpacingL">16</x:Double>
<x:Double x:Key="SpacingXL">24</x:Double>
<x:Double x:Key="SpacingXXL">32</x:Double>
```

### Border Radius
```xml
<x:Double x:Key="BorderRadiusS">4</x:Double>
<x:Double x:Key="BorderRadiusM">8</x:Double>
<x:Double x:Key="BorderRadiusL">12</x:Double>
<x:Double x:Key="BorderRadiusXL">16</x:Double>
```

---

## Component Library

### Cards
```xml
<Style x:Key="BaseCard" TargetType="Frame">
    <Setter Property="BackgroundColor" Value="{DynamicResource SurfaceColor}" />
    <Setter Property="CornerRadius" Value="{StaticResource BorderRadiusM}" />
    <Setter Property="HasShadow" Value="True" />
    <Setter Property="Padding" Value="{StaticResource SpacingM}" />
    <Setter Property="Margin" Value="{StaticResource SpacingS}" />
</Style>

<Style x:Key="AccountCard" TargetType="Frame" BasedOn="{StaticResource BaseCard}">
    <Setter Property="BorderColor" Value="{DynamicResource PrimaryColor}" />
</Style>

<Style x:Key="DebtCard" TargetType="Frame" BasedOn="{StaticResource BaseCard}">
    <Setter Property="BorderColor" Value="{DynamicResource NegativeColor}" />
</Style>

<Style x:Key="AssetCard" TargetType="Frame" BasedOn="{StaticResource BaseCard}">
    <Setter Property="BorderColor" Value="{DynamicResource PositiveColor}" />
</Style>
```

### Buttons
```xml
<Style x:Key="PrimaryButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="{DynamicResource PrimaryColor}" />
    <Setter Property="TextColor" Value="White" />
    <Setter Property="CornerRadius" Value="{StaticResource BorderRadiusM}" />
    <Setter Property="Padding" Value="16,12" />
    <Setter Property="FontAttributes" Value="Bold" />
</Style>

<Style x:Key="SecondaryButton" TargetType="Button">
    <Setter Property="BackgroundColor" Value="Transparent" />
    <Setter Property="TextColor" Value="{DynamicResource PrimaryColor}" />
    <Setter Property="BorderColor" Value="{DynamicResource PrimaryColor}" />
    <Setter Property="BorderWidth" Value="1" />
    <Setter Property="CornerRadius" Value="{StaticResource BorderRadiusM}" />
</Style>

<Style x:Key="DangerButton" TargetType="Button" BasedOn="{StaticResource PrimaryButton}">
    <Setter Property="BackgroundColor" Value="{DynamicResource NegativeColor}" />
</Style>
```

### Progress Bars
```xml
<Style x:Key="BudgetProgressBar" TargetType="ProgressBar">
    <Setter Property="ProgressColor" Value="{DynamicResource PositiveColor}" />
    <Setter Property="BackgroundColor" Value="{DynamicResource SurfaceColor}" />
    <Setter Property="HeightRequest" Value="8" />
</Style>
```

---

## Screen-Specific Guidelines

### Home Dashboard
```
Layout Structure:
┌─────────────────────────────┐
│ Header (Greeting + Theme)   │
├─────────────────────────────┤
│ Balance Summary Cards       │
│ ┌─────┐ ┌─────┐ ┌─────┐    │
│ │ Net │ │ Inc │ │ Exp │    │
│ └─────┘ └─────┘ └─────┘    │
├─────────────────────────────┤
│ Budget Overview Chart       │
├─────────────────────────────┤
│ Upcoming Obligations        │
│ • Bill Due Tomorrow         │
│ • Salary Expected Friday    │
└─────────────────────────────┘
```

**Key Elements:**
- Animated count-up for balance changes
- Color-coded budget progress bars
- Swipeable upcoming obligations list
- Quick action floating button

### Accounts Screen
```
Layout Structure:
┌─────────────────────────────┐
│ Total Balance (Large)       │
├─────────────────────────────┤
│ Account Cards (Scrollable)  │
│ ┌─────────────────────────┐ │
│ │ 💳 Checking Account     │ │
│ │ $2,450.00              │ │
│ │ ────────────────────── │ │
│ │ Recent: -$45.00 Coffee │ │
│ └─────────────────────────┘ │
└─────────────────────────────┘
```

**Interactions:**
- Tap to expand account details
- Swipe for quick transfer
- Long press for account options
- Pull-to-refresh for balance update

### Transaction Entry
```
Form Layout:
┌─────────────────────────────┐
│ Amount Input (Large)        │
├─────────────────────────────┤
│ Category Selector           │
├─────────────────────────────┤
│ Account Dropdown            │
├─────────────────────────────┤
│ Date Picker                 │
├─────────────────────────────┤
│ Notes Field                 │
├─────────────────────────────┤
│ Receipt Camera Button       │
├─────────────────────────────┤
│ [Cancel] [Save Transaction] │
└─────────────────────────────┘
```

### Budget Management
```
Budget Card Layout:
┌─────────────────────────────┐
│ 🍔 Food & Dining           │
│ $450 / $600 (75%)          │
│ ████████░░ Progress Bar    │
│ $150 remaining this month  │
└─────────────────────────────┘
```

---

## Animation & Transitions

### Page Transitions
```csharp
// Slide transition for navigation
public static Animation SlideInFromRight(View view)
{
    return new Animation(v => view.TranslationX = v, 
        view.Width, 0, Easing.CubicOut);
}

// Fade transition for modals
public static Animation FadeIn(View view)
{
    return new Animation(v => view.Opacity = v, 
        0, 1, Easing.Linear);
}
```

### Micro-Interactions
- **Button Press**: Scale down to 0.95, duration 100ms
- **Card Tap**: Subtle shadow increase, duration 150ms
- **Balance Update**: Count-up animation, duration 800ms
- **Progress Bar**: Smooth fill animation, duration 600ms

### Loading States
```xml
<ActivityIndicator x:Name="LoadingSpinner"
                   IsVisible="{Binding IsLoading}"
                   IsRunning="{Binding IsLoading}"
                   Color="{DynamicResource PrimaryColor}"
                   VerticalOptions="Center"
                   HorizontalOptions="Center" />
```

---

## Accessibility

### Color Contrast
- **Minimum Ratio**: 4.5:1 for normal text
- **Enhanced Ratio**: 7:1 for financial data
- **Large Text**: 3:1 minimum ratio

### Font Scaling
```csharp
// Support dynamic font sizing
public static double GetScaledFontSize(double baseSize)
{
    var scale = DeviceDisplay.MainDisplayInfo.Density;
    return baseSize * Math.Max(1.0, scale);
}
```

### Screen Reader Support
```xml
<Label Text="Account Balance"
       AutomationProperties.IsInAccessibleTree="True"
       AutomationProperties.Name="Current account balance is two thousand four hundred fifty dollars" />
```

### Touch Targets
- **Minimum Size**: 44x44 points
- **Recommended**: 48x48 points
- **Spacing**: 8 points between targets

---

## Responsive Design

### Breakpoints
```csharp
public enum ScreenSize
{
    Small,   // < 600dp width (phones)
    Medium,  // 600-840dp width (large phones, small tablets)
    Large    // > 840dp width (tablets)
}
```

### Adaptive Layouts
```xml
<OnPlatform x:TypeArguments="GridLength">
    <On Platform="iOS, Android">
        <OnIdiom x:TypeArguments="GridLength"
                 Phone="*"
                 Tablet="2*,*,2*" />
    </On>
</OnPlatform>
```

### Orientation Handling
- **Portrait**: Single column layout
- **Landscape**: Two-column layout for tablets
- **Rotation**: Preserve scroll position and form data

---

## Implementation Guidelines

### XAML Structure
```xml
<ContentPage x:Class="PersonalFinance.Views.DashboardPage"
             Style="{DynamicResource BasePage}">
    <ScrollView>
        <StackLayout Spacing="{StaticResource SpacingM}"
                     Padding="{StaticResource SpacingL}">
            <!-- Content here -->
        </StackLayout>
    </ScrollView>
</ContentPage>
```

### Theme Switching
```csharp
public void ApplyTheme(AppTheme theme)
{
    var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
    
    if (theme == AppTheme.Dark)
    {
        mergedDictionaries.Add(new DarkTheme());
    }
    else
    {
        mergedDictionaries.Add(new LightTheme());
    }
}
```

### Performance Considerations
- Use `CollectionView` for large lists
- Implement virtualization for 1000+ items
- Cache images and use appropriate formats
- Minimize layout passes with proper constraints

### Testing Guidelines
- Test on multiple screen sizes
- Verify color contrast ratios
- Test with system font scaling
- Validate touch target sizes
- Test theme switching animations

---

## Component Examples

### Account Balance Card
```xml
<Frame Style="{StaticResource AccountCard}">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <Label Grid.Row="0" 
               Text="{Binding AccountName}"
               Style="{StaticResource Heading2}" />
               
        <Label Grid.Row="1"
               Text="{Binding Balance, StringFormat='{0:C}'}"
               Style="{StaticResource CurrencyLarge}"
               TextColor="{Binding BalanceColor}" />
               
        <Label Grid.Row="2"
               Text="{Binding LastTransaction}"
               Style="{StaticResource Caption}" />
    </Grid>
</Frame>
```

### Budget Progress Indicator
```xml
<StackLayout>
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>
        
        <Label Grid.Column="0"
               Text="{Binding CategoryName}"
               Style="{StaticResource BodyText}" />
               
        <Label Grid.Column="1"
               Text="{Binding ProgressText}"
               Style="{StaticResource Caption}" />
    </Grid>
    
    <ProgressBar Progress="{Binding ProgressValue}"
                 Style="{StaticResource BudgetProgressBar}" />
                 
    <Label Text="{Binding RemainingAmount, StringFormat='${0:N0} remaining'}"
           Style="{StaticResource Caption}"
           HorizontalOptions="End" />
</StackLayout>
```

This comprehensive UI/UX design guide ensures a consistent, professional, and accessible financial application that meets the highest standards for financial software while providing an excellent user experience across all supported platforms.