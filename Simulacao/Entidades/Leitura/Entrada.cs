using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseSimulacao.Entidades.Leitura
{
    public class Entrada
    {
        public List<LRua> Ruas { get; set; } = new List<LRua>();
        public List<LSemaforo> Semaforos { get; set; } = new List<LSemaforo>();
        public List<LTaxaGeracao> TaxasGeracao { get; set; } = new List<LTaxaGeracao>();
        public List<int> ComprimentosVeiculos { get; set; } = new List<int>();
        public List<int> VelocidadeInicial { get; set; } = new List<int>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}