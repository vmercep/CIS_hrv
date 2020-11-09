// MainForm
using System;
using System.Runtime.Serialization;

[Serializable]
internal class NoOibException : Exception
{
    public NoOibException()
    {
    }

    public NoOibException(string message) : base(message)
    {
    }

    public NoOibException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected NoOibException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}