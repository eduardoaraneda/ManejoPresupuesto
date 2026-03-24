namespace ManejoPresupuesto.Models
{
    public class PaginacionRespuesta
    {
        public int Pagina { get; set; }
        public int RecordsPorPagina { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRecords / RecordsPorPagina);
        public string UrlBase { get; set; }

    }

    public class PaginacionRespuesta<T> : PaginacionRespuesta
    {
        public IEnumerable<T> Elementos { get; set; }
    }
}
