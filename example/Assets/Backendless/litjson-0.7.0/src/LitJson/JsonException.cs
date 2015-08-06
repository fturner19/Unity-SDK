#region Header
/**
 * JsonException.cs
 *   Base class throwed by LitJSON when a parsing error occurs.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;


namespace BackendlessAPI.LitJson
{
    public class JsonException : ApplicationException
    {
        public JsonException () : base ()
        {
        }

        internal JsonException (ParserToken token) :
            base (String.Format (
                    "Invalid token '{0}' in input string", token))
        {
        }

        internal JsonException (ParserToken token,
#if true // for BACKENDLESS
                                System.Exception inner_exception) :
#else
                                Exception inner_exception) :
#endif
            base (String.Format (
                    "Invalid token '{0}' in input string", token),
                inner_exception)
        {
        }

        internal JsonException (int c) :
            base (String.Format (
                    "Invalid character '{0}' in input string", (char) c))
        {
        }

#if true // for BACKENDLESS
        internal JsonException (int c, System.Exception inner_exception) :
#else
        internal JsonException (int c, Exception inner_exception) :
#endif
            base (String.Format (
                    "Invalid character '{0}' in input string", (char) c),
                inner_exception)
        {
        }


        public JsonException (string message) : base (message)
        {
        }

#if true // for BACKENDLESS
        public JsonException (string message, System.Exception inner_exception) :
#else
        public JsonException (string message, Exception inner_exception) :
#endif
            base (message, inner_exception)
        {
        }
    }
}
