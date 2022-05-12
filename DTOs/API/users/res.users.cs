namespace helloWebApp.DTOs;
public class responseUser{
    public string? id {get;set;}

    public string? email {get;set;}

    public string? firstname {get;set;}

    public string? lastname {get;set;}
    
    public DateTime birthdate {get;set;}

    public string? profileImage {get;set;}

    public decimal balance {get;set;}

    public string roles {get;set;} = null!;

    public bool emailConfirmed {get;set;}
    public bool phonenumberConfirmed {get;set;}

    public string? phonenumber {get;set;}
}