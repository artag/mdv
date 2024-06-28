using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Mdv.Common;

namespace Mdv.Views;

public partial class MainView : UserControl
{
    /// <summary>
    /// timer to update the rendered html when html in editor changes with delay
    /// </summary>
    private readonly Timer _updateHtmlTimer;

    /// <summary>
    /// used ignore html editor updates when updating separately
    /// </summary>
    private bool _updateLock;

    public MainView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// On tree view node click load the html to the html panel and html editor.
    /// </summary>
    private void OnTreeView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.AddedItems.OfType<TreeViewItem>().FirstOrDefault();
        var sample = item?.Tag as HtmlSample;
        if (sample != null)
        {
            _updateLock = true;

            _htmlEditor.Text = sample.Html;

            Cursor = new Cursor(StandardCursorType.Wait);

            try
            {
                _htmlPanel.AvoidImagesLateLoading = !sample.FullName.Contains("Many images");
                _htmlPanel.Text = sample.Html;
            }
            catch (Exception ex)
            {
                MessageBox(ex.ToString(), "Failed to render HTML");
            }

            Cursor = new Cursor(StandardCursorType.Arrow);
            _updateLock = false;
        }
    }

    /// <summary>
    /// On text change in the html editor update 
    /// </summary>
    private void OnHtmlEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_updateLock)
        {
            _updateHtmlTimer.Change(1000, int.MaxValue);
        }
    }

    private void MessageBox(string text, string title = null)
    {
        var window = new Window
        {
            Content = new SelectableTextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
            },
            SizeToContent = SizeToContent.Height,
            Width = 400,
            Title = title ?? "Message"
        };
        window.Show();
    }
}
