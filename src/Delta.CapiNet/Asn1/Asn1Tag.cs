using System;
using System.Linq;

namespace Delta.CapiNet.Asn1
{
    // See http://en.wikipedia.org/wiki/X.690#Identifier_octets
    public enum Asn1Tag : byte
    {
        Eoc = 0x00, // Nothing to do: represents the end of a TLV value when length is indeterminate (this is only valid in BER, not DER encoding).
        Boolean = 0x01,
        Integer = 0x02,
        BitString = 0x03,
        OctetString = 0x04,
        Null = 0x05,
        ObjectIdentifier = 0x06,
        ObjectDescriptor = 0x07, // TODO
        External = 0x08, // TODO
        Real = 0x09, // TODO
        Enumerated = 0x0a, // TODO
        EmbeddedPdv = 0x0b, // TODO
        Utf8String = 0x0c,
        RelativeOid = 0x0d, // TODO
        Reserved1 = 0x0e, // TODO
        Reserved2 = 0x0f, // TODO
        Sequence = 0x10,
        Set = 0x11,
        NumericString = 0x12,
        PrintableString = 0x13,
        T61String = 0x14, // TODO
        VideoTextString = 0x15, // TODO
        Ia5String = 0x16, // TODO
        UtcTime = 0x17,
        GeneralizedTime = 0x18, // TODO
        GraphicString = 0x19, // TODO
        VisibleString = 0x1a, // TODO
        GeneralString = 0x1b, // TODO
        UniversalString = 0x1c, // TODO
        CharacterString = 0x1d, // TODO
        BmpString = 0x1e // TODO
    }

    // Tag Class Values: bits 8 and 7
    internal enum Asn1TagClass : byte
    {
        Universal = 0,
        Application = 0x40,
        ContextSpecific = 0x80,
        Private = 0xc0, // = 0x40 + 0x80
    }

    // Masks
    internal enum Asn1TagMask : byte
    {
        LongForm = 0x1f,
        Kind = 0x20,
        Class = 0xc0
    }

    public static class Asn1TagExtensions
    {
        public static bool IsUniversalClass(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Class) == (byte)Asn1TagClass.Universal;
        }

        public static bool IsAplicationClass(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Class) == (byte)Asn1TagClass.Application;
        }

        public static bool IsContextSpecificClass(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Class) == (byte)Asn1TagClass.ContextSpecific;
        }

        public static bool IsPrivateClass(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Class) == (byte)Asn1TagClass.Private;
        }

        public static bool IsPrimitiveKind(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Kind) == 0;
        }

        public static bool IsConstructedKind(this ushort tag)
        {
            return (ExtractFirstByte(tag) & (byte)Asn1TagMask.Kind) == 1;
        }

        public static byte GetAsn1Class(this ushort tag)
        {
            return (byte)(ExtractFirstByte(tag) & (byte)Asn1TagMask.Class);
        }

        public static byte GetAsn1TagValue(this ushort tag)
        {
            return (byte)(ExtractFirstByte(tag) & (byte)Asn1TagMask.LongForm);
        }

        // TODO: store the relationship between a tag and its class name
        public static string GetAsn1ClassName(this ushort tag)
        {
            var tagClass = tag.GetAsn1Class();
            if (Enum.GetValues(typeof(Asn1TagClass)).Cast<byte>().Contains(tagClass))
                return ((Asn1TagClass)tagClass).ToString();
            return string.Format("0x{0:X2}", tagClass);
        }

        ////// TODO: store the relationship between a tag and its tag name
        ////public static string GetAsn1TagName(this ushort tag)
        ////{
        ////    var tagValue = tag.GetAsn1TagValue();
        ////    if (Enum.GetValues(typeof(Asn1Tags)).Cast<byte>().Contains(tagValue))
        ////        return ((Asn1Tags)tagValue).ToString();
        ////    return string.Format("0x{0:X2}", tag);
        ////}

        private static byte ExtractFirstByte(ushort tag)
        {
            if (tag <= byte.MaxValue)
                return (byte)tag;
            return (byte)(tag >> 8);
        }
    }
}
