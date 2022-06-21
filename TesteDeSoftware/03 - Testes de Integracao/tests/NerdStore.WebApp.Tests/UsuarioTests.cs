using Features.Tests;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public UsuarioTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Realizar cadastro com sucesso"), TestPriority(1)] //executa na ordem 1 dos testes
        [Trait("Categoria", "Integra��o Web - Usu�rio")]
        public async Task Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Arrange
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            initialResponse.EnsureSuccessStatusCode(); // verifica se o status code � 200

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            _testsFixture.GerarUserSenha();

            //capturando os input da pagina "/Identity/Account/Register"
            var formData = new Dictionary<string, string>
            {
                { _testsFixture.AntiForgeryFieldName, antiForgeryToken },
                { "Input.Email", _testsFixture.UsuarioEmail },
                { "Input.Password", _testsFixture.UsuarioSenha },
                { "Input.ConfirmPassword", _testsFixture.UsuarioSenha }
            };

            //Criando um postRequest da pagina "/Identity/Account/Register" e criando o conteudo "Content" para fazer o Post no m�todo "SendAsync"
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest); // fazendo o post na pagina

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync(); //transforma o conteudo "Content" HTML em string para fazer valida�oes
            postResponse.EnsureSuccessStatusCode(); // verifica se o status code � 200
            Assert.Contains($"Hello {_testsFixture.UsuarioEmail}!", responseString);
        }


        [Fact(DisplayName = "Realizar cadastro senha fraca"), TestPriority(3)] //executa na ordem 3 dos testes
        [Trait("Categoria", "Integra��o Web - Usu�rio")]
        public async Task Usuario_RealizarCadastroComSenhaFraca_DeveRetornarMensagemDeErro()
        {
            // Arrange
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            _testsFixture.GerarUserSenha();
            const string senhaFraca = "123456";

            var formData = new Dictionary<string, string>
            {
                { _testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", _testsFixture.UsuarioEmail },
                {"Input.Password", senhaFraca },
                {"Input.ConfirmPassword", senhaFraca }
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains("Passwords must have at least one non alphanumeric character.", responseString);
            Assert.Contains("Passwords must have at least one lowercase (&#x27;a&#x27;-&#x27;z&#x27;).", responseString);
            Assert.Contains("Passwords must have at least one uppercase (&#x27;A&#x27;-&#x27;Z&#x27;).", responseString);
        }


        [Fact(DisplayName = "Realizar login com sucesso"), TestPriority(2)] //executa na ordem 2 dos testes
        [Trait("Categoria", "Integra��o Web - Usu�rio")]
        public async Task Usuario_RealizarLogin_DeveExecutarComSucesso()
        {
            // Arrange
            var initialResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Login");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {_testsFixture.AntiForgeryFieldName, antiForgeryToken},
                {"Input.Email", _testsFixture.UsuarioEmail},
                {"Input.Password", _testsFixture.UsuarioSenha}
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {_testsFixture.UsuarioEmail}!", responseString);
        }
    }
}