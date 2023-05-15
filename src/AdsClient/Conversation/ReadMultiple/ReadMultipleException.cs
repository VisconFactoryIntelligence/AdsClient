using System;
using System.Collections.Generic;

namespace Ads.Client.Conversation.ReadMultiple;

internal class ReadMultipleException
{
    public static void Throw(IEnumerable<Exception> innerExceptions)
    {
        throw new AggregateException(
            "One or multiple variables could not be read, see InnerExceptions for details.", innerExceptions);
    }
}