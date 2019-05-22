using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseGrafo
{
    public class Vertice
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public int GraoVertice
        {
            get
            {
                return VerticesAdjacentes.Count;
            }
        }

        public int NumeroAdjacentes
        {
            get
            {
                return GraoVertice;
            }
        }

        public int NumeroVerticeAdjacentes
        {
            get
            {
                return VerticesAdjacentes.Count;
            }
        }

        public List<int> Adjacentes
        {
            get
            {
                return VerticesAdjacentes.Select(x => x).ToList();
            }
        }

        public void AdicionaVerticeAdjacente(int IdVertice)
        {
            if (IdVertice < 0)
                throw new Exception("Id de vertice deve ser positivo");
            VerticesAdjacentes.Add(IdVertice);
        }

        #region PrivateItens
        private List<int> VerticesAdjacentes { get; set; } = new List<int>();
        #endregion PrivateItens
    }
}
