public interface IInitializable
{
    bool Initialized { get; }
    void Init();
}