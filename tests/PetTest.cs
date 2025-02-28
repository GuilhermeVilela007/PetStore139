// 1 - Bibliotecas
using System.ComponentModel;
using Models;
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

   // Função de leitura de dados a partir de um arquivo csv
        public static IEnumerable<TestCaseData> getTestData()
    {
        String caminhoMassa = @"C:\iterasys\PetStore139\fixtures\pets.csv";

        using var reader = new StreamReader(caminhoMassa);

        // Pula a primeira linha com os cabeçalhos
        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(",");

            yield return new TestCaseData(int.Parse(values[0]), int.Parse(values[1]), values[2], values[3], values[4], values[5], values[6], values[7]);
        }

    }


   
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
      // Valida o PetId
      int petId = responseBody.id;
      Assert.That(petId, Is.EqualTo(1647765));

      //valida o nome do animal
      String name = responseBody.name.ToString();
      Assert.That(name, Is.EqualTo("Marci"));
      //ou
      //Assert.That(response.Body.name.ToString(), Is.EqualTo("marci"));
   
      //valida o STTS do animal na resposta
      String status = responseBody.status.ToString();
      Assert.That(status, Is.EqualTo("meu amor"));

      Environment.SetEnvironmentVariable("petId", petId.ToString());// Armazena os dados obtidos para usar nos proximos testes

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

   [Test, Order(3)]
   public void PutPetTest()
   {
      // Configura
      // Os dados de entrada vão formar o body da alteração
      PetModel petModel = new PetModel();
      petModel.id = 1647765;
      petModel.category = new Category(1, "MINHA");
      petModel.name = "Marci";
      petModel.photoUrls = new String[]{""};
      petModel.tags = new Tag[]{new Tag(1, "Só minhaaaaa")};
                            //,new Tag(1, "Só minhaaaaa")};
      petModel.status = "Minha Vida";

      // Transforma o modelo ACIMA em um arquivo Json
      String jsonBody = JsonConvert.SerializeObject(petModel, Formatting.Indented); // nesse caso vai MONTAR um JSON
      Console.WriteLine(jsonBody);

      var client = new RestClient(BASE_URL);
      var request = new RestRequest("pet", Method.Put);
      request.AddBody(jsonBody);

      // Executa
      var response = client.Execute(request);
      
      // VAlida
      var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
      Console.WriteLine(responseBody);

      Assert.That((int)response.StatusCode, Is.EqualTo(200));
      Assert.That((int)responseBody.id, Is.EqualTo(1647765));
      Assert.That((String)responseBody.tags[0].name, Is.EqualTo(petModel.tags[0].name));
      Assert.That((String)responseBody.status, Is.EqualTo(petModel.status));

   }

   [Test, Order(4)]
   public void DeletePetTest()
   {
      //configura
      String petId = Environment.GetEnvironmentVariable("petId"); // muda o ID de string para numero novamente

      var client = new RestClient(BASE_URL); // base url da API
      var request = new RestRequest($"pet/{petId}", Method.Delete); // base da URL + o andpoint

      // Executa
      var response = client.Execute(request);

      // Valida
      var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
      Assert.That((int)response.StatusCode, Is.EqualTo(200));
      Assert.That((int)responseBody.code, Is.EqualTo(200));
      Assert.That((String)responseBody.message, Is.EqualTo(petId));

   }


   [TestCaseSource("getTestData", new object[]{}), Order(5)]
   public void PostPetDDTest(int petId,
                             int categoryId,
                             String categoryName,
                             String petName,
                             String photoUrls,
                             String tagsIds,
                             String tagsName,
                             String status)

   {
      //configura
      PetModel petModel = new PetModel();
      petModel.id = petId;
      petModel.category = new Category(categoryId, categoryName);
      petModel.name = petName;
      petModel.photoUrls = new String[]{photoUrls};

      //codigo para gerar as multiplas tags que o pet pode ter
      String[] tagsIdsList = tagsIds.Split(";"); //Ler
      String[] tagsNameList = tagsName.Split(";"); // Ler
      List<Tag> tagList = new List<Tag>(); // grava depois do for

      for (int i = 0; i < tagsIdsList.Length; i++)
      {
         int tagId = int.Parse(tagsIdsList[i]);
         String tagName = tagsNameList[i];

         Tag tag = new Tag(tagId, tagName);
         tagList.Add(tag);
      }

      petModel.tags = tagList.ToArray();

      petModel.status = status;

      //a estrutura de dados esta pronta, agora vamos serealizar
      String jsonBody = JsonConvert.SerializeObject(petModel, Formatting.Indented);
      Console.WriteLine(jsonBody);

      var client = new RestClient(BASE_URL); // instancia o objeto do tipo RestClient com o endereço da API
      var request = new RestRequest("pet", Method.Post); // intancia o objeto do tipo RestRequest com o complemento de endereço com o "pet" configurando o metodo para ser um POST

      //executa
      request.AddBody(jsonBody); // adiciona na requisição o conteúdo do arquivo pet1.json
      var response = client.Execute(request); // é o que executa a requisição

      //valida
      var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content); // formatar e quebrar linhas do Json

      Console.WriteLine(responseBody);
     

      //Valide que na resposta, o status code é igual ao resultado esperado(200)
      Assert.That((int)response.StatusCode, Is.EqualTo(200)); //Valida se o retorno vai ser positivo(nesse caso 200)

      // Valida o PetId     
      Assert.That(responseBody.id, Is.EqualTo(petId));

      //valida o nome do animal
      
      Assert.That(responseBody.name, Is.EqualTo(petName));
   
      //valida o STTS do animal na resposta
      Assert.That(responseBody.status, Is.EqualTo(status));

   }

}

