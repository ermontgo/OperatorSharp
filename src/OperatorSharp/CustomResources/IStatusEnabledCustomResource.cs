namespace OperatorSharp.CustomResources
{
    public interface IStatusEnabledCustomResource<TStatus> where TStatus: CustomResourceStatus
    {
        TStatus Status { get; set; }
    }

    public interface IStatusEnabledCustomResource { }
}