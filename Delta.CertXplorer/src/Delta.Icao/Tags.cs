using System;
using System.Linq;

namespace Delta.Icao
{
    public static class Tags
    {
        /// <summary>
        /// This is the list of EF (data files) in an ICAO chip that contain data groups.
        /// </summary>
        public enum EF : ushort
        {
            Com = 0x60,
            DG1 = 0x61, // MRZ
            DG2 = 0x75, // Photo
            DG3 = 0x63, // Fingerprints
            DG4 = 0x76, // Irises
            DG5 = 0x65, // Facial image
            DG6 = 0x66, // DG6 is reserved for future use
            DG7 = 0x67, // Signature
            DG8 = 0x68, // Security Encoded Data
            DG9 = 0x69, // Security Structure
            DG10 = 0x6A, // Security Substance
            DG11 = 0x6B, // Personal Details
            DG12 = 0x6C, // Document Details
            DG13 = 0x6D, // Optional Details
            DG14 = 0x6E, // Chip Authentication
            DG15 = 0x6F, // Authentification PKI
            DG16 = 0x70, //Person to notify
            Sod = 0x77,
            Cvca = 0x42 // CVCA for EAC
        }

        public enum Icao : ushort
        {
            Mrz = 0x5F1F    // The MRZ tag inside DG1
        }

        public static EF? FindEF(ushort tag)
        {
            if (Enum.GetValues(typeof(EF)).Cast<ushort>().Contains(tag))
                return (EF)tag;
            return null;
        }
    }
}

