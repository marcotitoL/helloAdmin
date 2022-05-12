public class usersPage : PageModel{

    public IEnumerable<responseUser> users = null!;
    public int count = 0;

    private HttpClient _httpClient = null!;
    private HttpContext _httpContext = null!;
    private HttpResponseMessage response = null!;

    public readonly int perPage = 10;

   public usersPage(IApiClient apiClient, IHttpContextAccessor httpContextAccessor){
        _httpClient = apiClient.getApiClient();
        _httpContext = httpContextAccessor.HttpContext!;

       
    }

    public async Task<IActionResult> OnGet( ){

        var currentPage = _httpContext.Request.Query["page"].ToString() == "" ? "1" : _httpContext.Request.Query["page"].ToString();

        response = await _httpClient.GetAsync($"users?page={currentPage}&totalPerPage={this.perPage}&confirmedOnly=false");
        if( response.StatusCode == HttpStatusCode.OK ){
            Console.WriteLine("fetched users");
        }

        this.users = response.Content.ReadFromJsonAsync<IEnumerable<responseUser>>().Result!;

        response = await _httpClient.GetAsync("users/count");
        this.count = Convert.ToInt32( response.Content.ReadAsStringAsync().Result );

        return Page();
    }

    public async Task<IActionResult> OnGetDelete( string userId ){

        response = await _httpClient.DeleteAsync($"users/{userId}");

        return  Content(JsonConvert.SerializeObject( response.StatusCode == HttpStatusCode.OK ? new{ message = "deleted"} :  new{ error = "something went wrong"} ),"application/json");
        
    }
}