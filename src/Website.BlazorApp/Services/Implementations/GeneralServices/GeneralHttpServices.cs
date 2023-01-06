using System.Net;
using KSFramework.Enums;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Website.BlazorApp.Services.Contracts.GeneralContracts;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Website.BlazorApp.Services.Implementations.GeneralServices;

public class GeneralHttpServices : IGeneralHttpServices
{
    private readonly string HttpClientFactoryName = "WebsiteApi";
    private readonly IHttpClientFactory _clientFactory;

    public GeneralHttpServices(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public async Task<ResultMessage<IEnumerable<T>>> GetPaginated<T>(string address, int? pageIndex = 1, int? pageSize = 20,
        string? searchTerm = null, string? orderByPropertyName = "Id", bool desc = false)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
        var request = new HttpRequestMessage(
            HttpMethod.Get, $"{address}?pageNumber={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}&orderByPropertyName={orderByPropertyName}&desc={desc}");

        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        
        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
            return null;
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var convertResult = JsonConvert.DeserializeObject<ResultMessage<IEnumerable<T>>>(content);

        if (!convertResult.IsSuccess || convertResult.Status != ApiResultStatusCode.Success)
            throw new HttpRequestException(convertResult.Message != "" ? convertResult.Message :
                $"An error occured while trying to get the data from Api. {convertResult.Status}");

        return convertResult;
    }

    public async Task<T> GetOne<T>(string address)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
    
        var request = new HttpRequestMessage(
            HttpMethod.Get, address);
    
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
        }
    
        var content = await response.Content.ReadAsStringAsync();
    
        var convertResult = JsonConvert.DeserializeObject<OkResponse<T>>(content);
        
        if (convertResult == null)
            throw new ApplicationException("Could not convert the API result to the model");
        
        return convertResult.Result;
    }
    
    public async Task<bool> Add<T>(object viewModel, string address)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
    
        var request = new HttpRequestMessage(
            HttpMethod.Post, address)
        {
            Content = JsonContent.Create(viewModel)
        };
    
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
            return false;
        }
    
        var content = await response.Content.ReadAsStringAsync();
    
        var convertResult = JsonConvert.DeserializeObject<OkResponse<Guid>>(content);

        return convertResult.Code == 200;
    }
    
    public async Task<bool> Update<T>(object viewModel, string address)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
    
        var request = new HttpRequestMessage(
            HttpMethod.Put, address)
        {
            Content = JsonContent.Create(viewModel)
        };
    
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
            return false;
        }
    
        var content = await response.Content.ReadAsStringAsync();
    
        var convertResult = JsonConvert.DeserializeObject<BaseResponse<Guid>>(content);
        
        return convertResult.Code == 200;
    }
    
    public async Task<bool> Update(string address)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
    
        var request = new HttpRequestMessage(
            HttpMethod.Put, address);
    
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
            return false;
        }
    
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        var convertResult = JsonSerializer.Deserialize<OkResponse<Guid>>
        (
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        return convertResult?.Code == 200;
    }
    public async Task<bool> Delete(string address)
    {
        var httpClient = _clientFactory.CreateClient(HttpClientFactoryName);
    
        var request = new HttpRequestMessage(
            HttpMethod.Delete, address);
    
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
    
        if (!response.IsSuccessStatusCode)
        {
            CheckForErrors(response);
            return false;
        }
    
        var content = await response.Content.ReadAsStringAsync();
    
        var convertResult = JsonSerializer.Deserialize<OkResponse<Guid>>
        (
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        return convertResult?.Code == 200;
    }
    private void CheckForErrors(HttpResponseMessage response)
    {
        //TODO : Log errors and throw appropriate exception and exception message
        var statusCode = response.StatusCode;
        switch (statusCode)
        {
            case HttpStatusCode.NotFound:
            {
                throw new ApplicationException("Notfound Error");
                //_navManager.NavigateTo("/error/404");
                break;
            }
            case HttpStatusCode.Unauthorized:
            {
                throw new ApplicationException("Unauthorized Error");
                // _navManager.NavigateTo("/error/401"); 
                break;
            }
            case HttpStatusCode.Forbidden:
            {
                throw new ApplicationException("Forbidden Error");
                // _navManager.NavigateTo("/error/403");
                break;
            }
            default:
            {
                throw new ApplicationException("Server Error");
                // _navManager.NavigateTo("/error/500"); 
                break;
            }
        }
    }
}