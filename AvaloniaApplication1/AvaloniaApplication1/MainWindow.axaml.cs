using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    private static readonly HttpClient _http = new();

    public MainWindow()
    {
        InitializeComponent();
    }
    
    private async void OnSendClick(object? sender, RoutedEventArgs e){
        var url = UrlBox.Text?.Trim();
        var method = (MethodBox.SelectedItem as ComboBoxItem)?.Content as string ?? "GET";
        
        if (string.IsNullOrWhiteSpace(url))
        {
            StatusText.Text = "Enter a URL first.";
            return;
        }
        
        SendButton.IsEnabled = false;
        StatusText.Text = "Sending...";
        ResponseBox.Text = "";
        
        try{
            var httpMethod = method == "HEAD" ? HttpMethod.Head : HttpMethod.Get;
            
            using var request = new HttpRequestMessage(httpMethod, url);            
            using var response = await _http.SendAsync(request);
            
            ResponseBox.Text = await FormatResponse(response);
            StatusText.Text = "Done";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Error";
            ResponseBox.Text = ex.Message;
        }
        finally
        {
            SendButton.IsEnabled = true;  
        }
    }
    
    private static async Task<string> FormatResponse(HttpResponseMessage response)    {        
        var sb = new StringBuilder();
        
        sb.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");
        
        foreach (var header in response.Headers)
            sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
  
        foreach (var header in response.Content.Headers)
            sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        
        var body = await response.Content.ReadAsStringAsync();        
        if (!string.IsNullOrEmpty(body)){
            sb.AppendLine();
            sb.Append(body);
        }
        return sb.ToString();
    }
}
    
    
