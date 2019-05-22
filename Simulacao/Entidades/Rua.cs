using System;
using System.Collections.Generic;
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
        bool AdicionaVeiculo(Veiculo novoVeiculo)
        {
            VerificaInicializacao();
            if (novoVeiculo == null)
                throw new Exception("Veiculo não foi setado");
            for(int i = 0; i<NumeroFaixas; i++)
            {
                if(Comprimento >= (EspacoOcupado[i] + novoVeiculo.Comprimento))
                {
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
                    return VeiculosNaRua[i].Dequeue();
            }
            return null;
        }
        #region Metodos

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
                    if(EspacoOcupado.Count != 0)
                    {
                        EspacoOcupado.Clear();
                    }
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
