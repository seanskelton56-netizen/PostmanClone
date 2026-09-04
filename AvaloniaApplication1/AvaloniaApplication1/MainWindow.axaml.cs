using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    public class RequestHistoryEntry
    {
        public required string Method { get; init;}
        public required string Url { get; init;}
        public string? Params { get; init;}
        public string? Headers { get; init;}
        public string? Body { get; init;}
        
        public override string ToString() => $"{Method} {Url}";
    }
    private static readonly HttpClient _http = new();
    private readonly ObservableCollection<RequestHistoryEntry> _history = new();
    
    // Collection for HTTP methods/requests with body.
    private static readonly HashSet<HttpMethod> ReqWithBody = new(){
        HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch
    };
    
    public MainWindow()
    {
        InitializeComponent();
        HistoryList.ItemsSource = _history;
    }
    
    private async void OnSendClick(object? sender, RoutedEventArgs e){
        
        // Stores the URL, selected HTTP method and body of http request to local variables.
        var url = UrlBox.Text?.Trim();
        var method = (MethodBox.SelectedItem as ComboBoxItem)?.Content as string ?? "GET";
        var body = BodyBox.Text;
        
        
        if (string.IsNullOrWhiteSpace(url)){
            
            StatusText.Text = "Enter a URL first.";
            return;
        }
        
        var paramsUrl = BuildUrlWithParams(url, ParamsBox.Text);
        
        // Once user has clicked send, disable send button and update UI.
        SendButton.IsEnabled = false;
        StatusText.Text = "Sending...";
        ResponseBox.Text = "";
        
        try{
            // Map http method user selected to relevant HttpMethod object.
            var httpMethod = new HttpMethod(method);
            
            // Builds HTTP request from user input.
            using var request = new HttpRequestMessage(httpMethod, paramsUrl);
            
            AddCustomHeaders(request, HeadersBox.Text);
            
            // Attach body to relevant HTTP requests.
            if (ReqWithBody.Contains(httpMethod) && !string.IsNullOrWhiteSpace(body)){
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            
            using var response = await _http.SendAsync(request);
            
            ResponseBox.Text = await FormatResponse(response);
            StatusText.Text = "Done";
            
            _history.Insert(0, new RequestHistoryEntry
            {
                Method = method,
                Url = url,
                Params = ParamsBox.Text,
                Headers = HeadersBox.Text,
                Body = body
            });
        } /* end of try */ catch (Exception ex){
            StatusText.Text = "Error";
            ResponseBox.Text = ex.Message;
        } /* end of catch */ finally{
            SendButton.IsEnabled = true;  
        } /* end of finally */
    }
    
    private void OnHistorySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (HistoryList.SelectedItem is not RequestHistoryEntry entry) return;
        UrlBox.Text = entry.Url;
        ParamsBox.Text = entry.Params;
        HeadersBox.Text = entry.Headers;
        BodyBox.Text = entry.Body;
        
        foreach (var item in MethodBox.Items)
        {
            if (item is ComboBoxItem cbi && cbi.Content?.ToString() == entry.Method){
                MethodBox.SelectedItem = cbi;
                break;
            }
        }
    }
    
    private static string BuildUrlWithParams(string baseUrl, string? paramsText){
        if (string.IsNullOrWhiteSpace(paramsText))
            return baseUrl;
        
        
        
        var pairs = new List<String>();

        foreach (var line in paramsText.Split('\n')){
            var trimmed = line.Trim();
            
            if (string.IsNullOrEmpty(trimmed)) continue;
            
            var parts = trimmed.Split('=', 2);
            if (parts.Length != 2) continue;
            
            var key = Uri.UnescapeDataString(parts[0].Trim());
            var value = Uri.UnescapeDataString(parts[1].Trim());
            
            pairs.Add($"{key}={value}");
            
        }
        if (pairs.Count == 0) return baseUrl;
        
        var separator = baseUrl.Contains("?") ? "&" : "?";
        return $"{baseUrl}{separator}{string.Join("&", pairs)}";
    }
    
    private static void AddCustomHeaders(HttpRequestMessage request, string? headersText){
        if (string.IsNullOrWhiteSpace(headersText)) return;

        foreach (var line in headersText.Split('\n')){
            var trimmed = line.Trim();
            
            if (string.IsNullOrEmpty(trimmed)) continue;
            
            var parts = trimmed.Split(':', 2);
            if (parts.Length != 2) continue;
            
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            
            request.Headers.TryAddWithoutValidation(key, value);
            
        }
    }
    
    // Converts full HTTP response into human-readable format.
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
    
    
