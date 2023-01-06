using IdentityModel.Client;

namespace Website.BlazorApp.Handlers;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientCredentialsTokenRequest _clientCredentialsTokenRequest;


    public AuthenticationDelegatingHandler(
        IHttpClientFactory httpClientFactory,
        ClientCredentialsTokenRequest clientCredentialsTokenRequest)
    {
        _httpClientFactory = httpClientFactory;
        _clientCredentialsTokenRequest = clientCredentialsTokenRequest;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("IdPClient");
        var tokenRequest = await httpClient.RequestClientCredentialsTokenAsync(_clientCredentialsTokenRequest, cancellationToken);

        if (!tokenRequest.IsError)
        {
            request.SetBearerToken(tokenRequest.AccessToken);
        }
        else
        {
            Console.WriteLine($"\n\n\n\n\n TokenRequestError: {tokenRequest.ErrorDescription} \n\n\n\n\n\n");
            //Throw error, I don't agree with throwing error because some times we dont need token to request endpoints
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
