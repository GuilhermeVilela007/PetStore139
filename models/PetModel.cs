using System.Formats.Asn1;

namespace Models;

public class PetModel
{
    public int id;
    public Category category;
    public String name;
    public String[] photoUrls;
    public Tag[] tags;
    public String status;


}

public class Category 
{
    public int id;
    public String name;

  
    public Category(int id, String name) // Construtor - conecta o funcinamento interno da classe com o externo
    {
        this.id = id;
        this.name = name;

    }
}

public class Tag
{
    public int id;
    public String name;

     public Tag(int id, String name) // Construtor - conecta o funcinamento interno da classe com o externo
    {
        this.id = id;
        this.name = name;

    }
}