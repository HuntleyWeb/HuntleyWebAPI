namespace HuntleyWeb.Application.Configuration
{
    public abstract class BaseCosmosOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public int TtlSecs { get; set; }
    }
}
