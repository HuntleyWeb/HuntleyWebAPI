namespace HuntleyServicesAPI.Models.Validation
{
    public class ValidatedRequest<T>
    {
        public T Value { get; set; }

        public List<ErrorResult> Errors { get; set; }

        public ValidatedRequest()
        { 
            Errors = new List<ErrorResult>();
        }
    }
}
