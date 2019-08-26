using BaseSimulacao;
using System;

namespace TCC
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager simulacao = new Manager(){
                ImprimirLogTela = true,
                TempoDelayRotinas = 0,
                QtdIteracoes = 600,
                MargemErroViaLotada = 6
            };
            simulacao.CarregaMapaSimulacao("C:/entrada/mapa.json");
            simulacao.IniciaSimulacao("C:/LogsSimulacao/Veiculos");
            //simulacao.SalvarLogsGeracaoVeiculos("C:/entrada/Logs/VeiculosPorVertice.csv", "C:/entrada/Logs/VeiculosPorTempo.csv");
            //simulacao.SalvarLogVeiculosEspera("C:/entrada/Logs/VeiculosEsperaPorTempo.csv");
            simulacao.SalvarLogEspacoOcupadoVias("C:/LogsSimulacao/Ruas");
            Console.ReadKey();
        }
    }
}