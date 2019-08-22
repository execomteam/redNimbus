using System;

namespace RedNimbus.Either
{
    public class Either<TLeft, TRight>
    {
        public static implicit operator Either<TLeft,TRight>(TLeft obj)
        {
            return new Left<TLeft, TRight>(obj);
        }

        public static implicit operator Either<TLeft, TRight>(TRight obj)
        {
            return new Right<TLeft, TRight>(obj);
        }
    }  
}
