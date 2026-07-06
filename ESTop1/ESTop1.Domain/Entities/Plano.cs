namespace ESTop1.Domain;

public class Plano
{
    public Guid Id { get; set; }
    public PlanoAssinatura Tipo { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal ValorMensal { get; set; }
    public int LimiteJogadores { get; set; }
    public bool AcessoEstatisticas { get; set; }
    public bool AcessoBuscaIA { get; set; }
    public bool AcessoAPI { get; set; }
    public bool SuportePrioritario { get; set; }
    public bool Ativo { get; set; } = true;
}
