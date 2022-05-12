public class ApiClient : IApiClient{

    private HttpClient _httpClient;
    private HttpContext _httpContext = null!;
    private HttpResponseMessage response = null!;

    public ApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor){
        _httpClient = httpClientFactory.CreateClient("helloAPI");
        _httpContext = httpContextAccessor.HttpContext!;
        var refreshToken = _httpContext.Request.Cookies["__refreshToken"];
        var accessToken = _httpContext.Request.Cookies["__accessToken"];
        
        DateTime tokenExpiration = Convert.ToDateTime( _httpContext.Request.Cookies["__expiration"] );
        if( tokenExpiration <= DateTime.Now ){
            //request new token
            response = _httpClient.PostAsync( $"users/refreshToken?accessToken={HttpUtility.UrlEncode(accessToken)}&refreshToken={HttpUtility.UrlEncode(refreshToken)}", null ).Result;
            responseRefreshToken responseRefreshToken =  response.Content.ReadFromJsonAsync<responseRefreshToken>().Result!;

            accessToken = responseRefreshToken.accessToken;

            _httpContext.Response.Cookies.Append("__accessToken", responseRefreshToken?.accessToken! );
            _httpContext.Response.Cookies.Append("__refreshToken", responseRefreshToken?.refreshToken! );
            _httpContext.Response.Cookies.Append("__expiration", responseRefreshToken?.Expiration! );

        }

        _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", accessToken );
    }

    public HttpClient getApiClient(){
        return this._httpClient;
    }
}