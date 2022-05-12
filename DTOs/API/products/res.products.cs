public class responseProducts{
    public string? Id {get;set;}

    public string? name {get;set;}

    public string? description {get;set;}

    public int qty {get;set;}

    public decimal price {get;set;}

    public responseCategory category {get;set;} = null!;

    public responseUser seller {get;set;} = null!;

    public string? Added {get;set;}
    public string? Status {get;set;}
}

public class responseProductsGenericId{
    public string? Id {get;set;}

}