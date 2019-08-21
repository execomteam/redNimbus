namespace Either
{
    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private TRight _content;

        public Right(TRight content)
        {
            _content = content;
        }

        public static implicit operator TRight(Right<TLeft, TRight> obj)
        {
            return obj._content;
        }
    }
}
