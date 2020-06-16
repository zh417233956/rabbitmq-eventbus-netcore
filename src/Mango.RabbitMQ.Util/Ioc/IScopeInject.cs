namespace Mango.RabbitMQ.Util.Ioc
{
    /// <summary>
    /// （范围）生命周期在每次请求的时候被创建，在一次请求中，如果存在多次获取这个实例，那么返回的也是同一个实例
    /// </summary>
    public interface IScopeInject : INeedInject
    {
    }
}
