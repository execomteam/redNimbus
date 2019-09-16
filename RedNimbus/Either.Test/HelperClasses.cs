using System;
using System.Collections.Generic;
using System.Text;

namespace Either.Test
{
    public class Outcome
    {
        public string message;

        public Outcome(string message)
        {
            this.message = message;
        }
    }

    public class Success1 { }

    public class Success2 { }

    public interface IError { }

    public class Error1 : IError { }

    public class Error2 : IError { }

    public class Error3 : IError { }
}
