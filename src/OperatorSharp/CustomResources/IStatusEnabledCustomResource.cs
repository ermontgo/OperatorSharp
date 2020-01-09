namespace OperatorSharp.CustomResources
{
    public interface IStatusEnabledCustomResource<TStatus> where TStatus: IStatus
    {
        TStatus Status { get; set; }
    }

    public interface IStatusEnabledCustomResource { }
}