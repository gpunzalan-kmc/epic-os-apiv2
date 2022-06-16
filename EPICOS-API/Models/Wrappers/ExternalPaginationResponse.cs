namespace EPICOS_API.Models.Wrappers
{
    public class ExternalPaginationResponse<T>
    {
        public T[] Data { get; set; }
        public object links { get; set; }
        public Meta meta { get; set; }
    }
}