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

        public static Either<TLeftResult, TRight> Reduce<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either, Func<TLeft, TLeftResult> func)
        {
            return either is Left<TLeft, TRight> left ?
                   (Either<TLeftResult, TRight>)func(left) :
                   (TRight)(Right<TLeft, TRight>)either;
        }

    }
}
