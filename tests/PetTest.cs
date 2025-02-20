// 1 - Bibliotecas
using System.ComponentModel;
using Newtonsoft.Json; //dependencia para o JsonConvert
using RestSharp;

// 2 - NameSpace
namespace Pet;

// 3 - Classe
public class PetTest
{
   
   // 3.1 - Atributos
    private const string BASE_URL = "https://petstore.swagger.io/v2/";

   // 3.2 - Funções e Métodos
   [Test, Order(1)]
   public void PostPetTest()
   {
      //configura
      var client = new RestClient(BASE_URL); // instancia o objeto do tipo RestClient com o endereço da API
      var request = new RestRequest("pet", Method.Post); // intancia o objeto do tipo RestRequest com o complemento de endereço com o "pet" configurando o metodo para ser um POST
      string jsonBody = File.ReadAllText(@"C:\iterasys\PetStore139\fixtures\pet1.json");// armazena o conteudo do arquivo pet.json na memória

      //executa
      request.AddBody(jsonBody); // adiciona na requisição o conteúdo do arquivo pet1.json
      var response = client.Execute(request); // é o que executa a requisição

      //valida
      var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content); // formatar e quebrar linhas do Json

      Console.WriteLine(responseBody);
     

      //Valide que na resposta, o status code é igual ao resultado esperado(200)
      Assert.That((int)response.StatusCode, Is.EqualTo(200)); //Valida se o retorno vai ser positivo(nesse caso 200)
      
      //valida o nome do animal
      String name = responseBody.name.ToString();
      Assert.That(name, Is.EqualTo("Marci"));
      //ou
      //Assert.That(response.Body.name.ToString(), Is.EqualTo("marci"));
   
      //valida o STTS do animal na resposta
      String status = responseBody.status.ToString();
      Assert.That(status, Is.EqualTo("meu amor"));

   }

   [Test, Order(2)]

   public void GetPetTest()
   {
      //Configura
      int petId = 1647765; //campo de pesquisa
      String petName = "Marci"; // resultado esperado
      String categoryName = "gui";
      String tagsName = "doida";

      var cliente = new RestClient(BASE_URL); // base url da API
      var request = new RestRequest($"pet/{petId}", Method.Get); // base da URL + o andpoint

      //Executa
      var response = cliente.Execute(request);

      //Valida
      var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content); // formatar e quebrar linhas do Json
      Console.WriteLine(responseBody);

      Assert.That((int)response.StatusCode, Is.EqualTo(200)); //Valida se o retorno vai ser positivo(nesse caso 200)
      Assert.That((int)responseBody.id, Is.EqualTo(petId));
      Assert.That((String)responseBody.name, Is.EqualTo(petName));
      Assert.That((String)responseBody.category.name, Is.EqualTo(categoryName)); 
      Assert.That((String)responseBody.tags[0].name, Is.EqualTo(tagsName)); // valida quando tem mais de uma tag

   }

}

