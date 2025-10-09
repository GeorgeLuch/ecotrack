namespace EcoTrackAPI.ViewModels
{
    public class ComplianceViewModel
    {
        public string TipoNorma { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
    }
}