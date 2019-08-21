namespace Either
{
    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TLeft _content;

        public Left(TLeft content)
        {
            _content = content;
        }

        public static implicit operator TLeft(Left<TLeft, TRight> obj)
        {
            return obj._content;
        }
    }
}
