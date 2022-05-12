namespace helloWebApp.DTOs;
public class responseLoginOTP{
    public responseLoginUser user {get;set;} = null!;

    public string? token {get;set;}

    public string? refreshToken {get;set;}

    public string? expiration {get;set;}
}