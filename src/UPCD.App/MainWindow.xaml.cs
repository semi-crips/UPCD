using System.Windows;
using System.Windows.Controls;\nusing System.Diagnostics;
using UPCD.App.ViewModels;

namespace UPCD.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();
    private bool _initializing;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        Loaded += async (_, _) =>
        {
            _initializing = true;
            await _vm.InitializeAsync();
            SelectByTag(LanguageBox, _vm.Language);
            SelectByTag(ThemeBox, _vm.Theme);
            SelectByTag(ModeBox, _vm.ExperienceMode);
            _initializing = false;
            _vm.ApplyTheme();
        };
    }

    private static void SelectByTag(ComboBox box, string tag)
    {
        box.SelectedItem = box.Items.Cast<ComboBoxItem>().FirstOrDefault(x => (string?)x.Tag == tag);
    }

    private void Setting_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_initializing) return;
        if (LanguageBox.SelectedValue is string lang) _vm.Language = lang;
        if (ThemeBox.SelectedValue is string theme) _vm.Theme = theme;
        if (ModeBox.SelectedValue is string mode) _vm.ExperienceMode = mode;
        _vm.ApplyTheme();
    }

    private async void Save_Click(object sender, RoutedEventArgs e) => await _vm.SaveSettingsAsync();

    private void Accent_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string color })
        {
            _vm.AccentHex = color;
            _vm.ApplyTheme();
        }
    }

    private void Navigation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string section })
        {
            _vm.SelectedSection = section;
            _vm.Status = _vm.IsEnglish
                ? $"{section} module is planned; this foundation build does not perform hardware operations."
                : $"Модуль «{section}» запланирован; эта основа пока не выполняет операции с оборудованием.";
        }
    }

    private void Scan_Click(object sender, RoutedEventArgs e)
    {
        _vm.Status = _vm.IsEnglish
            ? "Hardware scanning is not implemented in this foundation preview."
            : "Определение оборудования ещё не реализовано в этой предварительной версии.";
    }
}
