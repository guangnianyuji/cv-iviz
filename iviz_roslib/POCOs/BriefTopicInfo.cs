﻿using System;
using System.Runtime.Serialization;
using Iviz.Msgs;
using Iviz.Roslib.Utils;

namespace Iviz.Roslib;

/// <summary>
/// Topic name and message type.
/// </summary>
[DataContract]
public readonly struct BriefTopicInfo : IComparable<BriefTopicInfo>
{
    /// <summary>
    /// Topic name
    /// </summary>
    [DataMember] public string Topic { get; }

    /// <summary>
    /// Topic type
    /// </summary>
    [DataMember] public string Type { get; }

    internal BriefTopicInfo(string topic, string type) => (Topic, Type) = (topic, type);
        
    public void Deconstruct(out string topic, out string type) => (topic, type) = (Topic, Type);
        
    public override string ToString() => $"[Topic='{Topic}' Type='{Type}']";

    public int CompareTo(BriefTopicInfo other)
    {
        return (Topic, Type).CompareTo((other.Topic, other.Type));
    }
}

