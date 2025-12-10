using Avalonia.Controls;
using Avalonia.Input;

namespace HeaderHero;

public partial class MessageBox3 : Window
{
    readonly int cancelIndex;
    public MessageBox3(string title, string message, params string[] buttons)
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            // otherwise keyboard navigation sometimes stays in the parent window
            if (Owner != null) Owner.IsEnabled = false;
        };

        Title = title;
        MessageText.Text = message;

        cancelIndex = buttons.Length - 1;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            var btn = new Button
            {
                Content = buttons[i],
                MinWidth = 60
            };
            btn.Click += (_, _) => Close(index);
            if (index == 0)
                btn.IsDefault = true;
            ButtonsPanel.Children.Add(btn);
        }

        Closing += OnClosing;
        KeyDown += OnKeyDown;
    }

    void OnClosing(object sender, WindowClosingEventArgs e)
    {
        if (Owner != null) Owner.IsEnabled = true;
        if (!e.IsProgrammatic)
            Close(cancelIndex);
    }

    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (Owner != null) Owner.IsEnabled = true;
            Close(cancelIndex);
        }
    }
}
