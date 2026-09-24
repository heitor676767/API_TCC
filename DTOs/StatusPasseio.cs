namespace API_TCC.DTOs
{
    // Valores possíveis de StatusPass. Centralizados aqui pra não espalhar strings mágicas
    // pelos controllers (não é um enum de verdade pra não mexer na migration já existente).
    public static class StatusPasseio
    {
        public const string Solicitado = "Solicitado";
        public const string Aceito = "Aceito";
        public const string Recusado = "Recusado";
        public const string EmAndamento = "EmAndamento";
        public const string Finalizado = "Finalizado";
        public const string Cancelado = "Cancelado";
    }
}
