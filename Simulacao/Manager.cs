using BaseGrafo;
using BaseSimulacao.AuxLogs;
using BaseSimulacao.Entidades;
using BaseSimulacao.Entidades.Leitura;
using BaseSimulacao.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
                Rua ruaAdicionar = new Rua()
                {
                    Comprimento = aresta.Peso,
                    NumeroFaixas = item.NumeroVias,
                    IdAresta = aresta.Id,
                    VelocidadeMaxima = item.VelocidadeMaxima,
                    Id = auxId++,
                    Descricao = $"Rua sentido {aresta.Origem} até {aresta.Destino}",
                };
                ruaAdicionar.PreparaRua();
                RuasSimulacao.Add(ruaAdicionar);
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
                throw new Exception("Quantidade de taxas de geração está inclopeta");
            #endregion ProcessaEntrada
        }

        public void IniciaSimulacao(string logVeiculos)
        {
            SegundoSimulacao = 0;
            IdVeiculo = 0;
            if (!VerificaCarregamentoDados())
                throw new Exception("Carregue os dados da simulacao");
            inicializaFilaEsperaVerice();
            while (SegundoSimulacao < QtdIteracoes)
            {
                GeradoraVeiculos(); // gera veiculos
                ProcessaVeiculoSimulacao();//Faz Entrada dos veículos nas ruas
                ProcessaVeiculosVias(logVeiculos);//Desloca veiculos nas ruas
                ProcessaSemaforos(); //Atualiza estatus dos semaforos
                TrocaVeiculosRua(SegundoSimulacao); //troca veiculos de rua
                Thread.Sleep(TempoDelayRotinas);
                SegundoSimulacao++;
            }
        }
        
        public Grafo GetGrafoSimulacao()
        {
            return grafo;
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
            if (ImprimirLogTela)
                Console.WriteLine("Iniciando rotina de geração de veículos");
            int n = grafo.NumeroVertices;
            for (int i = 0; i < n; i++)
            {
                if (RoletaSorteio.ExecutaRoleta(TaxaGeracao[i]))
                {
                    Veiculo veiculoAdicionar = geradorVeiculos.GeraVeiculoAleatorio(IdVeiculo, grafo, i);
                    veiculoAdicionar.LogVeiculo = new LogVeiculo()
                    {
                        IdVeiculo = veiculoAdicionar.Id,
                        InstanteCriacao = SegundoSimulacao,
                        VerticeOrigem = i,
                        VerticeDestino = veiculoAdicionar.PercursoVeiculo.Last()
                    };
                    VeiculosEsperaVertice[i].Enqueue(veiculoAdicionar);
                    if (ImprimirLogTela)
                        Console.WriteLine($"Realizado inserção de veículo no vértice {i}.");

                    #region TratativaLogs
                    LogGeracaoVeiculos.Add(new LogGeracaoVeiculo()
                    {
                        VerticeIncersao = i,
                        IdVeiculo = IdVeiculo,
                        SegundoSimulacao = SegundoSimulacao
                    });
                    LogTrajetos.Add(new LogTrajetosVeiculos {
                        IdVeiculo = IdVeiculo,
                        PercursoVeiculo = veiculoAdicionar.PercursoVeiculo
                    });
                    #endregion TratativaLogs
                    IdVeiculo++;
                }
            }
            #region TratativaLogs
            for(int i = 0; i < n; i++)
            {
                LogQtdVeiculosEsperaTempo.Add(
                    new LogQtdVeiculosEsperaVertice()
                    {
                        InstanteTempo = SegundoSimulacao,
                        QtdVeiculos = VeiculosEsperaVertice[i].Count
                    });
            }
            #endregion TrativaLogs
        }

        private void ProcessaVeiculoSimulacao()
        {
            if (ImprimirLogTela)
                Console.WriteLine("Iniciando rotina de Geração de veículos");
            foreach (var rua in RuasSimulacao)
            {
                Aresta ArestaCorrespondente = grafo.GetAresta(rua.IdAresta);
                Vertice VerticeOrigem = grafo.GetVertice(ArestaCorrespondente.Origem);
                if(VeiculosEsperaVertice[ArestaCorrespondente.Origem].Count > 0)
                {
                    if (rua.AdicionaVeiculo(VeiculosEsperaVertice[ArestaCorrespondente.Origem].Peek(), SegundoSimulacao))
                    {
                        if (ImprimirLogTela)
                            Console.WriteLine($"O veículo {VeiculosEsperaVertice[ArestaCorrespondente.Origem].Peek().Id} entrou na rua {rua.Id}");
                        VeiculosEsperaVertice[ArestaCorrespondente.Origem].Dequeue();
                    }
                }
                #region TrativaLogs
                LogOcupacaoVias.Add(new LogOcupacaoVias() {
                    IdAresta = rua.IdAresta,
                    EspacoOcupado = (int)rua.MediaOcupacaoVias(),
                    InstanteTempo = SegundoSimulacao
                });
                #endregion TrativaLogs
            }
        }

        private void ProcessaVeiculosVias(string folderLogsVeiculo)
        {
            foreach(Rua rua in RuasSimulacao)
            {
                rua.PocessaFilaVeiculos(SegundoSimulacao, folderLogsVeiculo, Semaforos, MargemErroViaLotada);
            }
        }

        private void ProcessaSemaforos()
        {
            foreach (var at in Semaforos)
            {
                at.TempoAtual++;
                if (at.EstadoSemaforo == Entidades.Enuns.EstadosSemaforo.ABERTO && at.TempoAtual >= at.TempoAberto)
                {
                    at.TempoAtual = 0;
                    at.EstadoSemaforo = Entidades.Enuns.EstadosSemaforo.AMARELO;
                }
                else
                {
                    if (at.EstadoSemaforo == Entidades.Enuns.EstadosSemaforo.AMARELO && at.TempoAtual >= at.TempoAmarelo)
                    {
                        at.TempoAtual = 0;
                        at.EstadoSemaforo = Entidades.Enuns.EstadosSemaforo.FECHADO;
                    }
                    else
                    {
                        if(at.EstadoSemaforo == Entidades.Enuns.EstadosSemaforo.FECHADO && at.TempoAtual >= at.TempoFechado)
                        {
                            at.TempoAtual = 0;
                            at.EstadoSemaforo = Entidades.Enuns.EstadosSemaforo.ABERTO;
                        }
                    }
                }
            }
        }

        private void  TrocaVeiculosRua(int instanteSimulacao)
        {
            foreach(var rua in RuasSimulacao)
            {
                var sema = Semaforos.Where(x=> x.RuasOrigem.Contains(rua.Id)).FirstOrDefault();
                bool temSem = sema != null;
                for(int i = 0; i<rua.NumeroFaixas; i++)
                {
                    var veiculos = rua.VeiculosNaRua[i].ToList();
                    foreach(var veiculo in veiculos)
                    {
                        if((rua.EspacoOcupado[i] + MargemErroViaLotada) >= rua.Comprimento)
                        {
                            bool removeVeiculo = true;
                            if(temSem && sema.EstadoSemaforo != Entidades.Enuns.EstadosSemaforo.ABERTO)
                            {
                                removeVeiculo = false;
                            }

                            if (removeVeiculo)
                            {
                                var arestaRuaAt = grafo.ObtenhaAresta(rua.IdAresta);
                                int verticeOrigemProximaRua = arestaRuaAt.Destino;
                                veiculo.VerticeAtual = veiculo.ProximoDestinoVeiculo();
                                int verticeDestinoProximaRua = veiculo.ProximoDestinoVeiculo();
                                var procimaAresta = grafo.ObtenhaAresta(verticeOrigemProximaRua, verticeDestinoProximaRua);
                                if(procimaAresta is object)
                                {
                                    var prua = RuasSimulacao.Where(x => x.IdAresta == procimaAresta.Id).FirstOrDefault();
                                    if (prua is object)
                                    {
                                        if (prua.AdicionaVeiculo(veiculo, instanteSimulacao))
                                        {
                                            rua.RemoveVeiculo();
                                        }
                                    }
                                }
                            }
                        }
                    }
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
        private int SegundoSimulacao = 0;
        private int IdVeiculo = 0;
        public int TempoDelayRotinas { get; set; }
        public int MargemErroViaLotada { get; set; } = 2;
        public int QtdIteracoes { get; set; }
        #endregion Propriedades

        #region PropriedadesLogs
        public List<LogGeracaoVeiculo> LogGeracaoVeiculos { get; set; } = new List<LogGeracaoVeiculo>();
        public List<LogQtdVeiculosEsperaVertice> LogQtdVeiculosEsperaTempo { get; set; } = new List<LogQtdVeiculosEsperaVertice>();
        public List<LogOcupacaoVias> LogOcupacaoVias { get; set; } = new List<LogOcupacaoVias>();
        public List<LogTrajetosVeiculos> LogTrajetos { get; set; } = new List<LogTrajetosVeiculos>();
        #endregion PropriedadesLogs

        #region SalvarLogs
        public void SalvarLogsGeracaoVeiculos(string caminhoVeiculosVertice, string CaminhoVeiculoPorTempo)
        {
            List<string> LogSalvar = new List<string>();
            for(int i = 0; i < grafo.NumeroVertices; i++)
            {
                LogSalvar.Add($"{i};{LogGeracaoVeiculos.Where((x)=>x.VerticeIncersao == i).Sum((x)=>1)}");
            }
            using (StreamWriter file = new StreamWriter(caminhoVeiculosVertice))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
            LogSalvar.Clear();
            foreach(var item in LogGeracaoVeiculos)
            {
                LogSalvar.Add($"{item.SegundoSimulacao};{LogGeracaoVeiculos.Where((x) => x.SegundoSimulacao == item.SegundoSimulacao).Sum((x) => 1)}");
            }
            using (StreamWriter file = new StreamWriter(CaminhoVeiculoPorTempo))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
        }
        public void SalvarLogVeiculosEspera(string caminhoVeiculosEsperaTempo)
        {
            List<string> LogSalvar = new List<string>();
            foreach(var item in LogQtdVeiculosEsperaTempo)
            {
                LogSalvar.Add($"{item.InstanteTempo};{LogQtdVeiculosEsperaTempo.Where((x)=>x.InstanteTempo == item.InstanteTempo).Sum((x)=>1)}");
            }
            using (StreamWriter file = new StreamWriter(caminhoVeiculosEsperaTempo))
            {
                if (file == null)
                    throw new Exception("arquivo não encontrao");
                file.Write(string.Join("\n", LogSalvar));
                file.Close();
            }
        }
        public void SalvarLogEspacoOcupadoVias(string caminhoPastaVia)
        {
            for (int i = 0; i<grafo.NumeroArestas; i++)
            {
                List<string> LogSalvar = new List<string>();
                List<LogOcupacaoVias> salvarAresta = LogOcupacaoVias.Where((x=>x.IdAresta == i)).ToList();
                foreach (var item in salvarAresta)
                {
                    LogSalvar.Add($"{item.InstanteTempo};{item.EspacoOcupado}");
                }
                using (StreamWriter file = new StreamWriter($"{caminhoPastaVia}/{i}.csv"))
                {
                    file.Write(string.Join("\n", LogSalvar));
                    file.Close();
                }
            }
            
        }
        #endregion SalvarLogs
    }
}
