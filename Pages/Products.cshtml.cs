
[ValidateAntiForgeryToken]
[Authorize]
public class ProductsPage : PageModel{

    private HttpClient _httpClient = null!;
    private HttpContext _httpContext = null!;
    public IEnumerable<responseProducts> products = null!;

    public IEnumerable<responseCategory> categories = null!;

    public List<responseUser> users = new List<responseUser>();

    private HttpResponseMessage response = null!;

    public responseProducts product = null!;
    public responseUser user = null!;

    public int productCount = 0;

    public readonly int productPerPage = 5;
    public ProductsPage(IApiClient apiClient,IHttpContextAccessor httpContextAccessor){
        this._httpClient = apiClient.getApiClient();
        _httpContext = httpContextAccessor.HttpContext!;

    }

    //public async Task<IActionResult> OnGet(){
    public async Task<IActionResult> OnGet(){

        //get all categories
        response = await _httpClient.GetAsync("category");
        this.categories = response.Content.ReadFromJsonAsync<IEnumerable<responseCategory>>().Result!;

        switch( HttpContext.Request.Path ){
            case "/products/add":
                await this.formAddProduct();
            break;

            case string path when path.StartsWith("/products/update"):
                var productId = path.Split("/").ToList()[3];
                await this.formUpdateProduct( productId );
            break;

            default:
                await this.listProducts();
            break;
        }
        return Page();

    }

    private async Task listProducts(){
        var currentPage = _httpContext.Request.Query["page"].ToString() == "" ? "1" : _httpContext.Request.Query["page"].ToString();
        response = await _httpClient.GetAsync($"products?page={currentPage}&totalPerPage={this.productPerPage}");
        Console.WriteLine(response.StatusCode);
        this.products = response.Content.ReadFromJsonAsync<IEnumerable<responseProducts>>().Result!;


        response = await _httpClient.GetAsync("products/count");
        this.productCount = Convert.ToInt32( response.Content.ReadAsStringAsync().Result! );




        //get users by product seller
        responseUser tmpUser = null!;
        foreach( var product in this.products ){
            if( users?.Any( u => u.id == product.seller.id) == true ){
                tmpUser = users.Where( u => u.id == product.seller.id).SingleOrDefault()!;
                //if( tmpUser != null ){
                    product.seller.email = tmpUser.email!;
                    product.seller.firstname = tmpUser.firstname!;
                    product.seller.lastname = tmpUser.lastname!;
                //}
            }
            else{
                Console.WriteLine("users/" +  product.seller.id);

                response = await _httpClient.GetAsync("users/" +  product.seller.id );
                tmpUser = response.Content.ReadFromJsonAsync<responseUser>().Result!;
                users?.Add(  tmpUser );
                product.seller.email = tmpUser.email;
                product.seller.firstname = tmpUser.firstname;
                product.seller.lastname = tmpUser.lastname;
            }

            //match category
            product.category.categoryName = this.categories.Where( c => c.Id == product.category.Id ).Select( c => c.categoryName ).SingleOrDefault();


        }

    }

    private async Task formAddProduct(){
        response = await _httpClient.GetAsync("users");
        this.users = response.Content.ReadFromJsonAsync<IEnumerable<responseUser>>().Result!.Where(u => u.roles == "User" ).ToList();
    }

    public IActionResult OnPostAddProduct(
        string productName,
        string description,
        int qty,
        decimal price,
        string categoryId,
        string sellerId

    ){
        var postProductsObject =  JsonConvert.SerializeObject( new{  
            name = productName,
            description = description,
            qty = qty,
            price = price,
            seller = new { id = sellerId },
            category = new { id = categoryId }
        } );
        response =  _httpClient.PostAsync("products", new StringContent( postProductsObject,System.Text.Encoding.UTF8,"application/json" ) ).Result!;

        Console.WriteLine(response.StatusCode);

        if( response.StatusCode == HttpStatusCode.Created ){
            return Content(JsonConvert.SerializeObject( new{ message = "product created"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "error occurred"} ),"application/json");
        }
    }

    public IActionResult OnGetDelete( string productId ){
        response = _httpClient.DeleteAsync($"products/{productId}").Result!;

        if( response.StatusCode == HttpStatusCode.NoContent ){
            return Content(JsonConvert.SerializeObject( new{ message = "deleted"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "something went wrong"} ),"application/json");
        }
    }

    /*public async Task<IActionResult> OnGet( string productId ){
        response = await _httpClient.GetAsync("users");
        this.users = response.Content.ReadFromJsonAsync<IEnumerable<responseUser>>().Result!.Where(u => u.roles == "User" ).ToList();
        
        response = await _httpClient.GetAsync($"products/{productId}");

        if( response.StatusCode == HttpStatusCode.OK ){
            this.product = response.Content.ReadFromJsonAsync<responseProducts>().Result!;
        }

        return Page();
    } */

    private async Task formUpdateProduct( string productId ){
        response = await _httpClient.GetAsync("users");
        this.users = response.Content.ReadFromJsonAsync<IEnumerable<responseUser>>().Result!.Where(u => u.roles == "User" ).ToList();

        response = await _httpClient.GetAsync($"products/{productId}");
        this.product = response.Content.ReadFromJsonAsync<responseProducts>().Result!;
    }

    public IActionResult OnPostUpdateProduct( string productId,
        string productName,
        string description,
        int qty,
        decimal price,
        string categoryId,
        string sellerId){
            var postProductsObject =  JsonConvert.SerializeObject( new{  
            name = productName,
            description = description,
            qty = qty,
            price = price,
            seller = new { id = sellerId },
            category = new { id = categoryId }
        } );
        response =  _httpClient.PutAsync($"products/{productId}", new StringContent( postProductsObject,System.Text.Encoding.UTF8,"application/json" ) ).Result!;

        Console.WriteLine(response.StatusCode);

        if( response.StatusCode == HttpStatusCode.NoContent ){
            return Content(JsonConvert.SerializeObject( new{ message = "product updated"} ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "error occurred"} ),"application/json");
        }
    }
}