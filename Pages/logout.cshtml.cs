using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class logoutModel : PageModel{
    public logoutModel(){

    }

    public async Task<IActionResult> OnGet(){
        var token =  HttpContext.User.Claims.Where(c => c.Type == "__token").Select( c => c.Value  ).SingleOrDefault();
        Console.WriteLine("token = " + token );

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect("/login");
    }
}