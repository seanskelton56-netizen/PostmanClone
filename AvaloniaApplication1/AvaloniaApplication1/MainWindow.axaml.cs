using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApplication1;

public partial class MainWindow : Window
{
    private static readonly HttpClient _http = new();
    
    // Collection for HTTP methods/requests with body.
    private static readonly HashSet<HttpMethod> ReqWithBody = new(){
        HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch
    };
    
    public MainWindow()
    {
        InitializeComponent();
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
        
        // Once user has clicked send, disable send button and update UI.
        SendButton.IsEnabled = false;
        StatusText.Text = "Sending...";
        ResponseBox.Text = "";
        
        try{
            // Map http method user selected to relevant HttpMethod object.
            var httpMethod = new HttpMethod(method);
            
            // Builds HTTP request from user input.
            using var request = new HttpRequestMessage(httpMethod, url);
            
            // Attach body to relevant HTTP requests.
            if (ReqWithBody.Contains(httpMethod) && !string.IsNullOrWhiteSpace(body)){
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            
            using var response = await _http.SendAsync(request);
            
            ResponseBox.Text = await FormatResponse(response);
            StatusText.Text = "Done";
            
        } /* end of try */ catch (Exception ex){
            StatusText.Text = "Error";
            ResponseBox.Text = ex.Message;
        } /* end of catch */ finally{
            SendButton.IsEnabled = true;  
        } /* end of finally */
    }
    
    // Converts full HTTP response into human readable format.
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
    
    
