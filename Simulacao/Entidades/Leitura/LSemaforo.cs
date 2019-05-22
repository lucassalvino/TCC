namespace BaseSimulacao.Entidades.Leitura
{
    public class LSemaforo
    {
        public int VerticeOrigemOrigem { get; set; }
        public int VerticeDestinoOrigem { get; set; }
        public int VerticeOrigemDestino { get; set; }
        public int VerticeDestinoDestino { get; set; }
        public int TempoAberto { get; set; }
        public int TempoFechado { get; set; }
        public int TempoAmarelo { get; set; }
    }
}
