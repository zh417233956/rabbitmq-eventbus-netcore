using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mango.RabbitMQ.Util.Ioc
{
    /// <summary>
    /// 为IserviceCollection扩展一个方法，实现对程序集进行操作
    /// </summary>
    public static class InjectServiceRegister
    {
        #region 将程序集中的所有符合条件的类型全部注册到IServiceCollection中     

        /// <summary>
        /// 注册程序集中的服务
        /// </summary>
        /// <param name="services">IServiceCollection类型的对象</param>
        /// <param name="assemblyNames">程序集名的字典</param>
        public static void AutoRegisterMQService(this IServiceCollection services, IDictionary<string, string> assemblyNames = null)
        {
            Type iNeedInject = typeof(INeedInject);
            Type iTransentInject = typeof(ITransentInject);
            Type iScopeInject = typeof(IScopeInject);
            Type iSingletonInject = typeof(ISingleTonInject);
            Type iNoNeedInject = typeof(INoNeedInject);
            if (assemblyNames == null)
            {
                assemblyNames = GetAssemblyNames();
            }
            foreach (var assemblyItem in assemblyNames)
            {
                string assemblyInterName = assemblyItem.Key;
                string assemblyObjName = assemblyItem.Key;
                Type[] interTypes = Assembly.Load(assemblyInterName).GetTypes().Where(t => t.IsInterface && iNeedInject.IsAssignableFrom(t) && t != iNeedInject && t != iTransentInject && t != iScopeInject && t != iSingletonInject).ToArray();
                foreach (Type interType in interTypes)
                {
                    Type objType = Assembly.Load(assemblyObjName).GetTypes().Where(t => t.IsClass && interType.IsAssignableFrom(t) && !iNoNeedInject.IsAssignableFrom(t)).SingleOrDefault();
                    if (objType == null)
                    {
                        throw new Exception($"********************当前接口={interType.Name}没有找到对应的实现类********************");
                    }
                    IList<Type> inJectTypeList = objType.GetInterfaces().Where(i => i == iTransentInject || i == iScopeInject || i == iSingletonInject).ToList();
                    if (inJectTypeList.Count != 1)
                    {
                        throw new Exception($"********************当前接口={interType.Name}没有找到合适的生命周期类型********************");
                    }
                    Type inJectType = inJectTypeList.Single();
                    string inJectTypeName = inJectType.Name;
                    switch (inJectTypeName)
                    {
                        case "ITransentInject": services.AddTransient(interType, objType); break;
                        case "IScopeInject": services.AddScoped(interType, objType); break;
                        case "ISingleTonInject": services.AddSingleton(interType, objType); break;
                        default:
                            throw new Exception($"********************当前接={interType.Name}没有指定注入实例的生命周期********************");
                    }
                }
                //注入没有继承接口的类
                Type iNeedInjectClass = typeof(INeedInjectClass);
                Type[] classTypes = Assembly.Load(assemblyInterName).GetTypes().Where(t => !t.IsInterface && iNeedInject.IsAssignableFrom(t) && iNeedInjectClass.IsAssignableFrom(t) && t != iNeedInject && t != iTransentInject && t != iScopeInject && t != iSingletonInject).ToArray();
                foreach (Type classType in classTypes)
                {
                    IList<Type> inJectTypeList = classType.GetInterfaces().Where(i => i == iTransentInject || i == iScopeInject || i == iSingletonInject).ToList();
                    if (inJectTypeList.Count != 1)
                    {
                        throw new Exception($"********************当前接口={classType.Name}没有找到合适的生命周期类型********************");
                    }
                    Type inJectType = inJectTypeList.Single();
                    string inJectTypeName = inJectType.Name;
                    switch (inJectTypeName)
                    {
                        case "ITransentInject": services.AddTransient(classType); break;
                        case "IScopeInject": services.AddScoped(classType); break;
                        case "ISingleTonInject": services.AddSingleton(classType); break;
                        default:
                            throw new Exception($"********************当前接={classType.Name}没有指定注入实例的生命周期********************");
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, string> GetAssemblyNames()
        {
            //AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(m =>
            //          (!m.FullName.Contains("Microsoft.")) &&
            //          (!m.FullName.Contains("System.")))
            //    .ToArray();

            var assemblies = RuntimeHelper.GetAllAssemblies().Select(m => m.GetName()).ToArray();

            IDictionary<string, string> assemblyNames = new Dictionary<string, string>();
            foreach (var item in assemblies)
            {
                assemblyNames.Add(item.Name, item.Name);
            }

            return assemblyNames;
        }

    }
}
