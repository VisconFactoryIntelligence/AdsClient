using System;
using System.Collections.Generic;

namespace Ads.Client.Conversation.WriteMultiple;

internal class WriteMultipleException
{
    public static void Throw(IEnumerable<Exception> itemExceptions)
    {
        throw new AggregateException("One or multiple items could not be written, see InnerExceptions for details.",
            itemExceptions);
    }
}