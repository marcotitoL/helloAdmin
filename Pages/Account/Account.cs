using Newtonsoft.Json.Linq;

public class accountPage : PageModel{
    private HttpClient _httpClient = null!;
    private HttpContext _httpContext = null!;
    private HttpResponseMessage response = null!;

    public responseUser user = null!;

    public accountPage(IApiClient apiClient, IHttpContextAccessor httpContextAccessor/*IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor*/){
        _httpClient = apiClient.getApiClient();
        _httpContext = httpContextAccessor.HttpContext!;
       
    }

    public async Task<IActionResult> OnGet(){
        response = await _httpClient.GetAsync($"users/{_httpContext.Request.Cookies["__userId"]}");
        user = response.Content.ReadFromJsonAsync<responseUser>().Result!;


        return Page();
    } 

    public IActionResult OnPostSave( string userId, string firstname, string lastname, string birthdate, IFormFile? profilePicture ){

        //Thread.Sleep(1000);
        var postUserObject =  JsonConvert.SerializeObject( new{  
            firstname = firstname,
            lastname = lastname,
            birthdate = birthdate
        } );
        response =  _httpClient.PutAsync($"users/{userId}", new StringContent( postUserObject,System.Text.Encoding.UTF8,"application/json" ) ).Result!;

        var newPicture = "";
        if( profilePicture != null ){
            Console.WriteLine("profile picture changed!");

    byte[] data;
    var br = new BinaryReader(profilePicture.OpenReadStream());
    data = br.ReadBytes((int)profilePicture.OpenReadStream().Length);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add( new ByteArrayContent(data), "profilePicture", profilePicture.FileName );
            var pictureResponse = _httpClient.PostAsync($"users/{userId}/uploadProfilePicture", multipartContent).Result;

            
            
            var pictureResponseJson = JObject.Parse( pictureResponse.Content.ReadAsStringAsync().Result );

            newPicture = pictureResponseJson["profilePicture"]?.ToString();
            
            if( pictureResponse.StatusCode == HttpStatusCode.OK ){
                
            }
            _httpContext.Response.Cookies.Append("__profilePicture", newPicture! );
        }

        if( response.StatusCode == HttpStatusCode.NoContent ){
            return Content(JsonConvert.SerializeObject( new{ message = "account updated", picture = newPicture } ),"application/json");
        }
        else{
            return Content(JsonConvert.SerializeObject( new{ error = "error occurred"} ),"application/json");
        }
    }
}