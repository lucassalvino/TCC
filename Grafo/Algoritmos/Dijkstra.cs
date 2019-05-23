using System;
using System.Collections.Generic;

namespace BaseGrafo.Algoritmos
{
    public class Dijkstra
    {
        public static int[] CalculaDjkstra(int VerticeOrigem, Grafo Grafo)
        {
            Queue<int> filaVisita = new Queue<int>();
            if (Grafo == null || Grafo.NumeroVertices == 0)
                throw new Exception("Grafo nao foi definido");
            int[] retorno = new int[Grafo.NumeroVertices];
            int[] peso = new int[Grafo.NumeroVertices];
            int[] visitas = new int[Grafo.NumeroVertices];
            for (int i = 0; i < retorno.Length; i++){ 
                retorno[i] = i;
                peso[i] = int.MaxValue;
                visitas[i] = 0;
            }

            filaVisita.Enqueue(VerticeOrigem);
            retorno[VerticeOrigem] = VerticeOrigem; // o Vertice de origem é a propria origem
            visitas[VerticeOrigem] = 1;
            peso[VerticeOrigem] = 0;

            while(filaVisita.Count > 0){
                Vertice verticeAtual = Grafo.GetVertice(filaVisita.Dequeue());
                List<int> adjacentes = verticeAtual.Adjacentes;
                foreach(var item in adjacentes){
                    Aresta arestaAt = Grafo.ObtenhaAresta(verticeAtual.Id, item);
                    if (visitas[item] != 1)
                        filaVisita.Enqueue(item);
                    visitas[item] = 1;//visitado
                    if((peso[verticeAtual.Id]+arestaAt.Peso) < peso[item]){
                        peso[item] = peso[verticeAtual.Id]+arestaAt.Peso;
                        retorno[item] = verticeAtual.Id;
                    }
                }
            }
            return retorno;
        }
    }
}
