namespace FortisService.Core.Payload.V1
{
    /// <summary>
    /// Route parameters when query a root resource by id.
    /// </summary>
    public class RouteIdParameters
    {
        /// <summary>
        /// Requested entity id.
        /// </summary>
        public int Id { get; set; }
    }
}
