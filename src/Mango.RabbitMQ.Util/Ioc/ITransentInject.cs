namespace Mango.RabbitMQ.Util.Ioc
{
    /// <summary>
    /// （瞬时）生命周期在他们每次请求的时候被创建，这一生命周期适合轻量级和无状态的服务。并且在一次请求中，如果存在多次获取这个实例，那这些实例也是不同的
    /// </summary>
    public interface ITransentInject : INeedInject
    {
    }
}
