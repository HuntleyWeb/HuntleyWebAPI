namespace HuntleyServicesAPI.Configuration.Options
{
    public abstract class BaseOptionsCreator<T>
    {        
        public abstract T Create(IApplicationConfiguration applicationConfiguration);        
    }
}
