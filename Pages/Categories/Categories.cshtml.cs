public class categoriesPage : PageModel{


    private HttpClient _httpClient = null!;
    private HttpContext _httpContext = null!;
    private HttpResponseMessage response = null!;

    public IEnumerable<responseCategory> categories = null!;

    public responseCategory category = null!;

    public readonly int perPage = 5;

    public int count = 0;

    public categoriesPage(IApiClient apiClient, IHttpContextAccessor httpContextAccessor){
        _httpClient = apiClient.getApiClient();
        _httpContext = httpContextAccessor.HttpContext!;

       
    }

    public async Task<IActionResult> OnGet(){
            switch( _httpContext.Request.Path ){
            /* case "/products/add":
                await this.formAddProduct();
            break; */

            case string path when path.StartsWith("/categories/update"):
                var productId = path.Split("/").ToList()[3];
                await this.formUpdate( productId );
            break; 

            default:
                await this.listCategories();
            break;

            
            }
        return Page();
    }

    private async Task listCategories(){
        var currentPage = _httpContext.Request.Query["page"].ToString() == "" ? "1" : _httpContext.Request.Query["page"].ToString();
        response = await _httpClient.GetAsync($"category?page={currentPage}&totalPerPage={this.perPage}");
        this.categories = response.Content.ReadFromJsonAsync<IEnumerable<responseCategory>>().Result!;

        response = await _httpClient.GetAsync("category/count");
        this.count = Convert.ToInt32( response.Content.ReadAsStringAsync().Result! );
    }

    public IActionResult OnPostAdd( string categoryName ){
        var postCategoryObject =  JsonConvert.SerializeObject( new{  
            categoryName = categoryName,
        } );
        response =  _httpClient.PostAsync("category", new StringContent( postCategoryObject,System.Text.Encoding.UTF8,"application/json" ) ).Result!;

        if( response.StatusCode == HttpStatusCode.Created ){
            return Content(JsonConvert.SerializeObject( new{ message = "category created"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "error occurred"} ),"application/json");
        }
    }

    public IActionResult OnPostUpdate( string categoryId,
        string categoryName ){
            var postCategoryObject =  JsonConvert.SerializeObject( new{  
            categoryName = categoryName,
        } );
        response =  _httpClient.PutAsync($"category/{categoryId}", new StringContent( postCategoryObject,System.Text.Encoding.UTF8,"application/json" ) ).Result!;

        Console.WriteLine(response.StatusCode);

        if( response.StatusCode == HttpStatusCode.NoContent ){
            return Content(JsonConvert.SerializeObject( new{ message = "category updated"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "error occurred"} ),"application/json");
        }
    }

    private async Task formUpdate( string categoryId ){
        response = await _httpClient.GetAsync($"category/{categoryId}");
        this.category = response.Content.ReadFromJsonAsync<responseCategory>().Result!;
    }

    public IActionResult OnGetDelete( string categoryId ){
        response = _httpClient.DeleteAsync($"category/{categoryId}").Result!;

        if( response.StatusCode == HttpStatusCode.NoContent ){
            return Content(JsonConvert.SerializeObject( new{ message = "deleted"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "something went wrong"} ),"application/json");
        }
    }

}