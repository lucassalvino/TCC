using System;
using BaseSimulacao;

namespace TCC
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager simulacao = new Manager();
            simulacao.CarregaMapaSimulacao("C:/entrada/simulacao.json");
        }
    }
}