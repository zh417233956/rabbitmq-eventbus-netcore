namespace Mango.RabbitMQ.Util.Ioc
{
    /// <summary>
    /// （单例）生命周期在它们第一次被请求的时候使用的时候创建，并且只创建一次，后续都是使用的同一个对象
    /// </summary>
    public interface ISingleTonInject : INeedInject
    {
    }
}
