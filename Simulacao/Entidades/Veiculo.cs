using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseSimulacao.Entidades
{
    public class Veiculo : BaseEntidade
    {
        #region Propiedades
        public int Velocidade { get; set; }
        public int PosicaoAtual { get; set; }
        public int Comprimento { get; set; }
        public List<int> PercursoVeiculo { get; set; } = new List<int>();
        #endregion Propiedades

        #region Construtores
        public Veiculo(int comprimento = 3, int velocidade = 2)
        {
            InicializaVeiculo(comprimento, velocidade);
        }
        public Veiculo(int id, int comprimento, int velocidade)
        {
            InicializaVeiculo(comprimento, velocidade, id);
        }
        #endregion Construtores

        #region Metodos
        public void InicializaVeiculo(int comprimento, int velocidade, int id = -1)
        {
            Comprimento = comprimento;
            Velocidade = velocidade;
            id = Id;
        }
        public List<int> GetAllPercursoVeiuculo()
        {
            return PercursoVeiculo.Select(x => x).ToList();
        }
        public void ResetaPercursoVeiculo()
        {
            PercursoVeiculo.Clear();
        }
        public void AdicionaPontoVisitaVeiculo(int PontoVisita)
        {
            if (PontoVisita < 0)
                throw new Exception("Id do vértice é inválido");
            PercursoVeiculo.Add(PontoVisita);
        }
        public int GetVerticeAtIndex(int Index)
        {
            if (Index <= 0 || Index >= PercursoVeiculo.Count)
                throw new Exception("Index está fora do tamanho do vetor de caminhos");
            return PercursoVeiculo[Index];
        }
        public void inicializaPercuro()
        {
            if (PercursoVeiculo.Count <= 0)
                throw new Exception("Caminho do veiculo ainda não foi definido");
            PosicaoAtual = PercursoVeiculo[0];
            if (Velocidade < 0)//veiculo estiver freiando, pare o mesmo
                Velocidade = 0;
        }
        public int ProximoDestinoVeiculo()
        {
            if (PercursoVeiculo.Count <= 0)
                throw new Exception("Veículo ainda não possui rota definida");
            if (PosicaoAtual == PercursoVeiculo[PercursoVeiculo.Count - 1])
                return -1;// veiculo ja chegou no destino
            for (int i = 0; i < PercursoVeiculo.Count; i++)
                if (PercursoVeiculo[i] == PosicaoAtual)
                    return PercursoVeiculo[i + 1];
            return -1;
        }
        #endregion Metodos
    }
}
