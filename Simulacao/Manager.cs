using BaseGrafo;
using BaseSimulacao.Entidades;
using BaseSimulacao.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BaseSimulacao.Entidades.Leitura;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BaseSimulacao
{
    public class Manager
    {
        #region Metodos
        public void CarregaMapaSimulacao(string CaminhoArquivoSimulacao)
        {
            Entrada DadosEntrada = new Entrada(); ;
            using (StreamReader file = new StreamReader(CaminhoArquivoSimulacao))
            {
                if (file == null)
                    throw new Exception($"Arquivo {CaminhoArquivoSimulacao} não foi encontrado!");
                string conteudoArquivo = file.ReadToEnd();
                DadosEntrada = JsonConvert.DeserializeObject<Entrada>(conteudoArquivo);
                file.Close();
            }
            if (DadosEntrada == null)
                throw new Exception("Não foi possível realizar a serialização do arquivo de entrada de dados");
            #region ProcessaEntrada
            int auxId = 0;

            /// grafo e ruas
            foreach (var item in DadosEntrada.Ruas)
            {
                grafo.AdicionaAresta(item.VerticeOrigem, item.VerticeDestino, item.Distancia, auxId);
                Aresta aresta = grafo.ObtenhaAresta(item.VerticeOrigem, item.VerticeDestino);
                RuasSimulacao.Add(new Rua() {
                    Comprimento = aresta.Peso,
                    NumeroFaixas = item.NumeroVias,
                    IdAresta = aresta.Id,
                    VelocidadeMaxima = item.VelocidadeMaxima,
                    Id = auxId++,
                    Descricao = $"Rua sentido {aresta.Origem} até {aresta.Destino}",
                });
            }

            /// comprimento veiculos
            foreach (var item in DadosEntrada.ComprimentosVeiculos)
            {
                if (item <= 0)
                    throw new Exception("Um comprimento não pode ser menor ou igual a zero");
                geradorVeiculos.AdicionarComprimentoPossivel(item);
            }

            /// velocidade inicial
            foreach (var item in DadosEntrada.VelocidadeInicial)
            {
                if (item <= 0)
                    throw new Exception("A velocidade inicial não pode ser menor ou igual a zero");
                geradorVeiculos.AdicionarVelocidadePossivel(item);
            }

            /// Semaforos
            auxId = 0;
            foreach (var item in DadosEntrada.Semaforos)
            {
                Semaforo auxSema = new Semaforo()
                {
                    Id = auxId++,
                    TempoAberto = item.TempoAberto,
                    TempoAmarelo = item.TempoAmarelo,
                    TempoFechado = item.TempoFechado,
                    EstadoSemaforo = Entidades.Enuns.EstadosSemaforo.ABERTO,
                    TempoAtual = 0
                };
                Rua RuaOrigem = GetRua(item.VerticeOrigemOrigem, item.VerticeDestinoOrigem);
                Rua RuaDestino = GetRua(item.VerticeOrigemDestino, item.VerticeDestinoDestino);
                if (RuaOrigem == null || RuaDestino == null)
                    throw new Exception("Rua de Origem/Destino não foi encontrada");
                auxSema.RuasOrigem.Add(RuaOrigem.Id);
                auxSema.RuasDestino.Add(RuaDestino.Id);
                Semaforos.Add(auxSema);
            }
            // taxa de geracao veiculos
            TaxaGeracao.AddRange(DadosEntrada.TaxasGeracao.OrderBy ((x)=>x.Vertice).Select((x)=>x.Taxa));
            if(TaxaGeracao.Count != grafo.NumeroVertices)
                throw new Exception("Quantidade de taxas de geração inclopeto");
            #endregion ProcessaEntrada
        }

        public void IniciaSimulacao()
        {
            if (!VerificaCarregamentoDados())
                throw new Exception("Carregue os dados da simulacao");
            inicializaFilaEsperaVerice();

            Task.Run(() => {
                GeradoraVeiculos();
            });

        }
        public Grafo GetGrafoSimulacao()
        {
            return grafo;
        }
        public void pararLoop()
        {
            ExecutaLoop = false;
        }
        public Rua GetRua(int origem, int destino)
        {
            return RuasSimulacao.Where((x) => x.IdAresta == grafo.ObtenhaAresta(origem, destino).Id).FirstOrDefault();
        }
        public bool ImprimirLogTela { get; set; }
        #endregion Metodos

        #region MetodosPrivados
        private void GeradoraVeiculos()
        {
            int n = grafo.NumeroVertices;
            while(true){
                for(int i = 0; i < n; i++){
                    //if()
                }
            }
        }
        private bool VerificaCarregamentoDados()
        {
            if (grafo == null || grafo.NumeroVertices == 0 || grafo.NumeroArestas == 0) return false;
            if (RuasSimulacao == null || RuasSimulacao.Count == 0) return false;
            return true;
        }
        private Veiculo GetVeiculo(int id)
        {
            return VeiculosSimulacao.Where((x) => x.Id == id).FirstOrDefault();
        }
        public void inicializaFilaEsperaVerice()
        {
            if(grafo.NumeroVertices > 0)
            {
                VeiculosEsperaVertice.Clear();
                for(int i = 0; i< grafo.NumeroVertices; i++)
                    VeiculosEsperaVertice.Add(new Queue<Veiculo>());
            }
        }
        #endregion MetodosPrivados

        #region Propriedades
        private List<Veiculo> VeiculosSimulacao { get; set; } = new List<Veiculo>();
        private List<Rua> RuasSimulacao { get; set; } = new List<Rua>();
        private List<Semaforo> Semaforos { get; set; } = new List<Semaforo>();
        private List<Queue<Veiculo>> VeiculosEsperaVertice { get; set; } = new List<Queue<Veiculo>>();
        private Grafo grafo = new Grafo();
        private List<int> TaxaGeracao = new List<int>();
        private GeradorVeiculos geradorVeiculos = new GeradorVeiculos();
        private int DiaSemana, SegundoSimulacao, IdVeiculo;
        private bool ExecutaLoop;
        #endregion Propriedades
    }
}
