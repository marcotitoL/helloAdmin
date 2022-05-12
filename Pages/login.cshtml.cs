
[IgnoreAntiforgeryToken(Order = 1001)]
    public class loginModel : PageModel
    {
        private HttpClient _httpClient;
        private responseLogin? _responseLogin;
        private responseLoginOTP? _responseLoginOTP;

        private responseUser? _responseUser;
        public loginModel(IHttpClientFactory httpClientFactory){
            _httpClient = httpClientFactory.CreateClient("helloAPI");
        }

        public void OnGet()
        {
            //return new RedirectResult("/Start");
            Console.WriteLine("get");
        }

        public async Task<IActionResult> OnPost( string username, string password ){


            var loggedIn = await login(username,password);

            if( loggedIn ){
                return Content( JsonConvert.SerializeObject( new{ Id = _responseLogin?.User.Id, Phonenumber = _responseLogin?.User.Phonenumber, OTP = _responseLogin?.User.OTP  } ),"application/json");
/*
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Administrator"),
        };


            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false,
                RedirectUri = "/"
            };

            return SignIn(userPrincipal,authenticationProperties,CookieAuthenticationDefaults.AuthenticationScheme);
            */
            }
            else{
                return Content( JsonConvert.SerializeObject( new{ error = "not loggedin"} ),"application/json");
            }
            
            
        }

        private async Task<bool> login( string username, string password ){
            var loginRequestObject =  JsonConvert.SerializeObject( new{ email = username, password = password } );
            Console.WriteLine(loginRequestObject);
            HttpResponseMessage response = await _httpClient.PostAsync("users/login", new StringContent( loginRequestObject,System.Text.Encoding.UTF8,"application/json" ) );
            Console.WriteLine("here");
            Console.WriteLine(response.StatusCode);
            if( response.StatusCode != HttpStatusCode.OK )
                return false;
            else{
                this._responseLogin = await response.Content.ReadFromJsonAsync<responseLogin>();
                return true;
            }
        }

        public async Task<IActionResult> OnPostLoginOTP( string userId, string otp ){
            /* var formData = new Dictionary<string,string>{
                {"Id",userId},{"smscode",otp}
            }; */
            
            HttpResponseMessage response = await _httpClient.PostAsync($"users/loginOTP?Id={userId}&smscode={otp}", null );

            if( response.StatusCode == HttpStatusCode.OK ){
                this._responseLoginOTP = await response.Content.ReadFromJsonAsync<responseLoginOTP>();
                Console.WriteLine(this._responseLoginOTP?.token);

                //get user data
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._responseLoginOTP?.token );
                response = await _httpClient.GetAsync($"users/{this._responseLoginOTP?.user.Id}" );
                if( response.StatusCode == HttpStatusCode.OK )
                    this._responseUser = await response.Content.ReadFromJsonAsync<responseUser>();
                

                

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, this._responseUser?.email!),
            //new Claim("__userId", userId),
            //new Claim("__token", this._responseLoginOTP?.token! ),
            //new Claim("__refreshToken", this._responseLoginOTP?.refreshToken!),
            //new Claim("__expiration", this._responseLoginOTP?.expiration!),

        };

        List<string> roles = this._responseUser?.roles.Split(",").ToList()!;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }


            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                IsPersistent = true,
                AllowRefresh = true,
                RedirectUri = "/"
            };

            //_ = SignIn(userPrincipal,authenticationProperties,CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties );

            HttpContext.Response.Cookies.Append("__accessToken", this._responseLoginOTP?.token! );
            HttpContext.Response.Cookies.Append("__refreshToken", this._responseLoginOTP?.refreshToken! );
            HttpContext.Response.Cookies.Append("__userId", this._responseLoginOTP?.user.Id! );
            HttpContext.Response.Cookies.Append("__expiration", this._responseLoginOTP?.expiration! );

            //get user details
            response = await _httpClient.GetAsync($"users/{this._responseLoginOTP?.user.Id!}");
            responseUser loggedInUser = response.Content.ReadFromJsonAsync<responseUser>().Result!;

            HttpContext.Response.Cookies.Append("__profilePicture", loggedInUser.profileImage! );
            

                return Content(JsonConvert.SerializeObject( new{ message = "otp verified"} ),"application/json");
            }
            else{
                return Content( JsonConvert.SerializeObject( new{ error = "invalid otp"} ),"application/json");
            }


        }

    }
