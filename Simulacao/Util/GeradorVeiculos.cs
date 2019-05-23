using BaseGrafo;
using BaseGrafo.Algoritmos;
using BaseSimulacao.Entidades;
using System;
using System.Collections.Generic;

namespace BaseSimulacao.Util
{
    public class GeradorVeiculos
    {
        #region Propriedades
        public List<int> ComprimentosPossiveis { get; set; } = new List<int>();
        public List<int> VelocidadesPossiveis { get; set; } = new List<int>();
        #endregion Propriedades

        #region Metodos
        public void AdicionarComprimentoPossivel(int comprimento)
        {
            if (comprimento <= 0)
                throw new Exception("O comprimento deo veículo não pode ser menor ou igual a zero");
            ComprimentosPossiveis.Add(comprimento);
        }
        public void AdicionarVelocidadePossivel(int velocidade)
        {
            if (velocidade <= 0)
                throw new Exception("A velocidade deve ser maior que zero");
            VelocidadesPossiveis.Add(velocidade);
        }
        public Veiculo GeraVeiculoAleatorio(int Id, Grafo grafo, int VerticeOrigem)
        {
            if (ComprimentosPossiveis.Count <= 0 || VelocidadesPossiveis.Count <= 0)
                throw new Exception("Sem comprimentos ou velocidades possiveis para selecao");
            int[] caminho = Dijkstra.CalculaDjkstra(VerticeOrigem, grafo);
            Veiculo Retorno = new Veiculo()
            {
                Comprimento = ComprimentosPossiveis[new Random().Next()%ComprimentosPossiveis.Count],
                Velocidade = VelocidadesPossiveis[new Random().Next()%VelocidadesPossiveis.Count],
                Id = Id,
                PosicaoAtual = VerticeOrigem,
                PercursoVeiculo = GeraPercurso(caminho, EscolheDestino(grafo.NumeroVertices, VerticeOrigem))
            };
            return Retorno;
        }
        #endregion Metodos

        #region Privados
        private int EscolheDestino(int n, int origem)
        {
            int retorno = new Random().Next() % n;
            return retorno == origem? EscolheDestino(n, origem): retorno;
        }
        private List<int> GeraPercurso(int [] DijkstraRet, int Destino)
        {
            List<int> retorno = new List<int>();
            int at = Destino;
            while(at != DijkstraRet[at])
            {
                retorno.Add(at);
                at = DijkstraRet[at];
            }
            retorno.Add(at);
            retorno.Reverse();
            return retorno;
        }
        #endregion Privados
    }
}
