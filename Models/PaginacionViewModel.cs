namespace ManejoPresupuesto.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        public int recordsPorPagina { get; set; } = 3;
        private readonly int maxRecordsPorPagina = 50;
        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > maxRecordsPorPagina) ? maxRecordsPorPagina : value;
            }
        }
        public int Skip => RecordsPorPagina * (Pagina - 1);
    }
}

