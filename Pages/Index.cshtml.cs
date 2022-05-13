[Authorize]
public class IndexModel : PageModel{
    HttpClient _httpClient = null!;

    HttpResponseMessage response = null!;

    public int noOfUsers = 0;
    public int noOfProducts = 0;

    public int noOfCategories = 0;
    
    public IndexModel(IApiClient apiClient){
        _httpClient = apiClient.getApiClient();
    }
    public async Task<IActionResult> OnGet(){
        response = await _httpClient.GetAsync("users/count");

        noOfUsers = Convert.ToInt32( response.Content.ReadAsStringAsync().Result );

        response = await _httpClient.GetAsync("products/count");

        noOfProducts = Convert.ToInt32( response.Content.ReadAsStringAsync().Result );

        response = await _httpClient.GetAsync("category/count");

        noOfCategories = Convert.ToInt32( response.Content.ReadAsStringAsync().Result );

        return Page();
    }
}