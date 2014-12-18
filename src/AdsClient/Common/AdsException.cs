/*  Copyright (c) 2011 Inando
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Ads.Client.Common
{
    public class AdsException : Exception
    {
        public AdsException(uint errorCode) : base(GetErrorMessage(errorCode))
        {
            this.errorCode = errorCode;
        }

        public AdsException(string message) : base(message)
        {
            this.errorCode = 0;
        }

        private uint errorCode;
        public uint ErrorCode { get { return errorCode; } }

        private static string GetErrorMessage(uint errorCode)
        {
            string msg = "";
            switch (errorCode)
            {
                case 0:  msg = "no error"; break;
                case 1 : msg = "Internal error"; break;
                case 2 : msg = "No Rtime"; break;
                case 3 : msg = "Allocation locked memory error"; break;
                case 4 : msg = "Insert mailbox error"; break;
                case 5 : msg = "Wrong receive HMSG"; break;
                case 6 : msg = "target port not found"; break;
                case 7 : msg = "target machine not found"; break;
                case 8 : msg = "Unknown command ID"; break;
                case 9 : msg = "Bad task ID"; break;
                case 10: msg = "No IO"; break;
                case 11: msg = "Unknown AMS command"; break;
                case 12: msg = "Win 32 error"; break;
                case 13: msg = "Port not connected"; break;
                case 14: msg = "Invalid AMS length"; break;
                case 15: msg = "Invalid AMS Net ID"; break;
                case 16: msg = "Low Installation level"; break;
                case 17: msg = "No debug available"; break;
                case 18: msg = "Port disabled"; break;
                case 19: msg = "Port already connected"; break;
                case 20: msg = "AMS Sync Win32 error"; break;
                case 21: msg = "AMS Sync Timeout"; break;
                case 22: msg = "AMS Sync AMS error"; break;
                case 23: msg = "AMS Sync no index map"; break;
                case 24: msg = "Invalid AMS port"; break;
                case 25: msg = "No memory"; break;
                case 26: msg = "TCP send error"; break;
                case 27: msg = "Host unreachable"; break;
                           
                case 1792: msg="error class <device error>"; break;
                case 1793: msg="Service is not supported by server"; break;
                case 1794: msg="invalid index group"; break;
                case 1795: msg="invalid index offset"; break;
                case 1796: msg="reading/writing not permitted"; break;
                case 1797: msg="parameter size not correct"; break;
                case 1798: msg="invalid parameter value(s)"; break;
                case 1799: msg="device is not in a ready state"; break;
                case 1800: msg="device is busy"; break;
                case 1801: msg="invalid context (must be in Windows)"; break;
                case 1802: msg="out of memory"; break;
                case 1803: msg="invalid parameter value(s)"; break;
                case 1804: msg="not found (files, ...)"; break;
                case 1805: msg="syntax error in command or file"; break;
                case 1806: msg="objects do not match"; break;
                case 1807: msg="object already exists"; break;
                case 1808: msg="symbol not found"; break;
                case 1809: msg="symbol version invalid"; break;
                case 1810: msg="server is in invalid state"; break;
                case 1811: msg="AdsTransMode not supported"; break;
                case 1812: msg="Notification handle is invalid"; break;
                case 1813: msg="Notification client not registered"; break;
                case 1814: msg="no more notification handles"; break;
                case 1815: msg="size for watch too big"; break;
                case 1816: msg="device not initialized"; break;
                case 1817: msg="device has a timeout"; break;
                case 1818: msg="query interface failed"; break;
                case 1819: msg="wrong interface required"; break;
                case 1820: msg="class ID is invalid"; break;
                case 1821: msg="object ID is invalid"; break;
                case 1822: msg="request is pending"; break;
                case 1823: msg="request is aborted"; break;
                case 1824: msg="signal warning"; break;
                case 1825: msg="invalid array index"; break;
                case 1826: msg="symbol not active -> release handle and try again"; break;
                case 1827: msg="access denied"; break;
                case 1856: msg="Error class <client error>"; break;
                case 1857: msg="invalid parameter at service"; break;
                case 1858: msg="polling list is empty"; break;
                case 1859: msg="var connection already in use"; break;
                case 1860: msg="invoke ID in use"; break;
                case 1861: msg="timeout elapsed"; break;
                case 1862: msg="error in win32 subsystem"; break;
                case 1863: msg="Invalid client timeout value"; break;
                case 1864: msg="ads-port not opened"; break;
                case 1872: msg="internal error in ads sync"; break;
                case 1873: msg="hash table overflow"; break;
                case 1874: msg="key not found in hash"; break;
                case 1875: msg="no more symbols in cache"; break;
                case 1876: msg="invalid response received"; break;
                case 1877: msg = "sync port is locked"; break;

            }

            return msg;
        }

    }
}
