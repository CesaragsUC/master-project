using Auth.Api.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shared.Kernel.Models;
using System.Net;
using System.Text.Json;

namespace Auth.Tests
{
    public class KeycloakAuthServiceTests
    {
        private readonly Mock<IOptions<KeycloakSettings>> _keyCloakOptionsMock;

        public KeycloakAuthServiceTests()
        {
            _keyCloakOptionsMock = new Mock<IOptions<KeycloakSettings>>();
        }


        [Fact(DisplayName = "Teste 01 - Obter token com sucesso")]
        [Trait("Auth", "KeycloakAuthServiceTests")]
        public async Task GetToken_ShouldReturnSuccessResult_WhenTokenAndUserInfoAreValid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            // Mock da resposta de token
            var tokenResponse = new TokenResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0L2F1dGgvcmVhbG1zL3JlYWxtIiwic3ViIjoidXNlcl9pZCIsImF1ZCI6ImNsaWVudF9pZCIsImV4cCI6MTc0NjI3ODQwMCwiaWF0IjoxNzQ2MTkyMDAwLCJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJyb2xlcyI6WyJ1c2VyIiwiYWRtaW4iXX0.cJYKi9Vy1AiOqK9e7sDjkUOZGfDi_8ENML3o6sdON90",
                RefreshToken = "fake_refresh_token",
                ExpiresIn = 3600
            };

            // Mock da resposta de informações do usuário
            var userInfoResponse = new UserInfoResponse
            {
                Sub = "user_id",
                Email = email,
                Name = "Test User"
            };

            // Mock das configurações de Keycloak
            var keycloakSettings = new KeycloakSettings
            {
                AuthServerUrl = "http://localhost",
                Resource = "client_id",
                Credentials = new Credentials { Secret = "client_secret" },
                Realm = "realm"
            };

            _keyCloakOptionsMock.Setup(x => x.Value).Returns(keycloakSettings);

            // Mock do HttpMessageHandler
            var mockHandler = new Mock<HttpMessageHandler>();

            // Mock da chamada para obter o token
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains("/token")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
                });

            // Mock da chamada para obter as informações do usuário
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/userinfo")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(userInfoResponse))
                });

            // Injetar o HttpClient com o mock do handler
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(keycloakSettings.AuthServerUrl)
            };


            // Act
            var result = await new KeycloakAuthService(httpClient, _keyCloakOptionsMock.Object).GetToken(email, password);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(email, result.Data.Email);
        }

        [Fact(DisplayName = "Teste 02 - GetUser Info should fail")]
        [Trait("Auth", "KeycloakAuthServiceTests")]
        public async Task GetUserInfo_Should_Fail()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            // Mock da resposta de token
            var tokenResponse = new TokenResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0L2F1dGgvcmVhbG1zL3JlYWxtIiwic3ViIjoidXNlcl9pZCIsImF1ZCI6ImNsaWVudF9pZCIsImV4cCI6MTc0NjI3ODQwMCwiaWF0IjoxNzQ2MTkyMDAwLCJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJyb2xlcyI6WyJ1c2VyIiwiYWRtaW4iXX0.cJYKi9Vy1AiOqK9e7sDjkUOZGfDi_8ENML3o6sdON90",
                RefreshToken = "fake_refresh_token",
                ExpiresIn = 3600
            };

            // Mock da resposta de informações do usuário
            var userInfoResponse = new UserInfoResponse
            {
                Sub = "user_id",
                Email = email,
                Name = "Test User"
            };

            // Mock das configurações de Keycloak
            var keycloakSettings = new KeycloakSettings
            {
                AuthServerUrl = "http://localhost",
                Resource = "client_id",
                Credentials = new Credentials { Secret = "client_secret" },
                Realm = "realm"
            };

            _keyCloakOptionsMock.Setup(x => x.Value).Returns(keycloakSettings);

            // Mock do HttpMessageHandler
            var mockHandler = new Mock<HttpMessageHandler>();

            // Mock da chamada para obter o token
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains("/token")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
                });

            // Mock da chamada para obter as informações do usuário
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/userinfo")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Injetar o HttpClient com o mock do handler
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(keycloakSettings.AuthServerUrl)
            };


            // Act
            var result = await new KeycloakAuthService(httpClient, _keyCloakOptionsMock.Object).GetToken(email, password);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Messages!);
            Assert.Equal(result.Code, 400);
        }


        [Fact(DisplayName = "Teste 03 - Falha ao obter token")]
        [Trait("Auth", "KeycloakAuthServiceTests")]
        public async Task GetToken_ShouldReturnFailureResult_WhenTokenRequestFails()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            var keycloakSettings = new KeycloakSettings
            {
                AuthServerUrl = "http://localhost",
                Resource = "client_id",
                Credentials = new Credentials { Secret = "client_secret" },
                Realm = "realm"
            };

            _keyCloakOptionsMock.Setup(x => x.Value).Returns(keycloakSettings);

            var mockHandler = new Mock<HttpMessageHandler>();

            // Mock da falha na chamada para obter o token
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(keycloakSettings.AuthServerUrl)
            };

            var result = await new KeycloakAuthService(httpClient, _keyCloakOptionsMock.Object).GetToken(email, password);

            // Assert
            Assert.False(result.Succeeded);
        }



        [Fact(DisplayName = "Teste 04 - Realizar logout com sucesso")]
        [Trait("Auth", "KeycloakAuthServiceTests")]
        public async Task GetToken_ShouldLogout_Success()
        {
            // Arrange

            var tokenResponse = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0L2F1dGgvcmVhbG1zL3JlYWxtIiwic3ViIjoidXNlcl9pZCIsImF1ZCI6ImNsaWVudF9pZCIsImV4cCI6MTc0NjI3ODQwMCwiaWF0IjoxNzQ2MTkyMDAwLCJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJyb2xlcyI6WyJ1c2VyIiwiYWRtaW4iXX0.cJYKi9Vy1AiOqK9e7sDjkUOZGfDi_8ENML3o6sdON90";
         
            var keycloakSettings = new KeycloakSettings
            {
                AuthServerUrl = "http://localhost",
                Resource = "client_id",
                Credentials = new Credentials { Secret = "client_secret" },
                Realm = "realm"
            };

            _keyCloakOptionsMock.Setup(x => x.Value).Returns(keycloakSettings);

            var mockHandler = new Mock<HttpMessageHandler>();

            // Mock da falha na chamada para obter o token
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains("/logout")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(keycloakSettings.AuthServerUrl)
            };

            var result = await new KeycloakAuthService(httpClient, _keyCloakOptionsMock.Object).Logout(tokenResponse);

            // Assert
            Assert.True(result.Succeeded);
        }


        [Fact(DisplayName = "Teste 05 - Realizar logout com falha")]
        [Trait("Auth", "KeycloakAuthServiceTests")]
        public async Task GetToken_Should_Fail()
        {
            // Arrange

            var tokenResponse = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0L2F1dGgvcmVhbG1zL3JlYWxtIiwic3ViIjoidXNlcl9pZCIsImF1ZCI6ImNsaWVudF9pZCIsImV4cCI6MTc0NjI3ODQwMCwiaWF0IjoxNzQ2MTkyMDAwLCJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJyb2xlcyI6WyJ1c2VyIiwiYWRtaW4iXX0.cJYKi9Vy1AiOqK9e7sDjkUOZGfDi_8ENML3o6sdON90";

            var keycloakSettings = new KeycloakSettings
            {
                AuthServerUrl = "http://localhost",
                Resource = "client_id",
                Credentials = new Credentials { Secret = "client_secret" },
                Realm = "realm"
            };

            _keyCloakOptionsMock.Setup(x => x.Value).Returns(keycloakSettings);

            var mockHandler = new Mock<HttpMessageHandler>();

            // Mock da falha na chamada para obter o token
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains("/logout")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(keycloakSettings.AuthServerUrl)
            };

            var result = await new KeycloakAuthService(httpClient, _keyCloakOptionsMock.Object).Logout(tokenResponse);

            // Assert
            Assert.False(result.Succeeded);
        }

    }
}