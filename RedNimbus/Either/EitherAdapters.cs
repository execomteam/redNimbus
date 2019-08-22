using RedNimbus.Either;
using System;
using System.Collections.Generic;
using System.Text;

namespace Either
{
    public static class EitherAdapters
    {
        
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRight, TRightResult> func)
        {
            return  either is Right<TLeft, TRight> right ?
                    (Either<TLeft,TRightResult>)func(right) :
                    (TLeft)(Left<TLeft,TRight>)either;
        }
        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft,TRight> either, Func<TRight, Either<TLeft, TRightResult>> func)
        {
            return  either is Right<TLeft, TRight> right ?
                    func(right) :
                    (TLeft)(Left<TLeft, TRight>)either;
        }

        public static Either<TLeft, TRightResult> Map<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either, Func<TRightResult> func)
        {
            return either is Right<TLeft, TRight> ? 
                   (Either<TLeft, TRightResult>) func() :
                   (TLeft)(Left<TLeft, TRight>)either;
        }

        public static TRight Reduce<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> func)
        {
            return either is Left<TLeft, TRight> left ?
                   func(left) :
                   (Right<TLeft, TRight>)either;
        }

        

    }
}
