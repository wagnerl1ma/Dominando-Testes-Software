using Xunit;

namespace Demo.Tests
{
    public class CalculadoraTests
    {

        //Padr�o de Nomenclatura para teste: NomeDaClasse_M�todo_OqueEsperarDoMetodo
        //Ex: Calculadora_Somar_RetornarValorSoma()
        [Fact]
        public void Calculadora_Somar_RetornarValorSoma()
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(2, 2);

            // Assert
            Assert.Equal(4, resultado);
        }


        //Teste Teorico
        [Theory]
        [InlineData(1, 1, 2)] // no "InlineData" � necess�rio passar os mesmo parametros do m�todo e ser�o inclu�dos valor no parametro p/ testar o m�todo
        [InlineData(2, 2, 4)]
        [InlineData(4, 2, 6)]
        [InlineData(7, 3, 10)]
        [InlineData(6, 6, 12)]
        [InlineData(9, 9, 18)]
        public void Calculadora_Somar_RetornarValoresSomaCorretos(double v1, double v2, double total)
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(v1, v2);

            // Assert
            Assert.Equal(total, resultado);
        }
    }
}
