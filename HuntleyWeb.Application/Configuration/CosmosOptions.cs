namespace HuntleyWeb.Application.Configuration
{
    public class CosmosOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public int TtlSecs { get; set; }
    }
}
