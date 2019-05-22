using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseGrafo
{
    public class Grafo
    {
        #region PropriedadesPrivadas
        private List<Aresta> Arestas { get; set; } = new List<Aresta>();
        private List<Vertice> Vertices { get; set; } = new List<Vertice>();
        #endregion PropriedadesPrivadas

        #region PropriedadesPublicas
        public int NumeroVertices
        {
            get
            {
                return Vertices.Count();
            }
        }
        public int NumeroArestas
        {
            get
            {
                return Arestas.Count();
            }
        }

        public Aresta GetArestaAtIndex(int index)
        {
            if (index < 0 || index >= NumeroArestas)
                throw new Exception("Index invalido");
            return Arestas[index];
        }

        public Vertice GetVerticeAtIndex(int index)
        {
            if (index < 0 || index >= NumeroVertices)
                throw new Exception("Index invalido");
            return Vertices[index];
        }
        public Vertice GetVertice(int IdVertice){
            return Vertices.Where((x)=> x.Id == IdVertice).FirstOrDefault();
        }
        #endregion PropriedadesPublicas

        #region OperadoresGrafo
        public Aresta ObtenhaAresta(int idOrigem, int idDestino)
        {
            return Arestas.Where(x => x.Origem == idOrigem && x.Destino == idDestino).FirstOrDefault();
        }

        public Vertice ObtenhaAresta(int Id)
        {
            return Vertices.Where(x => x.Id == Id).FirstOrDefault();
        }
        public void AdicionaVertice(Vertice Vertice)
        {
            AdicionaVertice(Vertice.Id);
        }

        public void AdicionaVertice(int IdVertice)
        {
            if (Vertices.Where(x => x.Id == IdVertice).FirstOrDefault() == null)
                Vertices.Add(new Vertice() { Id = IdVertice });
        }

        public void AdicionaAresta(Vertice Origem, Vertice Destino, int Peso)
        {
            AdicionaAresta(Origem.Id, Destino.Id, Peso);
        }

        public void AdicionaAresta(int Origem, int Destino, int Peso, int id = -1)
        {
            AdicionaVertice(Origem);
            AdicionaVertice(Destino);
            Aresta aresta = ObtenhaAresta(Origem, Destino);
            if (aresta is object)
            {
                EditarAresta(Origem, Destino, Peso);
            }
            else
            {
                Arestas.Add(new Aresta()
                {
                    Origem = Origem,
                    Destino = Destino,
                    Peso = Peso,
                    Id = id
                });
            }
            AdicionaVerticeAdjacente(Origem, Destino);
        }

        public void ImprimeGrafo()
        {
            Console.WriteLine("Percusos:");
            foreach (var a in Arestas)
                Console.WriteLine($"{a.Origem} -> {a.Destino} : {a.Peso}");
            Console.WriteLine();
        }
        #endregion OperadoresGrafo

        #region private
        private void EditarAresta(int Origem, int Destino, int Peso)
        {
            Arestas[Arestas.FindIndex(x => x.Origem == Origem && x.Destino == Destino)].Peso = Peso;
        }
        private void AdicionaVerticeAdjacente(int Origem, int Destino)
        {
            Vertices[Vertices.FindIndex(x => x.Id == Origem)].AdicionaVerticeAdjacente(Destino);
        }
        #endregion private
    }
}
