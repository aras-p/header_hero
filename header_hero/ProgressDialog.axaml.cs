using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using HeaderHero.Parser;

namespace HeaderHero;

public partial class ProgressDialog : Window
{
    private readonly ProgressFeedback _feedback = new();
    private Func<ProgressFeedback, Task>? _work;
    public ProgressDialog()
    {
        InitializeComponent();
    }

    public void Start(Func<ProgressFeedback, Task> work)
    {
        _work = work;

        // Show the dialog and then start the work
        this.Opened += async (_, _) =>
        {
            await RunWorkAsync();
        };
    }

    private async Task RunWorkAsync()
    {
        try
        {
            await Task.Run(() => _work!(_feedback));
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Console.WriteLine(ex); //@TODO: display error dialog?
            });
        }

        // Close the dialog when work is done
        await Dispatcher.UIThread.InvokeAsync(Close);
    }

    public void Poll()
    {
        ProgressBar.Maximum = Math.Max(1, _feedback.Count);
        ProgressBar.Value = Math.Clamp(_feedback.Item, 0, ProgressBar.Maximum);

        ProgressReportLabel.Text = $"{_feedback.Item}/{_feedback.Count}";
        MessageLabel.Text = _feedback.Message;
        this.Title = _feedback.Title;
    }

    public ProgressFeedback Feedback => _feedback;
}