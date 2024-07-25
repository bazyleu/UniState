namespace UniState;

public interface ILimitedStack<T>
{
    public T Push(T element);
    public T Peek();
    public T Pop();

    public int Capacity();
}