namespace MinhaEstante.Api.Requests
{

    public record DataLeituraRequest(DateOnly? Data);
    public record AvaliacaoRequest(int Nota);
    public record AnotacoesRequest(string? Anotacoes);
}
