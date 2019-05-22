using BaseSimulacao.Entidades.Enuns;
using System;
using System.Collections.Generic;

namespace BaseSimulacao.Entidades
{
    public class Semaforo : BaseEntidade
    {
        #region Propriedades
        public int TempoAberto { get; set; }
        public int TempoFechado { get; set; }
        public int TempoAmarelo { get; set; }
        public int TempoAtual { get; set; } = 0;
        public EstadosSemaforo EstadoSemaforo { get; set; }
        public List<int> RuasOrigem { get; set; } = new List<int>();
        public List<int> RuasDestino { get; set; } = new List<int>();
        #endregion Propriedades
        #region Contrutor
        public Semaforo()
        {
            Id = -1;
            TempoAberto = -1;
            TempoFechado = -1;
            TempoAmarelo = -1;
            TempoAtual = 0;
            EstadoSemaforo = EstadosSemaforo.FECHADO;
        }
        #endregion Contrutor
        #region Metodos
        public void InicializaSemaforo(int id, int tempoAberto, int tempoFechado, int tempoAmarelo, EstadosSemaforo Estado)
        {
            if (id < 0) throw new Exception("O Id do semáforo deve ser maior ou igual a zero");
            if (tempoAberto <= 0 || tempoAmarelo <= 0 || tempoFechado <= 0)
                throw new Exception("Os valores de tempo devem ser maior do que zero");
            Id = id;
            TempoAberto = tempoAberto;
            TempoFechado = tempoFechado;
            TempoAmarelo = tempoAmarelo;
            EstadoSemaforo = Estado;
            RuasOrigem.Clear();
            RuasDestino.Clear();
            TempoAtual = 0;
        }
        public void AtualizaStatusSemaforo(int tempoDecorrido)
        {
            TempoAtual += tempoDecorrido;
            switch (EstadoSemaforo)
            {
                case EstadosSemaforo.ABERTO:
                    if(TempoAtual >= TempoAberto)
                    {
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.AMARELO;
                    }
                    break;
                case EstadosSemaforo.FECHADO:
                    if(TempoAtual >= TempoFechado)
                    {
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.ABERTO;
                    }
                    break;
                case EstadosSemaforo.AMARELO:
                    if(TempoAtual >= TempoAmarelo)
                    {
                        TempoAtual = 0;
                        EstadoSemaforo = EstadosSemaforo.FECHADO;
                    }
                    break;
            }
        }
        #endregion Metodos
    }
}
