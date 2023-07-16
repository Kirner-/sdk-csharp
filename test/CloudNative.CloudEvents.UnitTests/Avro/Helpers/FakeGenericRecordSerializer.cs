﻿using Avro;
using Avro.Generic;
using CloudNative.CloudEvents.Avro.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudNative.CloudEvents.UnitTests.Avro.Helpers;

internal class FakeGenericRecordSerializer : IGenericRecordSerializer
{
    public byte[]? SerializeResponse { get; private set; }
    public GenericRecord DeserializeResponse { get; private set; }
    public int DeserializeCalls { get; private set; } = 0;
    public int SerializeCalls { get; private set; } = 0;

    public FakeGenericRecordSerializer()
    {
        DeserializeResponse = new GenericRecord(ParseEmbeddedSchema());
    }

    public GenericRecord Deserialize(Stream messageBody)
    {
        DeserializeCalls++;
        return DeserializeResponse;
    }

    public ReadOnlyMemory<byte> Serialize(GenericRecord value)
    {
        SerializeCalls++;
        return SerializeResponse;
    }

    public void SetSerializeResponse(byte[] response) => SerializeResponse = response;

    public void SetDeserializeResponseAttributes(string id, string type, string source) =>
        DeserializeResponse.Add("attribute", new Dictionary<string, object>()
        {
            { CloudEventsSpecVersion.SpecVersionAttribute.Name, CloudEventsSpecVersion.Default.VersionId },
            { CloudEventsSpecVersion.Default.IdAttribute.Name, id},
            { CloudEventsSpecVersion.Default.TypeAttribute.Name, type},
            { CloudEventsSpecVersion.Default.SourceAttribute.Name, source}
        });

    private static RecordSchema ParseEmbeddedSchema()
    {
        using var sr = (new StreamReader(typeof(FakeGenericRecordSerializer)
            .Assembly
            .GetManifestResourceStream("CloudNative.CloudEvents.UnitTests.AvroSchema.json")!));
        return (RecordSchema) Schema.Parse(sr.ReadToEnd());
    }
}
