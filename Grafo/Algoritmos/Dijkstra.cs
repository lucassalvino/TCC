using System;
using System.Collections.Generic;
using System.Text;

namespace BaseGrafo.Algoritmos
{
    public class Dijkstra
    {
        public int[] CalculaDjkstra(int VerticeOrigem, Grafo Grafo)
        {
            if (Grafo == null || Grafo.NumeroVertices == 0)
                throw new Exception("Grafo nao foi definido");
            int[] retorno = new int[Grafo.NumeroVertices];
            foreach(var item in retorno)
            return retorno;
        }
    }
}
