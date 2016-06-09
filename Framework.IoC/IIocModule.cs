namespace Framework.IoC
{
    using SimpleInjector;

    public interface IIocModule
    {
        void Register(Container container);
    }
}
