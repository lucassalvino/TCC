using BaseSimulacao.AuxLogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaseSimulacao.Entidades
{
    public class Rua : BaseEntidade
    {
        #region Propriedades
        public int Comprimento { get; set; }
        public int NumeroFaixas { get; set; }
        public int IdAresta { get; set; }
        public int VelocidadeMaxima { get; set; }
        public List<int> EspacoOcupado { get; set; } = new List<int>();
        public List<Queue<Veiculo>> VeiculosNaRua { get; set; } = new List<Queue<Veiculo>>();
        #endregion Propriedades

        #region Construtores
        public Rua (int Id = -1)
        {
            NumeroFaixas = -1;
            Comprimento = -1;
            VelocidadeMaxima = -1;
            this.Id = Id;            
        }
        public Rua(int id, int numeroFaixas, int comprimento, int velocidadeMaxima, int idAresta)
        {
            Id = id;
            IdAresta = idAresta;
            NumeroFaixas = numeroFaixas;
            Comprimento = comprimento;
            VelocidadeMaxima = velocidadeMaxima;
            PreparaRua();
        }

        #endregion Construtores
        public bool AdicionaVeiculo(Veiculo novoVeiculo, int instanteTempo)
        {
            VerificaInicializacao();
            if (novoVeiculo == null)
                throw new Exception("Veiculo não foi setado");
            for(int i = 0; i<NumeroFaixas; i++)
            {
                if(Comprimento >= (EspacoOcupado[i] + novoVeiculo.Comprimento))
                {
                    novoVeiculo.LogVeiculo.VelocidadesTempo.Add(new LogVelocidadeVeiculo() {
                        Velociadade = 0,
                        InstanteTempo = instanteTempo
                    });
                    VeiculosNaRua[i].Enqueue(novoVeiculo);
                    EspacoOcupado[i] += novoVeiculo.Comprimento;
                    return true;
                }
            }
            return false;
        }
        public Veiculo RemoveVeiculo()
        {
            VerificaInicializacao();
            for(int i=0; i < NumeroFaixas; i++)
            {
                if (VeiculosNaRua[i].Count > 0)
                {
                    EspacoOcupado[i] -= VeiculosNaRua[i].Peek().Comprimento;
                    return VeiculosNaRua[i].Dequeue();
                }
            }
            return null;
        }
        #region Metodos
        public void PocessaFilaVeiculos(int SegundoSimalcao, string FolderLogVeiculos, List<Semaforo> Semaforos, int margemErroViaLotada = 2)
        {
            var semAt = Semaforos.Where(x => x.RuasOrigem.Contains(Id)).FirstOrDefault();
            bool Existesema = semAt != null;
            for (int i = 0; i < NumeroFaixas; i++)
            {
                Queue<Veiculo> novaFila = new Queue<Veiculo>();
                List<Veiculo> veiculos = VeiculosNaRua[i].ToList();

                foreach(var veiculo in veiculos)
                {
                    veiculo.PosicaoAtualNaVia += veiculo.Velocidade;
                    if (veiculo.PosicaoAtualNaVia <= Comprimento)
                    {
                        if(Existesema && semAt.EstadoSemaforo == Enuns.EstadosSemaforo.ABERTO)
                        {
                            veiculo.Velocidade += 1;
                        }
                        else
                        {
                            if(!Existesema)
                                veiculo.Velocidade += 1;
                        }

                        if (Existesema && semAt.EstadoSemaforo != Enuns.EstadosSemaforo.ABERTO)
                            veiculo.Velocidade -= 1;
                    }
                    else
                    {
                        veiculo.Velocidade -= 1;
                    }

                    veiculo.LogVeiculo.VelocidadesTempo.Add(new LogVelocidadeVeiculo()
                    {
                        InstanteTempo = SegundoSimalcao,
                        Velociadade = veiculo.Velocidade
                    });

                    if((EspacoOcupado[i] + margemErroViaLotada) >= Comprimento)
                    {
                        if (veiculo.VerticeAtual == veiculo.PercursoVeiculo.Last())
                        {
                            Veiculo car = RemoveVeiculo();
                            if (!string.IsNullOrEmpty(FolderLogVeiculos))
                            {
                                List<string> logs = new List<string>();
                                foreach (var item in car.LogVeiculo.VelocidadesTempo)
                                {
                                    logs.Add($"{item.InstanteTempo};{item.Velociadade}");
                                }
                                using (StreamWriter file = new StreamWriter($"{FolderLogVeiculos}/{car.Id}.csv"))
                                {
                                    file.Write(string.Join("\n", logs));
                                    file.Close();
                                }
                            }
                        }
                        else
                        {
                            novaFila.Enqueue(veiculo);
                        }
                    }
                    else
                    {
                        novaFila.Enqueue(veiculo);
                    }
                }
                VeiculosNaRua[i] = novaFila;
            }
        }
        public float MediaOcupacaoVias()
        {
            return EspacoOcupado.Sum((x) => x) / NumeroFaixas;
        }
        #endregion Metodos
        #region MetodosPrivados
        public void PreparaRua() {
            if (Comprimento <= 0)
                throw new Exception("Defina o comprimento da rua");
            if (VelocidadeMaxima <= 0)
                throw new Exception("Defina a velocidade máxima do tráfego");
            if (NumeroFaixas <= 0)
                throw new Exception("Defina a quantidade de faixas");
            if(VeiculosNaRua.Count == 0)
            {
                for(int i = 0; i< NumeroFaixas; i++)
                {
                    VeiculosNaRua.Add(new Queue<Veiculo>());
                    EspacoOcupado.Add(0);
                }
            }
        }
        private void VerificaInicializacao()
        {
            if (VeiculosNaRua.Count != NumeroFaixas)
                throw new Exception("Rua ainda não foi inicializada");
        }
        #endregion MetodosPrivados
    }
}
