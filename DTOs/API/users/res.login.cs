namespace helloWebApp.DTOs;
public class responseLogin{
        public responseLoginUser User {get;set;} = null!;
}

public class responseLoginUser{
    public string? Id {get;set;}

    public string? Phonenumber {get;set;}

    public string? OTP {get;set;}
}