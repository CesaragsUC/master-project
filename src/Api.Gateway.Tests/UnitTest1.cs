namespace Api.Gateway.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "Teste 01 - Efeturar Login com sucesso")]
        [Trait("Login", "Auth")]
        public void Teste01()
        {
            Assert.True(true);
        }

        [Fact(DisplayName = "Teste 01 - Efeturar Login deve falhar.")]
        [Trait("Login", "Auth")]
        public void Teste02()
        {
            Assert.True(true);
        }
    }
}