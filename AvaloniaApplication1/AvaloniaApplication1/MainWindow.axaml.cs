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
    
    private static readonly HashSet<HttpMethod> ReqWithBody = new(){
        HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch
    };
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private async void OnSendClick(object? sender, RoutedEventArgs e){
        
        var url = UrlBox.Text?.Trim();
        var method = (MethodBox.SelectedItem as ComboBoxItem)?.Content as string ?? "GET";
        var body = BodyBox.Text;
        
        if (string.IsNullOrWhiteSpace(url)){
            
            StatusText.Text = "Enter a URL first.";
            return;
        }
        
        SendButton.IsEnabled = false;
        StatusText.Text = "Sending...";
        ResponseBox.Text = "";
        
        try{
            var httpMethod = method switch{
                "GET" => HttpMethod.Get,
                "HEAD" => HttpMethod.Head,
                "DELETE" => HttpMethod.Delete,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "PATCH" => HttpMethod.Patch,
                "OPTIONS" => HttpMethod.Options,
                // "TRACE" => HttpMethod.Trace,
               // "CONNECT" => HttpMethod.Connect,
                _ => HttpMethod.Get
                
            };
            
            using var request = new HttpRequestMessage(httpMethod, url);
            
            if (ReqWithBody.Contains(httpMethod) && !string.IsNullOrWhiteSpace(body)){
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            
            using var response = await _http.SendAsync(request);
            
            ResponseBox.Text = await FormatResponse(response);
            StatusText.Text = "Done";
        } catch (Exception ex){
            StatusText.Text = "Error";
            ResponseBox.Text = ex.Message;
        }finally{
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
    
    
