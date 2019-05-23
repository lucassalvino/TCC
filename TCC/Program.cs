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
                TempoDelayRotinas = 150,
                QtdIteracoes = 600
            };
            simulacao.CarregaMapaSimulacao("C:/entrada/simulacao.json");
            simulacao.IniciaSimulacao("C:/entrada/Logs/veiculos");
            simulacao.SalvarLogsGeracaoVeiculos("C:/entrada/Logs/VeiculosPorVertice.csv", "C:/entrada/Logs/VeiculosPorTempo.csv");
            simulacao.SalvarLogVeiculosEspera("C:/entrada/Logs/VeiculosEsperaPorTempo.csv");
            simulacao.SalvarLogEspacoOcupadoVias("C:/entrada/Logs/ruas");
            Console.ReadKey();
        }
    }
}