using System;
using BaseSimulacao;

namespace TCC
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager simulacao = new Manager(){
                ImprimirLogTela = true
            };
            simulacao.CarregaMapaSimulacao("C:/entrada/simulacao.json");
            simulacao.IniciaSimulacao();
        }
    }
}