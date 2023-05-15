using System;
using System.Collections.Generic;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

internal class ReadMultipleException
{
    public static void Throw(IEnumerable<Exception> innerExceptions)
    {
        throw new AggregateException(
            "One or multiple variables could not be read, see InnerExceptions for details.", innerExceptions);
    }
}